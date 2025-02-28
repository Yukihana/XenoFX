using System.IO;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public sealed partial class AssetIndexingService : IAssetIndexingService
{
    public async Task OnFileCreated(FileSystemEventArgs e)
    {
        await Task.Yield();
    }

    public async Task OnFileDeleted(FileSystemEventArgs e)
    {
        await Task.Yield();
    }

    public async Task OnFileModified(FileSystemEventArgs e)
    {
        await Task.Yield();
    }

    public async Task OnFileRenamed(RenamedEventArgs e)
    {
        await Task.Yield();
    }

    public async Task OnFileSystemError(ErrorEventArgs e)
    {
        await Task.Yield();
    }
}