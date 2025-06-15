using HeyRed.Mime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction.DTOs;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService
{
    public const string ResourceNotFoundMessage = "Resource not found.";

    // API

    public async Task<string> GetContentPathAsync(string relativePath, CancellationToken ctoken = default)
    {
        // Validate as relative path; should not be rooted or absolute (otherwise can be exploited to access arbitrary files on the server)
        if (string.IsNullOrWhiteSpace(relativePath) || Path.IsPathRooted(relativePath) || Path.IsPathFullyQualified(relativePath))
            throw new ArgumentException("The provided path must be a relative path.", nameof(relativePath));

        // Check if the path falls within parameters
        if (!await IsValidAssetPathAsync(relativePath, ctoken))
            throw new InvalidOperationException(ResourceNotFoundMessage);

        // Build the full path of the resource
        string resourcePath = Path.Combine(
            _configurationService.AssetsDirectory,
            relativePath);
        resourcePath = Path.GetFullPath(resourcePath);

        // Notify and throw if file doesn't exist
        if (!File.Exists(resourcePath))
        {
            FileSystemEventArgs fileDeletedEventArgs = new(
                WatcherChangeTypes.Deleted,
                Path.GetDirectoryName(resourcePath) ?? string.Empty,
                Path.GetFileName(resourcePath));

            _logger.LogWarning("Resource not found at path: {resourcePath}. Notifying the queue service.", resourcePath);
            await _assetQueue.OnFileDeletedAsync(fileDeletedEventArgs, ctoken);
            throw new InvalidOperationException(ResourceNotFoundMessage);
        }

        // Return the full path of the resource
        return resourcePath;
    }

    public async Task<AssetViewInfo> GetAssetDownloadInfoAsync(string relativePath, CancellationToken ctoken = default)
    {
        if (!await IsValidAssetPathAsync(relativePath, ctoken))
            throw new InvalidOperationException(ResourceNotFoundMessage);

        // TODO do not use relative path once AssetDb is implemented.
        return new AssetViewInfo()
        {
            Title = Path.GetFileNameWithoutExtension(relativePath),                 // TODO: replace from descriptions table
            AssetId = relativePath,                                                 // TODO: Retrieve the asset ID from the presence table (viable until we dont allow path viewing anymore)
            MimeType = MimeTypesMap.GetMimeType(Path.GetFileName(relativePath)),     // TODO: Retrieve the mime type from the media table
        };

        /* TODO after AssetDb is implemented.
        // - Check if a presence is registered for this path
        UInt128[] matches = await _assetPresence.ReadAsync(async (table, ct) =>
        {
            List<AssetPresenceInfo> copy = await table.AsNoTracking().ToListAsync(ctoken);
            return copy
                .Where(x => x.RelativePath.Equals(relativePath, FilenameNormalization.FilenameComparison))
                .Select(x => x.AssetId)
                .ToArray();
        }, ctoken);

        // Throw if not found, else get the ID of the first match.
        if (matches.Length < 1)
            throw new InvalidOperationException(ResourceNotFoundMessage);
        UInt128 id = matches[0];

        // Retrieve metadata from the asset table.

        AssetDownloadMetadata result = new();
        return new AssetDownloadMetadata
        {
            Path = await GetContentPathAsync(relativePath, ctoken),
            // AssetId = // TODO
            Title = Path.GetFileName(relativePath),
        };

        // - Return ID. Not path. Path is only for GetContentPath.
        */
    }

    // Internal

    private async Task<bool> IsValidAssetPathAsync(string relativePath, CancellationToken ctoken = default)
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