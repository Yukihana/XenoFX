using System.IO;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public interface IAssetIndexingService
{
    Task OnFileCreated(FileSystemEventArgs e);

    Task OnFileDeleted(FileSystemEventArgs e);

    Task OnFileModified(FileSystemEventArgs e);

    Task OnFileRenamed(RenamedEventArgs e);

    Task OnFileSystemError(ErrorEventArgs e);
}