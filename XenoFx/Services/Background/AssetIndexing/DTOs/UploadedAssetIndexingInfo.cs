namespace XenoFx.Services.Background.AssetIndexing.DTOs;

public sealed partial class UploadedAssetIndexingInfo
{
    public string RelativePath { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string OriginalFilename { get; set; } = string.Empty;
    public string PageUrl { get; set; } = string.Empty;
    public string DataUrl { get; set; } = string.Empty;
}