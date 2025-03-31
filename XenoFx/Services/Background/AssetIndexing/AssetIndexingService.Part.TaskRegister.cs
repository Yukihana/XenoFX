using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetIndexing.DTOs;

namespace XenoFx.Services.Background.AssetIndexing;

public sealed partial class AssetIndexingService
{
    private readonly ConcurrentBag<Task> _taskbag = [];

    public Func<string[]>? EnumerateCallback { get; set; } = null;

    // Incoming : Rescan

    public async Task OnFilesEnumeratedAsync(string[] files, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Legacy Code
        await _assetAbstraction.TotalRefreshAsync(files, ctoken);
    }

    // Incoming : File system events

    public async Task OnFileCreatedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        await _assetAbstraction.CreateAsync(path, ctoken);
    }

    public async Task OnFileDeletedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        await _assetAbstraction.RemoveAsync(path, ctoken);
    }

    public Task OnFileModifiedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        return Task.CompletedTask;
    }

    public async Task OnFileRenamedAsync(string oldPath, string newPath, RenamedEventArgs e, CancellationToken ctoken = default)
    {
        await _assetAbstraction.RemoveAsync(oldPath, ctoken);
        await _assetAbstraction.CreateAsync(newPath, ctoken);
    }

    public async Task OnFileSystemErrorAsync(ErrorEventArgs e, CancellationToken ctoken = default)
    {
        // If callback is registered, request an update on the files, then re-register them.
        if (EnumerateCallback is not null)
            await OnFilesEnumeratedAsync(EnumerateCallback(), ctoken);
    }

    // Incoming : Upload

    public async Task OnFileUploadedAsync(UploadedAssetIndexingInfo e, CancellationToken ctoken = default)
    {
        // Prepare asset info here: hash etc

        // Run registration

        // Legacy
        await _assetAbstraction.CreateAsync(e.RelativePath, ctoken);
    }
}