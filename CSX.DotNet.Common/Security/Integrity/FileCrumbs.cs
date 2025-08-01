using System;
using System.IO;

namespace CSX.Common.Cryptography;

public static partial class FileCrumbs
{
    // Spread indices

    public static int GetSampleIndices(
        long sourceLength,
        Span<long> indexBuffer)
    {
        if (sourceLength <= 0 || indexBuffer.IsEmpty)
            return 0;

        int count = (int)Math.Min(sourceLength, indexBuffer.Length);

        // If the source is smaller than the number of samples, clamp to length
        if (sourceLength <= count)
        {
            for (int i = 0; i < count; i++)
                indexBuffer[i] = i;
            return count;
        }

        // Map the indices evenly
        double step = (double)(sourceLength - 1) / (count - 1); // evenly spaced from 0 to length - 1
        for (int i = 0; i < count; i++)
            indexBuffer[i] = (long)Math.Round(i * step);

        return count;
    }

    public static int GetSampleIndices(
        int sourceLength,
        Span<int> indexBuffer)
    {
        if (sourceLength <= 0 || indexBuffer.IsEmpty)
            return 0;

        int count = Math.Min(sourceLength, indexBuffer.Length);

        // If the source is smaller than the number of samples, clamp to length
        if (sourceLength <= count)
        {
            for (int i = 0; i < sourceLength; i++)
                indexBuffer[i] = i;
            return sourceLength;
        }

        // Map the indices evenly
        double step = (double)(sourceLength - 1) / (count - 1); // evenly spaced from 0 to length - 1
        for (int i = 0; i < count; i++)
            indexBuffer[i] = (int)Math.Round(i * step);

        return count;
    }

    // Map to indices

    private static int MapSampleBytesFromIndices(
        ReadOnlySpan<byte> source,
        ReadOnlySpan<int> indices,
        Span<byte> sampleBuffer)
    {
        // Validate indices conforms to buffer length
        if (indices.Length > sampleBuffer.Length)
            throw new ArgumentException("Sample buffer is too small for the number of indices.");

        // Validate max index
        if (indices[^1] >= source.Length)
            throw new ArgumentOutOfRangeException(nameof(indices), "One or more indices are out of range for the source buffer.");

        // Copy the bytes as samples
        for (int i = 0; i < indices.Length; i++)
            sampleBuffer[i] = source[indices[i]];

        return indices.Length;
    }

    private static int MapSampleBytesFromIndices(
        Stream stream,
        long baseOffset,
        ReadOnlySpan<long> indices,
        Span<byte> sampleBuffer)
    {
        // Validate indices conforms to buffer length
        if (indices.Length > sampleBuffer.Length)
            throw new ArgumentException("Sample buffer is too small for the number of indices.");

        for (int i = 0; i < indices.Length; i++)
        {
            long targetPosition = baseOffset + indices[i];

            if (targetPosition >= stream.Length) // Optional — fail-fast
                throw new ArgumentOutOfRangeException(nameof(indices), "One or more indices are out of stream bounds.");

            stream.Position = targetPosition;
            int b = stream.ReadByte();
            if (b == -1)
                throw new EndOfStreamException($"Unexpected EOF at position {targetPosition}.");

            sampleBuffer[i] = (byte)b;
        }

        return indices.Length;
    }
}