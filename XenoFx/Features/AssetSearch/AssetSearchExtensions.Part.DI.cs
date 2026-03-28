using Microsoft.Extensions.DependencyInjection;

namespace XenoFx.Features.AssetSearch;

internal static partial class AssetSearchExtensions
{
    internal static IServiceCollection AddAssetSearch(
        this IServiceCollection services)
    {
        services.AddSingleton<IAssetSearchService, AssetSearchService>();

        return services;
    }
}