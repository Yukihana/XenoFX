using CSX.Common.Extensions.Instancing;
using CSX.Common.IO;
using CSX.Common.IO.Paths;
using Microsoft.Extensions.Logging;
using XenoFx.Environment;
using XenoFx.Services.Utility.Configuration.Models;

namespace XenoFx.Services.Utility.Configuration;

/// <summary>
/// This class is meant to read the profile and handle providing operational parameters.
/// </summary>
public sealed partial class ConfigurationService : IConfigurationService
{
    // Infrastructure

    private readonly XenoFxConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    // Data

    private readonly ParameterCache _cache = new();

    // Exposed (TODO decide if this needs to be encapsulated)

    public RuntimeContext RuntimeContext { get; } = new();

    public ConfigurationService(XenoFxConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        // snapshot to prevent configuration access leakage after this service has been generated.
        _configuration = configuration.MakeDecoupledCopy();
        _logger = logger;

        GenerateCache();
    }

    private void GenerateCache()
    {
        _cache.BaseDirectory = _configuration.GetBasePath();

        // Assets
        _cache.AssetsDirectory = PathExtensions.ResolveCombine(_cache.BaseDirectory, _configuration.Profile.AssetsDirectory);
        _cache.AssetUploadDirectory = PathExtensions.ResolveCombine(_cache.AssetsDirectory, _configuration.Profile.AssetUploadDirectory);

        // Cache (cache subpath names aren't specified by profile; use inline names)
        _cache.CacheDirectory = PathExtensions.ResolveCombine(_cache.BaseDirectory, _configuration.Profile.CacheDirectory);
        _cache.UploadDirectory = PathExtensions.ResolveCombine(_cache.CacheDirectory, "uploads");
    }

    // Core Data

    public PathFilterConfiguration AssetPathFilterConfiguration
        => _configuration.Profile.AssetFilterConfig.Copy();

    // Derived Data

    public string BaseDirectory
        => _cache.BaseDirectory;

    public string UploadDirectory
        => _cache.UploadDirectory;

    public string AssetsDirectory
        => _cache.AssetsDirectory;

    public string AssetUploadDirectory
        => _cache.AssetUploadDirectory;

    // Hosted

    public ulong AssetEnumerationIntervalSeconds
        => _configuration.Profile.AssetEnumerationIntervalSeconds;
}