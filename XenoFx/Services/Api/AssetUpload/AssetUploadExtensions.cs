using XenoFx.Services.Api.AssetUpload.DTOs;
using XenoFx.Services.Background.AssetQueue.Models;

namespace XenoFx.Services.Api.AssetUpload;

public static partial class AssetUploadExtensions
{
    public static AssetUploadedEventContext ToIndexingInfo(this AssetUploadRequest request, string relativePath) => new()
    {
        RelativePath = relativePath,
        Title = request.Title,
        ReportedFilename = request.Filename,
        MimeType = request.ContentMimeType,
        DataUrl = request.DataUrl,
        PageUrl = request.PageUrl,
    };
}