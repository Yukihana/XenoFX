using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction;

namespace XenoFx.Features.AssetSearch;

public sealed partial class AssetSearchService
    : IAssetSearchService
{
    private readonly IAssetAbstractionService _assetAbstraction;
    private readonly ILogger<AssetSearchService> _logger;

    public AssetSearchService(
        IAssetAbstractionService assetAbstraction,
        ILogger<AssetSearchService> logger)
    {
        _assetAbstraction = assetAbstraction;
        _logger = logger;
    }

    // Legacy GetHave

    public async Task<string[]> GetHaveAsync(
        string searchString,
        CancellationToken ctoken = default)
        => await _assetAbstraction.GetHaveAsync(searchString, ctoken);
}