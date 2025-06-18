using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public partial class AssetAbstractionService
{
    private string GetAssetFullPath(string relativePath)
    {
        // upgrade this to relative OR absolute; use a service method if already available
        string relativeToBasePath = Path.Combine(
            _configurationService.AssetsDirectory,
            relativePath);

        // Ensure the path is absolute
        return Path.GetFullPath(relativeToBasePath);
    }

    // Notify

    private async Task NotifyOnAssetMissingAsync(
        string fullPath,
        CancellationToken ctoken = default)
    {
        FileSystemEventArgs fileDeletedEventArgs = new(
                WatcherChangeTypes.Deleted,
                Path.GetDirectoryName(fullPath) ?? string.Empty,
                Path.GetFileName(fullPath));

        await _assetQueue.OnFileDeletedAsync(fileDeletedEventArgs, ctoken);
    }

    private Task NotifyOnAssetsMissingAsync(
        List<string> fullPaths,
        CancellationToken ctoken = default)
    {
        // Create a bulk event args type
        // Also unify the type for AssetQueue so that it is instead router within indexing and not in assetQueue.
        // AssetQueue should only have one method for processing queue items.

        throw new NotImplementedException();
    }
}