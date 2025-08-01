using System;
using System.Linq;
using System.Text;

namespace CSX.DotNet.Common.IO.Paths;

public static partial class FilenameKebabizer
{
    public static string Sanitize(string input)
    {
        input = input.Trim();
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // In case declared filename isn't legal
        string sanitized = input.ReplaceUnsafeFilenameChars();

        return sanitized.TrimAndCollapseSeparators('-', '_');
    }

    public static string FromUrl(string url)
    {
        url = url.Trim();
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        url = url.StripProtocol();
        return Sanitize(url);
    }

    public static string FromTitle(string title)
    {
        title = title.Trim();
        if (string.IsNullOrEmpty(title))
            return string.Empty;

        title = title.ToLowerInvariant();
        return Sanitize(title);
    }

    // Core

    private static string ReplaceUnsafeFilenameChars(
        this string input)
    {
        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            // Letters, digits, hyphens and underscores are allowed as is.
            if (char.IsLetterOrDigit(c) || c is '-' or '_')
                sb.Append(c);
            // Spaces are converted to hyphens and other symbols to underscores.
            else
                sb.Append(c is ' ' ? '-' : '_');
        }
        return sb.ToString();
    }

    private static string TrimAndCollapseSeparators(
        this string input,
        params char[] separators)
    {
        input = input.Trim(separators);

        StringBuilder sb = new();
        char? last = null;
        char c;

        for (int i = 0; i < input.Length; i++)
        {
            c = input[i];
            // Append if it's not a separator or
            // not the same separator as the last character
            if (!separators.Contains(c) || c != last)
                sb.Append(c);
            last = c;
        }

        return sb.ToString();
    }

    private static string StripProtocol(
        this string input)
    {
        int schemeEndIndex = input.IndexOf("://", StringComparison.Ordinal);
        return schemeEndIndex != -1 ? input[(schemeEndIndex + 3)..] : input;
    }
}