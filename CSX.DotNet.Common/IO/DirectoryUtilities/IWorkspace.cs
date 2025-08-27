using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.DirectoryUtilities;

public interface IWorkspace
{
    string RootPath { get; }

    // ExecuteWithLock: Async

    Task ExecuteWithLockAsync(
        Func<string, CancellationToken, Task> action,
        CancellationToken ctoken = default);

    Task<T> ExecuteWithLockAsync<T>(
        Func<string, CancellationToken, Task<T>> action,
        CancellationToken ctoken = default);

    Task<bool> TryExecuteWithLockAsync(
        Func<string, CancellationToken, Task> action,
        TimeSpan? timeout = null,
        CancellationToken ctoken = default);

    // ExecuteWithLock: Sync

    void ExecuteWithLock(
        Action<string> action);

    T ExecuteWithLock<T>(
        Func<string, T> action);

    bool TryExecuteWithLock(
        Action<string> action,
        TimeSpan? timeout = null);
}