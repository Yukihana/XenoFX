using System;

namespace CSX.DotNet.Common.EFC.Abstractions;

public interface IAuditable
{
    // Creation

    DateTimeOffset RecordCreatedAt { get; set; }
    string RecordCreatedBy { get; set; }

    // Updates

    DateTimeOffset? RecordUpdatedAt { get; set; }
    string? RecordUpdatedBy { get; set; }

    // Soft-Deletion

    bool IsRecordDeleted { get; set; }
    DateTimeOffset? RecordDeletedAt { get; set; }
    string? RecordDeletedBy { get; set; }
}