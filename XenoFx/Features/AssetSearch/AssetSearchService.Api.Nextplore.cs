using CSX.DotNet.Common.Data.DataGenerators;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.CacheDb.Models;
using XenoFx.Features.AssetSearch.Contracts;
using XenoFx.Features.AssetSearch.Models;

namespace XenoFx.Features.AssetSearch;

public partial class AssetSearchService
{
    public async Task<AssetSearchCardData?> NextploreAsync(
        AssetNextploreQuery query,
        string partialSeed,
        CancellationToken ctoken = default)
    {
        // Read from db
        List<AssetPresenceInfo> presences = await _assetAbstraction.GetPresencesAsync(ctoken);
        if (presences.Count == 0)
            return null;

        _logger.LogDebug("nextplore og id is empty. assigning random.");
        var origId = query.OriginalId;
        if (string.IsNullOrWhiteSpace(origId))
        {
            // Assign a random original id to ensure the seed is not the same for every empty query
            int randomIndex = new Random().Next(presences.Count);
            origId = presences[randomIndex].NormalizedPath;
        }

        // Prepare randomization
        // Daily since original id already narrows it down, if not keywords.
        int seed = Int32Generators.GenerateDailySeed(
            partialSeed,
            origId,
            query.Keywords.ToLowerInvariant());

        Random rng = new(seed);

        // Tokenize keywords and prepare next index
        string[] words = [.. query.Keywords.ToLowerInvariant()
            .Split([' ', ',', ';', '-', '_'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Distinct()];

        // Perform the search and remove the original from the list
        List<AssetPresenceInfo> searchResults = [];
        if (words.Length > 0)
        {
            searchResults = [.. NextploreFilterAndOrder(presences, words, rng)];
            searchResults.RemoveAll(x => x.NormalizedPath == query.OriginalId);
        }

        // Return directly if the index is within scope
        if (searchResults.Count > query.PlaybackIndex)
            return searchResults[query.PlaybackIndex].CreateAssetSearchCardData();

        // Otherwise shift the index to account for the already used filtered results
        int fallbackIndex = query.PlaybackIndex - searchResults.Count;

        // If search is exhausted,
        // generate a deterministic fallback list
        List<AssetPresenceInfo> randomizedResults = [.. NextploreRandomize(presences, rng)];

        // If no results, terminate early
        if (randomizedResults.Count == 0)
            return null;

        // Remove all search results from the fallback list to avoid duplicates,
        // using a HashSet for efficiency
        var searchPaths = searchResults
            .Select(x => x.NormalizedPath)
            .ToHashSet();
        searchPaths.Add(query.OriginalId);
        var diffedRandoms = randomizedResults
            .Where(x => !searchPaths.Contains(x.NormalizedPath))
            .ToList();

        // Serve the diffed randomization first if within range
        if (fallbackIndex < diffedRandoms.Count)
            return diffedRandoms[fallbackIndex].CreateAssetSearchCardData();

        // Otherwise continue into infinite randomized looping
        // using modulo to ensure effective index is positive and in-range
        int rc = randomizedResults.Count;
        int loopIndex = fallbackIndex - diffedRandoms.Count;
        int safeIndex = (loopIndex % rc + rc) % rc;

        return randomizedResults[safeIndex]
            .CreateAssetSearchCardData();

        // Consider caching based on seed.
        // On-demand progressive generation:
        // initial search,
        // the diff,
        // infinite randoms.

        // Operations in between would have negligible cost
        // - since they'll just hit the cache.

        // This also means setting up a cache in-memory.
        // Preferable within this module as it's only handling IDs, not file indexing.
    }

    // ensure diversity without using an algorithm as heavy as 'score by match order'
    private static IEnumerable<AssetPresenceInfo> NextploreFilterAndOrder(
        IEnumerable<AssetPresenceInfo> presences,
        string[] words,
        Random rng)
    {
        return presences
            // Apply initial ordering to ensure consistency (Use id when implemented)
            .OrderBy(x => x.NormalizedPath)
            // Attach a score and a deterministic shuffle key
            .Select(item => (item, shuffle: rng.NextInt64(), score: words.Count(x => item.NormalizedPath.Contains(x))))
            // Filter out items with no score (Consider caching at this point)
            .Where(x => x.score > 0)
            // Order by score descending first
            .OrderByDescending(x => x.score)
            // Shuffle while maintaining score grouping
            .ThenBy(x => x.shuffle)
            // Deconstruct and yield items
            .Select(x => x.item);
    }

    private static IEnumerable<AssetPresenceInfo> NextploreRandomize(
        IEnumerable<AssetPresenceInfo> presences,
        Random rng)
    {
        return presences
            // Apply initial ordering to ensure consistency (Use id when implemented)
            .OrderBy(x => x.NormalizedPath)
            // Order by a deterministic shuffle key
            .OrderBy(x => rng.NextInt64());
    }
}