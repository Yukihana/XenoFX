namespace XenoFx.Services.Api.AssetSearch.Contracts;

public class AssetSearchQuery
{
    public string SearchString { get; set; } = string.Empty;
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 30;
    public string SortBy { get; set; } = "relevance";
    public string SortDirection { get; set; } = "desc";
}