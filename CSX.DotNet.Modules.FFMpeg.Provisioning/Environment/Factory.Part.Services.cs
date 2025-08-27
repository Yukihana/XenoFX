using CSX.DotNet.Modules.FFMpeg.Provisioning.Environment.Configuration;
using CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Acquisition;
using CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Configuration;
using CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Decompression;
using CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Download;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Environment;

public static partial class Factory
{
    public static IServiceCollection AddFFMpegProvisioningDependencies(
        this IServiceCollection services,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        services.AddHttpClient();

        return services;
    }

    [Obsolete("Not Implemented")]
    public static IServiceCollection AddFFMpegProvisioningDatabases(
        this IServiceCollection services,
        ModuleConfiguration config,
        CancellationToken ctoken = default)
    {
        throw new NotImplementedException();
    }

    public static IServiceCollection AddFFMpegProvisioningServices(
        this IServiceCollection services,
        ModuleConfiguration configuration,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Utility
        services.AddConfigurationService(configuration);

        services.RegisterDownloadService();
        services.RegisterDecompressionService();

        // Public API entry-point
        services.RegisterAcquisitionService();

        return services;
    }
}