using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using XenoFx.Database.Cache;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Api.AssetSearch;
using XenoFx.Services.Api.AssetUpload;
using XenoFx.Services.Api.StateMonitor;
using XenoFx.Services.Background.AssetIndexing;
using XenoFx.Services.Hosted.AssetEnumeration;
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

        // Cache
        services.AddCacheDbContextUsingSqlite(xfc);

        // Assets
        // services.AddAssetsDbContextUsingSqlite(xfc); // Not implemented yet

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

        // Analysis layer

        // Abstraction layer
        services.AddSingleton<IAssetAbstractionService, AssetAbstractionService>();

        // Processing layer
        services.AddSingleton<IAssetIndexingService, AssetIndexingService>();

        // Hosted layer
        services.AddSingleton<IAssetTrackingService, AssetTrackingService>();
        services.AddSingleton<IAssetEnumerationService, AssetEnumerationService>();

        services.AddHostedService(provider => provider.GetRequiredService<IAssetTrackingService>());
        services.AddHostedService(provider => provider.GetRequiredService<IAssetEnumerationService>());

        // API layer
        services.AddSingleton<IAssetSearchService, AssetSearchService>();
        services.AddSingleton<IAssetUploadService, AssetUploadService>();
        services.AddSingleton<IStateMonitorService, StateMonitorService>();

        // Control layer

        // Finish
        return services;
    }
}