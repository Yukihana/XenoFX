using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using XenoFx.Database.Cache;

namespace XenoFx.Services.Storage.AssetPresence;

// TODO turn this into an in-memory database context wrapper
// Retain the OnUpdated functionality
public sealed partial class AssetPresenceService : IAssetPresenceService
{
    // Infrastructure

    private readonly IDbContextFactory<CacheDbContext> _cacheDbFactory;
    private readonly ILogger<AssetPresenceService> _logger;

    // Data

    private ulong _stateCounter = 0;
    private readonly ConcurrentDictionary<string, UInt128> Presences = [];

    // State : Any change should trigger a change

    private void OnUpdated()
        => Interlocked.Increment(ref _stateCounter);

    public ulong StateCounter
        => Interlocked.Read(ref _stateCounter);

    // Lifetime

    public AssetPresenceService(
        IDbContextFactory<CacheDbContext> cacheDbFactory,
        ILogger<AssetPresenceService> logger)
    {
        _cacheDbFactory = cacheDbFactory;
        _logger = logger;
    }

    // Registrations : Add, Remove only, since asset will be taken down for reevaluation anyway in case of changes.
    // TODO switch over to database instead

    public int TotalRefresh(string[] files)
    {
        int counter = 0;
        foreach (string file in files)
        {
            if (Create(file))
                counter++;
        }
        return counter;
    }

    public bool Create(string path, UInt128 id = default)
    {
        bool success = Presences.TryAdd(path, id);
        if (success)
            OnUpdated();
        return success;
    }

    public void Remove(string path)
    {
        Presences.TryRemove(path, out _);
        OnUpdated();
    }

    public void Remove(UInt128 id)
    {
        var paths = Presences
            .Where(x => x.Value.Equals(id))
            .Select(x => x.Key)
            .ToList();

        foreach (var path in paths)
            Presences.TryRemove(path, out _);
        OnUpdated();
    }

    // Queries

    public bool IsFile(string path)
    {
        return Presences.TryGetValue(path, out UInt128 id);
    }

    public bool IsAsset(string path)
    {
        return Presences.TryGetValue(path, out UInt128 id)
             && id != UInt128.Zero;
    }

    public bool IsAvailable(UInt128 id)
    {
        return Presences.Any(x => x.Value == id);
    }

    public string? GetPath(UInt128 id)
    {
        if (Presences.FirstOrDefault(x => x.Value == id) is KeyValuePair<string, UInt128> first)
            return first.Key;
        return null;
    }

    public UInt128 GetAssetId(string path)
    {
        if (Presences.TryGetValue(path, out UInt128 id))
            return id;
        else
            return UInt128.Zero;
    }

    // Bulk
}