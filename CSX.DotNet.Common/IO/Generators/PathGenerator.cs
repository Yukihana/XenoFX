using System;
using System.IO;
using System.Threading;

namespace CSX.DotNet.Common.IO.Generators;

public static class PathGenerator
{
    [Obsolete("This method needs to be modified for library usage.")]
    public static string MoveToFinalPath(
        string tempPath,
        string basename,
        string extension,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        bool emptyTitle = string.IsNullOrWhiteSpace(basename);
        string filename = emptyTitle ? $"{Guid.NewGuid()}" : basename;

        // Try for each possible name until one is available.
        while (true)
        {
            ctoken.ThrowIfCancellationRequested();

            string directory
                = Path.GetDirectoryName(basename)
                ?? throw new ArgumentException("Expected full path", nameof(basename));
            string selectedPath = Path.Combine(
                directory,
                $"{filename}.{extension}");

            try
            {
                File.Move(tempPath, selectedPath);
                return selectedPath;
            }
            catch (IOException) when (File.Exists(selectedPath))
            { }

            // prep new for next iteration
            string guid = Guid.NewGuid().ToString();
            filename = emptyTitle ? guid : $"{basename}_{guid}";
        }
    }
}