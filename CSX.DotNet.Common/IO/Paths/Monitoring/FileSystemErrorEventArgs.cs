using System;

namespace CSX.DotNet.Common.IO.Paths.Monitoring;

public class FileSystemErrorEventArgs
    : EventArgs, IFileSystemObserverEventArgs
{
    public FileSystemErrorEventArgs(
        Exception exception,
        DateTimeOffset timeStamp)
    {
        Exception = exception;
        TimeStamp = timeStamp;
    }

    public DateTimeOffset TimeStamp { get; }
    public Exception Exception { get; }
}