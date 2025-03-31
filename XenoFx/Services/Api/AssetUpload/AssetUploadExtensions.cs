using XenoFx.Services.Api.AssetUpload.DTOs;
using XenoFx.Services.Background.AssetIndexing.DTOs;

namespace XenoFx.Services.Api.AssetUpload;

public static partial class AssetUploadExtensions
{
    public static UploadedAssetIndexingInfo ToIndexingInfo(this AssetUploadRequest request, string relativePath) => new()
    {
        Title = request.Title,
        OriginalFilename = request.Filename,
        RelativePath = relativePath,
        DataUrl = request.DataUrl,
        PageUrl = request.PageUrl,
    };
}