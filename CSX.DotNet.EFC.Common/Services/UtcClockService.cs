using CSX.DotNet.EFC.Common.Abstractions;
using System;

namespace CSX.DotNet.EFC.Common.Services;

public class UtcClockService : IClock
{
    public DateTimeOffset Now
        => DateTimeOffset.UtcNow;
}