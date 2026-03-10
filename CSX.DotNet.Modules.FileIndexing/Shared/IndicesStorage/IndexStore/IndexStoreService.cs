using CSX.DotNet.Modules.FileIndexing.Shared.IndicesStorage.Database.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileIndexing.Shared.IndicesStorage.IndexStore;

public class IndexStoreService : IIndexStoreService
{
    // Internal data

    private readonly ReaderWriterLockSlim _lock = new();
    private readonly ConcurrentDictionary<string, List<FileIndex>> _tempStore = [];

    // API - Write

    public async Task WriteAsync(
        string tenantId,
        Func<List<FileIndex>, CancellationToken, bool> writeAction,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _lock.EnterWriteLock();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var data = _tempStore.GetOrAdd(tenantId, _ => []);
            bool confirmWrite = writeAction(data, cancellationToken);

            // placeholder for future db-save confirmation
            _ = confirmWrite;
        }
        finally { _lock.ExitWriteLock(); }

        await Task.CompletedTask;
    }

    public async Task WriteAsync(
        string tenantId,
        Func<List<FileIndex>, CancellationToken, Task<bool>> writeAction,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _lock.EnterWriteLock();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var data = _tempStore.GetOrAdd(tenantId, _ => []);
            bool confirmWrite = await writeAction(data, cancellationToken);

            // placeholder for future db-save confirmation
            _ = confirmWrite;
        }
        finally { _lock.ExitWriteLock(); }
    }

    // API - Read

    public async Task<TResult> ReadAsync<TResult>(
        string tenantId,
        Func<List<FileIndex>, CancellationToken, TResult> readFunc,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _lock.EnterReadLock();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var data = _tempStore.TryGetValue(tenantId, out var existing)
                ? existing : [];

            return await Task.FromResult(readFunc(data, cancellationToken));
        }
        finally { _lock.ExitReadLock(); }
    }

    public async Task<TResult> ReadAsync<TResult>(
        string tenantId,
        Func<List<FileIndex>, CancellationToken, Task<TResult>> readFunc,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _lock.EnterReadLock();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var data = _tempStore.TryGetValue(tenantId, out var existing)
                ? existing : [];

            return await readFunc(data, cancellationToken);
        }
        finally { _lock.ExitReadLock(); }
    }
}