using CSX.DotNet.Modules.FileIndexing.Api;
using CSX.DotNet.Modules.FileIndexing.Shared.Configuration;
using CSX.DotNet.Modules.FileIndexing.Shared.Configuration.Models;
using CSX.DotNet.Modules.FileIndexing.Shared.PathValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CSX.DotNet.Modules.FileIndexing.Bootstrap;

public static partial class Factory
{
    internal static IServiceCollection AddServices(
        this IServiceCollection services,
        ModuleConfiguration configuration)
    {
        services.AddConfiguration(configuration);

        services.AddPathValidation();

        // API
        services.AddApiCollection();

        return services;
    }
}