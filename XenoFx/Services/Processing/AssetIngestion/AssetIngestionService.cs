using CSX.DotNet.Common.IO.FileFormats;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.IO;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Processing.AssetIngestion;

public partial class AssetIngestionService : IAssetIngestionService
{
    // Infrastructure

    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetIngestionService> _logger;

    // Lifecycle

    public AssetIngestionService(
        IConfigurationService configuration,
        ILogger<AssetIngestionService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // Parameters

    public ImmutableArray<string> AllowedAssetExtensions
        => _configuration.AllowedAssetExtensions;

    // Shared Internal

    private string ValidateFormat(
        Stream stream)
    {
        // Magic bytes scan against allowed types
        foreach (string ext in _configuration.AllowedAssetExtensions)
        {
            if (VideoFormatDetectorSlim.IsFormat(stream, ext))
                return ext;
        }
        return string.Empty;
    }
}