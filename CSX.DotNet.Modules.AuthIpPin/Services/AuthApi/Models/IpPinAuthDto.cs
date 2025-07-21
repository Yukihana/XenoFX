using System.Net;

namespace CSX.DotNet.Modules.AuthIpPin.Services.AuthApi.Models;

public class IpPinAuthDto
{
    public IpPinAuthDto(IPAddress ip)
        => IPAddress = ip;

    // Query

    public IPAddress IPAddress { get; }
    public string? ClientId { get; init; } = null;
    public string? AuthToken { get; init; } = null;

    // Service

    public SessionStates SessionState { get; set; } = SessionStates.Available;
    public bool IsAuthorized { get; set; } = false;

    // Result

    public string? NewClientId { get; set; } = null;
    public bool TokenExpired { get; set; } = false;
}

public class IpPinAuthDto<TQuery, TResult> : IpPinAuthDto
{
    public IpPinAuthDto(IPAddress ip) : base(ip)
    { }

    // Payloads

    public TQuery RequestPayload { get; set; } = default!;
    public TResult ResponsePayload { get; set; } = default!;
}