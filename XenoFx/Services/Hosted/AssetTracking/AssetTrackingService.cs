using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetQueue;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Hosted.AssetTracking;

public sealed partial class AssetTrackingService : IAssetTrackingService
{
    // Infrastructure

    private readonly IAssetQueueService _assetQueue;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetTrackingService> _logger;

    // Resources

    private FileSystemWatcher? _watcher;
    private readonly ConcurrentBag<Task> _taskBag = [];
    private readonly CancellationTokenSource _cts = new();
    private readonly SemaphoreSlim _lock = new(1);
    private ulong _eventId = 0;

    // Derived: Public

    public string WatchPath
        => _configuration.AssetsDirectory;

    public bool IsAssetTrackingEnabled
        => _configuration.RuntimeContext.EnableAssetTracking;

    // Lifetime

    public AssetTrackingService(
        IAssetQueueService assetQueue,
        IConfigurationService configuration,
        ILogger<AssetTrackingService> logger)
    {
        _assetQueue = assetQueue;
        _configuration = configuration;
        _logger = logger;

        _assetQueue.RescanCallback = StartResync;
        _configuration.RuntimeContext.AssetTrackingConfigurationUpdatedCallback = OnConfigurationUpdated;
    }

    public void Dispose()
    {
        _lock.Wait();
        try
        {
            _configuration.RuntimeContext.AssetTrackingConfigurationUpdatedCallback = null;
            _cts.Cancel();

            _watcher?.Dispose();
            _watcher = null;
        }
        finally { _lock.Release(); }
    }

    // IHostedService : Start/Stop

    public async Task StartAsync(CancellationToken ctoken = default)
    {
        await _lock.WaitAsync(ctoken);
        try
        {
            // Start watcher

            if (_watcher is not null)
                return;

            if (!Directory.Exists(WatchPath))
                Directory.CreateDirectory(WatchPath);

            _watcher = new FileSystemWatcher(WatchPath)
            {
                EnableRaisingEvents = IsAssetTrackingEnabled,
                IncludeSubdirectories = true
            };

            _watcher.Created += OnCreated;
            _watcher.Changed += OnModified;
            _watcher.Renamed += OnRenamed;
            _watcher.Deleted += OnDeleted;
            _watcher.Error += OnError;

            // Start resync

            StartResync();
        }
        finally { _lock.Release(); }
    }

    public async Task StopAsync(CancellationToken ctoken = default)
    {
        await _lock.WaitAsync(ctoken);
        try
        {
            if (_watcher is null)
                return;

            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _watcher = null;
        }
        finally { _lock.Release(); }
    }

    // Configuration observer hooks

    private void OnConfigurationUpdated()
    {
        _taskBag.Add(Task.Run(async () => await ApplyConfigUpdateAsync(_cts.Token), _cts.Token));
    }

    private async Task ApplyConfigUpdateAsync(CancellationToken ctoken = default)
    {
        await _lock.WaitAsync(ctoken);
        try
        {
            if (_watcher is not null)
                _watcher.EnableRaisingEvents = _configuration.RuntimeContext.EnableAssetTracking;
        }
        finally { _lock.Release(); }
    }

    // Cleanup

    private void RunPartialCleanup()
    {
        while (_taskBag.TryTake(out var task))
        {
            if (!task.IsCompleted)
            {
                _taskBag.Add(task);
                return;
            }
            else if (task.IsFaulted)
            {
                _logger.LogError(task.Exception, "Failed task found during contiguous cleanup.");
            }
        }
    }

    private ulong GetNextId()
        => Interlocked.Increment(ref _eventId);
}