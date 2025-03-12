using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Storage.AssetPresence;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Background.AssetIndexing;

public sealed partial class AssetIndexingService : IAssetIndexingService
{
    // Infrastructure

    private readonly IAssetPresenceService _assetPresence;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetIndexingService> _logger;

    public AssetIndexingService(
        IAssetPresenceService assetPresence,
        IConfigurationService configuration,
        ILogger<AssetIndexingService> logger)
    {
        _assetPresence = assetPresence;
        _configuration = configuration;
        _logger = logger;
    }
}