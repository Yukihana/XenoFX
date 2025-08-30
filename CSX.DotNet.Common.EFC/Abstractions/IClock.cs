using System;

namespace CSX.DotNet.Common.EFC.Abstractions;

public interface IClock
{
    DateTimeOffset Now { get; }
}