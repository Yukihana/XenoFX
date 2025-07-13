using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public async Task<bool> IndexModifyEventAsync(
        string fullPath,
        CancellationToken ctoken = default)
    {
        if (!_pathValidator.TryTruncateAssetPath(fullPath, out string? relativePath))
            return false;

        var result = await OnModifiedAsync(relativePath, ctoken);

        if (!result)
            _logger.LogInformation("Indexed modification: {path}", relativePath);

        return result;
    }

    private Task<bool> OnModifiedAsync(string relativePath, CancellationToken ctoken)
    {
        _ = _pathValidator;
        _ = relativePath;
        return Task.FromResult(false); // Temporary bypass, re-evaluation not required.
    }
}