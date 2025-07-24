using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Processing.AssetIngestion.DTOs;

namespace XenoFx.Services.Processing.AssetIngestion;

/// <summary>
/// Responsible for preparing indexing data.
/// </summary>
public interface IAssetIngestionService
{
    Task<CachedAssetIndex> IngestFromUploadMetadataAsync(
        string uploadMetadataPath,
        CancellationToken ctoken = default);

    Task<CachedAssetIndex> IngestFromUploadMetadataAsync(
        AssetUploadMetadata metadata,
        string pathToCacheFile,
        CancellationToken ctoken = default);
}