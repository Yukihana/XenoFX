using XenoFx.Services.Processing.AssetIngestion.DTOs;
using XenoServe.Features.AssetUpload.DTOs;

namespace XenoServe.Features.AssetUpload;

public static class AssetUploadExtensions
{
    public static AssetUploadMetadata MapToUploadMetadata(
        this AssetUploadContext context,
        string cachedFilename)
    {
        return new()
        {
            TimeStamp = context.Authorization.TimeStamp,
            CachedFilename = cachedFilename,

            DeclaredFilename = context.Request.Data.FileName,
            DeclaredMimeType = context.Request.Data.ContentType,

            PreferredFilename = context.Request.PreferredFilename,
            Title = context.Request.Title,

            ExtraData = context.Request.ExtraData,
            DataUrl = context.Request.DataUrl,
            PageUrl = context.Request.PageUrl,
        };
    }
}