using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Storage.UploadCache;

/// <summary>
/// Tasked with saving the stream to cache with a unique id
/// </summary>
public interface IUploadCacheService
{
    Task<string> SaveToFileAsync(
        Stream sourceStream,
        CancellationToken ctoken = default);
}