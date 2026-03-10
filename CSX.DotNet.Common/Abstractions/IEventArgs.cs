using System;

namespace CSX.DotNet.Common.Abstractions;

public interface IEventArgs
{
    DateTimeOffset TimeStamp { get; }
}