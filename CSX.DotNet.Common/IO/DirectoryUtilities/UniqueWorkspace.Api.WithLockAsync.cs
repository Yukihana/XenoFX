using CSX.DotNet.Common.Data.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.DirectoryUtilities;

public partial class UniqueWorkspace
{
    // Async : void, generic, try

    public async Task ExecuteWithLockAsync(
        Func<string, CancellationToken, Task> action,
        CancellationToken ctoken = default)
    {
        await _lockAccess.WaitAsync(ctoken);
        try
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            await action(RootPath, ctoken);
        }
        catch (OperationCanceledException) { throw; }
        catch (ObjectDisposedException) { throw; }
        catch (Exception ex) { throw new ActionExecutionException(ex); }
        finally { _lockAccess.Release(); }
    }

    public async Task<T> ExecuteWithLockAsync<T>(
        Func<string, CancellationToken, Task<T>> action,
        CancellationToken ctoken = default)
    {
        await _lockAccess.WaitAsync(ctoken);
        try
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            return await action(RootPath, ctoken);
        }
        catch (OperationCanceledException) { throw; }
        catch (ObjectDisposedException) { throw; }
        catch (Exception ex) { throw new ActionExecutionException(ex); }
        finally { _lockAccess.Release(); }
    }

    public async Task<bool> TryExecuteWithLockAsync(
        Func<string, CancellationToken, Task> action,
        TimeSpan? timeout = null,
        CancellationToken ctoken = default)
    {
        try
        {
            // Acquire lock
            if (!timeout.HasValue)
                await _lockAccess.WaitAsync(ctoken);
            else if (!await _lockAccess.WaitAsync(timeout.Value, ctoken))
                return false;

            try
            {
                ObjectDisposedException.ThrowIf(IsDisposed, this);

                await action(RootPath, ctoken);
                return true;
            }
            finally { _lockAccess.Release(); }
        }
        catch (OperationCanceledException) { throw; }
        catch (ObjectDisposedException) { return false; }   // Swallow if disposed
        catch (Exception ex) { throw new ActionExecutionException(ex); }
    }
}