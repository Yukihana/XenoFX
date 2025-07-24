using CSX.Common.IO;
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

        AssetsDirectory = _configuration.AssetsDirectory;
        UploadsDirectory = _configuration.UploadsDirectory;             // PathExtensions.ResolveCombine(_configuration.DataDirectory, _configuration.UploadCacheDirectory);
        MetadataDirectory = _configuration.MetadataDirectory;
        SharedCacheDirectory = _configuration.SharedCacheDirectory;

        AssetsUploadDirectory = _configuration.AssetsUploadDirectory;   // PathExtensions.ResolveCombine(AssetsDirectory, _configuration.FinalUploadDirectory);

        AllowedAssetExtensions = ImmutableArray.Create(_configuration.AllowedAssetExtensions);
        AssetPathFilterConfiguration = _configuration.AssetFilterConfig.Copy(); // Ensure full decoupling
        AssetEnumerationIntervalSeconds = _configuration.AssetEnumerationIntervalSeconds;
    }

    // Shared

    public string AssetsDirectory { get; }
    public string UploadsDirectory { get; }
    public string MetadataDirectory { get; }
    public string SharedCacheDirectory { get; }

    // Directories

    public string AssetsUploadDirectory { get; }

    // Parameters

    public ImmutableArray<string> AllowedAssetExtensions { get; }
    public PathFilterConfiguration AssetPathFilterConfiguration { get; } // Make this immutable too? (wrap into a new class with immutable members perhaps)
    public ulong AssetEnumerationIntervalSeconds { get; }

    // Inter-service mutable (TODO Move these to function directly from configuration; no need for a separate type)

    public RuntimeContext RuntimeContext
        => _runtimeContext;
}