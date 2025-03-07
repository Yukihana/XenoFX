using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using XenoFx.Services.Utility.Configuration;
using XenoFx.Services.Utility.PathValidator;

namespace XenoFx.Services.Storage.AssetEnumeration;

public sealed partial class AssetEnumerationService : IAssetEnumerationService
{
    // Infrastructure

    private readonly IConfigurationService _profileService;
    private readonly IPathValidatorService _pathValidatorService;
    private readonly ILogger<AssetEnumerationService> _logger;

    // Resources

    public AssetEnumerationService(
        IConfigurationService profileService,
        IPathValidatorService pathValidatorService,
        ILogger<AssetEnumerationService> logger)
    {
        _profileService = profileService;
        _pathValidatorService = pathValidatorService;
        _logger = logger;
    }

    // Enumeration API

    public string[] ListFiles()
    {
        string path = _profileService.AssetsDirectory;

        return [.. Directory
            .GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Select(x => Path.GetRelativePath(path, x))
            .Where(_pathValidatorService.ValidateAssetPath)];
    }
}