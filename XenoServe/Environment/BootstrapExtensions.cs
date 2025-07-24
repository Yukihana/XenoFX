using Microsoft.Extensions.DependencyInjection;
using XenoServe.Features.AssetUpload;

namespace XenoServe.Environment;

public static class BootstrapExtensions
{
    // Until automatic scanning of orchestrators is possible

    public static IServiceCollection AddOrchestrators(
        this IServiceCollection services)
    {
        services.AddSingleton<IAssetUploadOrchestrator, AssetUploadOrchestrator>();

        return services;
    }
}