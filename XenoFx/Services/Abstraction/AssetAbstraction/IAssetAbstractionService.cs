using System;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public interface IAssetAbstractionService
{
    // Write

    Task TotalRefreshAsync(string[] files, CancellationToken ctoken = default);

    Task CreateAsync(string path, CancellationToken ctoken = default);

    Task RemoveAsync(string path, CancellationToken ctoken = default);

    // Read

    Task<string[]> GetHaveAsync(string searchString, CancellationToken ctoken = default);

    // State

    ulong StateIndex { get; }
    DateTime LastModified { get; }
}