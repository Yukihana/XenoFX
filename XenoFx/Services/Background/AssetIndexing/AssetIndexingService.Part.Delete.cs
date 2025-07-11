using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public async Task<bool> IndexDeleteEventAsync(
        string fullPath,
        CancellationToken ctoken = default)
    {
        try
        {
            if (!_pathValidator.TryTruncateAssetPath(fullPath, out string? relativePath))
                return false;

            var result = await OnDeletedAsync(relativePath, ctoken);

            if (!result)
                _logger.LogInformation("Indexed deletion: {path}", relativePath);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Indexing the deletion event failed for: {path}", fullPath);
            // Requeue unhandled exceptions for more log visibility, so it can be fixed. Maybe have an unhandled cases service to keep track.
            return true;
        }
    }

    private async Task<bool> OnDeletedAsync(
        string relativePath,
        CancellationToken ctoken = default)
    {
        await LegacyRemoveAsync(relativePath, ctoken);

        return false;
    }
}