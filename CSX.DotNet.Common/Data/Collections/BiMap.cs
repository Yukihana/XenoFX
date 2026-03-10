using System;
using System.Collections.Generic;
using System.Threading;

namespace CSX.DotNet.Common.Data.Collections;

public sealed class BiMap<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    private readonly ReaderWriterLockSlim _lock =
        new(LockRecursionPolicy.NoRecursion);

    private readonly Dictionary<TKey, TValue> _forward;
    private readonly Dictionary<TValue, TKey> _reverse;

    public BiMap(
        IEqualityComparer<TKey>? keyComparer = null,
        IEqualityComparer<TValue>? valueComparer = null)
    {
        _forward = new Dictionary<TKey, TValue>(keyComparer);
        _reverse = new Dictionary<TValue, TKey>(valueComparer);
    }

    /* ---------------- CONTAINS / GET ---------------- */

    public bool ContainsKey(TKey key)
    {
        _lock.EnterReadLock();
        try { return _forward.ContainsKey(key); }
        finally { _lock.ExitReadLock(); }
    }

    public bool ContainsValue(TValue value)
    {
        _lock.EnterReadLock();
        try { return _reverse.ContainsKey(value); }
        finally { _lock.ExitReadLock(); }
    }

    public bool TryGetByKey(TKey key, out TValue value)
    {
        _lock.EnterReadLock();
        try { return _forward.TryGetValue(key, out value!); }
        finally { _lock.ExitReadLock(); }
    }

    public bool TryGetByValue(TValue value, out TKey key)
    {
        _lock.EnterReadLock();
        try { return _reverse.TryGetValue(value, out key!); }
        finally { _lock.ExitReadLock(); }
    }

    /* ---------------- ADD ---------------- */

    public bool TryAdd(TKey key, TValue value)
    {
        _lock.EnterWriteLock();
        try
        {
            if (_forward.ContainsKey(key) || _reverse.ContainsKey(value))
                return false;

            _forward[key] = value;
            _reverse[value] = key;
            return true;
        }
        finally { _lock.ExitWriteLock(); }
    }

    public TValue GetOrAddByKey(
        TKey key,
        Func<TKey, TValue> valueFactory)
    {
        ArgumentNullException.ThrowIfNull(valueFactory);

        _lock.EnterWriteLock();
        try
        {
            if (_forward.TryGetValue(key, out var existing))
                return existing;

            var value = valueFactory(key);

            if (_reverse.ContainsKey(value))
                throw new InvalidOperationException(
                    "The value produced by the factory is already mapped.");

            _forward[key] = value;
            _reverse[value] = key;
            return value;
        }
        finally { _lock.ExitWriteLock(); }
    }

    public TKey GetOrAddByValue(
        TValue value,
        Func<TValue, TKey> keyFactory)
    {
        ArgumentNullException.ThrowIfNull(keyFactory);

        _lock.EnterWriteLock();
        try
        {
            if (_reverse.TryGetValue(value, out var existing))
                return existing;

            var key = keyFactory(value);

            if (_forward.ContainsKey(key))
                throw new InvalidOperationException(
                    "The key produced by the factory is already mapped.");

            _reverse[value] = key;
            _forward[key] = value;
            return key;
        }
        finally { _lock.ExitWriteLock(); }
    }

    /* ---------------- REMOVE ---------------- */

    public bool TryRemoveByKey(TKey key, out TValue value)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_forward.TryGetValue(key, out value!))
                return false;

            _forward.Remove(key);
            _reverse.Remove(value);
            return true;
        }
        finally { _lock.ExitWriteLock(); }
    }

    public bool TryRemoveByValue(TValue value, out TKey key)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_reverse.TryGetValue(value, out key!))
                return false;

            _reverse.Remove(value);
            _forward.Remove(key);
            return true;
        }
        finally { _lock.ExitWriteLock(); }
    }

    public void Clear()
    {
        _lock.EnterWriteLock();
        try
        {
            _forward.Clear();
            _reverse.Clear();
        }
        finally { _lock.ExitWriteLock(); }
    }

    /* ---------------- MISC ---------------- */

    public int Count
    {
        get
        {
            _lock.EnterReadLock();
            try { return _forward.Count; }
            finally { _lock.ExitReadLock(); }
        }
    }
}