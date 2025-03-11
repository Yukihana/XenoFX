using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public interface IAssetIndexingService
{
    // Hosted source : AssetEnumeration

    Task OnFilesEnumeratedAsync(string[] files, CancellationToken ctoken = default);

    // Hosted source : AssetTracking

    Task OnFileCreatedAsync(FileSystemEventArgs e, CancellationToken ctoken = default);

    Task OnFileDeletedAsync(FileSystemEventArgs e, CancellationToken ctoken = default);

    Task OnFileModifiedAsync(FileSystemEventArgs e, CancellationToken ctoken = default);

    Task OnFileRenamedAsync(RenamedEventArgs e, CancellationToken ctoken = default);

    Task OnFileSystemErrorAsync(ErrorEventArgs e, CancellationToken ctoken = default);
}