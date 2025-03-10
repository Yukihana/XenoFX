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

    public async Task<string[]> GetHave(CancellationToken ctoken = default)
    {
        await Task.Yield();
        return [];
    }

    public async Task<string[]> GetHave(string searchString, CancellationToken ctoken = default)
    {
        await Task.Yield();
        return [];
    }

    public async Task<Dictionary<string, float>> Search(string searchString, CancellationToken ctoken = default)
    {
        await Task.Yield();
        return [];
    }
}