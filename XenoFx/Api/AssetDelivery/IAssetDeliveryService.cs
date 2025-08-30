using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetMedia;
using XenoFx.Services.Abstraction.AssetMedia.DTOs;

namespace XenoFx.Api.AssetDelivery;

public interface IAssetDeliveryService
{
    Task<AssetMediaResult> GetFilePathAsync(
        string id,
        TranscodeOptions? options = null,
        CancellationToken ctoken = default);
}