using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Api.AssetDelivery;
using XenoFx.Services.Abstraction.AssetMedia;
using XenoFx.Services.Abstraction.AssetMedia.DTOs;

namespace XenoServe.Features.AssetDelivery;

public class AssetDeliveryOrchestrator : IAssetDeliveryOrchestrator
{
    private readonly IAssetDeliveryService _assetDelivery;
    private readonly ILogger<AssetDeliveryOrchestrator> _logger;

    public AssetDeliveryOrchestrator(
        IAssetDeliveryService assetDelivery,
        ILogger<AssetDeliveryOrchestrator> logger)
    {
        _logger = logger;
        _assetDelivery = assetDelivery;
    }

    // API

    public async Task<AssetMediaResult> GetFilePathAsync(
        string id,
        string? transcodeTypeString = null,
        CancellationToken ctoken = default)
    {
        TranscodeOptions options = CreateTranscodeOptions(
            transcodeTypeString);

        return await _assetDelivery.GetFilePathAsync(
            id: id,
            options: options,
            ctoken: ctoken);
    }

    // Internal

    private static TranscodeOptions CreateTranscodeOptions(
        string? transcodeTypeString)
    {
        transcodeTypeString ??= string.Empty;
        transcodeTypeString = transcodeTypeString.Trim().ToLowerInvariant();

        TranscodeFormat mode = transcodeTypeString switch
        {
            "universal" => TranscodeFormat.Universal,
            "original" => TranscodeFormat.Original,
            _ => TranscodeFormat.Universal
        };

        return new()
        {
            TranscodeFormat = mode,
        };
    }
}