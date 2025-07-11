using CSX.Common.Data.DataGenerators;
using CSX.Common.Data.Exceptions;
using CSX.Common.Data.Placeholders;
using CSX.Common.Extensions.Database;
using CSX.Common.Extensions.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.AuthDb;
using XenoFx.Database.AuthDb.Models;
using XenoFx.Services.Auth.IpPinAuth.Models;
using XenoFx.Services.Storage.IpPinAuthDatabase;

namespace XenoFx.Services.Auth.IpPinAuth;

public partial class IpPinAuthService : IIpPinAuthService
{
    // Infrastructure

    private readonly IIpPinAuthDbWorkerService _db;
    private readonly ILogger<IpPinAuthService> _logger;

    // Data

    public static TimeSpan RenewalExtension => TimeSpan.FromHours(4);

    // Lifecycle

    public IpPinAuthService(
        IIpPinAuthDbWorkerService db,
        ILogger<IpPinAuthService> logger)
    {
        _db = db;
        _logger = logger;
    }

    // Middleware

    public async Task<bool> AuthorizeIpAsync(
        IPAddress ip,
        CancellationToken ctoken = default)
    {
        string ipString = ip.ToString();

        // Check ip blacklist here

        // Get all sessions with this ip
        var sessions = await _db.ExecuteAsync(async (db, ct) =>
        {
            return await db.Sessions
                .Where(x => x.IpAddress == ipString)
                .AsNoTracking()
                .ToListAsync(ct);
        }, ctoken);

        // Check which session hasn't expired yet
        var now = DateTimeOffset.UtcNow;
        var allowIp = sessions.Any(x => x.GetState() == SessionStates.Active && x.IsIpActive(now));
        return allowIp;
    }

    // Shared

    private async Task<IpPinAuthSession> GetOrCreateSessionAsync(
        AuthDbContext db,
        IpPinAuthContext context,
        CancellationToken ctoken = default)
    {
        // Set defaults
        var now = DateTimeOffset.UtcNow;
        context.SessionState = SessionStates.Available;
        context.AuthTokenState = AuthTokenStates.None;
        bool isFaultedClient = false;

        // If clientId is provided, try to find an existing session
        if (!string.IsNullOrWhiteSpace(context.Dto.ClientId))
        {
            var session = await db.Sessions
                .Where(x => x.ClientId == context.Dto.ClientId)
                .FirstOrDefaultAsync(ctoken);

            // If a session with that client-id exists
            if (session is not null)
            {
                // Assess state and auth
                context.SessionState = session.GetState(now);
                if (context.SessionState == SessionStates.Disabled)
                    throw new ClientDisabledException();
                context.AuthTokenState = session.AuthorizeClientToken(context.Dto.AuthToken);
                context.Dto.IsAuthorized = context.IsRequestAuthorized;

                // Case: Authentic client, i.e. auth-token matches;
                // Authorize if not expired
                if (context.AuthTokenState == AuthTokenStates.Matched)
                {
                    return session;
                }
                // Case: Registered client, but token missing on either or both sides;
                // Treat as unauthenticated, but not faulted
                if (context.AuthTokenState == AuthTokenStates.None ||
                    context.AuthTokenState == AuthTokenStates.NotFound)
                {
                    return session;
                }
                // Case: Faulted client, stale or malicious token;
                // Fall-through for re-initialize, flag for logging the transition
                if (context.AuthTokenState == AuthTokenStates.Mismatched)
                {
                    isFaultedClient = true;
                }
                // Default: Fall through to re-assignment
            }
        }

        // Create a new client entry
        IpPinAuthSession newSession = new();
        while (true)
        {
            try
            {
                newSession.ClientId = HexcodeGenerator.GenerateRandomHexId();

                await db.Sessions.AddAsync(newSession, ctoken);
                await db.SaveChangesAsync(ctoken);

                // Assign the new client id to the dto
                context.Dto.NewClientId = newSession.ClientId;

                // Explicitly log faulted client transitions
                if (isFaultedClient)
                {
                    _logger.LogWarning(
                        "Stale or malicious token used by client {ClientId}. Assigning new id. Request: {context}",
                        context.Dto.ClientId, context);
                }

                // No need to set session state since it's 'available' by default
                return newSession;
            }
            catch (DbUpdateException ex)
            {
                if (ex.IsUniqueConstraintViolation())
                {
                    _logger.LogError(ex, "Duplicate id generated (extremely rare). Retrying...");
                    continue;
                }
                throw;
            }
        }
    }

