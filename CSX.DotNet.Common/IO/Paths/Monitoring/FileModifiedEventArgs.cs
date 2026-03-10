using System;

namespace CSX.DotNet.Common.IO.Paths.Monitoring;

public class FileModifiedEventArgs
    : EventArgs, IFileSystemObserverEventArgs
{
    public FileModifiedEventArgs(
        string fullPath,
        DateTimeOffset timeStamp)
    {
        FullPath = fullPath;
        TimeStamp = timeStamp;
    }

    public string FullPath { get; }
    public DateTimeOffset TimeStamp { get; }
}