using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public interface IAssetIndexingService
{
    // Enumerator

    Task<bool> IndexResyncEventAsync(
        string relativePath,
        CancellationToken ctoken = default);

    // File system watcher

    Task<bool> IndexCreateEventAsync(
        string relativePath,
        FileSystemEventArgs args,
        CancellationToken ctoken = default);

    Task<bool> IndexDeleteEventAsync(
        string relativePath,
        FileSystemEventArgs args,
        CancellationToken ctoken = default);

    Task<bool> IndexModifyEventAsync(
        string relativePath,
        FileSystemEventArgs args,
        CancellationToken ctoken = default);

    Task<bool> IndexRenameEventAsync(
        string oldRelativePath,
        string newRelativePath,
        RenamedEventArgs args,
        CancellationToken ctoken = default);

    // Uploads

    Task<bool> IndexUploadEventAsync(
        string relativePath,
        string reportedFilename,
        string title,
        string mimeType,
        string pageUrl,
        string dataUrl,
        CancellationToken ctoken = default);
}