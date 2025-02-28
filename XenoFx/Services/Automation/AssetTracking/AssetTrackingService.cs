using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using XenoFx.Services.Background.AssetIndexing;
using XenoFx.Services.Utility.Profile;
using XenoFx.Services.Utility.SharedOptions;

namespace XenoFx.Services.Automation.AssetTracking;

public sealed partial class AssetTrackingService : IAssetTrackingService, IDisposable
{
    // Infrastructure

    private readonly IAssetIndexingService _assetIndexingService;
    private readonly IProfileService _profileService;
    private readonly ISharedOptionsService _sharedOptionsService;
    private readonly IOptions<AssetTrackingOptions> _options;
    private readonly ILogger<AssetTrackingService> _logger;

    // Resources

    private readonly FileSystemWatcher _watcher;
    public const string AssetRoot = "D:\\Desktop\\Workset"; // "J:\\TTX\\Assets";
    private ulong _eventId = 0;

    // Lifetime

    public AssetTrackingService(
        IAssetIndexingService assetIndexingService,
        IProfileService profileService,
        ISharedOptionsService sharedOptionsService,
        IOptions<AssetTrackingOptions> options,
        ILogger<AssetTrackingService> logger)
    {
        _assetIndexingService = assetIndexingService;
        _profileService = profileService;
        _sharedOptionsService = sharedOptionsService;
        _options = options;
        _logger = logger;

        _watcher = new FileSystemWatcher(AssetRoot)
        {
            EnableRaisingEvents = true,
            IncludeSubdirectories = true
        };

        _watcher.Created += WatcherCreated;
        _watcher.Changed += WatcherChanged;
        _watcher.Renamed += WatcherRenamed;
        _watcher.Deleted += WatcherDeleted;
        _watcher.Error += WatcherError;
    }

    public void Dispose()
    {
        _watcher.Dispose();
    }

    // Internal

    private ulong GetNextId()
    {
        unchecked
        {
            return Interlocked.Increment(ref _eventId);
        }
    }

    // Updates forwarding

    private async void WatcherCreated(object sender, FileSystemEventArgs e)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File created: {path}", eventId, e.FullPath);
            await _assetIndexingService.OnFileCreated(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file created: {path}", eventId, e.FullPath);
        }
    }

    private async void WatcherDeleted(object sender, FileSystemEventArgs e)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File deleted: {file}", eventId, e.FullPath);
            await _assetIndexingService.OnFileDeleted(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file deleted: {path}", eventId, e.FullPath);
        }
    }

    private async void WatcherRenamed(object sender, RenamedEventArgs e)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File renamed: {file} from {old}", eventId, e.FullPath, e.OldFullPath);
            await _assetIndexingService.OnFileRenamed(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file renamed: {path}", eventId, e.FullPath);
        }
    }

    private async void WatcherChanged(object sender, FileSystemEventArgs e)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File modified: {file}", eventId, e.FullPath);
            await _assetIndexingService.OnFileModified(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file modified: {path}", eventId, e.FullPath);
        }
    }

    private async void WatcherError(object sender, ErrorEventArgs e)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File system error encountered: {msg}", eventId, e.GetException()?.Message);
            await _assetIndexingService.OnFileSystemError(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On watcher error.", eventId);
        }
    }
}