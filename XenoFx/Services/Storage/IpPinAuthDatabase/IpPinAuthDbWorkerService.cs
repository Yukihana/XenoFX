using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.AuthDb;

namespace XenoFx.Services.Storage.IpPinAuthDatabase;

public class IpPinAuthDbWorkerService : IIpPinAuthDbWorkerService
{
    // Infrastructure

    private readonly IDbContextFactory<AuthDbContext> _db;
    private readonly ILogger<IpPinAuthDbWorkerService> _logger;

    // Lifecycle

    public IpPinAuthDbWorkerService(
        IDbContextFactory<AuthDbContext> db,
        ILogger<IpPinAuthDbWorkerService> logger)
    {
        _db = db;
        _logger = logger;
    }

    // API

    public async Task ExecuteAsync(
        Func<AuthDbContext, CancellationToken, Task> executeFunc,
        CancellationToken ctoken = default)
    {
        try
        {
            ctoken.ThrowIfCancellationRequested();

            using var context = await _db.CreateDbContextAsync(ctoken);

            await executeFunc(context, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create database context.");
            throw;
        }
    }

    public async Task<T> ExecuteAsync<T>(
        Func<AuthDbContext, CancellationToken, Task<T>> executeFunc,
        CancellationToken ctoken = default)
    {
        try
        {
            ctoken.ThrowIfCancellationRequested();

            using var context = await _db.CreateDbContextAsync(ctoken);

            return await executeFunc(context, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create database context.");
            throw;
        }
    }
}