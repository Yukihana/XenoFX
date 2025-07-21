using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.AssetsDb;
using XenoFx.Database.CacheDb;
using XenoFx.Environment.Configuration;

namespace XenoFx.Environment;

public static partial class Factory
{
    // TODO Documentation: Preferred build method for the DI
    public async static Task<IServiceCollection> AddXenoFxAsync(
        this IServiceCollection services,
        IXenoFxOptions options,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        var config = await options.GetConfigAsync(ctoken: ctoken);

        services.AddXenoFxDatabases(config, ctoken);
        services.AddXenoFxServices(config, ctoken);

        return services;
    }

    // Attach middlewares to the pipeline
    public static IApplicationBuilder AddXenoFxMiddlewares(this IApplicationBuilder app)
    {
        return app;
    }

    // TODO Documentation: Handles pre-initialization for the framework before consumption.
    public async static Task<IServiceProvider> PreInitializeXenoFxAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ctoken = default)
    {
        // Auto-upgrade and ensure database
        await serviceProvider.InitializeAssetsDbContextAsync(ctoken);
        await serviceProvider.InitializeCacheDbContextAsync(ctoken);

        return serviceProvider;
    }

    // TODO Documentation: Handles activation after preinitializing has completed
    public static IServiceProvider Activate(
        this IServiceProvider provider)
    {
        return provider;
    }
}