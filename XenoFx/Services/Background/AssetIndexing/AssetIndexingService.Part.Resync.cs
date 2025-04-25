using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public async Task<bool> IndexResyncEventAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        if (!_pathValidator.TryTruncateAssetPath(eventArgs.FullPath, out string? relativePath))
            return false;

        await _assetAbstraction.CreateAsync(relativePath, ctoken);

        return false; // Re-evaluation not required.
    }
}