using System;
using System.Buffers;

namespace CSX.Common.Cryptography;

public static partial class FileCrumbs
{
    /// <summary>
    /// Samples spread bytes from a raw byte span.
    /// </summary>
    public static int SampleBytes(
        ReadOnlySpan<byte> source,
        Span<byte> sampleBuffer)
    {
        if (source.IsEmpty || sampleBuffer.IsEmpty)
            return 0;

        int[] rented = ArrayPool<int>.Shared.Rent(sampleBuffer.Length);
        try
        {
            // Get number of valid spread indices
            Span<int> indexSpan = rented.AsSpan();
            int count = GetSampleIndices(source.Length, indexSpan);

            // Map sampled bytes into buffer using only the valid portion
            return MapSampleBytesFromIndices(
                source,
                indexSpan[..count],
                sampleBuffer[..count]);
        }
        finally { ArrayPool<int>.Shared.Return(rented); }
    }
}