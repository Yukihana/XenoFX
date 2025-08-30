using CSX.DotNet.Common.EFC.Abstractions;
using System;

namespace CSX.DotNet.Common.EFC.Services;

public class UtcClockService : IClock
{
    public DateTimeOffset Now
        => DateTimeOffset.UtcNow;
}