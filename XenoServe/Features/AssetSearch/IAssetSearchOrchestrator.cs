using System.Net;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Api.AssetSearch.Contracts;
using XenoServe.Features.AssetSearch.DTOs;

namespace XenoServe.Features.AssetSearch;

public interface IAssetSearchOrchestrator
{
    Task<AssetSearchResult> SearchAsync(
        AssetSearchQueryParams queryParams,
        IPAddress? ipAddress,
        CancellationToken ctoken = default);

    Task<AssetSearchCardData?> NextploreAsync(
        AssetNextploreQueryParams queryParams,
        IPAddress? ipAddress,
        CancellationToken ctoken = default);

    Task<string[]> GetHaveAsync(
        string query,
        CancellationToken ctoken);
}