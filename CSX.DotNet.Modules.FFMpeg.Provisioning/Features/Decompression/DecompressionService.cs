using CSX.DotNet.Common.FileCompression;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Decompression;

public partial class DecompressionService : IDecompressionService
{
    // Infrastructure

    private readonly ILogger<DecompressionService> _logger;

    // Lifecycle

    public DecompressionService(
        ILogger<DecompressionService> logger)
    {
        _logger = logger;
    }

    // Public API

    public async Task<string> DecompressAsync(
        string archiveFilePath,
        string decompressionPath,
        ExtractionOverwriteMode overwriteMode = ExtractionOverwriteMode.Abort,
        bool deleteArchiveAfterUse = false,
        CancellationToken ctoken = default)
    {
        try
        {
            return await FileDecompressor.DecompressAsync(
                archiveFilePath: archiveFilePath,
                decompressionPath: decompressionPath,
                overwriteMode: overwriteMode,
                deleteArchiveAfterUse: deleteArchiveAfterUse,
                ctoken: ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract archive: {File}", archiveFilePath);
            throw;
        }
    }
}