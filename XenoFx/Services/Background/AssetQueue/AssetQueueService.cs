using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using XenoFx.Services.Background.AssetIndexing;

namespace XenoFx.Services.Background.AssetQueue;

public sealed partial class AssetQueueService : IAssetQueueService
{
    // Infrastructure

    private readonly IAssetIndexingService _assetIndexing;
    private readonly ILogger<AssetQueueService> _logger;

    // Data

    public Func<string[]>? EnumerateCallback { get; set; } = null;
    private bool _requireResync = false;
    private readonly CancellationTokenSource _shutdownTokenSource = new();

    // Lifecycle

    public AssetQueueService(
        IAssetIndexingService assetIndexing,
        IHostApplicationLifetime lifetime,
        ILogger<AssetQueueService> logger)
    {
        _assetIndexing = assetIndexing;
        _logger = logger;

        lifetime.ApplicationStopping.Register(OnStopping);
    }

    private void OnStopping()
    {
        _logger.LogInformation("Shutting down. Stopping all background tasks.");
        _shutdownTokenSource.Cancel();
        _queueProcessing?.Wait();
        _queueProcessing = null;
    }
}