using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public async Task<bool> IndexCreateEventAsync(
        FileSystemEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        try
        {
            if (!_pathValidator.TryTruncateAssetPath(eventArgs.FullPath, out string? relativePath))
                return false;

            using (FileStream fs = File.Open(eventArgs.FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            { } // Throws if the file is still being written to.

            await OnCreatedAsync(relativePath, ctoken);

            return false; // Re-evaluation not required.
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "File possibly locked or in use. Requeuing: {path}", eventArgs.FullPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Indexing the creation event failed for: {path}", eventArgs.FullPath);
            // Requeue unhandled exceptions for more log visibility, so it can be fixed. Maybe have an unhandled cases service to keep track.
            return true;
        }
    }

    private async Task<bool> OnCreatedAsync(
        string relativePath,
        CancellationToken ctoken = default)
    {
        await _assetAbstraction.CreateAsync(relativePath, ctoken);

        return false; // Re-evaluation not required.
    }
}