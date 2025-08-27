using System.Collections.Generic;
using System.IO;

namespace CSX.DotNet.Common.IO.DirectoryUtilities;

public static partial class DirectoryContents
{
    public static bool EnsureFilesInside(
        string directory,
        IEnumerable<string> fileList)
    {
        foreach (string fileName in fileList)
        {
            if (!File.Exists(Path.Combine(directory, fileName)))
                return false;
        }
        return true;
    }

    public static bool EnsureFilesInside(
        string directory,
        params string[] files)
    {
        return EnsureFilesInside(
            directory: directory,
            fileList: files);
    }
}