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
        return await _assetPresence.ReadAsync(async (table, ct) =>
        {
            var all = await table.ToListAsync(ct);

            return all
                .Where(x => Path.GetFileNameWithoutExtension(x.RelativePath).Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.RelativePath)
                .ToArray();
        }, ctoken);
    }
}