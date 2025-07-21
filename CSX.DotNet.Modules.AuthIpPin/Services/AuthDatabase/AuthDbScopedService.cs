using CSX.DotNet.Modules.AuthIpPin.Storage.AuthDb;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.AuthIpPin.Services.AuthDatabase;

public class AuthDbScopedService : IAuthDbScopedService
{
    // Infrastructure

    private readonly AuthDbContext _db;
    private readonly ILogger<AuthDbScopedService> _logger;

    // Lifecycle

    public AuthDbScopedService(
        AuthDbContext db,
        ILogger<AuthDbScopedService> logger)
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

            await executeFunc(_db, ctoken);
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

            return await executeFunc(_db, ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create database context.");
            throw;
        }
    }
}