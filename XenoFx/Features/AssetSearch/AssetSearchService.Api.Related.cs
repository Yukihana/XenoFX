using CSX.DotNet.Common.Data.DataGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.CacheDb.Models;
using XenoFx.Features.AssetSearch;
using XenoFx.Features.AssetSearch.Contracts;

namespace XenoFx.Features.AssetSearch;

public partial class AssetSearchService
{
    public async Task<AssetRelatedResult> RelatedAsync(
        AssetRelatedQuery query,
        string partialSeed,
        CancellationToken ctoken = default)
    {
        // Retrieve all entries
        List<AssetPresenceInfo> presences = await _assetAbstraction.GetPresencesAsync(ctoken);

        // TODO after id implementation: Replace query.Id with Title, tags etc from the actual item with that id
        var scores = ScoreRelevance(
            presences, [query.Id]);

        // TODO Actual logic
        // TODO Relevance logic made re-usable
        // TODO Cache (Use: IMemoryCache or persistent)

        // Filter and order the presences based on the query
        Random rng = new(Int32Generators.CreateHourSeed(partialSeed));
        int batchSize = rng.Next(10, 30);

        var results = scores
            .OrderByDescending(kvp => kvp.Value)                // Sort descending by score
            .Select(kvp => (item: kvp.Key, index: rng.Next()))  // Switch score with rng
            .GroupBy(x => x.index / batchSize)                  // Group by deterministically randomized batch size
            .SelectMany(batch => batch                          // Soft shuffle
                .OrderBy(x => x.index)                          // shuffle within batch by rng
                .Select(x => x.item)                            // pick the item
            )
            .Skip(query.Page * query.PageSize)                  // Apply pagination
            .Take(query.PageSize)
            .Select(AssetSearchExtensions.CreateAssetSearchCardData)    // Transform
            .ToList();                                                  // Materialize

        var response = query.ToResult();
        response.Results = results;
        response.Timestamp = DateTime.UtcNow;

        return response;
    }

    private Dictionary<AssetPresenceInfo, int> ScoreRelevance(
        List<AssetPresenceInfo> presences,
        string[] sources)
    {
        Dictionary<string, int> words = GetWords(sources);
        Dictionary<AssetPresenceInfo, int> scores = [];

        foreach (var presence in presences)
        {
            var src = GetWords(GetSources(presence));
            var score = ScoreItem(words, src);
            scores.Add(presence, score);
        }

        return scores;
    }

    private static string[] GetSources(
        AssetPresenceInfo info)
    {
        return [info.NormalizedPath];
    }

    private static Dictionary<string, int> GetWords(
        params string[] sources)
    {
        Dictionary<string, int> wordWeights = [];
        foreach (var source in sources)
        {
            string[] words = source.Split(
                [' ', '-', '_', ',', ';', '&'],
                options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (wordWeights.TryGetValue(word, out int value))
                    wordWeights[word] = ++value;
                else
                    wordWeights.Add(word, 1);
            }
        }

        return wordWeights;
    }

    private static int ScoreItem(
        Dictionary<string, int> a,
        Dictionary<string, int> b)
    {
        int score = 0;

        foreach (var kvp in a)
        {
            if (b.TryGetValue(kvp.Key, out int bValue))
            {
                score += kvp.Value * bValue;
            }
        }

        return score;
    }
}