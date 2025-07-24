using CSX.Common.Data.Text.Json;
using CSX.Common.IO.Paths;
using CSX.DotNet.Modules.FileUploader.Services.Configuration;
using CSX.DotNet.Modules.FileUploader.Services.UploadApi;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Storage.Sidecar;

public partial class SidecarService : ISidecarService
{
    // Infrastructure

    private readonly IConfigurationService _configuration;
    private readonly ILogger<UploadApiService> _logger;

    // Lifecycle

    public SidecarService(
        IConfigurationService configuration,
        ILogger<UploadApiService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // Parameters

    public string UploadsDirectory
        => _configuration.UploadsDirectory;

    // Api

    public async Task<string> WriteSidecarAsync<T>(
        T sidecar,
        string pathToCachedFile,
        CancellationToken ctoken = default)
    {
        // Note: no need to validate whether the cached file actually exists

        bool isWithin = PathExtensions.IsPathWithinDirectory(pathToCachedFile, UploadsDirectory);
        if (!isWithin)
            throw new InvalidOperationException("Provided path is not within bounds");

        string directory
            = Path.GetDirectoryName(pathToCachedFile)
            ?? throw new InvalidOperationException("Cannot infer directory from provided path.");

        string sidecarPath = Path.Combine(
            directory,
            _configuration.SidecarFilename);

        using FileStream fs = File.Create(
            path: sidecarPath,
            bufferSize: _configuration.WriteBufferSize);

        await JsonSerializer.SerializeAsync(
            utf8Json: fs,
            value: sidecar,
            options: JsonOptionsUtilities.HumanReadableJsonOptions,
            cancellationToken: ctoken);

        return sidecarPath;
    }
}