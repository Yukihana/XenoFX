namespace XenoFx.Features.AssetSearch.Contracts;

public class AssetRelatedQuery
{
    public string Id { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 30;
}