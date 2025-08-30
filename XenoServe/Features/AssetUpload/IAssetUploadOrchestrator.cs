using CSX.DotNet.Common.DI.Orchestrators;
using System.Threading;
using System.Threading.Tasks;
using XenoServe.Features.AssetUpload.DTOs;

namespace XenoServe.Features.AssetUpload;

public interface IAssetUploadOrchestrator : IOrchestrator
{
    Task<AssetUploadResult> CacheAndRegisterAsync(
        AssetUploadContext context,
        CancellationToken ctoken = default);
}