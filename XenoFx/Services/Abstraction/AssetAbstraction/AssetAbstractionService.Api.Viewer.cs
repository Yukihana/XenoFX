using HeyRed.Mime;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction.DTOs;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService
{
    // API

    public async Task<AssetViewerInfo> GetAssetViewerInfoAsync(string id, CancellationToken ctoken = default)
    {
        string relativePath = await GetFirstMatchingAssetPathAsync(id, ctoken);
        string extension = Path.GetExtension(relativePath).ToLowerInvariant();
        return new AssetViewerInfo()
        {
            // TODO: replace from descriptions table
            Title = Path.GetFileNameWithoutExtension(relativePath),
            // TODO: Placeholder relative path; Retrieve the asset ID from the presence table instead
            AssetId = relativePath,
            // TODO: Retrieve the mime type from the media table
            MimeType = MimeTypesMap.GetMimeType(extension),
            // Normalized extension required for video source verification parameter
            Extension = extension.TrimStart('.'),
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
}