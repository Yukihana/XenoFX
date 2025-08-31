using CSX.DotNet.Common.IO;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Abstraction.AssetMedia.DTOs;
using XenoFx.Services.Processing.MediaDetector;
using XenoFx.Services.Processing.VideoTranscode;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Abstraction.AssetMedia;

public class AssetMediaService : IAssetMediaService
{
    // Using AssetAbstraction until IDs are implemented
    // After that, this service will handle utilities and all the until database abstractions

    // Infrastructure

    private readonly IAssetAbstractionService _assetAbstraction;
    private readonly IMediaDetectorService _mediaDetector;
    private readonly IVideoTranscodeService _videoTranscode;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetMediaService> _logger;

    // Lifecycle

    public AssetMediaService(
        IAssetAbstractionService assetAbstraction,
        IMediaDetectorService mediaDetector,
        IVideoTranscodeService videoTranscode,
        IConfigurationService configuration,
        ILogger<AssetMediaService> logger)
    {
        _assetAbstraction = assetAbstraction;
        _mediaDetector = mediaDetector;
        _videoTranscode = videoTranscode;
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
        // Later just use a isWebSafe flag to skip this (intermediate: TranscodeForWeb: Unknown, Conformant, Transcoded)
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

    // Internal

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
        var type = await _mediaDetector.DetectMediaTypeAsync(
            sourcePath, ctoken);

        // Transcode Router
        return type switch
        {
            MediaTypes.Video => await TranscodeVideoAsync(id, sourcePath, format, ctoken),
            _ => throw new InvalidOperationException("Unsupported media type for transcode")
        };
    }

    private async Task<string> TranscodeVideoAsync(
        string id,
        string sourcePath,
        TranscodeFormat format,
        CancellationToken ctoken = default)
    {
        switch (format)
        {
            case TranscodeFormat.Universal:
                bool isConformant = await _videoTranscode.ValidateForWebAsync(sourcePath, ctoken);
                if (isConformant)
                    return sourcePath;

                return await _videoTranscode.TranscodeForWebAsync(sourcePath, id, ctoken);

            default:
                throw new InvalidOperationException($"Unsupported transcode format: {format}");
        }
    }
}