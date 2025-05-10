using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public async Task<bool> IndexRenameEventAsync(
        RenamedEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        try
        {
            bool oldValid = _pathValidator.TryTruncateAssetPath(eventArgs.OldFullPath, out string? oldRelativePath);
            bool newValid = _pathValidator.TryTruncateAssetPath(eventArgs.FullPath, out string? newRelativePath);

            var result = false;

            if (oldValid && newValid)
                result = await OnRenamedAsync(oldRelativePath!, newRelativePath!, ctoken);
            else if (oldValid)
                result = await OnDeletedAsync(oldRelativePath!, ctoken);
            else if (newValid)
                result = await OnCreatedAsync(newRelativePath!, ctoken);

            if (!result)
                _logger.LogInformation("Indexed rename: {path}", newRelativePath);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Indexing the rename event failed for: {eventArgs}", eventArgs);   // Pass the composite args.
            // Requeue unhandled exceptions for more log visibility, so it can be fixed. Maybe have an unhandled cases service to keep track.
            return true;
        }
    }

    private async Task<bool> OnRenamedAsync(string oldRelativePath, string newRelativePath, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Legacy start
        await _assetAbstraction.RemoveAsync(oldRelativePath, ctoken);
        await _assetAbstraction.CreateAsync(newRelativePath, ctoken);
        // Legacy end
        return false;
    }
}