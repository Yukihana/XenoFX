using System;
using XenoFx.Services.Abstraction.AssetAbstraction;

namespace XenoFx.Services.Api.StateMonitor;

public sealed partial class StateMonitorService : IStateMonitorService
{
    private readonly IAssetAbstractionService _assetAbstraction;

    public StateMonitorService(IAssetAbstractionService assetAbstraction)
    {
        _assetAbstraction = assetAbstraction;
    }

    public ulong GetAssetRepositoryStateIndex()
        => _assetAbstraction.StateIndex;

    public DateTime GetAssetRepositoryLastModified()
        => _assetAbstraction.LastModified;
}