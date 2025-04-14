using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService
{
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
}