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

            if (!File.Exists(eventArgs.FullPath))
            {
                _logger.LogWarning("Ignoring non-existent file: {path}", eventArgs.FullPath);
                return false; // File doesn't exist, no need to re-evaluate.
            }

            // Check if the file is still being written to by attempting to open it exclusively.
            using (FileStream fs = File.Open(eventArgs.FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                if (fs.Length == 0)
                {
                    _logger.LogWarning("File is empty. Possibly awaiting content: {path}", eventArgs.FullPath);
                    return true; // File is empty, awaiting content. Requeue.
                }
            } // Throws if the file is still being written to.

            await OnCreatedAsync(relativePath, ctoken);

            _logger.LogInformation("Indexed creation: {path}", relativePath);

            return false; // Re-evaluation not required.
        }
        catch (IOException ex)
        {
            bool exists = File.Exists(eventArgs.FullPath);
            if (exists)
                _logger.LogWarning(ex, "File possibly locked or in use. Requeuing: {path}", eventArgs.FullPath);
            else
                _logger.LogWarning(ex, "File not found, possibly moved. Ignoring: {path}", eventArgs.FullPath);

            return exists; // Re-evaluate if the file is still present.
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
        await LegacyCreateAsync(relativePath, ctoken);

        return false; // Re-evaluation not required.
    }
}