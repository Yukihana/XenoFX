using CSX.Common.Platform;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.Cache.Models;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService
{
    public const string ResourceNotFoundMessage = "Resource not found.";

    public async Task<string> GetContentPathAsync(string relativePath, CancellationToken ctoken = default)
    {
        if (!await IsValidAssetPathAsync(relativePath, ctoken))
            throw new InvalidOperationException(ResourceNotFoundMessage);

        string resourcePath = Path.Combine(
            _configurationService.AssetsDirectory,
            relativePath);

        return Path.GetFullPath(resourcePath);
    }

    private async Task<bool> IsValidAssetPathAsync(string relativePath, CancellationToken ctoken = default)
    {
        return await _assetPresence.ReadAsync(async (table, ct) =>
        {
            List<AssetPresenceInfo> copy = await table.AsNoTracking().ToListAsync(ctoken);
            return copy.Any(x => x.RelativePath.Equals(relativePath, FilenameNormalization.FilenameComparison));
        }, ctoken);
    }
}