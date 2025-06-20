using HeyRed.Mime;
using System;
using System.IO;
using XenoFx.Database.CacheDb.Models;
using XenoFx.Services.Api.AssetSearch.Contracts;

namespace XenoFx.Services.Api.AssetSearch;

public static class AssetSearchExtensions
{
    public static AssetSearchResult ToResult(this AssetSearchQuery query) => new()
    {
        SearchString = query.SearchString,
        Page = query.Page,
        PageSize = query.PageSize,
        SortBy = query.SortBy,
        SortDirection = query.SortDirection
    };

    public static AssetSearchCardData CreateAssetSearchCardData(this AssetPresenceInfo presence) => new()
    {
        // Assuming AssetPresenceInfo has properties that can be mapped to AssetSearchCardData
        // Temporarily mapping AssetPresenceInfo properties to AssetSearchCardData
        Id = presence.AssetId.ToString(),
        Title = presence.OriginalPath.ToMakeshiftTitle(),
        Source = presence.OriginalPath,                     // This needs to be removed when AssetIDs are implemented
        MediaType = MimeTypesMap.GetMimeType(Path.GetExtension(presence.OriginalPath)),
    };

    private static string ToMakeshiftTitle(this string path)
    {
        // Convert path to a makeshift title, e.g., by removing file extension or directory structure
        string filename = Path.GetFileNameWithoutExtension(path);
        char[] delimiters = { '_', '-', ' ' };
        string[] parts = filename.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(" ", parts).Trim();
    }
}