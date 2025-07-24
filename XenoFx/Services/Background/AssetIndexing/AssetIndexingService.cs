using Microsoft.Extensions.Logging;
using XenoFx.Services.Processing.AssetIngestion;
using XenoFx.Services.Storage.AssetPresence;
using XenoFx.Services.Utility.Configuration;
using XenoFx.Services.Utility.PathValidator;

namespace XenoFx.Services.Background.AssetIndexing;

public sealed partial class AssetIndexingService : IAssetIndexingService
{
    // Infrastructure

    private readonly IAssetPresenceService _assetPresence;
    private readonly IAssetIngestionService _assetIngestion;
    private readonly IPathValidatorService _pathValidator;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetIndexingService> _logger;

    public AssetIndexingService(
        IAssetPresenceService assetPresence,
        IAssetIngestionService assetIngestion,
        IPathValidatorService pathValidator,
        IConfigurationService configuration,
        ILogger<AssetIndexingService> logger)
    {
        _assetPresence = assetPresence;
        _assetIngestion = assetIngestion;
        _pathValidator = pathValidator;
        _configuration = configuration;
        _logger = logger;
    }
}