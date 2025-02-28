using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace XenoFx.Services.AssetPresence;

public sealed partial class AssetPresenceService(ILogger<AssetPresenceService> logger) : IAssetPresenceService
{
    // Infrastructure

    private readonly ILogger<AssetPresenceService> _logger = logger;

    // Data

    private readonly ReaderWriterLockSlim _lock = new();
    private ulong _stateCounter = 0;

    private readonly ConcurrentDictionary<string, UInt128> Presences = [];

    // State : Any change should trigger a change

    private void OnUpdated()
    {
        unchecked
        {
            Interlocked.Increment(ref _stateCounter);
        }
    }

    public ulong StateCounter => Interlocked.Read(ref _stateCounter);

    // Registrations : Add, Remove only, since asset will be taken down for reevaluation anyway in case of changes.

    public void Create(string path, UInt128 id = default)
    {
        _lock.EnterWriteLock();
        try
        {
            Presences.TryAdd(path, id);
            OnUpdated();
        }
        finally { _lock.ExitWriteLock(); }
    }

    public void Remove(string path)
    {
        _lock.EnterWriteLock();
        try
        {
            Presences.TryRemove(path, out _);
            OnUpdated();
        }
        finally { _lock.ExitWriteLock(); }
    }

    public void Remove(UInt128 id)
    {
        _lock.EnterUpgradeableReadLock();
        try
        {
            var paths = Presences.Where(x => x.Value.Equals(id)).Select(x => x.Key).ToList();

            _lock.EnterWriteLock();
            try
            {
                foreach (var path in paths)
                    Presences.TryRemove(path, out _);
                OnUpdated();
            }
            finally { _lock.ExitWriteLock(); }
        }
        finally { _lock.ExitUpgradeableReadLock(); }
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
}