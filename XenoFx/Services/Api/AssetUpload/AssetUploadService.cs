using CSX.Common.Data.Events;
using CSX.Common.Data.Exceptions;
using HeyRed.Mime;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment;
using XenoFx.Services.Api.AssetUpload.DTOs;
using XenoFx.Services.Background.AssetQueue;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Api.AssetUpload;

public sealed partial class AssetUploadService : IAssetUploadService
{
    // Infrastructure

    private readonly IAssetQueueService _assetQueue;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetUploadService> _logger;

    // Lifetime

    public AssetUploadService(
        IAssetQueueService assetQueue,
        IConfigurationService configurationService,
        ILogger<AssetUploadService> logger)
    {
        _assetQueue = assetQueue;
        _configuration = configurationService;
        _logger = logger;
    }

    // Derived

    public string UploadCacheDirectory
        => _configuration.UploadCacheDirectory;

    // API for Controller

    public async Task<AssetUploadResult> RegisterUploadAsync(
        AssetUploadRequest request,
        CancellationToken ctoken = default)
    {
        AssetUploadResult result = new();

        try
        {
            string tempPath = await WriteToTemporaryFileAsync(request.DataStream, ctoken);

            // Setup arguments
            FileUploadedEventArgs eventArgs = new(temporaryFileFullPath: tempPath)
            {
                ReportedFilename = request.Filename,
                ReportedMimeType = request.ContentMimeType,

                Title = request.Title,
                PageUrl = request.PageUrl,
                DataUrl = request.DataUrl,

                PreferredFilename = request.PreferredFilename,
                ExtraDataRaw = request.ExtraDataRaw,
            };

            // Rule out malicious file types
            await ThrowIfExecutableAsync(eventArgs, ctoken);

            // Quick validate media type
            await ThrowFastIfUnsupportedMediaFileAsync(eventArgs, ctoken);

            // Queue the event args for indexing
            await _assetQueue.OnFileUploadedAsync(eventArgs, ctoken);

            // Finish up (TODO: Issue uploader's token to keep track of the download, so frontend can follow up).
            result.Message = "File uploaded successfully.";
        }
        catch (UnsupportedFileTypeException ex)
        {
            const string errorMessage = "Unsupported file uploaded.";
            _logger.LogError(ex, errorMessage);
            result.Message = errorMessage;
        }
        catch (Exception ex)
        {
            const string errorMessage = "Upload failed.";
            _logger.LogError(ex, errorMessage);
            result.Message = errorMessage;
        }

        return result;
    }

    // Internal

    private async Task<string> WriteToTemporaryFileAsync(
        Stream sourceStream,
        CancellationToken ctoken = default)
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

    private FileStream GetTemporaryFileStream(CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        string uploadDirectory = UploadCacheDirectory;
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

    private async static Task ThrowIfExecutableAsync(FileUploadedEventArgs eventArgs, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Ensure it's not an executable by mime type
        string[] executableMimeTypes = [
            "application/x-msdownload",
            "application/x-executable",
            "application/x-msdos-program",
            "application/x-dosexec",
            "application/x-sharedlib" ];
        if (executableMimeTypes.Contains(eventArgs.ReportedMimeType, StringComparer.OrdinalIgnoreCase))
        {
            throw new UnsupportedFileTypeException(
                message: "Executable file mime-type detected.",
                type: eventArgs.ReportedMimeType);
        }

        // Ensure it's not an executable by file extension
        string[] executableExtensions = [".exe", ".bat", ".cmd", ".com", ".msi", ".pif", ".scr", ".cpl"];
        string fileExtension = Path.GetExtension(eventArgs.ReportedFilename);
        if (executableExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            throw new UnsupportedFileTypeException(
                message: "Executable file extension detected.",
                type: fileExtension);
        }

        await Task.Yield();
    }

    private Task ThrowFastIfUnsupportedMediaFileAsync(
        FileUploadedEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        string inferredExtension = MimeTypesMap.GetExtension(eventArgs.ReportedMimeType);
        string reportedExtension = Path.GetExtension(eventArgs.ReportedFilename);

        if (string.IsNullOrWhiteSpace(inferredExtension) ||
            string.IsNullOrWhiteSpace(reportedExtension) ||
            !string.Equals(inferredExtension, reportedExtension, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Reported mime-type does not correspond to the reported file extension: {eventArgs}", eventArgs);
        }

        if (!XenoFxConstants.AllowedAssetExtensions.Contains(inferredExtension, StringComparer.OrdinalIgnoreCase) &&
            !XenoFxConstants.AllowedAssetExtensions.Contains(reportedExtension, StringComparer.OrdinalIgnoreCase))
        {
            throw new UnsupportedFileTypeException(
                message: "Unsupported file type detected.",
                type: eventArgs.ReportedMimeType);
        }

        return Task.CompletedTask;
    }
}