using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace CSX.DotNet.Modules.FileIndexing.Shared.Configuration;

internal static partial class ConfigurationExtensions
{
    internal static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        Models.IModuleConfiguration configuration,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        var service = new ConfigurationService(configuration);
        services.AddSingleton<IConfigurationService>(service);
        return services;
    }
}