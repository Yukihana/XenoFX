using System;
using System.IO;

namespace CSX.DotNet.Common.IO.Generators;

public static class DirectoryGenerator
{
    public static string GenerateGuidDirectory(
        string parentDirectory)
    {
        if (string.IsNullOrEmpty(parentDirectory))
            throw new ArgumentNullException(nameof(parentDirectory));

        // Ensure parent directory, or let it fail through
        if (!Directory.Exists(parentDirectory))
            Directory.CreateDirectory(parentDirectory);

        // Enumerate through guids and create a directory
        while (true)
        {
            // Do not try-catch this to let the permission errors bubble up
            // The uniqueness of this operation should mostly cover other issues.

            string guid = Guid.NewGuid().ToString();
            string path = Path.Combine(parentDirectory, guid);

            // If it exists already, regenerate guid
            if (Directory.Exists(path))
                continue;

            // Unique + Small time frame since checking it exists or not.
            // Assume this operation adequately atomic for this purpose.
            Directory.CreateDirectory(path);
            return path;
        }
    }
}