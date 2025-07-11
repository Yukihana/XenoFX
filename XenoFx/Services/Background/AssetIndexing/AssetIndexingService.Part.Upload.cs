using CSX.Common.Data.Events;
using HeyRed.Mime;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public string AssetUploadDirectory
        => _configuration.AssetUploadDirectory;

    // Indexing

    // Make this obsolete
    // Upload to cache/uploads with 128bit_uid
    // Then move to assets/uploaded/128bit_uid/file_original_name.ext
    // The file tracker will grab that and send it to 'onCreatedAsync'
    // there can be a separate uploaded table for recording metadata,
    // if the newly found file doesn't have existing metadata in the assets table,
    // the indexer will check the 'uploaded' table for metadata and sync it
    // matching will be done based on the saved path in the table (not hashing)
    // user can move it as required later

    public async Task<bool> IndexUploadEventAsync(
        FileUploadedEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        string finalPath = string.Empty;

        try
        {
            finalPath = await PrepareUploadForIndexingAsync(eventArgs, ctoken);

            // Bail early if the final upload path falls within indexing exclusions.
            if (!_pathValidator.TryTruncateAssetPath(finalPath, out string? relativePath))
                return false;

            // Apply for indexing
            await OnCreatedAsync(relativePath, ctoken); // TODO, recieve id on creation

            // TODO use recieved id for further database access, eg
            // await StoreMetadataAsync(id, eventArgs, ctoken);

            _logger.LogInformation("Indexed upload: {path}", relativePath);

            return false; // Assimilation was successful. No need to requeue.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Asset upload faulted at {finalPath} for: {eventArgs}", finalPath, eventArgs);
            return false;
        }
    }

    // TODO align this migrated code.
    private async Task<string> PrepareUploadForIndexingAsync(FileUploadedEventArgs eventArgs, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();
        await Task.Yield();

        string extension = await DetermineExtensionAsync(eventArgs, ctoken);

        string filenameBase = eventArgs.GetAssetBaseName();

        return MoveToFinalPath(eventArgs.TemporaryFileFullPath, filenameBase, extension, ctoken);
    }

    private string MoveToFinalPath(
        string tempPath,
        string basename,
        string extension,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        bool emptyTitle = string.IsNullOrWhiteSpace(basename);
        string filename = emptyTitle ? $"{Guid.NewGuid()}" : basename;

        // Try for each possible name until one is available.
        while (true)
        {
            ctoken.ThrowIfCancellationRequested();

            string selectedPath = Path.Combine(
                    AssetUploadDirectory,
                    $"{filename}.{extension}");

            try
            {
                File.Move(tempPath, selectedPath);
                return selectedPath;
            }
            catch (IOException) when (File.Exists(selectedPath))
            { }

            // prep new for next iteration
            string guid = Guid.NewGuid().ToString();
            filename = emptyTitle ? guid : $"{basename}_{guid}";
        }
    }

    private async static Task<string> DetermineExtensionAsync(
        FileUploadedEventArgs eventArgs,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // By original extension
        var reportedExtension = Path.GetExtension(eventArgs.ReportedFilename)?.ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(reportedExtension) &&
            XenoFxConstants.AllowedAssetExtensions.Contains(reportedExtension, StringComparer.OrdinalIgnoreCase))
        {
            return reportedExtension;
        }

        // By mimetype inference
        var inferredExtension = MimeTypesMap.GetExtension(eventArgs.ReportedMimeType);
        if (!string.IsNullOrWhiteSpace(inferredExtension))
        {
            var dotExtension = "." + inferredExtension.ToLowerInvariant();
            if (XenoFxConstants.AllowedAssetExtensions.Contains(dotExtension, StringComparer.OrdinalIgnoreCase))
            {
                return dotExtension;
            }
        }

        // Placeholder for magic bytes detection methods
        await Task.Yield();

        // Fallback
        return ".bin";
    }
}