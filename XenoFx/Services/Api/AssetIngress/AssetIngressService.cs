using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetQueue;

namespace XenoFx.Services.Api.AssetIngress;

public class AssetIngressService : IAssetIngressService
{
    // Infrastructure

    private readonly IAssetQueueService _assetQueue;
    private readonly ILogger<AssetIngressService> _logger;

    // Lifecycle

    public AssetIngressService(
        IAssetQueueService assetQueue,
        ILogger<AssetIngressService> logger)
    {
        _assetQueue = assetQueue;
        _logger = logger;
    }

    public async Task EnqueueUploadAsync(
        string uploadMetadataPath,
        Func<string, CancellationToken, Task>? cleanupCallback,
        CancellationToken ctoken = default)
    {
        await _assetQueue.OnFileUploadedAsync(
            uploadMetadataPath: uploadMetadataPath,
            cleanupCallback: cleanupCallback,
            ctoken: ctoken);
    }
}