using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.Abstractions;

public interface IFFMpegProvider
{
    // Used as the API contract for
    // CSX.DotNet.Modules.FFMpeg.Provisioning

    Task PreInitializeFFMpegAsync(
        CancellationToken ctoken = default);

    Task<string> AcquireFFMpegAsync(
        CancellationToken ctoken = default);

    Task<string> AcquireFFProbeAsync(
        CancellationToken ctoken = default);

    Task<string> AcquireFFPlayAsync(
        CancellationToken token = default);
}