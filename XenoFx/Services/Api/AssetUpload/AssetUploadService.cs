using HeyRed.Mime;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment;
using XenoFx.Services.Api.AssetUpload.DTOs;
using XenoFx.Services.Background.AssetIndexing;
using XenoFx.Services.Background.AssetIndexing.DTOs;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Api.AssetUpload;

public sealed partial class AssetUploadService : IAssetUploadService
{
    private const string UnsupportedUploadMessage = "Unsupported file type uploaded.";

    // Infrastructure

    private readonly IAssetIndexingService _assetIndexing;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<AssetUploadService> _logger;

    // Lifetime

    public AssetUploadService(
        IAssetIndexingService assetIndexing,
        IConfigurationService configurationService,
        ILogger<AssetUploadService> logger)
    {
        _assetIndexing = assetIndexing;
        _configurationService = configurationService;
        _logger = logger;
    }

    // Derived

    public string AssetUploadDirectoryPath
        => _configurationService.AssetUploadDirectory;

    public string AssetsDirectoryPath
        => _configurationService.AssetsDirectory;

    // API for Controller

    public async Task<AssetUploadResult> RegisterUploadAsync(AssetUploadRequest request, CancellationToken ctoken = default)
    {
        AssetUploadResult result = new();

        try
        {
            string tempPath = await WriteToTemporaryFileAsync(request.DataStream, ctoken);
            string finalPath = await MoveToFinalPathAsync(tempPath, request, ctoken);

            await RegisterWithIndexingServiceAsync(request, finalPath, ctoken);

            result.Message = "File uploaded successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register upload.");

            if (ex.Message is UnsupportedUploadMessage)
                result.Message = UnsupportedUploadMessage;
            else
                result.Message = "Failed to upload file.";
        }

        return result;
    }

    // Internal

    private FileStream GetTemporaryFileStream(CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        string uploadDirectory = AssetUploadDirectoryPath;
        string uploadExtension = XenoFxConstants.DefaultUploadExtension;

        // Ensure target location.
        try
        {
            Directory.CreateDirectory(uploadDirectory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create the upload directory.");
            throw; // Fail fast if directory creation is impossible
        }

        // Attempt to create a new file.
        while (true)
        {
            ctoken.ThrowIfCancellationRequested();

            string guid = Guid.NewGuid().ToString();    // ensures uniqueness
            string tempPath = Path.Combine(
                uploadDirectory,
                guid + uploadExtension);

            try
            {
                return new FileStream(
                    path: tempPath,
                    mode: FileMode.CreateNew, // avoids pre-check
                    access: FileAccess.ReadWrite,
                    share: FileShare.None,
                    bufferSize: XenoFxConstants.WriteBufferSize,
                    options: FileOptions.Asynchronous);
            }
            catch (IOException ex) when (ex.HResult == -2147024816) // ERROR_FILE_EXISTS
            { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a temporary file.");
                throw; // Bubble up fatal errors
            }
        }
    }

    private async Task<string> WriteToTemporaryFileAsync(Stream sourceStream, CancellationToken ctoken = default)
    {
        string tempPath = string.Empty;
        try
        {
            using FileStream tempFile = GetTemporaryFileStream(ctoken);
            tempPath = tempFile.Name;
            await sourceStream.CopyToAsync(tempFile, ctoken);
            await tempFile.FlushAsync(ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write to storage.");

            // Cleanup on failure.
            if (!string.IsNullOrEmpty(tempPath) && File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogWarning(cleanupEx, "Failed to delete temporary file.");
                }
            }

            throw;
        }

        return tempPath;
    }

    private async Task<string> MoveToFinalPathAsync(string tempPath, AssetUploadRequest request, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Determine and validate based on the exact nature of the file.
        string extension = await DetermineExtensionAsync(tempPath, request, ctoken);

        // Figure out the naming of the file.
        string title = Path.GetInvalidFileNameChars().Aggregate(request.Title, (current, c) => current.Replace(c, '_'));

        bool emptyTitle = string.IsNullOrWhiteSpace(title);
        string filename = emptyTitle ? $"{Guid.NewGuid()}" : title;

        // Try for each possible name until one is available.
        while (true)
        {
            ctoken.ThrowIfCancellationRequested();

            try
            {
                string selectedPath = Path.Combine(
                    AssetUploadDirectoryPath,
                    $"{filename}.{extension}");

                if (!File.Exists(selectedPath))
                {
                    File.Move(tempPath, selectedPath);
                    return selectedPath;
                }

                // prep new for next iteration
                string guid = Guid.NewGuid().ToString();
                filename = emptyTitle ? guid : $"{title}_{guid}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to finalize upload.");
                throw;
            }
        }
    }

    private async Task<string> DetermineExtensionAsync(string tempPath, AssetUploadRequest request, CancellationToken ctoken = default)
    {
        // If available, determine the extension from the filename
        // Use content-type as a fallback
        // Else use MimeDetective
        // Preferably move this into asset analysis.
        // Store mime/file type in database
        // Discard file from there, not here.
        // Instead of using an upload extension, use extension as is.
        // Set up an exclusion-list for files being uploaded, which Tracker ignores.
        // Once completed uploading, pass the filepath to the tracker for indexing.

        await Task.Yield();
        return MimeTypesMap.GetExtension(request.ContentMimeType);
    }

    private async Task RegisterWithIndexingServiceAsync(AssetUploadRequest request, string finalPath, CancellationToken ctoken)
    {
        string relativePath = Path.GetRelativePath(
            AssetsDirectoryPath,
            finalPath);

        UploadedAssetIndexingInfo dto = request.ToIndexingInfo(relativePath);

        await _assetIndexing.OnFileUploadedAsync(dto, ctoken);
    }
}