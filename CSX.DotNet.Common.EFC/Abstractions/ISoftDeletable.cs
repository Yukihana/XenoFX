using System;

namespace CSX.DotNet.Common.EFC.Abstractions;

public interface ISoftDeletable
{
    bool IsRecordDeleted { get; set; }
    DateTimeOffset? RecordDeletedAt { get; set; }
}