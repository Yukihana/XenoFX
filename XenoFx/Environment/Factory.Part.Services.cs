using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using XenoFx.Database.AssetsDb;
using XenoFx.Database.CacheDb;
using XenoFx.Environment.Configuration;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Api.AssetIngress;
using XenoFx.Services.Api.AssetSearch;
using XenoFx.Services.Api.StateMonitor;
using XenoFx.Services.Background.AssetIndexing;
using XenoFx.Services.Background.AssetQueue;
using XenoFx.Services.Hosted.AssetTracking;
using XenoFx.Services.Processing.AssetIngestion;
using XenoFx.Services.Storage.AssetPresence;
using XenoFx.Services.Utility.Configuration;
using XenoFx.Services.Utility.PathValidator;

namespace XenoFx.Environment;

public static partial class FactoryExtensions
{
    // TODO Documentation: Registers the database contexts with the provided IServiceCollection.
    public static IServiceCollection AddXenoFxDatabases(
        this IServiceCollection services,
        XenoFxConfiguration config,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        services.AddAssetsDbContext(config);
        services.AddCacheDbContext(config);

        return services;
    }

    // TODO Documentation: Registers the framework's services with the provided IServiceCollection.
    public static IServiceCollection AddXenoFxServices(
        this IServiceCollection services,
        XenoFxConfiguration config,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Utility
        services.AddXenoFxConfigurationService(config);
        services.AddSingleton<IPathValidatorService, PathValidatorService>();

        // Storage layer : Unscoped
        services.AddSingleton<IAssetPresenceService, AssetPresenceService>();

        // Storage layer : Scoped

        // Data layer (processing)
        services.AddSingleton<IAssetIngestionService, AssetIngestionService>();

        // Data layer (processing router)
        services.AddSingleton<IAssetIndexingService, AssetIndexingService>();

        // Background layer (long tasks; queue management)
        services.AddSingleton<IAssetQueueService, AssetQueueService>();

        // Hosted layer
        services.AddSingleton<IAssetTrackingService, AssetTrackingService>();

        services.AddHostedService(provider => provider.GetRequiredService<IAssetTrackingService>());

        // Abstraction layer (reader and queue notifier; no write tasks)
        services.AddSingleton<IAssetAbstractionService, AssetAbstractionService>();

        // API layer : Scoped (Avoid unless using a state is fundamental)

        // API layer : Singleton (stateless, thread-safe)
        services.AddSingleton<IAssetSearchService, AssetSearchService>();
        services.AddSingleton<IAssetIngressService, AssetIngressService>();
        services.AddSingleton<IStateMonitorService, StateMonitorService>();

        // Control layer

        // Middlewares
        // Note: They must also be registered with the app
        // e.g. app.UseMiddleware<Middleware>(), where Middleware : IMiddleware

        // Finish
        return services;
    }
}