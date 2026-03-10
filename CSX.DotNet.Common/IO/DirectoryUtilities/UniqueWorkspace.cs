using CSX.DotNet.Common.Data.Guids;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.DirectoryUtilities;

public partial class UniqueWorkspace :
    IWorkspace,
    IDisposable,
    IAsyncDisposable
{
    // Defaults

    public const string LockFileName = "temp.lock";

    // Infrastructure

    private readonly FileStream _lockStream;
    private readonly SemaphoreSlim _lockAccess = new(1, 1);

    // Public Properties

    public string RootPath { get; }
    public bool ClearContentsOnDispose { get; set; } = true;
    public Action<Exception>? OnCleanupFailure { get; set; } = null;
    public bool IsDisposed { get; private set; } = false;

    // Lifecycle

    protected UniqueWorkspace() : this("")
    {
        // block direct access to the heavy constructor
        // enforce factory pattern
    }

    protected UniqueWorkspace(
        string parentDirectory = "",
        bool clearContentsOnDispose = true)
    {
        // Use working environment if no parent directory is specified
        parentDirectory
            = !string.IsNullOrWhiteSpace(parentDirectory)
            ? Path.GetFullPath(parentDirectory)
            : Environment.CurrentDirectory;

        // In case of bad address or permissions issue, allow this to throw early.
        Directory.CreateDirectory(parentDirectory);

        // Create and lock a temp directory
        string rootPath = AcquireDirectory(parentDirectory);
        _lockStream = AcquireLock(rootPath);
        RootPath = rootPath;

        // Addon parameters
        ClearContentsOnDispose = clearContentsOnDispose;
    }

    private async Task DisposeBaseAsync()
    {
        await _lockAccess.WaitAsync();
        try
        {
            if (IsDisposed)
                return;

            // Release lock file
            try { await _lockStream.DisposeAsync(); }
            catch (Exception ex) { OnCleanupFailure?.Invoke(ex); }

            // Attempt lock cleanup regardless
            string lockFilePath = Path.Combine(
                RootPath,
                LockFileName);

            try { File.Delete(lockFilePath); }
            catch (Exception ex) { OnCleanupFailure?.Invoke(ex); }

            // Clear contents
            try
            {
                if (ClearContentsOnDispose && Directory.Exists(RootPath))
                    Directory.Delete(RootPath, true);
            }
            catch (Exception ex) { OnCleanupFailure?.Invoke(ex); }

            // Flag that this instance is now useless
            IsDisposed = true;
        }
        finally { _lockAccess.Release(); }
    }

    [SuppressMessage(
        "Threading",
        "VSTHRD002:Avoid problematic synchronous waits",
        Justification = "Dispose is a sync bridge to DisposeBaseAsync")]
    public virtual void Dispose()
    {
        DisposeBaseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }

    public async virtual ValueTask DisposeAsync()
    {
        await DisposeBaseAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    // Factory Sugar

    public static UniqueWorkspace Create(
        string parentDirectory = "",
        bool clearContentsOnDispose = true)
        => new(parentDirectory, clearContentsOnDispose);

    // Internal

    private static string AcquireDirectory(
        string parentDirectory)
    {
        while (true)
        {
            string dirPath = Path.Combine(
                parentDirectory,
                DMC212710Guid.FromUtcNow().ToString("N"));

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                return dirPath;
            }
        }
    }

    private FileStream AcquireLock(string dirPath)
    {
        string lockFilePath = Path.Combine(
            dirPath,
            LockFileName);

        _lockAccess.Wait();
        try
        {
            return new FileStream(
                path: lockFilePath,
                mode: FileMode.CreateNew,
                access: FileAccess.Write,
                share: FileShare.None,
                bufferSize: 1, // We're not actually writing to it.
                useAsync: false);
        }
        finally { _lockAccess.Release(); }
    }
}