using CSX.DotNet.Modules.FileIndexing.Shared.Configuration.Models;
using CSX.DotNet.Modules.FileIndexing.Shared.IndicesStorage.IndexStore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileIndexing.Shared.IndicesStorage;

internal static partial class IndexStoreExtensions
{
    internal static IServiceCollection AddIndexStore(
        this IServiceCollection services,
        IModuleConfiguration configuration)
    {
        // Add service wrapper
        services.AddScoped<IIndexStoreService, IndexStoreService>();

        return services;
    }

    internal static Task InitializeIndexStoreAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        // Do not add database initialization here.
        // Db depends on scopes.
        // Let dispatcher/Tenant store handle this.

        return Task.CompletedTask;
        /*
        await serviceProvider
            .InitializeTenantIndicesDatabaseAsync(cancellationToken)
            .ConfigureAwait(false);
        */
    }
}