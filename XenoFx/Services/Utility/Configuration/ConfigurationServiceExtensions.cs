using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XenoFx.Environment;

namespace XenoFx.Services.Utility.Configuration;

public static partial class ConfigurationServiceExtensions
{
    // DI registration preset
    public static IServiceCollection AddXenoFxConfigurationService(this IServiceCollection services, XenoFxConfiguration configuration)
    {
        return services.AddSingleton<IConfigurationService, ConfigurationService>(serviceProvider => new(
            configuration: configuration,
            logger: serviceProvider.GetRequiredService<ILogger<ConfigurationService>>()));
    }
}