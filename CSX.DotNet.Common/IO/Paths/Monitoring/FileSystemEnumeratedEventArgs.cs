using System;

namespace CSX.DotNet.Common.IO.Paths.Monitoring;

public class FileSystemEnumeratedEventArgs
    : EventArgs, IFileSystemObserverEventArgs
{
    public FileSystemEnumeratedEventArgs(
        string[] filePaths,
        DateTimeOffset timeStamp)
    {
        FilePaths = filePaths;
        TimeStamp = timeStamp;
    }

    public string[] FilePaths { get; }
    public DateTimeOffset TimeStamp { get; }
}