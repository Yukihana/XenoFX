using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetMedia;
using XenoFx.Services.Abstraction.AssetMedia.DTOs;

namespace XenoFx.Api.AssetDelivery;

// TODO: Upgrade this to HLS
public class AssetDeliveryService : IAssetDeliveryService
{
    // Infrastructure

    private readonly IAssetMediaService _assetMedia;
    private readonly ILogger<AssetDeliveryService> _logger;

    // Lifecycle

    public AssetDeliveryService(
        IAssetMediaService assetMedia,
        ILogger<AssetDeliveryService> logger)
    {
        _assetMedia = assetMedia;
        _logger = logger;
    }

    //  API

    public Task<AssetMediaResult> GetFilePathAsync(
        string id,
        TranscodeOptions? options = null,
        CancellationToken ctoken = default)
    {
        return _assetMedia.GetFilePathAsync(id, options, ctoken);
    }
}