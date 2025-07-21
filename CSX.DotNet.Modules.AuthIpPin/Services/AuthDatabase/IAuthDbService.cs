using CSX.DotNet.Modules.AuthIpPin.Storage.AuthDb;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.AuthIpPin.Services.AuthDatabase;

public interface IAuthDbService
{
    Task ExecuteAsync(
        Func<AuthDbContext, CancellationToken, Task> executeFunc,
        CancellationToken ctoken = default);

    Task<T> ExecuteAsync<T>(
        Func<AuthDbContext, CancellationToken, Task<T>> executeFunc,
        CancellationToken ctoken = default);

    // Table scaffold derivatives:  Sessions, Ip, etc
}

/// <summary>
/// Use this service for operations that are scoped to a single request.
/// </summary>
public interface IAuthDbScopedService : IAuthDbService
{ }

/// <summary>
/// Use this service for operations that are not tied to a scope.
/// </summary>
public interface IAuthDbWorkerService : IAuthDbService
{ }