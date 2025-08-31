using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Processing.VideoTranscode;

public interface IVideoTranscodeService
{
    Task<bool> ValidateForWebAsync(
        string sourcePath,
        CancellationToken ctoken = default);

    Task<string> TranscodeForWebAsync(
        string sourcePath,
        string id,  // Used for output name
        CancellationToken ctoken = default);
}