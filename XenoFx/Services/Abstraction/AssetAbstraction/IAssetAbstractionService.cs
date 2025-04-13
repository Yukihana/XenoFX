using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction.DTOs;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public interface IAssetAbstractionService
{
    // Write

    Task CreateAsync(string path, CancellationToken ctoken = default);

    Task RemoveAsync(string path, CancellationToken ctoken = default);

    // Read

    Task<string[]> GetHaveAsync(string searchString, CancellationToken ctoken = default);

    Task<AssetViewInfo> GetAssetDownloadInfoAsync(string relativePath, CancellationToken ctoken = default);

    // Content

    Task<string> GetContentPathAsync(string path, CancellationToken ctoken = default);

    // State

    ulong StateIndex { get; }
    DateTime LastModified { get; }
}