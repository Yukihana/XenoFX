using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public async Task<bool> IndexModifyEventAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        if (!_pathValidator.TryTruncateAssetPath(eventArgs.FullPath, out string? relativePath))
            return false;

        return await OnModifiedAsync(relativePath, ctoken);
    }

    private Task<bool> OnModifiedAsync(string relativePath, CancellationToken ctoken)
    {
        _ = _pathValidator;
        _ = relativePath;
        return Task.FromResult(false); // Temporary bypass, re-evaluation not required.
    }
}