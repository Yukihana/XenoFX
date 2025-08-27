using CSX.DotNet.Modules.FFMpeg.Provisioning.Environment.Configuration;
using CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Acquisition;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Environment;

public static partial class Factory
{
    public async static Task<IServiceCollection> AddFFMpegProvisioningAsync(
        this IServiceCollection services,
        IFFMpegProvisioningOptions options,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        var config = await options.GetConfigAsync(ctoken: ctoken);

        services.AddFFMpegProvisioningDependencies(ctoken);
        // services.AddFFMpegProvisioningDatabases(config, ctoken); // Not Implemented
        services.AddFFMpegProvisioningServices(config, ctoken);

        return services;
    }

    // Attach middlewares to the pipeline
    [Obsolete("Not Implemented")]
    public static IApplicationBuilder AddFFMpegProvisioningMiddlewares(
        this IApplicationBuilder app)
    {
        throw new NotImplementedException();
    }

    // TODO Documentation: Handles pre-initialization for the framework before consumption.
    public static async Task<IServiceProvider> PreInitializeFFMpegProvisioningAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ctoken = default)
    {
        var acq = serviceProvider.GetRequiredService<IAcquisitionService>();
        await acq.PreInitializeFFMpegAsync(ctoken);
        return serviceProvider;
    }

    // TODO Documentation: Handles activation after preinitializing has completed
    [Obsolete("Not Implemented")]
    public static IServiceProvider ActivateFFMpegProvisioning(
        this IServiceProvider provider)
    {
        throw new NotImplementedException();
    }
}