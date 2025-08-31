using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Processing.MediaDetector;

public interface IMediaDetectorService
{
    Task<MediaTypes> DetectMediaTypeAsync(
        string sourcePath,
        CancellationToken ctoken = default);
}