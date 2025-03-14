using System;
using System.Threading;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService
{
    private ulong _stateIndex = 0;
    private DateTime _lastModified = DateTime.UtcNow;

    private void OnUpdated()
    {
        Interlocked.Increment(ref _stateIndex);
        _lastModified = DateTime.UtcNow;
    }

    public ulong StateIndex
        => Interlocked.Read(ref _stateIndex);

    public DateTime LastModified
        => _lastModified;
}