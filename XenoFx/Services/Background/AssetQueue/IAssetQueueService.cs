using CSX.Common.Data.Events;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetQueue;

public interface IAssetQueueService
{
    // Event hooks

    Func<string[]>? EnumerateCallback { get; set; } // Replace with config -> file system error count, and check if assetQueue is empty.

    // Hosted source : AssetEnumeration

    Task OnFilesEnumeratedAsync(string[] files, CancellationToken ctoken = default);

    // Hosted source : AssetTracking

    Task OnFileCreatedAsync(FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileDeletedAsync(FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileModifiedAsync(FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileRenamedAsync(RenamedEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileSystemErrorAsync(ErrorEventArgs eventArgs, CancellationToken ctoken = default);

    // API : AssetUpload

    Task OnFileUploadedAsync(FileUploadedEventArgs eventArgs, CancellationToken ctoken = default);
}