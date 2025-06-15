using CSX.Common.Data.Events;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public interface IAssetIndexingService
{
    // Enumerator

    Task<bool> IndexResyncEventAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default);

    // File system watcher

    Task<bool> IndexCreateEventAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default);

    Task<bool> IndexDeleteEventAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default);

    Task<bool> IndexModifyEventAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default);

    Task<bool> IndexRenameEventAsync(
        RenamedEventArgs eventArgs,
        CancellationToken ctoken = default);

    // Uploads

    Task<bool> IndexUploadEventAsync(
        FileUploadedEventArgs eventArgs,
        CancellationToken ctoken = default);

    // State (Move this to a state service; rework)

    ulong StateIndex { get; }
    DateTime LastModified { get; }
}