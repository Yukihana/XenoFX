using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public async Task<int> IndexResyncEventAsync(
        List<string> fileList,
        CancellationToken ctoken = default)
    {
        List<string> handled = [];
        foreach (var file in fileList)
        {
            try
            {
                ctoken.ThrowIfCancellationRequested();

                bool result
                    = File.Exists(file)
                    ? await IndexResyncAsync(file, ctoken)
                    : await IndexDeleteEventAsync(file, ctoken);

                if (!result)
                    handled.Add(file);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle resync event for file: {file}", file);
            }
        }

        // Remove handled files from the list, and return unhandled count
        fileList.RemoveAll(handled.Contains);
        return fileList.Count;
    }

    private async Task<bool> IndexResyncAsync(string file, CancellationToken ctoken = default)
    {
        // Validate the file path
        if (!_pathValidator.TryTruncateAssetPath(file, out string? relativePath))
            return false;

        // Create the asset
        await LegacyCreateAsync(relativePath, ctoken);
        _logger.LogInformation("Indexed resync: {path}", relativePath);

        return false; // Re-evaluation not required.
    }
}