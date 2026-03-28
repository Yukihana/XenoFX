using System;
using System.Collections.Generic;
using XenoFx.Features.AssetSearch.Models;

namespace XenoFx.Features.AssetSearch.Contracts;

public sealed class AssetRelatedResult : AssetRelatedQuery
{
    public List<AssetSearchCardData> Results { get; set; } = [];

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Derived

    public int Count => Results.Count;
}