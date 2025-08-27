using Microsoft.Extensions.DependencyInjection;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Decompression;

public static partial class DecompressionExtensions
{
    internal static IServiceCollection RegisterDecompressionService(
        this IServiceCollection services)
    {
        services.AddSingleton<IDecompressionService, DecompressionService>();
        return services;
    }
}