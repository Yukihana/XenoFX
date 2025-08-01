using CSX.DotNet.Common.Platform;
using System;
using System.IO;

namespace CSX.DotNet.Common.IO.Paths;

public static partial class PathExtensions
{
    public static string ResolveCombine(
        string basePath,
        string path)
    {
        string selectedPath
            = Path.IsPathFullyQualified(path)
            ? path
            : Path.Combine(basePath, path);

        string resolvedPath = Path.GetFullPath(selectedPath);
        string normalizedBase = Path.GetFullPath(basePath);

        // If not within base path, return as is.
        if (!resolvedPath.StartsWith(normalizedBase, FilenameNormalization.FilenameComparison))
            return resolvedPath;

        // Else, get relative, then combine with original basePath
        string relativePath = Path.GetRelativePath(normalizedBase, resolvedPath);
        return Path.Combine(basePath, relativePath);
    }

    public static bool IsPathWithinDirectory(
        string path,
        string basePath)
    {
        basePath = basePath.TrimEnd(Path.DirectorySeparatorChar);
        string fullBasePath = Path.GetFullPath(basePath + Path.DirectorySeparatorChar);

        string fullPath = Path.GetFullPath(path);

        return fullPath.StartsWith(
            fullBasePath,
            StringComparison.OrdinalIgnoreCase);
    }
}