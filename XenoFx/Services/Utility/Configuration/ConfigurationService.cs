using CSX.DotNet.Common.IO.Paths;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using XenoFx.Environment.Configuration;
using XenoFx.Services.Utility.Configuration.Models;

namespace XenoFx.Services.Utility.Configuration;

/// <summary>
/// This class is meant to read the profile and handle providing operational parameters.
/// </summary>
public sealed partial class ConfigurationService : IConfigurationService
{
    // Data

    private readonly RuntimeContext _runtimeContext = new();

    // Infrastructure

    private readonly XenoFxConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    // Lifecycle

    public ConfigurationService(
        XenoFxConfiguration configuration,
        ILogger<ConfigurationService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Snapshot and Cache ----
        // Note: Resolve in configuration. Only cache here.

        // Shared
        AssetsDirectory = _configuration.AssetsDirectory;
        UploadsDirectory = _configuration.UploadsDirectory;
        MetadataDirectory = _configuration.MetadataDirectory;
        SharedCacheDirectory = _configuration.SharedCacheDirectory;

        // Directories
        ThumbsDirectory = _configuration.ThumbsDirectory;
        AssetsUploadDirectory = _configuration.AssetsUploadDirectory;

        // Parameters
        AllowedAssetUploadExtensions = ImmutableArray.Create(_configuration.AllowedAssetUploadExtensions);
        AssetPathFilterConfiguration = _configuration.AssetFilterConfig.Copy(); // Ensure full decoupling
        AssetEnumerationIntervalSeconds = _configuration.AssetEnumerationIntervalSeconds;
    }

    // Shared

    public string AssetsDirectory { get; }
    public string UploadsDirectory { get; }
    public string MetadataDirectory { get; }
    public string SharedCacheDirectory { get; }

    // Directories

    public string ThumbsDirectory { get; }
    public string AssetsUploadDirectory { get; }

    // Parameters

    public ImmutableArray<string> AllowedAssetUploadExtensions { get; }
    public PathFilterConfiguration AssetPathFilterConfiguration { get; } // Make this immutable too? (wrap into a new class with immutable members perhaps)
    public ulong AssetEnumerationIntervalSeconds { get; }

    // Inter-service mutable (TODO Move these to function directly from configuration; no need for a separate type)

    public RuntimeContext RuntimeContext
        => _runtimeContext;
}