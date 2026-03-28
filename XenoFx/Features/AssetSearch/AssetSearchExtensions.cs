using HeyRed.Mime;
using System;
using System.Collections.Generic;
using System.IO;
using XenoFx.Database.CacheDb.Models;
using XenoFx.Features.AssetSearch.Contracts;
using XenoFx.Features.AssetSearch.Models;

namespace XenoFx.Features.AssetSearch;

internal static partial class AssetSearchExtensions
{
    public static AssetSearchResult ToResult(this AssetSearchQuery query) => new()
    {
        Keywords = query.Keywords,
        Page = query.Page,
        PageSize = query.PageSize,
        SortBy = query.SortBy,
        SortDirection = query.SortDirection
    };

    public static AssetRelatedResult ToResult(this AssetRelatedQuery query) => new()
    {
        Id = query.Id,
        Keywords = query.Keywords,
        Page = query.Page,
        PageSize = query.PageSize,
    };

    public static AssetSearchCardData CreateAssetSearchCardData(this AssetPresenceInfo presence) => new()
    {
        // Assuming AssetPresenceInfo has properties that can be mapped to AssetSearchCardData
        // Temporarily mapping AssetPresenceInfo properties to AssetSearchCardData
        Id = presence.AssetId.ToString(),
        Title = presence.OriginalPath.ToMakeshiftTitle(),
        Source = Path.GetFileNameWithoutExtension(presence.OriginalPath),                     // This needs to be removed when AssetIDs are implemented
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

    // For unspecified
    public static IEnumerable<T> Randomize<T>(
        this IEnumerable<T> source,
        int seed)
    {
        List<T> copy = [.. source];
        Random rng = new(seed);

        while (copy.Count > 0)
        {
            int index = rng.Next(copy.Count);
            yield return copy[index];
            copy.RemoveAt(index);
        }
    }
}