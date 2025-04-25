using System.IO;

namespace XenoFx.Services.Api.AssetUpload.DTOs;

public sealed class AssetUploadRequest(Stream stream)
{
    // Data

    public Stream DataStream { get; set; } = stream;
    public string ContentMimeType { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;

    // Metadata

    public string PageUrl { get; set; } = string.Empty;
    public string DataUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    // Optional

    public string PreferredFilename { get; set; } = string.Empty;
    public string ExtraDataRaw { get; set; } = string.Empty;
}