using CSX.DotNet.Common.Extensions.Collections;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.CacheDb.Models;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public partial class AssetAbstractionService
{
    public async Task<List<AssetPresenceInfo>> GetPresencesAsync(
        CancellationToken ctoken = default)
    {
        // Copy and close connection (Immediate)
        var copy = await _assetPresence.ReadAsync(
            readFunc: async (table, ct) => await table.AsNoTracking().ToListAsync(ct),
            ctoken: ctoken);

        // Return the copy
        return copy;
    }

    public async Task<IEnumerable<AssetPresenceInfo>> QueryAsync<TQuery>(
        Func<AssetPresenceInfo, TQuery, bool> predicate,
        Func<IEnumerable<AssetPresenceInfo>, TQuery, IEnumerable<AssetPresenceInfo>> ordering,
        TQuery query,
        int skip,
        int take,
        CancellationToken ctoken = default)
    {
        // in future, from asset table; not presence
        // and finally validate presence before Skip/Take
        // (Ensure presence is an in-memory storage then; ie remove it from db)

        // Copy and close connection (Immediate)
        var copy = await _assetPresence.ReadAsync(
            readFunc: async (table, ct) => await table.AsNoTracking().ToListAsync(ct),
            ctoken: ctoken);

        // Filter, order, validate, paginate (Lazy)
        var filtered = copy
            // Filter rows
            .Where(x => predicate(x, query))
            // Apply ordering
            .Let(ordering, query)
            // Validate presence
            /*placeholder*/
            // Paginate
            .Skip(skip).Take(take);

        // Do not materialize here, hand over control.
        return filtered;
    }
}