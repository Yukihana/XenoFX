namespace XenoFx.Services.Auth.IpPinAuth.Models;

public enum AuthTokenStates
{
    None,           // No token provided
    NotFound,       // No token issued for the session
    Matched,        // Token is valid
    Mismatched,     // Token is stale or possibly malicious
}