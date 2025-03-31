using System.IO;

namespace XenoFx.Services.Api.AssetUpload.DTOs;

public sealed class AssetUploadRequest(Stream stream)
{
    public Stream DataStream { get; set; } = stream;
    public string ContentMimeType { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public string PageUrl { get; set; } = string.Empty;
    public string DataUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}