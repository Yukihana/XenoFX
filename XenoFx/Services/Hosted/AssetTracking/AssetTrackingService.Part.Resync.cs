using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Hosted.AssetTracking;

public partial class AssetTrackingService
{
    // Callback API

    private Task? _resyncTask = null;

    private void StartResync()
    {
        try
        {
            if (_resyncTask is not null && !_resyncTask.IsCompleted)
                return;

            _resyncTask = Task.Run(async () => await StartResyncingAllFilesAsync(_cts.Token));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start resync task.");
        }
    }

    private async Task StartResyncingAllFilesAsync(CancellationToken ctoken = default)
    {
        try
        {
            ctoken.ThrowIfCancellationRequested();

            if (!Directory.Exists(WatchPath))
                Directory.CreateDirectory(WatchPath);

            string[] paths = Directory.GetFiles(WatchPath, "*.*", SearchOption.AllDirectories);

            _logger.LogInformation("Enumerated {count} files. Pushing for indexing...", paths.Length);

            foreach (var path in paths)
            {
                FileSystemEventArgs eventArgs = new(
                    WatcherChangeTypes.All,
                    Path.GetDirectoryName(path)!,
                    Path.GetFileName(path)!);
                await _assetQueue.OnFileResyncingAsync(eventArgs, ctoken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resyncing files failed.");
        }
    }
}