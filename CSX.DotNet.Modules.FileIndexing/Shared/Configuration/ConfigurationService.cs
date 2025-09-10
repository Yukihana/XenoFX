using CSX.DotNet.Modules.FileIndexing.Shared.Configuration.Models;

namespace CSX.DotNet.Modules.FileIndexing.Shared.Configuration;

public sealed class ConfigurationService : IConfigurationService
{
    // Infrastructure

    private readonly IModuleConfiguration _configuration;

    // Lifecycle

    public ConfigurationService(
        IModuleConfiguration configuration)
    {
        _configuration = configuration;
    }

    // API
}