using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Api.AssetSearch.Contracts;

namespace XenoFx.Services.Api.AssetSearch;

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

    // Legacy, requires revamp
    Task<string[]> GetHaveAsync(
        string searchString,
        CancellationToken ctoken = default);
}