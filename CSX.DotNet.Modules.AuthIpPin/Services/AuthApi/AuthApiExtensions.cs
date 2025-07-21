using CSX.DotNet.Modules.AuthIpPin.Services.AuthApi.Models;
using CSX.DotNet.Modules.AuthIpPin.Storage.AuthDb.Models;
using System;
using System.Security.Cryptography;

namespace CSX.DotNet.Modules.AuthIpPin.Services.AuthApi;

public static class AuthApiExtensions
{
    public static SessionStates GetState(
        this IpPinAuthSession session,
        DateTimeOffset? now = null)
    {
        if (!session.IsEnabled)
            return SessionStates.Disabled;

        if (session.AuthenticatedAt is null)
            return SessionStates.Available;

        if (session.RevokedAt is not null)
            return SessionStates.Revoked;

        if (!now.HasValue)
            now = DateTimeOffset.UtcNow;

        if (session.ExpiresAt.HasValue && session.ExpiresAt.Value < now)
            return SessionStates.Expired;

        return SessionStates.Active;
    }

    public static AuthTokenStates AuthorizeClientToken(
        this IpPinAuthSession session,
        string? authToken)
    {
        // Note this method doesn't check for expiry
        // It's only purpose is to validate the token's data itself

        if (string.IsNullOrWhiteSpace(authToken))
            return AuthTokenStates.None; // No token provided

        if (session.AuthToken is null)
            return AuthTokenStates.NotFound; // No token set in the session

        return session.AuthToken == authToken.Trim()
            ? AuthTokenStates.Matched
            : AuthTokenStates.Mismatched; // Stale or possibly malicious token (includes malformed)
    }

    public static bool IsIpActive(
        this IpPinAuthSession session,
        DateTimeOffset? now = null)
    {
        if (!now.HasValue)
            now = DateTimeOffset.UtcNow;
        return session.ActivatedIpAt.HasValue &&
            session.DeactivatesIpAt.HasValue &&
            session.DeactivatesIpAt > now;
    }

    /// <summary>
    /// Generate a cryptographically randomized auth token
    /// </summary>
    public static string GenerateAuthToken(int sizeInBytes = 32)
    {
        byte[] tokenBytes = new byte[sizeInBytes];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }

        string base64 = Convert.ToBase64String(tokenBytes);
        return base64
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}