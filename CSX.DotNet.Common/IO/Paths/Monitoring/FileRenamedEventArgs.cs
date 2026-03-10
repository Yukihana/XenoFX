using System;

namespace CSX.DotNet.Common.IO.Paths.Monitoring;

public class FileRenamedEventArgs
    : EventArgs, IFileSystemObserverEventArgs
{
    public FileRenamedEventArgs(
        string fullPath,
        string oldFullPath,
        DateTimeOffset timeStamp)
    {
        FullPath = fullPath;
        OldFullPath = oldFullPath;
        TimeStamp = timeStamp;
    }

    public string FullPath { get; }
    public string OldFullPath { get; }
    public DateTimeOffset TimeStamp { get; }
}