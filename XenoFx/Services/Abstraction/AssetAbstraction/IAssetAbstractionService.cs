using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction.DTOs;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public interface IAssetAbstractionService
{
    // Search

    Task<string[]> GetHaveAsync(string searchString, CancellationToken ctoken = default);

    // Download

    Task<AssetViewerInfo> GetAssetViewerInfoAsync(string id, CancellationToken ctoken = default);

    // Placeholders

    Task<string> GetFirstMatchingAssetPathAsync(string id, CancellationToken ctoken = default);

    Task<string> GetAssetFilePathAsync(string id, CancellationToken ctoken = default);
}