using System;
using System.IO;

namespace CSX.DotNet.Common.IO.Streams;

public static partial class GuardExtensions
{
    public static void EnsureReadable(
        this Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead)
            throw new ArgumentException("Stream must support reading.", nameof(stream));
    }

    public static void EnsureSeekable(
        this Stream stream,
        string? message = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        string guardMessage = message ?? "Stream must support seeking.";
        if (!stream.CanSeek)
            throw new ArgumentException(guardMessage, nameof(stream));
    }

    public static void EnsureSeekableReadable(
        this Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead || !stream.CanSeek)
            throw new NotSupportedException("Stream must support both reading and seeking.");
    }
}