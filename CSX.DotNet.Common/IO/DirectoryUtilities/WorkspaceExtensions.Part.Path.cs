using CSX.DotNet.Common.Platform;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace CSX.DotNet.Common.IO.DirectoryUtilities;

public static partial class WorkspaceExtensions
{
    private static string ReslashPathEnd(
        string path)
    {
        return Path.GetFullPath(path)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
    }

    public static int SegmentCount(
        string path)
    {
        // Legacy code kept as patch-up in case existing 'IsWorkspaceRoot' logic fails.

        string[] segments = path.Split([
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar],
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        return segments.Length;
    }

    // Public API : Core

    public static WorkspacePathInfo AnalyzePath(
        this IWorkspace workspace,
        string path)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(path);

        // Sanitize root
        string sanitizedRoot = ReslashPathEnd(workspace.RootPath);

        // Absolute path
        string absolutePath = Path.IsPathRooted(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(sanitizedRoot, path));

        // Failfast if not a workspace path
        string endSlashedPath = ReslashPathEnd(absolutePath); // Ensure end slash consistency for comparison
        if (!endSlashedPath.StartsWith(sanitizedRoot, FilenameNormalization.FilenameComparison))
            throw new InvalidDataException("Not a valid workspace path.");

        // Relative path
        string relativePath = Path.GetRelativePath(sanitizedRoot, absolutePath);

        // Root detection
        bool isWorkspaceRoot = string.IsNullOrEmpty(relativePath) || // Handle edge cases
            string.Equals(relativePath, ".", StringComparison.Ordinal);

        // Finish
        return new WorkspacePathInfo(
            AbsolutePath: absolutePath,
            RelativePath: relativePath,
            RootPath: sanitizedRoot,
            IsWorkspaceRoot: isWorkspaceRoot);
    }

    public static bool TryAnalyzePath(
        this IWorkspace workspace,
        string path,
        [NotNullWhen(true)] out WorkspacePathInfo? pathInfo)
    {
        try
        {
            pathInfo = AnalyzePath(workspace, path);
            return true;
        }
        catch
        {
            pathInfo = null;
            return false;
        }
    }

    // Public API : Convenience wrappers

    public static string GetAbsolutePath(this IWorkspace workspace, string path)
        => workspace.AnalyzePath(path).AbsolutePath;

    public static string GetRelativePath(this IWorkspace workspace, string path)
        => workspace.AnalyzePath(path).RelativePath;

    public static bool IsInWorkspace(this IWorkspace workspace, string path)
        => workspace.TryAnalyzePath(path, out _);

    public static bool IsWorkspaceRoot(this IWorkspace workspace, string path)
        => workspace.TryAnalyzePath(path, out var info) && info.Value.IsWorkspaceRoot;

    public static bool IsWithinWorkspace(this IWorkspace workspace, string path)
        => workspace.TryAnalyzePath(path, out var info) && info.Value.IsInsideWorkspace;
}