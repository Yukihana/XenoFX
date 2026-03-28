namespace XenoFx.Features.AssetSearch.Models;

public class AssetSearchCardData
{
    public string Id { get; set; } = string.Empty;          // unique identifier for the asset
    public string MediaType { get; set; } = string.Empty;   // mime type
    public string Title { get; set; } = string.Empty;       // title of the asset
    public string SubText { get; set; } = string.Empty;     // subtitle or additional text
    public string ThumbText { get; set; } = string.Empty;   // thumbnail text

    // Legacy

    public string Source { get; set; } = string.Empty;      // original path of the asset
    // Large octet data should be accessed separately
}