    // Endpoints : Status, Generate

    public async Task GetStatusAsync(
        IpPinAuthDto<Unit, IpPinAuthClientInfo> dto,
        CancellationToken ctoken = default)
    {
        IpPinAuthContext context = new(dto);

        var result = await _db.ExecuteAsync(async (db, ct) =>
        {
            // Get or pre-allocate
            IpPinAuthSession session = await GetOrCreateSessionAsync(db, context, ctoken);

            // Return a copy to close the db session as early as possible
            return session.Copy();
        }, ctoken);

        // Map to response
        IpPinAuthClientInfo info = new()
        {
            CurrentIP = dto.IPAddress.EnsureNotNull().ToString(),
            IsAuthorized = context.Dto.IsAuthorized, // Ensures session is authenticated and request is authorized
        };

        // Give more details if it's an authorized request
        if (info.IsAuthorized)
        {
            info.TokenExpiresAt = result.ExpiresAt.EnsureNotNull();

            // If any ip is active, give details
            if (result.IpAddress is not null)
            {
                info.ActiveIP = result.IpAddress;
                info.LeaseStartTime = result.ActivatedIpAt;
                info.LeaseEndTime = result.DeactivatesIpAt;
            }
        }

        // Assign if the response was successfully built
        dto.ResponsePayload = info;
    }

    public async Task GeneratePinAsync(
        IpPinAuthDto<Unit, int> dto,
        CancellationToken ctoken = default)
    {
        IpPinAuthContext context = new(dto);
        var now = DateTimeOffset.UtcNow;

        await _db.ExecuteAsync(async (db, ct) =>
        {
            // Get or pre-allocate
            IpPinAuthSession session = await GetOrCreateSessionAsync(db, context, ctoken);

            // If a pin exists and within cooldown period:
            // Return the already generated pin
            if (session.Pin.HasValue &&
                session.PinGenerationDisabledUntil.HasValue &&
                session.PinGenerationDisabledUntil.Value < now)
            {
                dto.ResponsePayload = session.Pin.Value;
                return;
            }

            // Else generate a new pin:
            // Possibly more secure than Random.Shared.Next(100000, 999999);
            int newPin = RandomNumberGenerator.GetInt32(100000, 1000000);
            session.Pin = newPin;

            // Set a new cooldown:
            // TODO use IClock and TimeSpan from config in production
            session.PinGenerationDisabledUntil = now.AddMinutes(5);
            await db.SaveChangesAsync(ct);

            // Set the payload
            dto.ResponsePayload = newPin;
        }, ctoken);
    }

    // Endpoints : Login, Logout

    public async Task LoginAsync(
        IpPinAuthDto<int?, IpPinAuthentication> dto,
        CancellationToken ctoken = default)
    {
        IpPinAuthContext context = new(dto);
        var now = DateTimeOffset.UtcNow;

        // Short circuit input validation
        if (!dto.RequestPayload.HasValue)
            throw new BadRequestException("A pin is required for authentication.");

        await _db.ExecuteAsync(async (db, ct) =>
        {
            // Get or pre-allocate
            IpPinAuthSession session = await GetOrCreateSessionAsync(db, context, ctoken);

            // Ensure pin is still valid
            if (session.Pin.HasValue &&
                session.PinGenerationDisabledUntil.HasValue &&
                session.PinGenerationDisabledUntil.Value > now)
            {
                // On pin match, authenticate the session
                if (dto.RequestPayload == session.Pin.Value)
                {
                    // Optionally make a snapshot here for history

                    // setup and assign the new auth data
                    string authToken = IpPinAuthExtensions.GenerateAuthToken();
                    DateTimeOffset expiry = now.AddYears(20);

                    session.AuthToken = authToken;
                    session.AuthenticatedAt = now;
                    session.ExpiresAt = expiry;
                    session.RevokedAt = null;

                    // Delete pin since it's been used up
                    session.Pin = null;
                    session.PinGenerationDisabledUntil = null;

                    // Commit and set payload
                    await db.SaveChangesAsync(ct);
                    dto.ResponsePayload = new(authToken, expiry);
                    return;
                }
            }

            // Default fail
            throw new UnauthorizedAccessException();
        }, ctoken);
    }

