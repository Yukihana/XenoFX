using CSX.DotNet.Modules.AuthIpPin.Environment.Configuration;
using Microsoft.Extensions.Logging;

namespace CSX.DotNet.Modules.AuthIpPin.Services.Configuration;

public class ConfigurationService : IConfigurationService
{
    private readonly AuthIpPinConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(
        AuthIpPinConfiguration configuration,
        ILogger<ConfigurationService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Cache configuration values
        NoAuth = _configuration.NoAuth;
    }

    // Public API

    public bool NoAuth { get; }
}