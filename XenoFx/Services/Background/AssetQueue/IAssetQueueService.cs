using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using XenoFx.Services.Background.AssetQueue.Models;

namespace XenoFx.Services.Background.AssetQueue;

public interface IAssetQueueService
{
    // Event hooks

    Func<string[]>? EnumerateCallback { get; set; } // Replace with config -> file system error count, and check if assetQueue is empty.

    // Hosted source : AssetEnumeration

    Task OnFilesEnumeratedAsync(string[] files, CancellationToken ctoken = default);

    // Hosted source : AssetTracking

    Task OnFileCreatedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileDeletedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileModifiedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileRenamedAsync(string oldPath, string newPath, RenamedEventArgs e, CancellationToken ctoken = default);

    Task OnFileSystemErrorAsync(ErrorEventArgs e, CancellationToken ctoken = default);

    // API : AssetUpload

    Task OnFileUploadedAsync(AssetUploadedEventContext e, CancellationToken ctoken = default);
}