using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.CacheDb.Models;
using XenoFx.Services.Abstraction.AssetAbstraction.DTOs;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public interface IAssetAbstractionService
{
    // Library

    Task<List<AssetPresenceInfo>> GetPresencesAsync(
        CancellationToken ctoken = default);

    // Search

    Task<IEnumerable<AssetPresenceInfo>> QueryAsync<TQuery>(
        Func<AssetPresenceInfo, TQuery, bool> predicate,
        Func<IEnumerable<AssetPresenceInfo>, TQuery, IEnumerable<AssetPresenceInfo>> ordering,
        TQuery query,
        int skip,
        int take,
        CancellationToken ctoken = default);

    Task<string[]> GetHaveAsync(
        string searchString,
        CancellationToken ctoken = default);

    // Download

    Task<AssetViewerInfo> GetAssetViewerInfoAsync(
        string id,
        CancellationToken ctoken = default);

    // Placeholders

    Task<string> GetFirstMatchingAssetPathAsync(
        string id,
        CancellationToken ctoken = default);

    Task<string> GetAssetFilePathAsync(
        string id,
        CancellationToken ctoken = default);
}