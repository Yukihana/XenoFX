using System;

namespace XenoFx.Services.Auth.IpPinAuth.Models;

public class IpPinAuthClientInfo
{
    public string? CurrentIP { get; set; } = null;
    public bool IsAuthorized { get; set; } = false;

    // If authorized

    public DateTimeOffset TokenExpiresAt { get; set; } = DateTimeOffset.MinValue;
    public string? ActiveIP { get; set; } = null;

    // If active IP is set

    public DateTimeOffset? LeaseStartTime { get; set; } = null;
    public DateTimeOffset? LeaseEndTime { get; set; } = null;
}