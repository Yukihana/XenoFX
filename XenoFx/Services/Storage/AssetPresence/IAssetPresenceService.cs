using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.CacheDb.Models;

namespace XenoFx.Services.Storage.AssetPresence;

public interface IAssetPresenceService
{
    // Read

    Task<T> ReadAsync<T>(
        Func<DbSet<AssetPresenceInfo>, T> readFunc,
        CancellationToken ctoken = default);

    Task<T> ReadAsync<T>(
        Func<DbSet<AssetPresenceInfo>, CancellationToken, Task<T>> readFunc,
        CancellationToken ctoken = default);

    // Write

    Task<int> WriteAsync(
        Func<DbSet<AssetPresenceInfo>, bool> writeFunc,
        CancellationToken ctoken = default);

    Task<int> WriteAsync(
        Func<DbSet<AssetPresenceInfo>, CancellationToken, Task<bool>> writeFunc,
        CancellationToken ctoken = default);

    // Transact

    Task TransactAsync(
        IEnumerable<Func<DbSet<AssetPresenceInfo>, CancellationToken, Task<bool>>> transactFuncs,
        CancellationToken ctoken = default);
}