using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Api.AssetSearch;
using XenoFx.Services.Api.AssetSearch.Contracts;
using XenoServe.Features.AssetSearch.DTOs;

namespace XenoServe.Features.AssetSearch;

public class AssetSearchOrchestrator : IAssetSearchOrchestrator
{
    private readonly IAssetSearchService _assetSearch;
    private readonly ILogger<AssetSearchOrchestrator> _logger;

    public AssetSearchOrchestrator(
        IAssetSearchService assetSearch,
        ILogger<AssetSearchOrchestrator> logger)
    {
        _assetSearch = assetSearch;
        _logger = logger;
    }

    // Public API

    public async Task<AssetSearchResult> SearchAsync(
        AssetSearchQueryParams queryParams,
        IPAddress? ipAddress,
        CancellationToken ctoken = default)
    {
        var query = queryParams.MapToQueryDto();
        var partialSeed = ipAddress?.ToString() ?? "unknown-ip";

        var response = await _assetSearch.SearchAsync(
            query: query,
            partialSeed: partialSeed,
            ctoken: ctoken);

        return response;
    }

    public Task<AssetSearchCardData?> NextploreAsync(
        AssetNextploreQueryParams queryParams,
        IPAddress? ipAddress,
        CancellationToken ctoken = default)
    {
        var query = queryParams.MapToQueryDto();
        var partialSeed = ipAddress?.ToString() ?? "unknown-ip";

        var response = _assetSearch.NextploreAsync(
            query: query,
            partialSeed: partialSeed,
            ctoken: ctoken);

        return response;
    }

    public Task<string[]> GetHaveAsync(
        string query,
        CancellationToken ctoken)
    {
        var response = _assetSearch.GetHaveAsync(
            searchString: query,
            ctoken: ctoken);

        return response;
    }
}