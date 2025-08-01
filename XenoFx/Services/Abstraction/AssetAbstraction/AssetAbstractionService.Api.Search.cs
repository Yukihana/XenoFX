using CSX.DotNet.Common.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public partial class AssetAbstractionService
{
    // Makeshift

    public async Task<string[]> GetHaveAsync(string searchString, CancellationToken ctoken = default)
    {
        string normalizedSearchString = searchString.ToLowerInvariant();
        return await _assetPresence.ReadAsync(async (table, ct) =>
        {
            // Optimize the query by filtering on the normalized path first
            var paths = await table
                .Where(x => x.NormalizedPath.Contains(normalizedSearchString))
                .Select(x => x.OriginalPath)
                .ToListAsync(ct);

            // Filter the results based on the actual filename (to prevent unnecessary match with a directory name)
            return paths.Where(x => Path
                .GetFileNameWithoutExtension(x)
                .Contains(searchString, StringComparison.OrdinalIgnoreCase)
            ).ToArray();
        }, ctoken);
    }

    public async Task<string> GetFirstMatchingAssetPathAsync(string id, CancellationToken ctoken = default)
    {
        // Note:
        // For now id is the search key of the asset.
        // - mainly so it can handle dynamic content address changed without a backing database
        // Later it will be changed to the actual asset ID.
        // - as the table tallied with presences will then provide the actual path

        var results = await GetHaveAsync(id, ctoken);

        if (results.Length < 1 || string.IsNullOrEmpty(results[0]))
            throw new ResourceNotFoundException($"No asset found for id: {id}");

        return results[0];
    }
}