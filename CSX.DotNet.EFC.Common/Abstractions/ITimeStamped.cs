using System;

namespace CSX.DotNet.EFC.Common.Abstractions;

public interface ITimeStamped
{
    DateTimeOffset RecordCreatedAt { get; set; }
    DateTimeOffset RecordUpdatedAt { get; set; }
}