using CSX.DotNet.Common.IO.Paths;
using CSX.DotNet.Modules.FileUploader.Services.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Storage.Cleanup;

public class CleanupService : ICleanupService
{
    // Infrastructure

    private readonly IConfigurationService _configuration;
    private readonly ILogger<CleanupService> _logger;

    // Lifecycle

    public CleanupService(
        IConfigurationService configuration,
        ILogger<CleanupService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // Parameters

    public string UploadsDirectory
        => _configuration.UploadsDirectory;

    public string CompletedFilename
        => _configuration.CompletedFilename;

    public string IncompleteFilename
        => _configuration.IncompleteFilename;

    public string SidecarFilename
        => _configuration.SidecarFilename;

    // Api

    public Task CleanupUploadAsync(
        string sidecarPath,
        CancellationToken ctoken = default)
    {
        // Validate if parameters are within bounds
        sidecarPath = Path.GetFullPath(sidecarPath);
        bool isWithin = PathExtensions.IsPathWithinDirectory(sidecarPath, UploadsDirectory);
        if (!isWithin)
            throw new UnauthorizedAccessException("Provided path is not within bounds.");
        if (!Path.GetFileName(sidecarPath).Equals(SidecarFilename, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Invalid filename provided for cleanup.");

        // Warn and bail if directory doesn't exist
        string directory
            = Path.GetDirectoryName(sidecarPath)
            ?? throw new InvalidOperationException("Unable to determine the current directory.");
        if (!Directory.Exists(directory))
        {
            _logger.LogWarning("Cleanup was unable to find the upload cache at {path}", directory);
            return Task.CompletedTask;
        }

        // Check if the cached file wasn't cleared
        string cachedFile = Path.Combine(
            directory,
            CompletedFilename);
        if (File.Exists(cachedFile))
            throw new InvalidOperationException($"Cleanup cannot proceed with cached file within: {cachedFile}");

        // Clear the sidecar or warn if it doesn't exist

        if (File.Exists(sidecarPath))
            File.Delete(sidecarPath);
        else
            _logger.LogWarning("Cleanup was unable to find an upload sidecar file at {path}", sidecarPath);

        // Bail if the cache directory isn't empty
        if (Directory.EnumerateFileSystemEntries(directory, "*", SearchOption.AllDirectories).Any())
        {
            _logger.LogWarning("Upload cache directory is not empty. Unable to delete: {directory}", directory);
            return Task.CompletedTask;
        }

        // Attempt to purge
        try
        {
            Directory.Delete(directory);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete upload cache directory: {dir}", directory);
            // Don't rethrow — it's not critical.
        }

        // Since no async method is awaited above
        return Task.CompletedTask;
    }
}