using CSX.Common.Extensions.Instancing;
using CSX.Common.IO;
using CSX.Common.IO.Paths;
using Microsoft.Extensions.Logging;
using XenoFx.Environment;
using XenoFx.Services.Utility.Configuration.Models;
using XenoFx.Services.Utility.Profile.Models;

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
        _cache.AssetsDirectory = PathExtensions.ResolveCombine(_cache.BaseDirectory, _configuration.Profile.AssetsDirectory);
        _cache.UploadsDirectory = PathExtensions.ResolveCombine(_cache.BaseDirectory, _configuration.Profile.UploadsDirectory);
    }

    // Core Data

    public PathFilterConfiguration AssetPathFilterConfiguration
        => _configuration.Profile.AssetFilterConfig.Copy();

    // Derived Data

    public string BaseDirectory
        => _cache.BaseDirectory;

    public string AssetsDirectory
        => _cache.AssetsDirectory;

    public string UploadsDirectory
        => _cache.UploadsDirectory;

    // Hosted

    public ulong AssetEnumerationIntervalSeconds
        => _configuration.Profile.AssetEnumerationIntervalSeconds;
}