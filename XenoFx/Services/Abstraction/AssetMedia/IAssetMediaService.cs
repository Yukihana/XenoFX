using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetMedia.DTOs;

namespace XenoFx.Services.Abstraction.AssetMedia;

public interface IAssetMediaService
{
    Task<AssetMediaResult> GetFilePathAsync(
        string id,
        TranscodeOptions? options = null,
        CancellationToken ctoken = default);
}