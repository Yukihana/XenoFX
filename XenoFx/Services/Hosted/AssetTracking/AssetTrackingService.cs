using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetIndexing;
using XenoFx.Services.Background.AssetQueue;
using XenoFx.Services.Utility.Configuration;
using XenoFx.Services.Utility.PathValidator;

namespace XenoFx.Services.Hosted.AssetTracking;

public sealed partial class AssetTrackingService : IAssetTrackingService
{
    // Infrastructure

    private readonly IAssetQueueService _assetQueue;
    private readonly IPathValidatorService _pathValidator;
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
        IPathValidatorService pathValidator,
        IConfigurationService configuration,
        ILogger<AssetTrackingService> logger)
    {
        _assetQueue = assetQueue;
        _pathValidator = pathValidator;
        _configuration = configuration;
        _configuration.RuntimeContext.AssetTrackingConfigurationUpdatedCallback = OnConfigurationUpdated;
        _logger = logger;
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

    // FileSystemWatcher Events
    // - Check config anyway incase update-sync is late.
    // - Discard and re-route based on path filtering.
    // - Register and hand the task off to its own thread.
    // - Then run a contiguous partial cleanup for completed tasks.

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        if (!_configuration.RuntimeContext.EnableAssetTracking ||
            !_pathValidator.TryTruncateAssetPath(e.FullPath, out string? path))
            return;

        _taskBag.Add(Task.Run(async () => await OnCreatedAsync(path, e, _cts.Token)));
        RunPartialCleanup();
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        if (!_configuration.RuntimeContext.EnableAssetTracking ||
            !_pathValidator.TryTruncateAssetPath(e.FullPath, out string? path))
            return;

        _taskBag.Add(Task.Run(async () => await OnDeletedAsync(path, e, _cts.Token)));
        RunPartialCleanup();
    }

    private void OnModified(object sender, FileSystemEventArgs e)
    {
        if (!_configuration.RuntimeContext.EnableAssetTracking ||
            !_pathValidator.TryTruncateAssetPath(e.FullPath, out string? path))
            return;

        _taskBag.Add(Task.Run(async () => await OnModifiedAsync(path, e, _cts.Token)));
        RunPartialCleanup();
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        if (!_configuration.RuntimeContext.EnableAssetTracking)
            return;

        bool oldValid = _pathValidator.TryTruncateAssetPath(e.OldFullPath, out string? oldPath);
        bool newValid = _pathValidator.TryTruncateAssetPath(e.FullPath, out string? newPath);

        if (oldValid && oldPath is not null && newValid && newPath is not null)
            _taskBag.Add(Task.Run(async () => await OnRenamedAsync(oldPath!, newPath!, e, _cts.Token)));
        else if (oldValid)
            _taskBag.Add(Task.Run(async () => await OnDeletedAsync(oldPath!, e, _cts.Token)));
        else if (newValid)
            _taskBag.Add(Task.Run(async () => await OnCreatedAsync(newPath!, e, _cts.Token)));
        else
            return;

        RunPartialCleanup();
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        if (_configuration.RuntimeContext.EnableAssetTracking)
            _taskBag.Add(Task.Run(async () => await OnErrorAsync(e, _cts.Token)));

        RunPartialCleanup();
    }

    // FileSystemWatcher initiated Tasks

    private async Task OnCreatedAsync(string relativePath, FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File created: {path}", eventId, eventArgs.FullPath);
            await _assetQueue.OnFileCreatedAsync(relativePath, eventArgs, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file created: {path}", eventId, eventArgs.FullPath);
        }
    }

    private async Task OnDeletedAsync(string relativePath, FileSystemEventArgs e, CancellationToken ctoken = default)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File deleted: {file}", eventId, e.FullPath);
            await _assetQueue.OnFileDeletedAsync(relativePath, e, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file deleted: {path}", eventId, e.FullPath);
        }
    }

    private async Task OnModifiedAsync(string relativePath, FileSystemEventArgs e, CancellationToken ctoken = default)
    {
        ulong eventId = 0;
        try
        {
            eventId = GetNextId();
            _logger.LogInformation("[E{id}] File modified: {file}", eventId, e.FullPath);
            await _assetQueue.OnFileModifiedAsync(relativePath, e, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file modified: {path}", eventId, e.FullPath);
        }
    }

    private async Task OnRenamedAsync(string oldPath, string newPath, RenamedEventArgs e, CancellationToken ctoken = default)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File renamed: {file} from {old}", eventId, e.FullPath, e.OldFullPath);
            await _assetQueue.OnFileRenamedAsync(oldPath, newPath, e, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file renamed: {path}", eventId, e.FullPath);
        }
    }

    private async Task OnErrorAsync(ErrorEventArgs e, CancellationToken ctoken = default)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File system error encountered: {msg}", eventId, e.GetException()?.Message);
            await _assetQueue.OnFileSystemErrorAsync(e, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On watcher error.", eventId);
        }
    }
}