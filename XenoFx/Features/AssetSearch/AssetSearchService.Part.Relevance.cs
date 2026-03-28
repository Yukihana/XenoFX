using System;
using System.Collections.Generic;
using System.Linq;
using XenoFx.Database.CacheDb.Models;
using XenoFx.Features.AssetSearch.Contracts;

namespace XenoFx.Features.AssetSearch;

public partial class AssetSearchService
{
    private static IEnumerable<AssetPresenceInfo> FilterAndOrderByRelevance(
        IEnumerable<AssetPresenceInfo> items,
        AssetSearchQuery query)
    {
        // Score the items based on match relevance
        // Note: Passing the whole query to provision for custom scoring based on non standard fields
        Dictionary<AssetPresenceInfo, float> scoreDict = ScoreItems(items, query);

        // Truncate items that have no score
        var truncated = scoreDict
            .Where(x => x.Value > 0f);

        // Order and return
        var ordered
            = query.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
            ? truncated.OrderBy(x => x.Value)
            : truncated.OrderByDescending(x => x.Value);

        return ordered.Select(x => x.Key);
    }

    private static Dictionary<AssetPresenceInfo, float> ScoreItems(
        IEnumerable<AssetPresenceInfo> items,
        AssetSearchQuery query)
    {
        // Prepare input
        string normalizedQueryString = query.Keywords.ToLowerInvariant().Trim();
        var normalizedQueryTokens = Tokenize(normalizedQueryString);

        // Prepare output
        Dictionary<AssetPresenceInfo, float> scoreDict = [];

        foreach (var item in items)
        {
            // Short circuit scoring for raw order matching
            if (item.NormalizedPath.Contains(normalizedQueryString))
            {
                scoreDict[item] = normalizedQueryString.Length / (float)item.NormalizedPath.Length;
                continue;
            }

            // Order weighted scoring using partial tokens
            var normalizedItemTokens = Tokenize(item.NormalizedPath);
            var score
                = ScoreTokenized(normalizedQueryTokens, normalizedItemTokens)
                * ComputeOrderScore([.. normalizedQueryTokens], [.. normalizedItemTokens]);
            scoreDict[item] = score;
        }

        return scoreDict;
    }

    private static IEnumerable<string> Tokenize(string input) => input
        .Split([' ', '-', '_'], StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Trim());

    private static float ScoreTokenized(
        IEnumerable<string> normalizedQueryTokens,
        IEnumerable<string> normalizedTargetTokens)
    {
        int queryWeight = 0;

        foreach (var queryWord in normalizedQueryTokens)
        {
            if (normalizedTargetTokens.Any(x => x.Contains(queryWord)))
                queryWeight += queryWord.Length;
        }

        int targetWeight = normalizedTargetTokens.Sum(x => x.Length);

        return targetWeight > 0
            ? (float)queryWeight / targetWeight
            : 0f;
    }

    private static float ComputeOrderScore(
        string[] queryWords,
        string[] targetWords)
    {
        if (queryWords.Length == 0)
            return 0f;

        // Initialize progress counters
        int queryIndex = 0, targetIndex = 0;
        int inOrder = 0;

        // Iterate through both arrays to find in-order matches
        while (queryIndex < queryWords.Length && targetIndex < targetWords.Length)
        {
            if (targetWords[targetIndex].Contains(queryWords[queryIndex]))
            {
                inOrder++;
                queryIndex++;
            }
            targetIndex++;
        }

        // If no words matched, return 0
        return (float)inOrder / queryWords.Length;
    }
}