using CSX.Common.Data.Guids;
using CSX.DotNet.Modules.FileUploader.Services.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Storage.UploadCache;

public partial class UploadCacheService : IUploadCacheService
{
    // Infrastructure

    private readonly IConfigurationService _configuration;
    private readonly ILogger<UploadCacheService> _logger;

    // Lifecycle

    public UploadCacheService(
        IConfigurationService configuration,
        ILogger<UploadCacheService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // Computed

    public string UploadsDirectory
        => _configuration.UploadsDirectory;

    public int WriteBufferSize
        => _configuration.WriteBufferSize;

    // Api

    public async Task<string> SaveToFileAsync(
        Stream sourceStream,
        CancellationToken ctoken = default)
    {
        // Secure a directory in uploads
        string cacheDirectory = CreateCacheDirectory();

        // Write the file
        string filePath = await WriteToCacheAsync(
            sourceStream: sourceStream,
            cacheDirectory: cacheDirectory,
            ctoken: ctoken);

        // Return the filepath
        return filePath;
    }

    // Internal
    private string CreateCacheDirectory()
    {
        while (true)
        {
            try
            {
                Guid guid = DMC212710Guid.FromUtcNow();
                string directory = Path.Combine(
                    UploadsDirectory,
                    guid.ToString());

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    return directory;
                }
            }
            catch (IOException)
            { }
        }
    }

    private async Task<string> WriteToCacheAsync(
        Stream sourceStream,
        string cacheDirectory,
        CancellationToken ctoken)
    {
        string filepath = Path.Combine(
            cacheDirectory,
            _configuration.IncompleteFilename);

        using FileStream fs = File.Create(
            path: filepath,
            bufferSize: _configuration.WriteBufferSize,
            options: FileOptions.SequentialScan);

        await sourceStream.CopyToAsync(fs, ctoken);

        return filepath;
    }
}