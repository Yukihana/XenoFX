using CSX.DotNet.Common.Data.DataGenerators;
using CSX.DotNet.Common.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.CacheDb.Models;
using XenoFx.Services.Api.AssetSearch.Contracts;

namespace XenoFx.Services.Api.AssetSearch;

public partial class AssetSearchService
{
    public async Task<AssetSearchResult> SearchAsync(
        AssetSearchQuery query,
        string partialSeed,
        CancellationToken ctoken = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Retrieve all entries
        List<AssetPresenceInfo> presences = await _assetAbstraction.GetPresencesAsync(ctoken);

        // Filter and order the presences based on the query
        var sorted
            = string.IsNullOrWhiteSpace(query.Keywords)
            ? presences.Randomize(Int32Generators.CreateHourSeed(partialSeed))
            : presences.Let(FilterAndOrder, query);

        // Apply pagination
        var results = sorted
            .Skip(query.Page * query.PageSize)
            .Take(query.PageSize)
            .Select(AssetSearchExtensions.CreateAssetSearchCardData)
            .ToList();

        // Record time elapsed
        var elapsed = stopwatch.Elapsed;

        // Map results to DTO
        // TODO: Use hasMore logic if total count is expensive; May require frontend edit
        // TODO: Consider approximating total count based on traversed count.
        // TODO: Pagination can reflect approximate count. Or stream FilterAndOrder directly.
        var response = query.ToResult();
        response.Results = results;
        response.Total = sorted.Count();
        response.Duration = elapsed;
        response.Timestamp = DateTime.UtcNow;

        return response;
    }

    private static IEnumerable<AssetPresenceInfo> FilterAndOrder(
        IEnumerable<AssetPresenceInfo> presence,
        AssetSearchQuery query)
    {
        // Sorting method router
        // Note: Handle reversal inside the respective sorting methods to allow for more control
        return query.SortBy switch
        {
            _ => FilterAndOrderByRelevance(presence, query),
        };
    }
}