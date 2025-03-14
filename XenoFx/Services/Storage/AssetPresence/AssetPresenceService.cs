using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.Cache;
using XenoFx.Database.Cache.Models;

namespace XenoFx.Services.Storage.AssetPresence;

// TODO turn this into an in-memory database context wrapper
// Retain the OnUpdated functionality
public sealed partial class AssetPresenceService : IAssetPresenceService
{
    // Infrastructure

    private readonly IDbContextFactory<CacheDbContext> _cacheDbFactory;
    private readonly ILogger<AssetPresenceService> _logger;

    // Lifetime

    public AssetPresenceService(
        IDbContextFactory<CacheDbContext> cacheDbFactory,
        ILogger<AssetPresenceService> logger)
    {
        _cacheDbFactory = cacheDbFactory;
        _logger = logger;
    }

    // Read

    public async Task<T> ReadAsync<T>(
        Func<DbSet<AssetPresenceInfo>, T> readFunc,
        CancellationToken ctoken = default)
    {
        try
        {
            ctoken.ThrowIfCancellationRequested();

            using var dbcontext = await _cacheDbFactory.CreateDbContextAsync(ctoken);

            return await Task.Run(() => readFunc(dbcontext.AssetPresences), ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Read error occurred.");
            throw;
        }
    }

    public async Task<T> ReadAsync<T>(
        Func<DbSet<AssetPresenceInfo>, CancellationToken, Task<T>> readFunc,
        CancellationToken ctoken = default)
    {
        try
        {
            ctoken.ThrowIfCancellationRequested();

            using var dbcontext = await _cacheDbFactory.CreateDbContextAsync(ctoken);

            return await readFunc(dbcontext.AssetPresences, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Read error occurred.");
            throw;
        }
    }

    // Write

    public async Task<int> WriteAsync(
        Func<DbSet<AssetPresenceInfo>, bool> writeFunc,
        CancellationToken ctoken = default)
    {
        try
        {
            ctoken.ThrowIfCancellationRequested();

            using var dbcontext = await _cacheDbFactory.CreateDbContextAsync(ctoken);
            if (!await Task.Run(() => writeFunc(dbcontext.AssetPresences)))
                return 0;

            int writeCount = await dbcontext.SaveChangesAsync(ctoken);
            _logger.LogInformation("Updated {count} entries in the database.", writeCount);
            return writeCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Write error occurred.");
            throw;
        }
    }

    public async Task<int> WriteAsync(
        Func<DbSet<AssetPresenceInfo>, CancellationToken, Task<bool>> writeFunc,
        CancellationToken ctoken = default)
    {
        try
        {
            ctoken.ThrowIfCancellationRequested();

            using var dbcontext = await _cacheDbFactory.CreateDbContextAsync(ctoken);
            var presences = dbcontext.AssetPresences;
            if (!await writeFunc(presences, ctoken))
                return 0;

            int writeCount = await dbcontext.SaveChangesAsync(ctoken);
            _logger.LogInformation("Updated {count} entries in the database.", writeCount);
            return writeCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Write error occurred.");
            throw;
        }
    }

    // Transaction

    public async Task TransactAsync(
        IEnumerable<Func<DbSet<AssetPresenceInfo>, CancellationToken, Task<bool>>> transactFuncs,
        CancellationToken ctoken = default)
    {
        try
        {
            ctoken.ThrowIfCancellationRequested();

            using var dbcontext = await _cacheDbFactory.CreateDbContextAsync(ctoken);
            var presences = dbcontext.AssetPresences;

            using var transaction = await dbcontext.Database.BeginTransactionAsync(ctoken);
            foreach (var transactFunc in transactFuncs)
            {
                if (await transactFunc(presences, ctoken))
                    await dbcontext.SaveChangesAsync(ctoken);
            }
            await transaction.CommitAsync(ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Write error occurred.");
            throw;
        }
    }
}