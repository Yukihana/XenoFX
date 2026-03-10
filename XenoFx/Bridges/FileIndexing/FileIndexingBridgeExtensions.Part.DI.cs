using Microsoft.Extensions.DependencyInjection;
using XenoFx.Bridges.FileIndexing.ControlPanel;
using XenoFx.Bridges.FileIndexing.FileCatalogue;

namespace XenoFx.Bridges.FileIndexing;

internal static partial class FileCatalogueBridgeExtensions
{
    internal static IServiceCollection AddFileCatalogueBridge(
        this IServiceCollection services)
    {
        services.AddSingleton<IControlPanelBridge, ControlPanelBridge>();
        services.AddSingleton<IFileCatalogueBridge, FileCatalogueBridge>();
        return services;
    }
}