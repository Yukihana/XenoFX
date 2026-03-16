using CSX.DotNet.Common.IO.Paths;
using CSX.DotNet.Common.Platform;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Utility.PathValidator;

public sealed partial class PathValidatorService
    : IPathValidatorService
{
    // Infrastructure

    private readonly IConfigurationService _configuration;
    private readonly ILogger<PathValidatorService> _logger;

    // Parameters

    private readonly ImmutableArray<string> _allowedExtensions;
    private readonly PathFilter _assetPathFilter;

    // Lifecycle

    public PathValidatorService(
        IConfigurationService configuration,
        ILogger<PathValidatorService> logger)
    {
        _assetPathFilter = new(configuration.AssetPathFilterConfiguration);
        _allowedExtensions = configuration.AllowedAssetExtensions;
        _configuration = configuration;
        _logger = logger;
    }

    // API

    public bool TryTruncateAssetPath(string path, [NotNullWhen(true)] out string? relativePath)
    {
        relativePath = null;

        try
        {
            // Normalize to full path for comparison.
            string fullBasePath = Path.GetFullPath(_configuration.AssetsDirectory);
            string fullPath = Path.GetFullPath(path);

            // Bail if root doesn't match
            if (!fullPath.StartsWith(fullBasePath, FilenameNormalization.FilenameComparison))
                return false;

            // Bail if no extensions are specified
            if (!_allowedExtensions.Any())
                return false;

            // Bail if no wildcard and extension doesn't match
            string ext = Path.GetExtension(fullPath).ToLowerInvariant().TrimStart('.');
            if (!_allowedExtensions.Contains("*") &&
                !_allowedExtensions.Contains(ext))
                return false;

            // Else attempt to get relative path
            relativePath = Path
                .GetRelativePath(fullBasePath, fullPath)
                .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Validate the relative path using the filter
            return _assetPathFilter.Validate(relativePath);
        }
        catch (Exception)
        {
            return false; // Handle invalid paths safely
        }
    }
}