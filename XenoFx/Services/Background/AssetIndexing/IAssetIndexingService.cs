using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public interface IAssetIndexingService
{
    // Hosted source : AssetEnumeration

    Task OnFilesEnumerated(string[] files, CancellationToken ctoken);

    // Hosted source : AssetTracking

    Task OnFileCreated(FileSystemEventArgs e);

    Task OnFileDeleted(FileSystemEventArgs e);

    Task OnFileModified(FileSystemEventArgs e);

    Task OnFileRenamed(RenamedEventArgs e);

    Task OnFileSystemError(ErrorEventArgs e);
}