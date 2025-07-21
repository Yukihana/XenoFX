using CSX.DotNet.Modules.AuthIpPin.Environment.Configuration;
using CSX.DotNet.Modules.AuthIpPin.Middlewares;
using CSX.DotNet.Modules.AuthIpPin.Storage.AuthDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.AuthIpPin.Environment;

public static partial class Factory
{
    // TODO Documentation: Preferred build method for the DI
    public async static Task<IServiceCollection> AddAuthIpPinAsync(
        this IServiceCollection services,
        IAuthIpPinOptions options,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        var config = await options.GetConfigAsync(ctoken);

        services.AddAuthIpPinDatabases(config, ctoken);
        services.AddAuthIpPinServices(config, ctoken);

        return services;
    }

    // Attach middlewares to the pipeline
    public static IApplicationBuilder AddAuthIpPinMiddlewares(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthIpPinMiddleware>();
    }

    // TODO Documentation: Handles pre-initialization for the framework before consumption.
    public async static Task<IServiceProvider> PreInitializeAuthIpPinAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ctoken = default)
    {
        // Auto-upgrade and ensure database
        await serviceProvider.InitializeAuthDbContextAsync(ctoken);

        return serviceProvider;
    }

    // TODO Documentation: Handles activation after preinitializing has completed
    public static IServiceProvider Activate(
        this IServiceProvider provider)
    {
        return provider;
    }
}