namespace XenoServe.Features.AssetSearch.DTOs;

public class AssetRelatedQueryParams
{
    public string Id { get; set; } = string.Empty;
    public string? Keywords { get; set; } = string.Empty;
    public int? Page { get; set; } = 0;
    public int? PageSize { get; set; } = 30;
}