using CSX.DotNet.Common.DI.Orchestrators;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Features.AssetSearch.Contracts;
using XenoFx.Features.AssetSearch.Models;
using XenoServe.Features.AssetSearch.DTOs;

namespace XenoServe.Features.AssetSearch;

public interface IAssetSearchOrchestrator :
    IOrchestrator
{
    // TODO Replace IPAddress with client unique id (Not Auth token)

    Task<AssetSearchResult> SearchAsync(
        AssetSearchQueryParams queryParams,
        IPAddress? ipAddress,
        CancellationToken ctoken = default);

    Task<AssetSearchCardData?> NextploreAsync(
        AssetNextploreQueryParams queryParams,
        IPAddress? ipAddress,
        CancellationToken ctoken = default);

    Task<AssetRelatedResult> RelatedAsync(
        AssetRelatedQueryParams queryParams,
        IPAddress? ipAddress,
        CancellationToken ctoken = default);

    Task<string[]> GetHaveAsync(
        string query,
        CancellationToken ctoken);
}