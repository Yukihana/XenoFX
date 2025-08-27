using System;
using System.Collections.Generic;

namespace CSX.DotNet.Common.Data.Text.Sanitization;

public static partial class SharedSanitizers
{
    public static string SanitizeSymbols(
        this ReadOnlySpan<char> input,
        char replacement = '-',
        HashSet<char>? exclusions = null)
    {
        if (input.IsEmpty)
            return string.Empty;

        exclusions ??= ['.']; // default exclusion

        Span<char> buffer = stackalloc char[input.Length];
        int index = 0;

        foreach (char c in input)
        {
            bool passthrough =
                char.IsLetterOrDigit(c) ||
                char.IsWhiteSpace(c) ||
                exclusions.Contains(c) ||
                c == replacement;

            buffer[index++]
                = passthrough
                ? c
                : replacement;
        }

        return new string(buffer[..index]);
    }

    public static string SanitizeWhiteSpace(
        this ReadOnlySpan<char> input,
        char replacement = '_')
    {
        if (input.IsEmpty)
            return string.Empty;

        Span<char> buffer = stackalloc char[input.Length];
        int index = 0;

        foreach (char c in input)
        {
            buffer[index++]
                = char.IsWhiteSpace(c)
                ? replacement
                : c;
        }

        return new string(buffer[..index]);
    }

    public static string CompressArtifacts(
        this ReadOnlySpan<char> input,
        HashSet<char> artifacts,
        bool sharedCompress = false)
    {
        if (input.IsEmpty || artifacts == null || artifacts.Count == 0)
            return input.ToString();

        Span<char> buffer = stackalloc char[input.Length];
        int index = 0;
        char? lastArtifact = null;

        foreach (char c in input)
        {
            if (!artifacts.Contains(c))
            {
                buffer[index++] = c;
                lastArtifact = null; // reset
                continue;
            }

            if (lastArtifact == c)
                continue;

            if (lastArtifact == null || !sharedCompress)
            {
                buffer[index++] = c;
                lastArtifact = c;
            }
        }

        return new string(buffer[..index]);
    }

    public static string CompressAndTrimArtifacts(
        string input,
        HashSet<char> artifacts,
        bool sharedCompress = false)
    {
        string compressed = CompressArtifacts(
            input: input,
            artifacts: artifacts,
            sharedCompress: sharedCompress);

        return compressed.Trim([.. artifacts]);
    }
}