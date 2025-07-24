using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Services.UploadApi;

public interface IUploadApiService
{
    Task<string> CacheUploadAsync(
        Stream stream,
        CancellationToken ctoken = default);

    Task<string> AttachSideCarAsync<T>(
        T sidecarObject,
        string pathToCachedFile,
        CancellationToken ctoken = default);

    Task ClearUploadAsync(
        string pathToSidecar,
        CancellationToken ctoken = default);
}