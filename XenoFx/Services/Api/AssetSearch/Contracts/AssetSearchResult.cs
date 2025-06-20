using System;
using System.Collections.Generic;

namespace XenoFx.Services.Api.AssetSearch.Contracts;

public class AssetSearchResult
{
    // Query passthrough

    public string SearchString { get; set; } = string.Empty;
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 30;
    public string SortBy { get; set; } = "relevance";
    public string SortDirection { get; set; } = "desc";

    // Results

    public List<AssetSearchCardData> Matches { get; set; } = [];
    public int Total { get; set; } = 0;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Derived

    public int Count => Matches.Count;
}