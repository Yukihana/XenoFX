using System;

namespace CSX.DotNet.EFC.Common.Abstractions;

public interface IClock
{
    DateTimeOffset Now { get; }
}