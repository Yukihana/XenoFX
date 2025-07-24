using System.Threading;
using System.Threading.Tasks;
using XenoServe.Features.AssetUpload.DTOs;

namespace XenoServe.Features.AssetUpload;

public interface IAssetUploadOrchestrator
{
    Task<AssetUploadResult> CacheAndRegisterAsync(
        AssetUploadContext context,
        CancellationToken ctoken = default);
}