using System;

namespace XenoFx.Services.Processing.AssetIngestion.DTOs;

public class AssetUploadMetadata
{
    // Data

    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;
    public string CachedFilename { get; set; } = string.Empty;
    public string DeclaredMimeType { get; set; } = string.Empty;

    // Metadata

    public string DeclaredFilename { get; set; } = string.Empty;
    public string PreferredFilename { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    // Optional

    public string ExtraData { get; set; } = string.Empty;
    public string PageUrl { get; set; } = string.Empty;
    public string DataUrl { get; set; } = string.Empty;
}