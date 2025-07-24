using CSX.Common.IO.Streams;
using System;
using System.Buffers;
using System.IO;

namespace CSX.Common.Cryptography;

public static partial class FileCrumbs
{
    /// <summary>
    /// Samples spread bytes from specific offset in the stream.
    /// Does not restore the stream position.
    /// </summary>
    public static int SampleBytes(
        Stream stream,
        long start,
        long length,
        Span<byte> sampleBuffer)
    {
        stream.EnsureSeekableReadable();

        if (length <= 0 || sampleBuffer.IsEmpty)
            return 0;

        if (start < 0 || start + length > stream.Length)
            throw new ArgumentOutOfRangeException(nameof(length), "Requested range exceeds stream bounds.");

        long[] rented = ArrayPool<long>.Shared.Rent(sampleBuffer.Length);
        try
        {
            Span<long> indexSpan = rented.AsSpan();
            int count = GetSampleIndices(length, indexSpan);

            return MapSampleBytesFromIndices(
                stream,
                start,
                indexSpan[..count],
                sampleBuffer[..count]);
        }
        finally { ArrayPool<long>.Shared.Return(rented); }
    }

    /// <summary>
    /// Samples spread bytes from current stream position to (position + length).
    /// Does not restore the stream position.
    /// </summary>
    public static int SampleBytes(
        Stream stream,
        long length,
        Span<byte> sampleBuffer)
    {
        stream.EnsureSeekableReadable();

        long start = stream.Position;
        return SampleBytes(
            stream: stream,
            start: start,
            length: length,
            sampleBuffer: sampleBuffer);
    }

    /// <summary>
    /// Samples spread bytes from the entire stream (position 0 to end).
    /// Does not restore the stream position.
    /// </summary>
    public static int SampleBytesFull(Stream stream, Span<byte> sampleBuffer)
    {
        stream.EnsureSeekableReadable();

        stream.Position = 0;
        long length = stream.Length;

        return SampleBytes(
            stream: stream,
            start: 0,
            length: length,
            sampleBuffer: sampleBuffer);
    }
}