using System;

namespace XenoFx.Database.AuthDb.Models;

public class IpPinAuthSession
{
    public int Id { get; set; }

    // Authorization

    public string ClientId { get; set; } = string.Empty;
    public string? AuthToken { get; set; } = null;

    public DateTimeOffset? AuthenticatedAt { get; set; } = null;
    public DateTimeOffset? ExpiresAt { get; set; } = null;
    public DateTimeOffset? RevokedAt { get; set; } = null;

    // Verification

    public int? Pin { get; set; } = null;
    public DateTimeOffset? PinGenerationDisabledUntil { get; set; } = null;

    public int RetryCount { get; set; } = 0; // Reject past configured max count
    public DateTimeOffset? RetryDisabledUntil { get; set; } = null; // Adaptive increment

    // Ip

    public string? IpAddress { get; set; } = null;
    public DateTimeOffset? ActivatedIpAt { get; set; } = null;
    public DateTimeOffset? DeactivatesIpAt { get; set; } = null;

    // Adminstrative and Metadata

    public bool IsEnabled { get; set; } = true;                             // False to disable the client
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;  // Automatically set to current time
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.MinValue;
    public string Notes { get; set; } = string.Empty;

    // Factory

    public IpPinAuthSession Copy() => new()
    {
        Id = Id,

        ClientId = ClientId,
        AuthToken = AuthToken,

        AuthenticatedAt = AuthenticatedAt,
        ExpiresAt = ExpiresAt,
        RevokedAt = RevokedAt,

        Pin = Pin,
        PinGenerationDisabledUntil = PinGenerationDisabledUntil,

        RetryCount = RetryCount,
        RetryDisabledUntil = RetryDisabledUntil,

        IpAddress = IpAddress,
        ActivatedIpAt = ActivatedIpAt,
        DeactivatesIpAt = DeactivatesIpAt,

        IsEnabled = IsEnabled,
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt,
        Notes = Notes,
    };
}