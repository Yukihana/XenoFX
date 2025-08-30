using CSX.DotNet.Common.DI.Orchestrators;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetMedia.DTOs;

namespace XenoServe.Features.AssetDelivery;

public interface IAssetDeliveryOrchestrator : IOrchestrator
{
    Task<AssetMediaResult> GetFilePathAsync(
        string id,
        string? transcodeType = null,
        CancellationToken ctoken = default);
}