using CSX.DotNet.Common.Data.Events;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetQueue.Models;

namespace XenoFx.Services.Background.AssetQueue;

public partial class AssetQueueService
{
    // Incoming : Resync

    public async Task OnFileResyncingAsync(
        FileListingEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetResyncEventContext(
            fileList: [.. eventArgs.FilePaths]);
        await EnqueueAsync(context, ctoken);
    }

    // Incoming : File system events

    public async Task OnFileCreatedAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetCreatedEventContext(
            fullPath: eventArgs.FullPath);
        await EnqueueAsync(context, ctoken);
    }

    public async Task OnFileDeletedAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetDeletedEventContext(
            fullPath: eventArgs.FullPath);
        await EnqueueAsync(context, ctoken);
    }

    public async Task OnFileModifiedAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetModifiedEventContext(
            fullPath: eventArgs.FullPath);
        await EnqueueAsync(context, ctoken);
    }

    public async Task OnFileRenamedAsync(
        RenamedEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        var context = new AssetRenamedEventContext(
            fullPath: eventArgs.FullPath,
            oldFullPath: eventArgs.OldFullPath);
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

    // Ingress : Upload

    public async Task OnFileUploadedAsync(
        string uploadMetadataPath,
        Func<string, CancellationToken, Task>? cleanupCallback,
        CancellationToken ctoken = default)
    {
        var context = new AssetUploadedEventContext(
            uploadMetadataPath: uploadMetadataPath,
            cleanupCallback: cleanupCallback);
        await EnqueueAsync(context, ctoken);
    }
}