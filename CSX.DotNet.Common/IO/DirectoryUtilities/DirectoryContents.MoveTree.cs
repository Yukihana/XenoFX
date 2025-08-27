using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace CSX.DotNet.Common.IO.DirectoryUtilities;

public static partial class DirectoryContents
{
    public static int MoveTreesRecursive(
        IEnumerable<string> topLevelEntries,        // Files and directories at the top level
        string targetRootDirectory,
        bool overwrite = false,
        bool overwriteOnEntryTypeMismatch = false,  // Only works if overwrite is also true
        Func<string, int>? onUpdate = null,         // int for batching between updates
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(topLevelEntries);
        if (string.IsNullOrWhiteSpace(targetRootDirectory))
            throw new ArgumentNullException(nameof(targetRootDirectory));

        int movedCount = 0;
        int batchSize = 1;

        foreach (string entry in topLevelEntries)
        {
            string name = Path.GetFileName(entry);
            if (string.IsNullOrEmpty(name))
                continue;

            string destPath = Path.Combine(targetRootDirectory, name);

            if (Directory.Exists(entry))
            {
                // Remove existing file if it conflicts with a directory move
                if (File.Exists(destPath))
                {
                    if (overwrite && overwriteOnEntryTypeMismatch)
                        File.Delete(destPath);
                    else
                        continue; // Skip this entry if we can't overwrite
                }

                // Remember whether the destination directory already exists
                var destExists = Directory.Exists(destPath);

                // Create target directory if missing
                if (!destExists)
                    Directory.CreateDirectory(destPath);

                // Recurse into subdirectories & files
                var subEntries = Directory.EnumerateFileSystemEntries(entry);
                movedCount += MoveTreesRecursive(
                    topLevelEntries: subEntries,
                    targetRootDirectory: destPath,
                    overwrite: overwrite,
                    onUpdate: onUpdate,
                    ctoken: ctoken);

                // Preserve metadata if directory was created
                if (!destExists)
                    CopyDirectoryMetadata(entry, destPath);

                // Delete source directory if empty
                if (!Directory.EnumerateFileSystemEntries(entry).Any())
                    Directory.Delete(entry);
            }
            else if (File.Exists(entry))
            {
                // File move
                try
                {
                    // Remove existing directory if it conflicts with a file move
                    if (Directory.Exists(destPath))
                    {
                        if (overwrite && overwriteOnEntryTypeMismatch)
                            Directory.Delete(destPath, recursive: true);
                        else
                            continue; // Skip this entry if we can't overwrite
                    }

                    File.Move(entry, destPath, overwrite);
                    movedCount++;
                }
                catch (IOException)
                {
                    if (!(File.Exists(destPath) || Directory.Exists(destPath)))
                        throw; // rethrow unexpected I/O errors
                }
            }

            // Progress callback batching (allows changing batch size dynamically)
            if (onUpdate != null && movedCount % batchSize == 0)
            {
                batchSize = Math.Max(1, onUpdate(entry));
                ctoken.ThrowIfCancellationRequested();
            }
        }

        return movedCount;
    }

    private static void CopyDirectoryMetadata(string sourceDir, string destDir)
    {
        DirectoryInfo srcInfo = new(sourceDir);
        _ = new DirectoryInfo(destDir)
        {
            CreationTimeUtc = srcInfo.CreationTimeUtc,
            LastWriteTimeUtc = srcInfo.LastWriteTimeUtc,
            LastAccessTimeUtc = srcInfo.LastAccessTimeUtc
        };
    }

    [Obsolete("Use MoveTreesRecursive instead.")]
    public static void MoveTopLevelEntries(
        string[] topLevelEntries,
        string targetRootDirectory)
    {
        foreach (string entry in topLevelEntries)
        {
            string destPath = Path.Combine(targetRootDirectory, Path.GetFileName(entry));

            if (File.Exists(entry))
            {
                File.Move(entry, destPath, overwrite: true); // .NET 6+
            }
            else if (Directory.Exists(entry))
            {
                Directory.Move(entry, destPath); // Moves entire directory recursively
            }
        }
    }
}