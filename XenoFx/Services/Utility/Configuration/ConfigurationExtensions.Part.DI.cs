using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XenoFx.Environment.Configuration;

namespace XenoFx.Services.Utility.Configuration;

public static partial class ConfigurationExtensions
{
    internal static IServiceCollection AddConfigurationService(
        this IServiceCollection services,
        XenoFxConfiguration configuration)
    {
        return services.AddSingleton<IConfigurationService, ConfigurationService>(serviceProvider => new(
            configuration: configuration,
            logger: serviceProvider.GetRequiredService<ILogger<ConfigurationService>>()));
    }
}