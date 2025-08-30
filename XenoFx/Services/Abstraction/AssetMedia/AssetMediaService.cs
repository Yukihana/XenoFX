using CSX.DotNet.Common.IO;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Abstraction.AssetMedia.DTOs;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Abstraction.AssetMedia;

public class AssetMediaService : IAssetMediaService
{
    // Using AssetAbstraction until IDs are implemented
    // After that, this service will handle utilities and all the until database abstractions

    // Infrastructure

    private readonly IAssetAbstractionService _assetAbstraction;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetMediaService> _logger;

    // Lifecycle

    public AssetMediaService(
        IAssetAbstractionService assetAbstraction,
        IConfigurationService configuration,
        ILogger<AssetMediaService> logger)
    {
        _assetAbstraction = assetAbstraction;
        _configuration = configuration;
        _logger = logger;
    }

    // Public API

    public async Task<AssetMediaResult> GetFilePathAsync(
        string id,
        TranscodeOptions? options = null,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        options ??= new();

        // make this more OO instead of calling one off methods
        // ie get the AssetInfo, then use abstraction as a function facilitator

        // Placeholder for [ID lookup -> AssetInfo]
        // currently using [searchKey(as id) -> relativePath]
        string path = await _assetAbstraction.GetFirstMatchingAssetPathAsync(id, ctoken);

        // Placeholder for cross-checking asset info with presences for the file's current location;
        // returns usable full path;
        // currently using [relativePath -> fullPath] and notifies if the file is missing
        string fullPath = await _assetAbstraction.GetAssetFilePathAsync(path, ctoken);

        // Transcode if applicable
        string finalPath = options.TranscodeFormat == TranscodeFormat.Original
            ? fullPath
            : await TranscodeAsync(id, fullPath, options.TranscodeFormat, ctoken);

        // Until actual implementation, just base contentType on extension
        string extension = Path.GetExtension(finalPath).TrimStart('.').ToLowerInvariant(); // Normalize extension to lowercase without leading dot
        string contentType = MimeTyping.GetMimeType(extension);

        // On completion return the path
        return new AssetMediaResult()
        {
            FullPath = finalPath,
            ContentType = contentType,
        };
    }

    private async Task<string> TranscodeAsync(
        string id,
        string sourcePath,
        TranscodeFormat format,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        if (format == TranscodeFormat.Original)
            throw new NotSupportedException("Original → Original transcode is not supported.");

        // Placeholder for actual operation
        _ = _configuration.TranscodeDirectory;
        string writePath = Path.Combine(_configuration.TranscodeDirectory, id);
        _ = writePath;
        await Task.Yield();

        // Temporary
        return sourcePath;
    }
}