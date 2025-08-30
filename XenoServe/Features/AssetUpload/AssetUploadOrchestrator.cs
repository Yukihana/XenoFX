using CSX.DotNet.Common.DI;
using CSX.DotNet.Modules.FileUploader.Services.UploadApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Api.AssetIngress;
using XenoFx.Services.Processing.AssetIngestion.DTOs;
using XenoServe.Features.AssetUpload.DTOs;

namespace XenoServe.Features.AssetUpload;

[DependencyLifetime(ServiceLifetime.Singleton)]
public class AssetUploadOrchestrator : IAssetUploadOrchestrator
{
    // Infrastructure

    private readonly IUploadApiService _uploadApi;
    private readonly IAssetIngressService _assetIngress;
    private readonly ILogger<AssetUploadOrchestrator> _logger;

    // Lifecycle

    public AssetUploadOrchestrator(
        IUploadApiService uploadApi,
        IAssetIngressService assetIngress,
        ILogger<AssetUploadOrchestrator> logger)
    {
        _uploadApi = uploadApi;
        _assetIngress = assetIngress;
        _logger = logger;
    }

    // API

    public async Task<AssetUploadResult> CacheAndRegisterAsync(
        AssetUploadContext context,
        CancellationToken ctoken = default)
    {
        // Cache the content first
        string pathToCachedFile;
        using (Stream stream = context.Request.Data.OpenReadStream())
        {
            pathToCachedFile = await _uploadApi.CacheUploadAsync(
                stream: stream,
                ctoken: ctoken);
        }

        // Write sidecar
        AssetUploadMetadata metadata = context.MapToUploadMetadata(
            cachedFilename: Path.GetFileName(pathToCachedFile));
        string sidecarPath = await _uploadApi.AttachSideCarAsync(
            sidecarObject: metadata,
            pathToCachedFile: pathToCachedFile,
            ctoken: ctoken);

        // Enqueue sidecar for indexing
        await _assetIngress.EnqueueUploadAsync(
            uploadMetadataPath: string.Empty,
            cleanupCallback: _uploadApi.ClearUploadAsync,
            ctoken: ctoken);

        return new()
        {
            Message = "File uploaded successfully."
        };
    }
}