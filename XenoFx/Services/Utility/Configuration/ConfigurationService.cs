using CSX.Common.IO;
using CSX.Common.IO.Paths;
using Microsoft.Extensions.Logging;
using System.IO;
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

    private readonly ILogger<ConfigurationService> _logger;

    // Data

    private readonly XenoFxProfile _profile;
    private readonly XenoFxOptions _options;
    private readonly string _startupPath;

    private readonly ParameterCache _cache = new();

    // Exposed (TODO decide if this needs to be encapsulated)

    public RuntimeContext RuntimeContext { get; } = new();

    public ConfigurationService(XenoFxConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _logger = logger;

        _startupPath = configuration.StartupPath;
        _profile = configuration.Profile;
        _options = configuration.Options;

        GenerateCache();
    }

    private void GenerateCache()
    {
        _cache.BaseDirectory
            = Path.GetDirectoryName(_startupPath)
            ?? Directory.GetCurrentDirectory();

        _cache.AssetsDirectory = PathExtensions.ResolveCombine(_cache.BaseDirectory, _profile.AssetsDirectory);
    }

    // Core Data

    public PathFilterConfiguration AssetPathFilterConfiguration
        => _profile.AssetFilterConfig.Copy();

    // Derived Data

    public string BaseDirectory
        => _cache.BaseDirectory;

    public string AssetsDirectory
        => _cache.AssetsDirectory;

    // Hosted

    public ulong AssetEnumerationIntervalSeconds
        => _profile.AssetEnumerationIntervalSeconds;
}