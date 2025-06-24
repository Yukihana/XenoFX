using System;

namespace XenoServe.Features.IpPinAuthApi.Payloads;

public class ClientInfoResponse
{
    // Show anyway

    public string? CurrentIp { get; set; } = null;
    public bool IsAuthorized { get; set; } = false;

    // If authorized

    public DateTimeOffset TokenExpiresAt { get; set; } = DateTimeOffset.MinValue;
    public string? ActiveIp { get; set; } = null;

    // If active IP is set

    public DateTimeOffset? LeaseStartTime { get; set; } = null;
    public DateTimeOffset? LeaseEndTime { get; set; } = null;
}