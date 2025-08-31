using Microsoft.Extensions.DependencyInjection;

namespace XenoFx.Services.Processing.VideoTranscode;

public static partial class VideoTranscodeExtensions
{
    public static IServiceCollection AddVideoTranscode(
        this IServiceCollection services)
    {
        services.AddSingleton<IVideoTranscodeService, VideoTranscodeService>();

        return services;
    }
}