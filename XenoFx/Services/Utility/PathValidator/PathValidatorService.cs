using CSX.Common.IO;
using System.IO;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Utility.PathValidator;

public sealed partial class PathValidatorService : IPathValidatorService
{
    private readonly IConfigurationService _profileService;
    private readonly PathFilter _assetPathFilter;

    public PathValidatorService(IConfigurationService profileService)
    {
        _profileService = profileService;
        _assetPathFilter = new(profileService.AssetPathFilterConfiguration);
    }

    public bool ValidateAssetPath(string path)
    {
        string relativePath
            = (Path.IsPathRooted(path) || Path.IsPathFullyQualified(path))
            ? Path.GetRelativePath(_profileService.AssetsDirectory, path)
            : path;

        return _assetPathFilter.Validate(relativePath);
    }
}