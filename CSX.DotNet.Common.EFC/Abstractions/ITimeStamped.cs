using System;

namespace CSX.DotNet.Common.EFC.Abstractions;

public interface ITimeStamped
{
    DateTimeOffset RecordCreatedAt { get; set; }
    DateTimeOffset RecordUpdatedAt { get; set; }
}