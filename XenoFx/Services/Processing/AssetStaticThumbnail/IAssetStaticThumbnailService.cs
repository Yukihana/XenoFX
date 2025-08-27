using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Processing.AssetStaticThumbnail;

public interface IAssetStaticThumbnailService
{
    Task<string> GetFilePathAsync(
        string id,
        CancellationToken ctoken = default);
}