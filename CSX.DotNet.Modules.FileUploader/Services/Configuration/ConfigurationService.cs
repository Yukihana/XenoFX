using CSX.DotNet.Modules.FileUploader.Environment.Configuration;
using Microsoft.Extensions.Logging;

namespace CSX.DotNet.Modules.FileUploader.Services.Configuration;

public class ConfigurationService : IConfigurationService
{
    private readonly FileUploaderConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(
        FileUploaderConfiguration configuration,
        ILogger<ConfigurationService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Cache parameter values

        UploadsDirectory = _configuration.UploadsDirectory;
        IncompleteFilename = _configuration.IncompleteFilename;
        CompletedFilename = _configuration.CompletedFilename;
        SidecarFilename = _configuration.SidecarFilename;
    }

    // Directories

    public string UploadsDirectory { get; }

    // Filenames

    public string IncompleteFilename { get; } // file.caching
    public string CompletedFilename { get; } // file.cached
    public string SidecarFilename { get; } // metajson.json

    // Parameters

    public int WriteBufferSize => _configuration.WriteBufferSize;
}