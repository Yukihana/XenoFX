using CSX.DotNet.Common.Platform;
using System;
using System.IO.Enumeration;

namespace CSX.DotNet.Common.IO.Paths;

public static partial class PathFilterExtensions
{
    // PathFilter

    private static string NormalizeSeparators(string input)
    {
        // Replace '\' with '/' for consistent cross-platform matching
        // Note: Path.DirectorySeparatorChar is platform dependent. Do not use!
        return input.ToString().Replace('\\', '/');
    }

    public static bool MatchFilenameByPattern(
        this ReadOnlySpan<char> name,
        ReadOnlySpan<char> expression)
    {
        return FileSystemName.MatchesSimpleExpression(expression, name,
            ignoreCase: !FilenameNormalization.PlatformIsCaseSensitive);
    }
}