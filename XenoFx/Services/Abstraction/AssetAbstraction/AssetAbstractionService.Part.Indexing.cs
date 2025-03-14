using CSX.Common.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.Cache.Models;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService
{
    public async Task TotalRefreshAsync(string[] files, CancellationToken ctoken = default)
    {
        await _assetPresence.WriteAsync(async (table, ct) =>
        {
            try
            {
                var all = await table.ToListAsync(ct);

                List<AssetPresenceInfo> existing = [];
                List<AssetPresenceInfo> ToRemove = [];

                foreach (var row in all)
                {
                    if (files.Contains(row.RelativePath, FilenameNormalization.FilenameComparer))
                        existing.Add(row);
                    else
                        ToRemove.Add(row);
                }
                table.RemoveRange(ToRemove);

                var existingPaths = existing.Select(x => x.RelativePath).ToList();
                var toAdd = files.Except(existingPaths)
                    .Select(x => new AssetPresenceInfo() { RelativePath = x })
                    .ToList();

                if (toAdd.Count > 0)
                    await table.AddRangeAsync(toAdd, ct);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register asset presences.");
                return false;
            }
        }, ctoken);
        OnUpdated();
    }

    public async Task CreateAsync(string path, CancellationToken ctoken = default)
    {
        int result = await _assetPresence.WriteAsync(async (table, ct) =>
        {
            await table.AddAsync(new AssetPresenceInfo() { RelativePath = path }, ct);
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
                .ExecuteDeleteAsync();
            return true;
        });
        OnUpdated();
    }
}