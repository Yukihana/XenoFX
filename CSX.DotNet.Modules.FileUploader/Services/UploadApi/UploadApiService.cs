using CSX.DotNet.Modules.FileUploader.Services.Configuration;
using CSX.DotNet.Modules.FileUploader.Storage.Cleanup;
using CSX.DotNet.Modules.FileUploader.Storage.Sidecar;
using CSX.DotNet.Modules.FileUploader.Storage.UploadCache;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Services.UploadApi;

public sealed partial class UploadApiService : IUploadApiService
{
    // Infrastructure

    private readonly IUploadCacheService _uploadCache;
    private readonly ISidecarService _sidecar;
    private readonly ICleanupService _cleanup;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<UploadApiService> _logger;

    // Lifetime

    public UploadApiService(
        IUploadCacheService uploadCache,
        ISidecarService sidecar,
        ICleanupService cleanup,
        IConfigurationService configuration,
        ILogger<UploadApiService> logger)
    {
        _uploadCache = uploadCache;
        _sidecar = sidecar;
        _cleanup = cleanup;
        _configuration = configuration;
        _logger = logger;
    }

    // Derived

    // API for Controller

    public async Task<string> CacheUploadAsync(
        Stream stream,
        CancellationToken ctoken = default)
    {
        // Saves the file to disk in a unique directory
        // Returns the path to the file
        return await _uploadCache.SaveToFileAsync(
            sourceStream: stream,
            ctoken: ctoken);
    }

    public async Task<string> AttachSideCarAsync<T>(
        T sidecar,
        string pathToCachedFile,
        CancellationToken ctoken = default)
    {
        // Writes sidecar beside cached file
        // Returns path to the sidecar
        return await _sidecar.WriteSidecarAsync(
            sidecar: sidecar,
            pathToCachedFile: pathToCachedFile,
            ctoken: ctoken);
    }

    public async Task ClearUploadAsync(
        string sidecarPath,
        CancellationToken ctoken = default)
    {
        // Deletes the sidecar and its directory
        // Throws if the cached file is still in there.
        await _cleanup.CleanupUploadAsync(
            sidecarPath: sidecarPath,
            ctoken: ctoken);
    }
}