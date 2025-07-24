using CSX.Common.Data.Events;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetQueue;

public interface IAssetQueueService
{
    // Event hooks

    Action? RescanCallback { get; set; }

    // Hosted source : AssetEnumeration

    Task OnFileResyncingAsync(FileListingEventArgs eventArgs, CancellationToken ctoken = default);

    // Hosted source : AssetTracking

    Task OnFileCreatedAsync(FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileDeletedAsync(FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileModifiedAsync(FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileRenamedAsync(RenamedEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileSystemErrorAsync(ErrorEventArgs eventArgs, CancellationToken ctoken = default);

    // API : Ingress

    Task OnFileUploadedAsync(
        string uploadMetadataPath,
        Func<string, CancellationToken, Task>? cleanupCallback,
        CancellationToken ctoken = default);

    // Obsolete: (but keep code until new module is fully set up)
    // Task OnFileUploadedAsync(FileUploadedEventArgs eventArgs, CancellationToken ctoken = default);
}