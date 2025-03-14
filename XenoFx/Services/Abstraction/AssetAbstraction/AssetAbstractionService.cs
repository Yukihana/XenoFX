using Microsoft.Extensions.Logging;
using XenoFx.Services.Storage.AssetPresence;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService : IAssetAbstractionService
{
    private readonly IAssetPresenceService _assetPresence;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<AssetAbstractionService> _logger;

    public AssetAbstractionService(
        IAssetPresenceService assetPresence,
        IConfigurationService configurationService,
        ILogger<AssetAbstractionService> logger)
    {
        _assetPresence = assetPresence;
        _configurationService = configurationService;
        _logger = logger;
    }
}