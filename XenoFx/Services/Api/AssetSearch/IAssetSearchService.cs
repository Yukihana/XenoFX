using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Api.AssetSearch.Contracts;

namespace XenoFx.Services.Api.AssetSearch;

public interface IAssetSearchService
{
    Task<string[]> GetHaveAsync(
        string searchString,
        CancellationToken ctoken = default);

    Task<AssetSearchResult> SearchAsync(
        AssetSearchQuery query,
        CancellationToken ctoken = default);
}