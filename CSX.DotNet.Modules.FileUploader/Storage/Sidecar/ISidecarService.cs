using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Storage.Sidecar;

/// <summary>
/// Adds metadata beside the cached file
/// </summary>
public interface ISidecarService
{
    Task<string> WriteSidecarAsync<T>(
        T sidecar,
        string pathToCachedFile,
        CancellationToken ctoken = default);
}