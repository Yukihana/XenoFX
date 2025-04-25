using CSX.Common.Data.Events;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetQueue.Models;

namespace XenoFx.Services.Background.AssetQueue;

public partial class AssetQueueService
{
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
            var eventArgs = new FileSystemEventArgs(
                WatcherChangeTypes.All,
                Path.GetDirectoryName(file)!,
                Path.GetFileName(file)!);

            var context = new AssetResyncEventContext(eventArgs);
            await EnqueueAsync(context, ctoken);
        }
    }

    // Incoming : File system events

    public async Task OnFileCreatedAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetCreatedEventContext(
            eventArgs: eventArgs);
        await EnqueueAsync(context, ctoken);
    }

    public async Task OnFileDeletedAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetDeletedEventContext(
            eventArgs: eventArgs);
        await EnqueueAsync(context, ctoken);
    }

    public async Task OnFileModifiedAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetModifiedEventContext(
            eventArgs: eventArgs);
        await EnqueueAsync(context, ctoken);
    }

    public async Task OnFileRenamedAsync(
        RenamedEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetRenamedEventContext(
            eventArgs: eventArgs);
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
        FileUploadedEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetUploadedEventContext(
            eventArgs: eventArgs);
        await EnqueueAsync(context, ctoken);
    }
}