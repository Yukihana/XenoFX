using CSX.Common.IO;
using Microsoft.Extensions.Logging;
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

        UploadCacheDirectory = _configuration.UploadCacheDirectory;     // PathExtensions.ResolveCombine(_configuration.DataDirectory, _configuration.UploadCacheDirectory);
        AssetsUploadDirectory = _configuration.AssetsUploadDirectory;    // PathExtensions.ResolveCombine(AssetsDirectory, _configuration.FinalUploadDirectory);

        AssetPathFilterConfiguration = _configuration.AssetFilterConfig.Copy(); // Ensure full decoupling
        AssetEnumerationIntervalSeconds = _configuration.AssetEnumerationIntervalSeconds;
    }

    // Shared

    public string AssetsDirectory { get; }

    // Directories

    public string UploadCacheDirectory { get; }

    public string AssetsUploadDirectory { get; }

    // Parameters

    public PathFilterConfiguration AssetPathFilterConfiguration { get; }

    public ulong AssetEnumerationIntervalSeconds { get; }

    // Inter-service mutable (Move these to function directly from configuration; no need for a separate type)

    public RuntimeContext RuntimeContext
        => _runtimeContext;
}