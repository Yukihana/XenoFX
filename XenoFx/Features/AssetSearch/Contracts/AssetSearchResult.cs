using System;
using System.Collections.Generic;
using XenoFx.Features.AssetSearch.Models;

namespace XenoFx.Features.AssetSearch.Contracts;

public class AssetSearchResult : AssetSearchQuery
{
    // Results

    public List<AssetSearchCardData> Results { get; set; } = [];
    public int Total { get; set; } = 0;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Derived

    public int Count => Results.Count;
}