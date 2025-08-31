using CSX.DotNet.Common.Abstractions;
using CSX.DotNet.Common.Platform.Processes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Processing.AssetStaticThumbnail;

public partial class AssetStaticThumbnailService : IAssetStaticThumbnailService
{
    // Infrastructure

    private readonly IAssetAbstractionService _abstraction;
    private readonly IFFMpegProvider _ffmpegProvider;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetStaticThumbnailService> _logger;

    // Lifecycle

    public AssetStaticThumbnailService(
        IAssetAbstractionService abstraction,
        IFFMpegProvider ffmpegProvider,
        IConfigurationService configuration,
        ILogger<AssetStaticThumbnailService> logger)
    {
        _abstraction = abstraction;
        _ffmpegProvider = ffmpegProvider;
        _configuration = configuration;
        _logger = logger;
    }

    // Parameters

    // Public API

    public async Task<string> GetFilePathAsync(
        string id,
        CancellationToken ctoken = default)
    {
        // Placeholder until IDs are implemented:
        // Generate filepath based on source name
        // Helps make the path resolution more deterministic
        // Conflicts might return the wrong thumbnail, but that's okay until production.
        string sourcePath = await _abstraction.GetFirstMatchingAssetPathAsync(id, ctoken);
        string thumbPath = $"{Path.GetFileName(sourcePath)}.jpg";

        return await GenerateThumbAsync(
            Path.Combine(_configuration.AssetsDirectory, sourcePath),
            thumbPath,
            ctoken);
    }

    // Internal

    private async Task<string> GenerateThumbAsync(
        string sourceFile,
        string targetFileName,
        CancellationToken ctoken = default)
    {
        // Ensure the thumbnails directory exists
        string staticThumbsDirectory = _configuration.ThumbsDirectory;
        if (!Directory.Exists(staticThumbsDirectory))
            Directory.CreateDirectory(staticThumbsDirectory);
        string thumbPath = Path.Combine(staticThumbsDirectory, targetFileName);

        // Acquire FFmpeg path and build arguments
        string ffmpegPath = await _ffmpegProvider.AcquireFFMpegAsync(ctoken);
        string arguments = $"-ss 00:00:05 -i \"{sourceFile}\" -vframes 1 \"{thumbPath}\" -y";

        // Run FFmpeg
        await ProcessExecution.RunProcessAsync(ffmpegPath, arguments, ctoken);

        // Verify thumbnail was created
        if (!File.Exists(thumbPath))
            throw new Exception($"Thumbnail generation failed for file: {sourceFile}");

        return thumbPath;
    }
}