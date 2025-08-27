using CSX.DotNet.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Acquisition;

public static partial class AcquisitionExtensions
{
    internal static IServiceCollection RegisterAcquisitionService(
        this IServiceCollection services)
    {
        services.AddSingleton<IAcquisitionService, AcquisitionService>();

        // abstraction for cross-module injection
        services.AddSingleton<IFFMpegProvider>(sp => sp.GetRequiredService<IAcquisitionService>());

        return services;
    }
}