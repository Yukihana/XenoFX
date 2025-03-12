using Microsoft.Extensions.Logging;
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

    public Task OnFilesEnumeratedAsync(string[] files, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Legacy Code
        _assetPresence.TotalRefresh(files);

        return Task.CompletedTask;
    }

    public Task OnFileCreatedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        _assetPresence.Create(path);
        return Task.CompletedTask;
    }

    public Task OnFileDeletedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        _assetPresence.Remove(path);
        return Task.CompletedTask;
    }

    public Task OnFileModifiedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        return Task.CompletedTask;
    }

    public Task OnFileRenamedAsync(string oldPath, string newPath, RenamedEventArgs e, CancellationToken ctoken = default)
    {
        _assetPresence.Remove(oldPath);
        _assetPresence.Create(newPath);
        return Task.CompletedTask;
    }

    public Task OnFileSystemErrorAsync(ErrorEventArgs e, CancellationToken ctoken = default)
    {
        // If callback is registered, request an update on the files, then re-register them.
        if (EnumerateCallback is not null)
            return OnFilesEnumeratedAsync(EnumerateCallback(), ctoken);

        return Task.CompletedTask;
    }
}