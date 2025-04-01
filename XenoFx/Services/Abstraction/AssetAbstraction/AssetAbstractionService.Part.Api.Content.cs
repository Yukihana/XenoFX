using CSX.Common.Platform;
using HeyRed.Mime;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.Cache.Models;
using XenoFx.Services.Abstraction.AssetAbstraction.DTOs;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService
{
    public const string ResourceNotFoundMessage = "Resource not found.";

    // API

    public async Task<string> GetContentPathAsync(string relativePath, CancellationToken ctoken = default)
    {
        if (!await IsValidAssetPathAsync(relativePath, ctoken))
            throw new InvalidOperationException(ResourceNotFoundMessage);

        string resourcePath = Path.Combine(
            _configurationService.AssetsDirectory,
            relativePath);

        return Path.GetFullPath(resourcePath);
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
        return await _assetPresence.ReadAsync(async (table, ct) =>
        {
            List<AssetPresenceInfo> copy = await table.AsNoTracking().ToListAsync(ctoken);
            return copy.Any(x => x.RelativePath.Equals(relativePath, FilenameNormalization.FilenameComparison));
        }, ctoken);
    }
}