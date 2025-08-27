using Microsoft.Extensions.DependencyInjection;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Download;

public static partial class DownloadExtensions
{
    internal static IServiceCollection RegisterDownloadService(
        this IServiceCollection services)
    {
        services.AddSingleton<IDownloadService, DownloadService>();
        return services;
    }
}