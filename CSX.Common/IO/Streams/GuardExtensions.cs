using System;
using System.IO;

namespace CSX.Common.IO.Streams;

public static partial class GuardExtensions
{
    /// <summary>
    /// Validates the stream can be read and seeked.
    /// </summary>
    public static void EnsureSeekableReadable(
        this Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead || !stream.CanSeek)
            throw new NotSupportedException("Stream must support both reading and seeking.");
    }
}