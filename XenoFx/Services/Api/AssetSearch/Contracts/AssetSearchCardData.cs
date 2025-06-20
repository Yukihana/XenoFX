namespace XenoFx.Services.Api.AssetSearch.Contracts;

public class AssetSearchCardData
{
    public string Id { get; set; } = string.Empty;          // unique identifier for the asset
    public string MediaType { get; set; } = string.Empty;   // mime type
    public string Title { get; set; } = string.Empty;       // title of the asset
    public string Source { get; set; } = string.Empty;      // original path of the asset
    // Large octet data should be accessed separately
}