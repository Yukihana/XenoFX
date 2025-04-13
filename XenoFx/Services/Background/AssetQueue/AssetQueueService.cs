using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetIndexing;
using XenoFx.Services.Background.AssetQueue.Models;

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

    // Rescan (incoming + bounced): TODO, redo this to unify the logic. (Merge Enumerate and Watcher services)

    private async Task TryResyncAsync(CancellationToken ctoken = default)
    {
        if (EnumerateCallback is null)
        {
            _logger.LogWarning("Resync callback is not registered. Unable to proceed with resync.");
            return;
        }

        var files = EnumerateCallback.Invoke();
        await OnFilesEnumeratedAsync(files, ctoken);
    }

    public async Task OnFilesEnumeratedAsync(string[] files, CancellationToken ctoken = default)
    {
        foreach (var file in files)
        {
            var context = new AssetResyncEventContext(file);
            await EnqueueAsync(context, ctoken);
        }
    }

    // Incoming : File system events

    public async Task OnFileCreatedAsync(
        string path,
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetCreatedEventContext(
            relativePath: path,
            eventArgs: eventArgs);
        await EnqueueAsync(context, ctoken);
    }

    public async Task OnFileDeletedAsync(
        string path,
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetDeletedEventContext(
            relativePath: path,
            eventArgs: eventArgs);
        await EnqueueAsync(context, ctoken);
    }

    public async Task OnFileModifiedAsync(
        string path,
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetModifiedEventContext(
            relativePath: path,
            eventArgs: eventArgs);
        await EnqueueAsync(context, ctoken);
    }

    public async Task OnFileRenamedAsync(
        string oldPath,
        string newPath,
        RenamedEventArgs e,
        CancellationToken ctoken = default)
    {
        var context = new AssetRenamedEventContext(
            oldRelativePath: oldPath,
            newRelativePath: newPath,
            eventArgs: e);
        await EnqueueAsync(context, ctoken);
    }

    public Task OnFileSystemErrorAsync(
        ErrorEventArgs e,
        CancellationToken ctoken = default)
    {
        _logger.LogError(e.GetException(), "File system error occurred. Marking for deferred resync.");
        _requireResync = true;
        return Task.CompletedTask;
    }

    // Incoming : Upload

    public async Task OnFileUploadedAsync(
        AssetUploadedEventContext e,
        CancellationToken ctoken = default)
    {
        await EnqueueAsync(e, ctoken);
    }
}