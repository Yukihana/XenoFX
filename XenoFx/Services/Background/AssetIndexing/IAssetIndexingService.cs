using CSX.Common.Data.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public interface IAssetIndexingService
{
    // Enumerator

    Task<int> IndexResyncEventAsync(
        List<string> fileList,
        CancellationToken ctoken = default);

    // File system watcher

    Task<bool> IndexCreateEventAsync(
        string fullPath,
        CancellationToken ctoken = default);

    Task<bool> IndexDeleteEventAsync(
        string fullPath,
        CancellationToken ctoken = default);

    Task<bool> IndexModifyEventAsync(
        string fullPath,
        CancellationToken ctoken = default);

    Task<bool> IndexRenameEventAsync(
        string fullPath,
        string oldFullPath,
        CancellationToken ctoken = default);

    // Uploads

    Task<bool> IndexUploadEventAsync(
        FileUploadedEventArgs eventArgs,
        CancellationToken ctoken = default);

    // State (Move this to a state service; rework)

    ulong StateIndex { get; }
    DateTime LastModified { get; }
}