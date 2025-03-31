using System;
using System.IO;
using System.IO.Enumeration;
using System.Linq;

namespace CSX.Common.Platform;

public static class FilenameNormalization
{
    private static readonly bool _platformIsCaseSensitive;
    private static readonly StringComparison _filenameComparison;
    private static readonly StringComparer _filenameComparer;

    static FilenameNormalization()
    {
        _platformIsCaseSensitive = !(
            OperatingSystem.IsWindows() ||
            OperatingSystem.IsMacOS() ||
            OperatingSystem.IsIOS() ||
            OperatingSystem.IsTvOS() ||
            OperatingSystem.IsWatchOS());
        _filenameComparison = _platformIsCaseSensitive
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;
        _filenameComparer = StringComparer.FromComparison(_filenameComparison);
    }

    public static bool PlatformIsCaseSensitive
        => _platformIsCaseSensitive;

    public static StringComparison FilenameComparison
        => _filenameComparison;

    public static StringComparer FilenameComparer
        => _filenameComparer;

    public static bool MatchFilenameByPattern(this ReadOnlySpan<char> name, ReadOnlySpan<char> expression)
        => FileSystemName.MatchesWin32Expression(expression, name, ignoreCase: !_platformIsCaseSensitive);

    // Filename Sanitization

    public static string SanitizeForFilename(this ReadOnlySpan<char> input)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        Span<char> buffer = stackalloc char[input.Length];

        int index = 0;

        foreach (char c in input)
        {
            if (invalidChars.Contains(c) || char.IsWhiteSpace(c))
                buffer[index++] = '_'; // Replace invalid chars and spaces with underscores
            else
                buffer[index++] = c;
        }

        return new string(buffer[..index]);
    }
}