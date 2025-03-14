using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public sealed partial class AssetIndexingService
{
    private readonly ConcurrentBag<Task> _taskbag = [];

    public Func<string[]>? EnumerateCallback { get; set; } = null;

    // Incoming events

    public async Task OnFilesEnumeratedAsync(string[] files, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Legacy Code
        await _assetAbstraction.TotalRefreshAsync(files, ctoken);
    }

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
        await _assetAbstraction.RemoveAsync(oldPath);
        await _assetAbstraction.CreateAsync(newPath);
    }

    public async Task OnFileSystemErrorAsync(ErrorEventArgs e, CancellationToken ctoken = default)
    {
        // If callback is registered, request an update on the files, then re-register them.
        if (EnumerateCallback is not null)
            await OnFilesEnumeratedAsync(EnumerateCallback(), ctoken);
    }
}