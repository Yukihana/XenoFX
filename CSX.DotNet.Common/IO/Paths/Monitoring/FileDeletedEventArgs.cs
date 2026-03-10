using System;

namespace CSX.DotNet.Common.IO.Paths.Monitoring;

public class FileDeletedEventArgs
    : EventArgs, IFileSystemObserverEventArgs
{
    public FileDeletedEventArgs(
        string fullPath,
        DateTimeOffset timeStamp)
    {
        FullPath = fullPath;
        TimeStamp = timeStamp;
    }

    public string FullPath { get; }
    public DateTimeOffset TimeStamp { get; }
}