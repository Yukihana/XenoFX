using CSX.Common.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction.DTOs;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public partial class AssetAbstractionService
{
    public async Task ValidateAssetAsync(string fullPath, CancellationToken ctoken = default)
    {
        // If the file exists, bail early
        if (File.Exists(fullPath))
            return;

        _logger.LogWarning("Resource not found at path: {fullPath}. Notifying the queue service.", fullPath);

        // Notify the queue service
        FileSystemEventArgs fileDeletedEventArgs = new(
                WatcherChangeTypes.Deleted,
                Path.GetDirectoryName(fullPath) ?? string.Empty,
                Path.GetFileName(fullPath));

        await _assetQueue.OnFileDeletedAsync(fileDeletedEventArgs, ctoken);

        // Throw
        throw new ResourceNotFoundException($"Resource not found at path: {fullPath}");
    }

    // Internal

    private async Task<bool> IsRegisteredAssetPathAsync(string relativePath, CancellationToken ctoken = default)
    {
        string normalizedPath = relativePath.ToLowerInvariant();
        return await _assetPresence.ReadAsync(async (table, ct) =>
        {
            return await table.AnyAsync(
                predicate: x => x.NormalizedPath == normalizedPath,
                cancellationToken: ct);
        }, ctoken);
    }
}