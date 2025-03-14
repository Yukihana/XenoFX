using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction;

namespace XenoFx.Services.Api.AssetSearch;

public sealed partial class AssetSearchService : IAssetSearchService
{
    private readonly IAssetAbstractionService _assetAbstraction;

    public AssetSearchService(IAssetAbstractionService assetAbstraction)
    {
        _assetAbstraction = assetAbstraction;
    }

    public async Task<string[]> GetHaveAsync(string searchString, CancellationToken ctoken = default)
        => await _assetAbstraction.GetHaveAsync(searchString, ctoken);

    public async Task<Dictionary<string, float>> SearchAsync(string searchString, CancellationToken ctoken = default)
    {
        await Task.Yield();
        return [];
    }
}