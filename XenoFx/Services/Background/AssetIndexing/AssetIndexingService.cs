using Microsoft.Extensions.Logging;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Background.AssetIndexing;

public sealed partial class AssetIndexingService : IAssetIndexingService
{
    // Infrastructure

    private readonly IAssetAbstractionService _assetAbstraction;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetIndexingService> _logger;

    public AssetIndexingService(
        IAssetAbstractionService assetAbstraction,
        IConfigurationService configuration,
        ILogger<AssetIndexingService> logger)
    {
        _assetAbstraction = assetAbstraction;
        _configuration = configuration;
        _logger = logger;
    }
}