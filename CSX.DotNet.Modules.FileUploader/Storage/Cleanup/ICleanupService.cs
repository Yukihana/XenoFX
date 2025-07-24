using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Storage.Cleanup;

public interface ICleanupService
{
    Task CleanupUploadAsync(
        string sidecarPath,
        CancellationToken ctoken = default);
}