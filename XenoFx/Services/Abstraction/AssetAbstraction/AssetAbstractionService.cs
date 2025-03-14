using Microsoft.Extensions.Logging;
using XenoFx.Services.Storage.AssetPresence;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService : IAssetAbstractionService
{
    private readonly IAssetPresenceService _assetPresence;
    private readonly ILogger<AssetAbstractionService> _logger;

    public AssetAbstractionService(
        IAssetPresenceService assetPresence,
        ILogger<AssetAbstractionService> logger)
    {
        _assetPresence = assetPresence;
        _logger = logger;
    }
}