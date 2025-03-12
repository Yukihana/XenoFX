using CSX.Common.IO;
using CSX.Common.Platform;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Utility.PathValidator;

public sealed partial class PathValidatorService : IPathValidatorService
{
    private readonly IConfigurationService _configuration;
    private readonly PathFilter _assetPathFilter;

    public PathValidatorService(IConfigurationService profileService)
    {
        _configuration = profileService;
        _assetPathFilter = new(profileService.AssetPathFilterConfiguration);
    }

    public bool TryTruncateAssetPath(string path, [NotNullWhen(true)] out string? relativePath)
    {
        relativePath = null;

        try
        {
            // Normalize to full path for comparison.
            string fullBasePath = Path.GetFullPath(_configuration.AssetsDirectory);
            string fullPath = Path.GetFullPath(path);

            // Bail if root doesn't match. Else attempt to get relative path.
            if (!fullPath.StartsWith(fullBasePath, FilenameNormalization.FilenameComparison))
                return false;
            relativePath = Path.GetRelativePath(fullBasePath, fullPath);

            // Validate the asset path using the filter
            return _assetPathFilter.Validate(relativePath);
        }
        catch (Exception)
        {
            return false; // Handle invalid paths safely
        }
    }
}