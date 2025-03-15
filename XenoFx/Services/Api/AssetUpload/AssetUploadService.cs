using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Api.AssetUpload.Models;

namespace XenoFx.Services.Api.AssetUpload;

public sealed partial class AssetUploadService : IAssetUploadService
{
    public Task<AssetUploadResult> RegisterUploadAsync(AssetUploadRequest request, CancellationToken ctoken = default)
    {
    }
}