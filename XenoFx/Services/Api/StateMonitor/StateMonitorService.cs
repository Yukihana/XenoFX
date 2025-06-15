using System;
using XenoFx.Services.Background.AssetIndexing;

namespace XenoFx.Services.Api.StateMonitor;

public sealed partial class StateMonitorService : IStateMonitorService
{
    private readonly IAssetIndexingService _assetIndexing;

    public StateMonitorService(IAssetIndexingService assetIndexing)
    {
        _assetIndexing = assetIndexing;
    }

    public ulong GetAssetRepositoryStateIndex()
        => _assetIndexing.StateIndex;

    public DateTime GetAssetRepositoryLastModified()
        => _assetIndexing.LastModified;
}