using CSX.Common.Data.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XenoFx.Services.Background.AssetIndexing;

public static partial class IndexingExtensions
{
    // Storage : Move this to commons

    private static char[] EnforcedInvalidFileNameCharacters
        => ['/', '\\', ':', '*', '?', '"', '<', '>', '|', '&'];

    private static char[] InvalidFileNameCharacters
        => [.. Path.GetInvalidFileNameChars().Concat(EnforcedInvalidFileNameCharacters).Distinct()];

    // Functions

    public static string GetAssetBaseName(this FileUploadedEventArgs eventArgs)
    {
        foreach (string candidate in GetFilenameCandidates(eventArgs))
        {
            string sanitized = Sanitize(candidate);

            if (!string.IsNullOrWhiteSpace(sanitized))
                return sanitized;
        }

        // Fallback to a prefix if all candidates are invalid (it will be guid suffixed when move is attempted)
        return "unnamed_asset_upload";
    }

    public static IEnumerable<string> GetFilenameCandidates(FileUploadedEventArgs eventArgs)
    {
        yield return Path.GetFileNameWithoutExtension(eventArgs.PreferredFilename);

        yield return eventArgs.Title;

        yield return eventArgs.DataUrl;

        yield return eventArgs.PageUrl;

        yield return Path.GetFileNameWithoutExtension(eventArgs.ReportedFilename);

        yield return Path.GetFileNameWithoutExtension(eventArgs.TemporaryFileFullPath);
    }

    public static string Sanitize(string raw)
    {
        raw = raw.Trim();
        if (string.IsNullOrWhiteSpace(raw))
            return string.Empty;

        // Remove the scheme (e.g., "http://", "https://")
        int schemeEndIndex = raw.IndexOf("://");
        if (schemeEndIndex != -1)
            raw = raw[(schemeEndIndex + 3)..];

        // Use StringBuilder for better performance when replacing invalid characters
        var sanitized = new StringBuilder(raw);

        // Remove any invalid characters
        foreach (char invalidChar in InvalidFileNameCharacters)
            sanitized = sanitized.Replace(invalidChar, '_');

        return sanitized.ToString();
    }
}