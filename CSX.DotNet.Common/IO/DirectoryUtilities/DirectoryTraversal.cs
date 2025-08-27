using System;
using System.IO;
using System.Linq;

namespace CSX.DotNet.Common.IO.DirectoryUtilities;

public static class DirectoryTraversal
{
    public static string TraverseUntilBranchOutOrDeadEnd(
        string rootDirectory,
        string stopAtDirectoryName = "")
    {
        if (string.IsNullOrWhiteSpace(rootDirectory))
            throw new ArgumentException("Root path cannot be null or empty.", nameof(rootDirectory));

        if (!Directory.Exists(rootDirectory))
            throw new DirectoryNotFoundException($"The specified root path does not exist: {rootDirectory}");

        string currentDirectory = rootDirectory;
        stopAtDirectoryName = stopAtDirectoryName.Trim();

        while (true)
        {
            var subDirectories = Directory.GetDirectories(currentDirectory);

            if (subDirectories.Length == 0)
            {
                // Dead end
                return currentDirectory;
            }

            // If stopAt matches one of the subdirectories, go straight there
            if (!string.IsNullOrWhiteSpace(stopAtDirectoryName))
            {
                var match = subDirectories.FirstOrDefault(d =>
                    string.Equals(Path.GetFileName(d), stopAtDirectoryName, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                    return match;
            }

            if (subDirectories.Length == 1)
            {
                // Continue down single child
                currentDirectory = subDirectories[0];
            }
            else
            {
                // Branch out
                return currentDirectory;
            }
        }
    }
}