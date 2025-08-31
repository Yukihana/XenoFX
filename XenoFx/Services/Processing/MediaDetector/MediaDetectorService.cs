using CSX.DotNet.Common.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Processing.MediaDetector;

public class MediaDetectorService : IMediaDetectorService
{
    // Infrastructure

    private readonly IFFMpegProvider _ffmpegProvider;
    private readonly ILogger<MediaDetectorService> _logger;

    // Lifecycle

    public MediaDetectorService(
        IFFMpegProvider ffmpegProvider,
        ILogger<MediaDetectorService> logger)
    {
        _ffmpegProvider = ffmpegProvider;
        _logger = logger;
    }

    // API

    public async Task<MediaTypes> DetectMediaTypeAsync(
        string sourcePath,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Analyse using ffprobe
        await Task.Yield();

        return MediaTypes.Video; // Temporary
    }
}