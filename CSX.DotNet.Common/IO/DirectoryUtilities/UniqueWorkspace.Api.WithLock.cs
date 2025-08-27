using CSX.DotNet.Common.Data.Exceptions;
using System;

namespace CSX.DotNet.Common.IO.DirectoryUtilities;

public partial class UniqueWorkspace
{
    // Sync : void, generic, try

    public void ExecuteWithLock(
        Action<string> action)
    {
        _lockAccess.Wait();
        try
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            action(RootPath);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { throw new ActionExecutionException(ex); }
        finally { _lockAccess.Release(); }
    }

    public T ExecuteWithLock<T>(
        Func<string, T> action)
    {
        _lockAccess.Wait();
        try
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            return action(RootPath);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { throw new ActionExecutionException(ex); }
        finally { _lockAccess.Release(); }
    }

    public bool TryExecuteWithLock(
        Action<string> action,
        TimeSpan? timeout = null)
    {
        try
        {
            // Acquire lock
            if (!timeout.HasValue)
                _lockAccess.Wait();
            else if (!_lockAccess.Wait(timeout.Value))
                return false;

            try
            {
                ObjectDisposedException.ThrowIf(IsDisposed, this);

                action(RootPath);
                return true;
            }
            finally { _lockAccess.Release(); }
        }
        catch (OperationCanceledException) { throw; }
        catch (ObjectDisposedException) { return false; }   // Swallow if disposed
        catch (Exception ex) { throw new ActionExecutionException(ex); }
    }
}