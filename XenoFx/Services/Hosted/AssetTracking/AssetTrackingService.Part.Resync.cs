using CSX.Common.Data.Events;
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

            // Prepare a list of all files in designated areas
            string[] paths = Directory.GetFiles(WatchPath, "*.*", SearchOption.AllDirectories);
            _logger.LogInformation("Queueing resync: Enumerated {count} files.", paths.Length);

            // Queue for resyncing
            FileListingEventArgs eventArgs = new(paths);
            await _assetQueue.OnFileResyncingAsync(eventArgs, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resyncing files failed.");
        }
    }
}