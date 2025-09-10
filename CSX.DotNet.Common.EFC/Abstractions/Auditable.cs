using System;

namespace CSX.DotNet.Common.EFC.Abstractions;

public abstract class Auditable : IAuditable
{
    // Creation

    public DateTimeOffset RecordCreatedAt { get; set; } = DateTimeOffset.MinValue;
    public string RecordCreatedBy { get; set; } = string.Empty;

    // Updates

    public DateTimeOffset? RecordUpdatedAt { get; set; } = null;
    public string? RecordUpdatedBy { get; set; } = null;

    // Soft-deletion

    public bool IsRecordDeleted { get; set; } = false;
    public DateTimeOffset? RecordDeletedAt { get; set; } = null;
    public string? RecordDeletedBy { get; set; } = null;
}