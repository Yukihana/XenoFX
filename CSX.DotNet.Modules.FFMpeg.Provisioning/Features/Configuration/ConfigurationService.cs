using CSX.DotNet.Modules.FFMpeg.Provisioning.Environment.Configuration;
using Microsoft.Extensions.Logging;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Configuration;

public partial class ConfigurationService : IConfigurationService
{
    // Infrastructure

    private readonly ModuleConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(
        ModuleConfiguration configuration,
        ILogger<ConfigurationService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Snapshot and Cache ----
        // Note: Resolve in configuration. Only cache here.

        // Shared
        BinariesDirectory = _configuration.BinariesDirectory;
        SharedCacheDirectory = _configuration.SharedCacheDirectory;
    }

    // Shared

    public string BinariesDirectory { get; }
    public string SharedCacheDirectory { get; }
}