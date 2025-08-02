using System;

namespace CSX.DotNet.EFC.Common.Abstractions;

public interface ISoftDeletable
{
    bool IsRecordDeleted { get; set; }
    DateTimeOffset? RecordDeletedAt { get; set; }
}