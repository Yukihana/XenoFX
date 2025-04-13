namespace XenoFx.Services.Background.AssetQueue.Models;

public sealed class AssetUploadedEventContext : AssetQueueEventContextBase
{
    // Request

    public string RelativePath { get; set; } = string.Empty;
    public string ReportedFilename { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PageUrl { get; set; } = string.Empty;
    public string DataUrl { get; set; } = string.Empty;
}