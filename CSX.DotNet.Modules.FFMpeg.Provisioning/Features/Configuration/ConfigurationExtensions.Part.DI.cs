using CSX.DotNet.Modules.FFMpeg.Provisioning.Environment.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Configuration;

public static partial class ConfigurationExtensions
{
    public static IServiceCollection AddConfigurationService(
        this IServiceCollection services,
        ModuleConfiguration configuration)
    {
        return services.AddSingleton<IConfigurationService, ConfigurationService>(serviceProvider => new(
            configuration: configuration,
            logger: serviceProvider.GetRequiredService<ILogger<ConfigurationService>>()));
    }
}