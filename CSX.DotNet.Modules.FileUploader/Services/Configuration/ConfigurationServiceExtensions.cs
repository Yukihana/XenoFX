using CSX.DotNet.Modules.FileUploader.Environment.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CSX.DotNet.Modules.FileUploader.Services.Configuration;

public static partial class ConfigurationServiceExtensions
{
    public static IServiceCollection AddConfigurationService(
        this IServiceCollection services,
        FileUploaderConfiguration configuration)
    {
        return services.AddSingleton<IConfigurationService, ConfigurationService>(serviceProvider => new(
            configuration: configuration,
            logger: serviceProvider.GetRequiredService<ILogger<ConfigurationService>>()));
    }
}