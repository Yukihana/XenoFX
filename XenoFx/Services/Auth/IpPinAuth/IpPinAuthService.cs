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

    // Core

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

                // Case: Authentic client, i.e. auth-token matches;
                // Authorize if not expired
                if (context.AuthTokenState == AuthTokenStates.Matched)
                {
                    if (context.SessionState == SessionStates.Active)
                        context.Dto.IsRequestAuthenticated = true;
                    return session;
                }
                // Case: Registered client, but token missing on either or both sides;
                // Treat as unauthenticated, but not faulted
                else if (context.AuthTokenState == AuthTokenStates.None ||
                    context.AuthTokenState == AuthTokenStates.NotFound)
                {
                    return session;
                }
                // Case: Faulted client, stale or malicious token;
                // Fall-through for re-initialize, flag for logging the transition
                else if (context.AuthTokenState == AuthTokenStates.Mismatched)
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
            IsAuthorized = context.Dto.IsRequestAuthenticated, // Ensures session is authenticated and request is authorized
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
            session.PinGenerationDisabledUntil = now + TimeSpan.FromMinutes(5);
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
                session.PinGenerationDisabledUntil.Value < now &&
                session.Pin.Value is int pin)
            {
                // On pin match, authenticate the session
                if (dto.RequestPayload == pin)
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

            // In case of an authentic token, do an actual logout
            // session state doesn't matter
            if (context.AuthTokenState == AuthTokenStates.Matched)
            {
                // optionally record history here

                // logout
                session.RevokedAt = DateTimeOffset.UtcNow;
                await db.SaveChangesAsync(ct);
            }

            // No response needed:
            // Logout should always report success
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
            TimeSpan max = TimeSpan.FromDays(7);

            // Verify token authenticity
            if (context.SessionState == SessionStates.Active &&
                context.AuthTokenState == AuthTokenStates.Matched &&
                dto.IPAddress is IPAddress ip &&
                dto.RequestPayload.HasValue &&
                dto.RequestPayload.Value is TimeSpan duration &&
                duration <= max)
            {
                // Enable Ip
                var now = DateTimeOffset.UtcNow;
                session.IpAddress = ip.ToString();
                session.ActivatedIpAt = now;
                session.DeactivatesIpAt = now + duration;
                await db.SaveChangesAsync(ct);
                return;
            }

            // Default fail
            throw new UnauthorizedAccessException();
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

            // Verify token authenticity
            if (context.SessionState == SessionStates.Active &&
                context.AuthTokenState == AuthTokenStates.Matched)
            {
                // Save history if applicable
                // Since we are nulling all the fields, no need to set revokedAt
                // If stored in history, it can be added explicitly.

                // Disable Ip
                session.IpAddress = null;
                session.ActivatedIpAt = null;
                session.DeactivatesIpAt = null;
                await db.SaveChangesAsync(ct);
                return;
            }

            // Default fail
            throw new UnauthorizedAccessException();
        }, ctoken);
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
        return sessions.Any(x => x.GetState() == SessionStates.Active && x.IsIpActive(now));
    }
}