    public async Task LogoutAsync(
        IpPinAuthDto<Unit, Unit> dto,
        CancellationToken ctoken = default)
    {
        IpPinAuthContext context = new(dto);

        await _db.ExecuteAsync(async (db, ct) =>
        {
            // Get or pre-allocate
            IpPinAuthSession session = await GetOrCreateSessionAsync(db, context, ctoken);

            // In case of an authentic token:
            if (context.AuthTokenState == AuthTokenStates.Matched)
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;

                // If already expired or revoked: silently ignore it
                // This prevents stale session probing by attackers
                if (session.RevokedAt.HasValue ||
                    session.ExpiresAt < now)
                    return;

                // Otherwise, revoke the session
                session.RevokedAt = now;
                await db.SaveChangesAsync(ct);
            }

            // Silently succeed even if unauthorized
        }, ctoken);
    }

    // Endpoints : IP Activation

    public async Task ActivateIpAsync(
        IpPinAuthDto<TimeSpan?, Unit> dto,
        CancellationToken ctoken = default)
    {
        IpPinAuthContext context = new(dto);

        await _db.ExecuteAsync(async (db, ct) =>
        {
            // Get or pre-allocate
            IpPinAuthSession session = await GetOrCreateSessionAsync(db, context, ctoken);

            // Operation is only allowed for authorized requests
            if (!context.IsRequestAuthorized)
                throw new UnauthorizedAccessException();

            // Only proceed if verified
            TimeSpan max = TimeSpan.FromDays(30);
            TimeSpan leaseTime = TimeSpan.FromDays(7); // if no duration is specified this will be used

            // if requested time is beyond request parameters
            if (dto.RequestPayload.HasValue)
            {
                if (dto.RequestPayload.Value > max)
                    throw new BusinessRuleViolationException($"Requested duration below Zero or above {max}.");
                leaseTime = dto.RequestPayload.Value;
            }

            // Enable Ip
            var now = DateTimeOffset.UtcNow;
            session.IpAddress = dto.IPAddress.EnsureNotNull().ToString();
            session.ActivatedIpAt = now;
            session.DeactivatesIpAt = now + leaseTime;
            await db.SaveChangesAsync(ct);
        }, ctoken);
    }

    public async Task DeactivateIpAsync(
        IpPinAuthDto<Unit, Unit> dto,
        CancellationToken ctoken = default)
    {
        IpPinAuthContext context = new(dto);

        await _db.ExecuteAsync(async (db, ct) =>
        {
            // Get or pre-allocate
            IpPinAuthSession session = await GetOrCreateSessionAsync(db, context, ctoken);

            // Operation is only allowed for authorized requests
            if (!context.IsRequestAuthorized)
                throw new UnauthorizedAccessException();

            // Since we are nulling all the fields, no need for revokedAt
            // If saving the expired lease in history,
            // set {StaleLeaseEntry}.RevokedAt to DateTimeOffset.UtcNow explicitly.

            // Disable Ip
            session.IpAddress = null;
            session.ActivatedIpAt = null;
            session.DeactivatesIpAt = null;
            await db.SaveChangesAsync(ct);
        }, ctoken);
    }
}