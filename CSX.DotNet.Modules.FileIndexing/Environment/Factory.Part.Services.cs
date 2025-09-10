using CSX.DotNet.Modules.FileIndexing.Shared.Configuration;
using CSX.DotNet.Modules.FileIndexing.Shared.Configuration.Models;
using CSX.DotNet.Modules.FileIndexing.Shared.PathValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CSX.DotNet.Modules.FileIndexing.Environment;

public static partial class Factory
{
    internal static IServiceCollection AddServices(
        IServiceCollection services,
        ModuleConfiguration configuration)
    {
        services.AddConfiguration(configuration);

        services.AddPathValidation();

        return services;
    }
}