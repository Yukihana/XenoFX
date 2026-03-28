using System.Threading;
using System.Threading.Tasks;
using XenoFx.Features.AssetSearch.Contracts;
using XenoFx.Features.AssetSearch.Models;

namespace XenoFx.Features.AssetSearch;

public interface IAssetSearchService
{
    Task<AssetSearchResult> SearchAsync(
        AssetSearchQuery query,
        string partialSeed,
        CancellationToken ctoken = default);

    Task<AssetSearchCardData?> NextploreAsync(
        AssetNextploreQuery query,
        string partialSeed,
        CancellationToken ctoken);

    Task<AssetRelatedResult> RelatedAsync(
        AssetRelatedQuery query,
        string partialSeed,
        CancellationToken ctoken = default);

    // Legacy, requires revamp
    Task<string[]> GetHaveAsync(
        string searchString,
        CancellationToken ctoken = default);
}