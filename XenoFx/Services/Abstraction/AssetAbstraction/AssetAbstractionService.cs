using CSX.Common.Platform;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using XenoFx.Services.Storage.AssetPresence;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public sealed partial class AssetAbstractionService : IAssetAbstractionService
{
    private readonly IAssetPresenceService _assetPresence;
    private readonly ILogger<AssetAbstractionService> _logger;

    public AssetAbstractionService(
        IAssetPresenceService assetPresence,
        ILogger<AssetAbstractionService> logger)
    {
        _assetPresence = assetPresence;
        _logger = logger;
    }

    public string[] GetHave(string searchString, CancellationToken ctoken = default)
    {
        return _assetPresence.GetPaths(x => SearchMatch(x, searchString));
    }

    private bool SearchMatch(string path, string searchString)
    {
        // FxHD
        string filename = Path.GetFileNameWithoutExtension(path);
        return filename.Contains(searchString, FilenameNormalization.FilenameComparison);
    }
}