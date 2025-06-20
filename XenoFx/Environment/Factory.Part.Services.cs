using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using XenoFx.Database.AssetsDb;
using XenoFx.Database.CacheDb;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Api.AssetSearch;
using XenoFx.Services.Api.AssetUpload;
using XenoFx.Services.Api.StateMonitor;
using XenoFx.Services.Background.AssetIndexing;
using XenoFx.Services.Background.AssetQueue;
using XenoFx.Services.Hosted.AssetTracking;
using XenoFx.Services.Storage.AssetPresence;
using XenoFx.Services.Utility.Configuration;
using XenoFx.Services.Utility.PathValidator;

namespace XenoFx.Environment;

public static partial class FactoryExtensions
{
    // TODO Documentation: Registers the database contexts with the provided IServiceCollection.
    public static IServiceCollection AddXenoFxDatabases(
        this IServiceCollection services,
        XenoFxConfiguration xfc,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Databases
        services.AddAssetsDbContextUsingSqlite(xfc);
        services.AddCacheDbContextUsingSqlite(xfc);

        return services;
    }

    // TODO Documentation: Registers the framework's services with the provided IServiceCollection.
    public static IServiceCollection AddXenoFxServices(
        this IServiceCollection services,
        XenoFxConfiguration xfc,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Utility
        services.AddXenoFxConfigurationService(xfc);
        services.AddSingleton<IPathValidatorService, PathValidatorService>();

        // Storage layer
        services.AddSingleton<IAssetPresenceService, AssetPresenceService>();

        // Data layer (processing)

        // Data layer (processing router)
        services.AddSingleton<IAssetIndexingService, AssetIndexingService>();

        // Background layer (long tasks; queue management)
        services.AddSingleton<IAssetQueueService, AssetQueueService>();

        // Hosted layer
        services.AddSingleton<IAssetTrackingService, AssetTrackingService>();

        services.AddHostedService(provider => provider.GetRequiredService<IAssetTrackingService>());

        // Abstraction layer (reader and queue notifier; no write tasks)
        services.AddSingleton<IAssetAbstractionService, AssetAbstractionService>();

        // API layer (TODO Ensure all services here are changed to scoped)
        services.AddScoped<IAssetSearchService, AssetSearchService>();
        services.AddSingleton<IAssetUploadService, AssetUploadService>();
        services.AddSingleton<IStateMonitorService, StateMonitorService>();

        // Control layer

        // Finish
        return services;
    }
}