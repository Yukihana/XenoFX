using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public interface IAssetIndexingService
{
    // Hosted source : AssetEnumeration

    Task OnFilesEnumeratedAsync(string[] files, CancellationToken ctoken = default);

    // Hosted source : AssetTracking

    Task OnFileCreatedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileDeletedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileModifiedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default);

    Task OnFileRenamedAsync(string oldPath, string newPath, RenamedEventArgs e, CancellationToken ctoken = default);

    Task OnFileSystemErrorAsync(ErrorEventArgs e, CancellationToken ctoken = default);
}