using System;
using System.Collections.Generic;

namespace CSX.DotNet.Common.Data.Events;

public class FileListingEventArgs : EventArgs
{
    public FileListingEventArgs(IEnumerable<string> filePaths)
    {
        FilePaths = filePaths ?? throw new ArgumentNullException(nameof(filePaths));
    }

    public IEnumerable<string> FilePaths { get; }
}