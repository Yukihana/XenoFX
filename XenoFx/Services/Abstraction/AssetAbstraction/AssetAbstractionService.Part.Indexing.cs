using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.CacheDb.Models;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService
{
    public async Task CreateAsync(string path, CancellationToken ctoken = default)
    {
        string normalizedPath = path.ToLowerInvariant();
        int result = await _assetPresence.WriteAsync(async (table, ct) =>
        {
            var existing = await table
                .Where(x => x.NormalizedPath == normalizedPath)
                .FirstOrDefaultAsync(cancellationToken: ct);

            // If the asset already exists, we don't need to add it again.
            if (existing != null)
                return false;

            // If the asset doesn't exist, add it to the table.
            await table.AddAsync(new AssetPresenceInfo()
            {
                OriginalPath = path,
                NormalizedPath = normalizedPath,
            }, ct);

            // Tell the wrapper to save changes.
            return true;
        }, ctoken);
        OnUpdated();
    }

    public async Task RemoveAsync(string path, CancellationToken ctoken = default)
    {
        string normalizedPath = path.ToLowerInvariant();
        int result = await _assetPresence.WriteAsync(async (table, ct) =>
        {
            await table
                .Where(x => x.NormalizedPath == normalizedPath)
                .ExecuteDeleteAsync(cancellationToken: ct);
            return true;
        }, ctoken);
        OnUpdated();
    }
}