using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public partial class AssetAbstractionService
{
    // Internal

    private async Task<bool> CheckIfLiveAssetAsync(string relativePath, CancellationToken ctoken = default)
    {
        string normalizedPath = relativePath.ToLowerInvariant();
        return await _assetPresence.ReadAsync(async (table, ct) =>
        {
            return await table.AnyAsync(
                predicate: x => x.NormalizedPath == normalizedPath,
                cancellationToken: ct);
        }, ctoken);
    }
}