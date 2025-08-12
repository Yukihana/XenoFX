using CSX.DotNet.Modules.AuthIpPin.Environment.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CSX.DotNet.Modules.AuthIpPin.Services.Configuration;

public static partial class ConfigurationExtensions
{
    // DI registration preset
    public static IServiceCollection AddAuthIpPinConfigurationService(
        this IServiceCollection services,
        AuthIpPinConfiguration configuration)
    {
        return services.AddSingleton<IConfigurationService, ConfigurationService>(serviceProvider => new(
            configuration: configuration,
            logger: serviceProvider.GetRequiredService<ILogger<ConfigurationService>>()));
    }
}