using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public sealed partial class AssetIndexingService
{
    public async Task<bool> IndexCreateEventAsync(
        string relativePath,
        FileSystemEventArgs args,
        CancellationToken ctoken = default)
    {
        await _assetAbstraction.CreateAsync(relativePath, ctoken);

        return false; // Re-evaluation not required.
    }

    public async Task<bool> IndexDeleteEventAsync(
        string relativePath,
        FileSystemEventArgs args,
        CancellationToken ctoken = default)
    {
        await _assetAbstraction.RemoveAsync(relativePath, ctoken);

        return false; // Re-evaluation not required.
    }

    public Task<bool> IndexModifyEventAsync(
        string relativePath,
        FileSystemEventArgs args,
        CancellationToken ctoken = default)
    {
        return Task.FromResult(false); // Temporary bypass, re-evaluation not required.
    }

    public async Task<bool> IndexRenameEventAsync(
        string oldRelativePath,
        string newRelativePath,
        RenamedEventArgs args,
        CancellationToken ctoken = default)
    {
        await _assetAbstraction.RemoveAsync(oldRelativePath, ctoken);
        await _assetAbstraction.CreateAsync(newRelativePath, ctoken);

        return false; // Re-evaluation not required.
    }

    public async Task<bool> IndexResyncEventAsync(
        string relativePath,
        CancellationToken ctoken = default)
    {
        await _assetAbstraction.CreateAsync(relativePath, ctoken);

        return false; // Re-evaluation not required.
    }

    public async Task<bool> IndexUploadEventAsync(
        string relativePath,
        string reportedFilename,
        string title,
        string mimeType,
        string pageUrl,
        string dataUrl,
        CancellationToken ctoken = default)
    {
        // Move asset to the correct location

        // Prepare asset info here: hash etc

        // Run registration

        // Legacy
        string fullPath = Path.Combine(_configuration.AssetsDirectory, relativePath);
        try
        {
            using (var stream = new FileStream(relativePath, FileMode.Open, FileAccess.Read, FileShare.None))
            { } // throws if file is locked.

            await _assetAbstraction.CreateAsync(relativePath, ctoken);
            return true; // Re-evaluation not required.
        }
        catch (IOException ex)
        {
            if (File.Exists(fullPath))
            {
                _logger.LogWarning(ex, "Re-evaluation required for upload. File likely locked or incomplete at: {path}", fullPath);
                return true; // retry
            }

            // File doesn't exist. No point re-evaluating.
            _logger.LogWarning(ex, "Upload indexing aborted. File no longer exists at: {path}", fullPath);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process upload at: {path}", fullPath);
            return true; // requires monitoring. Pushing to re-evaluation for now.
        }
    }
}