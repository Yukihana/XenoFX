using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetIndexing;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Hosted.AssetTracking;

public sealed partial class AssetTrackingService : IAssetTrackingService
{
    // Infrastructure

    private readonly IAssetIndexingService _assetIndexing;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetTrackingService> _logger;

    // Resources

    private FileSystemWatcher? _watcher;
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
        IAssetIndexingService assetIndexing,
        IConfigurationService configuration,
        ILogger<AssetTrackingService> logger)
    {
        _assetIndexing = assetIndexing;
        _configuration = configuration;
        _logger = logger;
    }

    public void Dispose()
    {
        _lock.Wait();
        try
        {
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

    // Internal

    private async Task<bool> IsRaisingEventsAllowed(CancellationToken ctoken = default)
    {
        bool currentState = _configuration.RuntimeContext.EnableAssetTracking;

        await _lock.WaitAsync(ctoken);
        try
        {
            if (_watcher is not null)
                _watcher.EnableRaisingEvents = currentState;
        }
        finally { _lock.Release(); }

        return currentState;
    }

    private ulong GetNextId()
    {
        unchecked
        {
            return Interlocked.Increment(ref _eventId);
        }
    }

    // Updates forwarding

    private async void OnCreated(object sender, FileSystemEventArgs e)
    {
        if (!await IsRaisingEventsAllowed())
            return;

        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File created: {path}", eventId, e.FullPath);
            await _assetIndexing.OnFileCreated(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file created: {path}", eventId, e.FullPath);
        }
    }

    private async void OnDeleted(object sender, FileSystemEventArgs e)
    {
        if (!await IsRaisingEventsAllowed())
            return;

        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File deleted: {file}", eventId, e.FullPath);
            await _assetIndexing.OnFileDeleted(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file deleted: {path}", eventId, e.FullPath);
        }
    }

    private async void OnRenamed(object sender, RenamedEventArgs e)
    {
        if (!await IsRaisingEventsAllowed())
            return;

        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File renamed: {file} from {old}", eventId, e.FullPath, e.OldFullPath);
            await _assetIndexing.OnFileRenamed(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file renamed: {path}", eventId, e.FullPath);
        }
    }

    private async void OnModified(object sender, FileSystemEventArgs e)
    {
        if (!await IsRaisingEventsAllowed())
            return;

        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File modified: {file}", eventId, e.FullPath);
            await _assetIndexing.OnFileModified(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file modified: {path}", eventId, e.FullPath);
        }
    }

    private async void OnError(object sender, ErrorEventArgs e)
    {
        if (!await IsRaisingEventsAllowed())
            return;

        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File system error encountered: {msg}", eventId, e.GetException()?.Message);
            await _assetIndexing.OnFileSystemError(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On watcher error.", eventId);
        }
    }
}