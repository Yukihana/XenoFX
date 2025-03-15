using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Api.AssetUpload.Models;

namespace XenoFx.Services.Api.AssetUpload;

public interface IAssetUploadService
{
    Task<AssetUploadResult> RegisterUploadAsync(AssetUploadRequest request, CancellationToken ctoken = default);
}