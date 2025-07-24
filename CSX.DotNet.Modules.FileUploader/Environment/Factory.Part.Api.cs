using CSX.DotNet.Modules.FileUploader.Environment.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Environment;

public static partial class Factory
{
    // TODO Documentation: Preferred build method for the DI
    public async static Task<IServiceCollection> AddFileUploaderAsync(
        this IServiceCollection services,
        IFileUploaderOptions options,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        var config = await options.GetConfigAsync(ctoken);

        services.AddFileUploaderServices(config, ctoken);

        return services;
    }

    // Attach middlewares to the pipeline
    [Obsolete("This method is not implemented.", true)]
    public static IApplicationBuilder AddFileUploaderMiddlewares(
        this IApplicationBuilder app)
    {
        throw new NotImplementedException();
        // return app;
    }

    // TODO Documentation: Handles pre-initialization for the framework before consumption.
    [Obsolete("This method is not implemented.", true)]
    public static Task<IServiceProvider> PreInitializeFileUploaderAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ctoken = default)
    {
        throw new NotImplementedException();

        // ctoken.ThrowIfCancellationRequested();

        // return Task.FromResult(serviceProvider);
    }

    // TODO Documentation: Handles activation after preinitializing has completed
    [Obsolete("This method is not implemented.", true)]
    public static IServiceProvider ActivateFileUploader(
        this IServiceProvider provider)
    {
        throw new NotImplementedException();

        // return provider;
    }
}