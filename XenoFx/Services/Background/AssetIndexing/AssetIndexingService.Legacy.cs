using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.CacheDb.Models;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public async Task LegacyCreateAsync(string path, CancellationToken ctoken = default)
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

    public async Task LegacyRemoveAsync(string path, CancellationToken ctoken = default)
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

    // State (Rework and reintegrate this with AssetIndex OR make an actual state service that StateMonitor API service will call)

    private ulong _stateIndex = 0;
    private DateTime _lastModified = DateTime.UtcNow;

    private void OnUpdated()
    {
        Interlocked.Increment(ref _stateIndex);
        _lastModified = DateTime.UtcNow;
    }

    public ulong StateIndex
        => Interlocked.Read(ref _stateIndex);

    public DateTime LastModified
        => _lastModified;
}