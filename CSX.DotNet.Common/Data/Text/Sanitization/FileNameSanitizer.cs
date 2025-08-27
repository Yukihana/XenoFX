using System;
using System.IO;

namespace CSX.DotNet.Common.Data.Text.Sanitization;

public static partial class FileNameSanitizer
{
    /// <summary>
    /// Sanitizes an input string:
    /// 1. Replace symbols with '-'
    /// 2. Replace spaces with '_'
    /// 3. Compress consecutive replacements into one
    /// </summary>
    public static string Sanitize(string input,
        char symbolReplacement = '-',
        char whiteSpaceReplacement = '_',
        bool compressReplacementsTogether = true)
    {
        input = input.Trim();

        if (string.IsNullOrEmpty(input))
            return input;

        string result = SharedSanitizers.SanitizeSymbols(
            input: input,
            replacement: symbolReplacement);
        result = SharedSanitizers.SanitizeWhiteSpace(
            input: result,
            replacement: whiteSpaceReplacement);
        result = SharedSanitizers.CompressAndTrimArtifacts(
            input: result,
            artifacts: [symbolReplacement, whiteSpaceReplacement],
            sharedCompress: compressReplacementsTogether);

        return result;
    }

    /// <summary>
    /// Extracts path + query from a Uri, then sanitizes it.
    /// Keeps domains/extensions intact due to exclusions.
    /// </summary>
    public static string FromPathAndQuery(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        string input = uri.PathAndQuery;
        return Sanitize(input);
    }

    public static string FromPathAndQuery(string url)
        => FromPathAndQuery(new Uri(url));

    /// <summary>
    /// Strips protocol (http/https/etc.) from URL string, then sanitizes.
    /// Keeps domain + extension intact.
    /// </summary>
    public static string FromUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        if (Uri.TryCreate(url, UriKind.Absolute, out var absolute))
        {
            // Combine host + path + query, leaving out scheme/protocol
            string withoutScheme = absolute.Host + absolute.PathAndQuery;
            return Sanitize(withoutScheme);
        }
        else
        {
            // Not an absolute URL, treat as raw input
            return Sanitize(url);
        }
    }

    public static string FromTitle(string title)
    {
        string sanitized = Sanitize(title);
        return sanitized.ToLowerInvariant();
    }

    public static string FromFileName(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        // Extract the last path segment
        string fileName = Path.GetFileName(filePath);

        return Sanitize(fileName);
    }
}