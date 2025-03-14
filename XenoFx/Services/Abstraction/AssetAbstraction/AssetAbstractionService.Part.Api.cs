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
                .Select(x => Path.GetFileNameWithoutExtension(x.RelativePath))
                .Where(x => x.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }, ctoken);
    }
}