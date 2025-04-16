using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public async Task<bool> IndexCreateEventAsync(
        string relativePath,
        FileSystemEventArgs args,
        CancellationToken ctoken = default)
    {
        await _assetAbstraction.CreateAsync(relativePath, ctoken);

        return false; // Re-evaluation not required.
    }
}