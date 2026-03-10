using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Paths.Monitoring;

public sealed class FileSystemObserver
    : IDisposable
{
    private readonly Func<object?, IFileSystemObserverEventArgs, CancellationToken, Task> _eventCallback;
    private readonly ILogger? _logger;

    private readonly FileSystemWatcher _watcher;
    private CancellationTokenSource _cts = new();

    // Lifecycle

    public FileSystemObserver(
        string baseLocation,
        Func<object?, IFileSystemObserverEventArgs, CancellationToken, Task> eventCallback,
        string filterPattern = "*",

        ILogger? logger = null)
    {
        _eventCallback = eventCallback;
        _logger = logger;

        // Instantiate
        _watcher = new FileSystemWatcher();
        {
            _watcher.Path = baseLocation;
            _watcher.EnableRaisingEvents = false;
            _watcher.IncludeSubdirectories = true;
            _watcher.Filter = filterPattern;
        }

        // Register handlers
        _watcher.Created += OnFileCreated;
        _watcher.Deleted += OnFileDeleted;
        _watcher.Changed += OnFileModified;
        _watcher.Renamed += OnFileRenamed;
        _watcher.Error += OnError;
    }

    public void Dispose()
    {
        Stop(); // Cancel
        _watcher.Dispose();
        _cts.Dispose();
    }

    // Start/Stop

    public void Start()
    {
        if (_cts.IsCancellationRequested)
        {
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }

        _watcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
        _cts.Cancel();
        _watcher.EnableRaisingEvents = false;
    }

    public bool IsEnabled
    {
        get => _watcher.EnableRaisingEvents;
        set
        {
            if (value == _watcher.EnableRaisingEvents)
                return;

            if (value) Start();
            else Stop();
        }
    }

    // API (Live-Read passthrough)

    public async Task<FileSystemEnumeratedEventArgs> EnumerateAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Offload the blocking enumeration to a sync pipeline.
        var filePaths = await Task.Run(
            () => Directory.GetFiles(
                _watcher.Path,
                _watcher.Filter,
                SearchOption.AllDirectories),
            cancellationToken);

        var args = new FileSystemEnumeratedEventArgs(
            filePaths: filePaths,
            timeStamp: DateTimeOffset.UtcNow);

        return args;
    }

    // Handler Core

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Legacy non-async code starting an async chain.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Incorrect assumption that above suppression is not being used.")]
    public async void Handle(
        object? sender,
        Func<IFileSystemObserverEventArgs> createArgs,
        Exception? primaryException = null)
    {
        try
        {
            var args = createArgs();

            await _eventCallback(sender, args, _cts.Token);
        }
        catch (Exception ex)
        {
            Exception logEx = primaryException is not null
                 ? new AggregateException(primaryException, ex)
                 : ex;

            _logger?.LogError(logEx, "Error handling file system event.");
        }
    }

    // Handlers

    private void OnFileCreated(
        object? sender,
        FileSystemEventArgs e)
    {
        Handle(sender, () => new FileCreatedEventArgs(
            fullPath: e.FullPath,
            timeStamp: DateTimeOffset.UtcNow));
    }

    private void OnFileDeleted(
        object? sender,
        FileSystemEventArgs e)
    {
        Handle(sender, () => new FileDeletedEventArgs(
            fullPath: e.FullPath,
            timeStamp: DateTimeOffset.UtcNow));
    }

    private void OnFileModified(
        object? sender,
        FileSystemEventArgs e)
    {
        Handle(sender, () => new FileModifiedEventArgs(
            fullPath: e.FullPath,
            timeStamp: DateTimeOffset.UtcNow));
    }

    private void OnFileRenamed(
        object? sender,
        RenamedEventArgs e)
    {
        Handle(sender, () => new FileRenamedEventArgs(
            fullPath: e.FullPath,
            oldFullPath: e.OldFullPath,
            timeStamp: DateTimeOffset.UtcNow));
    }

    private void OnError(
        object? sender,
        ErrorEventArgs e)
    {
        var fsexception = e.GetException();
        Handle(sender, () => new FileSystemErrorEventArgs(
            exception: fsexception,
            timeStamp: DateTimeOffset.UtcNow),
            primaryException: fsexception);
    }
}