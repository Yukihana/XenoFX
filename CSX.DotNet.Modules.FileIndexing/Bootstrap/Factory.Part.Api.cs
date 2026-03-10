using CSX.DotNet.Modules.FileIndexing.Bootstrap.Configuration;
using CSX.DotNet.Modules.FileIndexing.Bootstrap.Tenants;
using CSX.DotNet.Modules.FileIndexing.Shared.Configuration.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileIndexing.Bootstrap;

public static partial class Factory
{
    public static async Task<IServiceCollection> AddFileIndexingAsync(
        this IServiceCollection services,
        Action<IFileIndexingOptions> configureOptions,
        Func<IFileIndexingProfile, bool> configureProfile,
        Func<ITenantRegistryBuilder, bool> configureTenants,
        CancellationToken ctoken = default)
    {
        ModuleConfiguration configuration = await GetConfigAsync(
            services: services,
            configureOptions: configureOptions,
            configureProfile: configureProfile,
            configureTenants: configureTenants,
            ctoken: ctoken)
            .ConfigureAwait(false);

        services.AddServices(configuration);

        return services;
    }

    public static async Task PreInitializeFileIndexingAsync(
        this IServiceProvider provider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = provider.CreateAsyncScope();
        await Task.Yield();
    }
}