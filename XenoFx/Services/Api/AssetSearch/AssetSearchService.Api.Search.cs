using CSX.Common.Extensions.Collections;
using Microsoft.Extensions.Logging;
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
        CancellationToken ctoken = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Retrieve all entries
        List<AssetPresenceInfo> presences = await _assetAbstraction.GetPresencesAsync(ctoken);

        // Filter and order the presences based on the query
        var sorted
            = string.IsNullOrWhiteSpace(query.Keywords)
            ? presences.Randomize()
            : presences.Let(FilterAndOrder, query);

        // Apply pagination
        var selection = sorted
            .Skip(query.Page * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        // Materialize and Map; Record time elapsed.
        var results = selection
            .Select(AssetSearchExtensions.CreateAssetSearchCardData)
            .ToList();
        var elapsed = stopwatch.Elapsed;

        // Map results to DTO
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