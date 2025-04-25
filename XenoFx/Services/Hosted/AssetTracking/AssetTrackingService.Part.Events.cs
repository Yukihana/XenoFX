using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Hosted.AssetTracking;

public partial class AssetTrackingService
{
    // Event: Created

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        if (!_configuration.RuntimeContext.EnableAssetTracking)
            return;

        _taskBag.Add(Task.Run(async () => await OnCreatedAsync(e, _cts.Token)));
        RunPartialCleanup();
    }

    private async Task OnCreatedAsync(FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File created: {path}", eventId, eventArgs.FullPath);
            await _assetQueue.OnFileCreatedAsync(eventArgs, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file created: {path}", eventId, eventArgs.FullPath);
        }
    }

    // Event: Deleted

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        if (!_configuration.RuntimeContext.EnableAssetTracking)
            return;

        _taskBag.Add(Task.Run(async () => await OnDeletedAsync(e, _cts.Token)));
        RunPartialCleanup();
    }

    private async Task OnDeletedAsync(FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File deleted: {file}", eventId, eventArgs.FullPath);
            await _assetQueue.OnFileDeletedAsync(eventArgs, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file deleted: {path}", eventId, eventArgs.FullPath);
        }
    }

    // Event: Modified

    private void OnModified(object sender, FileSystemEventArgs e)
    {
        if (!_configuration.RuntimeContext.EnableAssetTracking)
            return;

        _taskBag.Add(Task.Run(async () => await OnModifiedAsync(e, _cts.Token)));
        RunPartialCleanup();
    }

    private async Task OnModifiedAsync(FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File modified: {file}", eventId, eventArgs.FullPath);
            await _assetQueue.OnFileModifiedAsync(eventArgs, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file modified: {path}", eventId, eventArgs.FullPath);
        }
    }

    // Event: Renamed

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        if (!_configuration.RuntimeContext.EnableAssetTracking)
            return;

        _taskBag.Add(Task.Run(async () => await OnRenamedAsync(e, _cts.Token)));
        RunPartialCleanup();
    }

    private async Task OnRenamedAsync(RenamedEventArgs eventArgs, CancellationToken ctoken = default)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File renamed: {file} from {old}", eventId, eventArgs.FullPath, eventArgs.OldFullPath);
            await _assetQueue.OnFileRenamedAsync(eventArgs, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On file renamed: {path}", eventId, eventArgs.FullPath);
        }
    }

    // Event: Error

    private void OnError(object sender, ErrorEventArgs e)
    {
        if (_configuration.RuntimeContext.EnableAssetTracking)
            _taskBag.Add(Task.Run(async () => await OnErrorAsync(e, _cts.Token)));

        RunPartialCleanup();
    }

    private async Task OnErrorAsync(ErrorEventArgs eventArgs, CancellationToken ctoken = default)
    {
        ulong eventId = GetNextId();
        try
        {
            _logger.LogInformation("[E{id}] File system error encountered: {msg}", eventId, eventArgs.GetException()?.Message);
            await _assetQueue.OnFileSystemErrorAsync(eventArgs, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[E{id} Error] On watcher error: {error}", eventId, eventArgs);
        }
    }
}