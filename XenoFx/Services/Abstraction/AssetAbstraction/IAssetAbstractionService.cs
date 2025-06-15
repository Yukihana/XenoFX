using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction.DTOs;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public interface IAssetAbstractionService
{
    // Search

    Task<string[]> GetHaveAsync(string searchString, CancellationToken ctoken = default);

    Task<string> GetFirstMatchingAssetPathAsync(string id, CancellationToken ctoken);

    // Download

    Task<AssetViewerInfo> GetAssetViewerInfoAsync(string id, CancellationToken ctoken = default);

    Task<string> GetContentFullPathAsync(string id, CancellationToken ctoken = default);

    Task ValidateAssetAsync(string fullPath, CancellationToken ctoken);
}