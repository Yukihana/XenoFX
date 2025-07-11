using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public async Task<bool> IndexCreateEventAsync(
        string fullPath,
        CancellationToken ctoken = default)
    {
        try
        {
            if (!_pathValidator.TryTruncateAssetPath(fullPath, out string? relativePath))
                return false;

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("Ignoring non-existent file: {path}", fullPath);
                return false; // File doesn't exist, no need to re-evaluate.
            }

            // Check if the file is still being written to by attempting to open it exclusively.
            using (FileStream fs = File.Open(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                if (fs.Length == 0)
                {
                    _logger.LogWarning("File is empty. Possibly awaiting content: {path}", fullPath);
                    return true; // File is empty, awaiting content. Requeue.
                }
            } // Throws if the file is still being written to.

            await OnCreatedAsync(relativePath, ctoken);

            _logger.LogInformation("Indexed creation: {path}", relativePath);

            return false; // Re-evaluation not required.
        }
        catch (IOException ex)
        {
            bool exists = File.Exists(fullPath);
            if (exists)
                _logger.LogWarning(ex, "File possibly locked or in use. Requeuing: {path}", fullPath);
            else
                _logger.LogWarning(ex, "File not found, possibly moved. Ignoring: {path}", fullPath);

            return exists; // Re-evaluate if the file is still present.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Indexing the creation event failed for: {path}", fullPath);
            // Requeue unhandled exceptions for more log visibility, so it can be fixed. Maybe have an unhandled cases service to keep track.
            return true;
        }
    }

    private async Task<bool> OnCreatedAsync(
        string relativePath,
        CancellationToken ctoken = default)
    {
        await LegacyCreateAsync(relativePath, ctoken);

        return false; // Re-evaluation not required.
    }
}