using Microsoft.Extensions.DependencyInjection;

namespace XenoFx.Services.Hosted.AssetTracking;

internal static partial class AssetTrackingExtensions
{
    /// <summary>
    /// Registers the AssetTrackingService with the provided IServiceCollection.
    /// </summary>
    public static IServiceCollection AddAssetTracking(
        this IServiceCollection services)
    {
        services.AddHostedService<AssetTrackingService>();
        services.AddSingleton<IAssetTrackingService>(
            sp => sp.GetRequiredService<AssetTrackingService>());

        return services;
    }
}