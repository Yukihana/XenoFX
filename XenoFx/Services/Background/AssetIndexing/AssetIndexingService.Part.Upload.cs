using CSX.Common.Data.Guids;
using CSX.Common.Data.Text.Json;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Processing.AssetIngestion.DTOs;

namespace XenoFx.Services.Background.AssetIndexing;

public partial class AssetIndexingService
{
    public string AssetUploadDirectory
        => _configuration.AssetsUploadDirectory;

    // Indexing

    public async Task<bool> IndexUploadEventAsync(
        string uploadMetadataPath,
        Func<string, CancellationToken, Task>? cleanupCallback,
        CancellationToken ctoken = default)
    {
        try
        {
            // Validate the request, contents and build indexable parameters
            CachedAssetIndex index = await _assetIngestion.IngestFromUploadMetadataAsync(
                uploadMetadataPath: uploadMetadataPath,
                ctoken: ctoken);

            // Integrate with the database
            var guid = await CreateFromUploadAsync(index, ctoken);
            _logger.LogInformation("Asset with id {guid} ingress successful.", guid);

            // Integration complete. No cancellation beyond this point.

            // Move the cached file to its final location
            File.Move(
                index.CacheFilePath,
                index.FinalFullPath,
                overwrite: true); // Id is deterministic. Any conflict would be malicious. Overwrite and kill.

            // Backup metadata (until the required tables are implemented)
            await BackupIndexAsync(
                index: index,
                ctoken: CancellationToken.None);

            // Cleanup
            if (cleanupCallback is not null)
                await cleanupCallback(
                    uploadMetadataPath,
                    CancellationToken.None);

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process upload metadata at {path}", uploadMetadataPath);
            return true; // Retry
        }
    }

    private async Task<Guid> CreateFromUploadAsync(
        CachedAssetIndex index,
        CancellationToken ctoken = default)
    {
        while (true)
        {
            try
            {
                ctoken.ThrowIfCancellationRequested();

                // Designate a unique id
                Guid guid = DMC212710Guid.FromUtcNow();

                // Assign ingress location
                string finalFullPath = Path.Combine(
                    _configuration.AssetsUploadDirectory,
                    guid.ToString(),
                    $"{index.DeterminedFileName}.{index.FileExtension}");
                string finalPath = Path.GetRelativePath(
                    relativeTo: _configuration.AssetsDirectory,
                    path: finalFullPath);

                // Integrate in database here
                await Task.Yield(); // Simulate DB operation

                // On success assign to index
                index.AssetId = guid; // Note, if record already exists, use the new guid instead to prevent overwriting the old file
                index.Location = finalPath;
                index.FinalFullPath = finalFullPath;
                return guid;
            }
            catch (IOException) // Placeholder for unique constraint
            { }
        }
    }

    public async Task BackupIndexAsync(
        CachedAssetIndex index,
        CancellationToken ctoken = default)
    {
        // For now serialize the context to a backup directory
        // Since metadata has parameters for co-relation
        // can be used until full integration is implemented
        ctoken.ThrowIfCancellationRequested();

        string writePath = Path.Combine(
            _configuration.MetadataDirectory,
            $"{index.AssetId}.index.json");

        await using (FileStream fs = new(
            path: writePath,
            mode: FileMode.Create,
            access: FileAccess.Write,
            share: FileShare.None,
            bufferSize: 1024,
            useAsync: true))
        {
            await JsonSerializer.SerializeAsync(
                utf8Json: fs,
                value: index,
                options: JsonOptionsUtilities.HumanReadableJsonOptions,
                cancellationToken: ctoken);
        }

        _logger.LogInformation(
            "Operation metadata was backed up in: {path}",
            writePath);
    }
}