using System.Net;

namespace XenoFx.Services.Auth.IpPinAuth.Models;

public class IpPinAuthDto
{
    // Query

    public IPAddress? IPAddress { get; set; } = null;
    public string? ClientId { get; set; } = null;
    public string? AuthToken { get; set; } = null;

    // Service

    public SessionStates SessionState { get; set; } = SessionStates.Available;
    public bool IsAuthorized { get; set; } = false;

    // Result

    public string? NewClientId { get; set; } = null;
    public bool TokenExpired { get; set; } = false;
}

public class IpPinAuthDto<TQuery, TResult> : IpPinAuthDto
{
    // Payloads

    public TQuery RequestPayload { get; set; } = default!;
    public TResult ResponsePayload { get; set; } = default!;
}