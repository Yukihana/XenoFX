using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.Cache;

namespace XenoFx.Environment;

public static partial class Factory
{
    // TODO Documentation: Preferred build method for XenoFx
    public async static Task<IServiceCollection> AddXenoFxAsync(
        this IServiceCollection services,
        IConfiguration configuration,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        XenoFxConfiguration xfc = await configuration.GetXenoFxConfigurationAsync(ctoken: ctoken);

        services.AddXenoFxDatabases(xfc, ctoken);
        services.AddXenoFxServices(xfc, ctoken);

        return services;
    }

    // TODO Documentation: Handles pre-initialization for the framework before consumption.
    public async static Task<IServiceProvider> PreInitializeXenoFxAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ctoken = default)
    {
        // Validate database connections
        await serviceProvider.InitializeCacheDbContextAsync(ctoken);

        // Warm up the file tracker

        // Detect assets and run quick-mode integrity tests

        // Register available assets for consumption

        return serviceProvider;
    }

    // TODO Documentation: Activates parallel subroutines
    public static IServiceProvider Activate(this IServiceProvider provider)
    {
        // Start database connections

        // Warm up the file tracker

        // Detect assets and run quick-mode integrity tests

        // Register available assets for consumption

        return provider;
    }
}