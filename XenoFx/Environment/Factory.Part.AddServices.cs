using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Api.AssetSearch;
using XenoFx.Services.AssetPresence;
using XenoFx.Services.Background.AssetIndexing;
using XenoFx.Services.Hosted.AssetEnumeration;
using XenoFx.Services.Hosted.AssetTracking;
using XenoFx.Services.Utility.Configuration;
using XenoFx.Services.Utility.PathValidator;

namespace XenoFx.Environment;

public static partial class FactoryExtensions
{
    //
    public async static Task<IServiceCollection> AddXenoFx(
        this IServiceCollection services,
        IConfiguration configuration,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        XenoFxConfiguration xfc = await configuration.GetXenoFxConfiguration(ctoken: ctoken);
        return services.AddXenoFx(xfc, ctoken);
    }

    // TODO Documentation: Registers the framework's services with the provided IServiceCollection.
    public static IServiceCollection AddXenoFx(
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
        services.AddHostedService(provider => provider.GetRequiredService<IAssetTrackingService>());

        services.AddSingleton<IAssetEnumerationService, AssetEnumerationService>();
        services.AddHostedService(provider => provider.GetRequiredService<IAssetEnumerationService>());

        // API layer
        services.AddSingleton<IAssetSearchService, AssetSearchService>();

        // Control layer

        // Finish
        return services;
    }
}