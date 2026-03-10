using System;

namespace CSX.DotNet.Common.IO.Paths.Monitoring;

public class FileCreatedEventArgs
    : EventArgs, IFileSystemObserverEventArgs
{
    public FileCreatedEventArgs(
        string fullPath,
        DateTimeOffset timeStamp)
    {
        FullPath = fullPath;
        TimeStamp = timeStamp;
    }

    public string FullPath { get; }
    public DateTimeOffset TimeStamp { get; }
}