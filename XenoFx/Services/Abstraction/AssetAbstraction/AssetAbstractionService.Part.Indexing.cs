using CSX.Common.Platform;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.Cache.Models;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService
{
    public async Task CreateAsync(string path, CancellationToken ctoken = default)
    {
        int result = await _assetPresence.WriteAsync(async (table, ct) =>
        {
            var linked = await table.ToListAsync(cancellationToken: ct);
            var existing = linked.Where(x => x.RelativePath.Equals(path, FilenameNormalization.FilenameComparison)).FirstOrDefault();
            if (existing != null)
            {
                // If the asset already exists, we don't need to add it again.
                return false;
            }

            // If the asset doesn't exist, add it to the table.
            await table.AddAsync(new AssetPresenceInfo() { RelativePath = path }, ct);

            // Tell the wrapper to save changes.
            return true;
        }, ctoken);
        OnUpdated();
    }

    public async Task RemoveAsync(string path, CancellationToken ctoken = default)
    {
        await _assetPresence.WriteAsync(async (table, ct) =>
        {
            await table
                .Where(x => x.RelativePath.Equals(path, FilenameNormalization.FilenameComparison))
                .ExecuteDeleteAsync(cancellationToken: ct);
            return true;
        }, ctoken);
        OnUpdated();
    }
}