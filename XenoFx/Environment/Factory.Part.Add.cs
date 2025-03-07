using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Automation.AssetTracking;
using XenoFx.Services.Storage.AssetEnumeration;
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

        // Analysis layer

        // Abstraction layer

        // Processing layer

        // API layer

        // Automation
        services.AddSingleton<IAssetTrackingService, AssetTrackingService>();
        services.AddSingleton<IAssetEnumerationService, AssetEnumerationService>();

        // Finish
        return services;
    }
}