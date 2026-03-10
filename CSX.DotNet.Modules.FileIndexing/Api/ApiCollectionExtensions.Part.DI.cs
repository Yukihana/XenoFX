using CSX.DotNet.Modules.FileIndexing.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CSX.DotNet.Modules.FileIndexing.Api;

internal static partial class ApiCollectionExtensions
{
    internal static IServiceCollection AddApiCollection(
        this IServiceCollection services)
    {
        // Control layer
        services.AddSingleton<IFileIndexingControlPanel, ControlPanelService>();

        // Data layer
        services.AddSingleton<IFileIndexingCatalogue, FileCatalogueService>();

        return services;
    }
}