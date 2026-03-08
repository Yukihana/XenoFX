using CSX.DotNet.Common.Data.Validations;
using CSX.DotNet.Common.IO.Streams;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.ByteSampling;

public partial class ByteSampler
{
    private readonly long[] _relativeOffsets;

    private readonly int[] _orderedIndices;
    private int _nextOrderedIndicesPointer;
    private long _readBytes;

    private readonly byte[] _crumbs;
    private readonly BitArray _captured;

    // Lifecycle

    public ByteSampler(ReadOnlySpan<long> relativeOffsets)
    {
        _ = relativeOffsets.Length.EnsurePositive(
            argumentName: nameof(relativeOffsets),
            errorMessage: "At least one offset must be provided.");

        _relativeOffsets = [.. relativeOffsets];
        _crumbs = new byte[relativeOffsets.Length];
        _captured = new(relativeOffsets.Length);

        // Build the index stack in descending order for ordered pull
        _orderedIndices = [.. Enumerable
            .Range(0, _relativeOffsets.Length)
            .OrderBy(i => _relativeOffsets[i])];
        _nextOrderedIndicesPointer = 0;
    }

    public void Update(ReadOnlySpan<byte> block)
    {
        long startIndex = _readBytes;
        long endIndex = _readBytes + block.Length;

        while (_nextOrderedIndicesPointer < _orderedIndices.Length)
        {
            int idx = _orderedIndices[_nextOrderedIndicesPointer];
            long offset = _relativeOffsets[idx];

            if (offset >= endIndex)
                break; // leave for future blocks

            int localIndex = (int)(offset - startIndex);
            _crumbs[idx] = block[localIndex];
            _captured[idx] = true;

            _nextOrderedIndicesPointer++;
        }

        _readBytes = endIndex;
    }

    public int FinalizeSampling(
        Span<byte> outputBuffer)
    {
        EnsureBufferSize(_relativeOffsets, outputBuffer);

        for (int i = 0; i < _relativeOffsets.Length; i++)
        {
            if (!_captured[i])
            {
                // Missing data: upstream source didn't provide enough bytes
                throw new InvalidOperationException(
                    $"Cannot finalize sampling: no byte was captured at offset {_relativeOffsets[i]}. " +
                    "Ensure that all relevant blocks have been processed via Update().");
            }

            outputBuffer[i] = _crumbs[i];
        }

        return _relativeOffsets.Length;
    }

    public byte[] FinalizeSampling()
    {
        byte[] outputBuffer = new byte[_relativeOffsets.Length];
        _ = FinalizeSampling(outputBuffer);
        return outputBuffer;
    }

    public static async Task<int> ComputeAsync(
        Stream stream,
        long[] relativeOffsets,
        byte[] outputBuffer,
        int bufferSize = 8192,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        if (stream.CanSeek)
        {
            return CopySamples(
                stream: stream,
                relativeOffsets: relativeOffsets,
                outputBuffer: outputBuffer);
        }

        stream.EnsureReadable();
        EnsureBufferSize(relativeOffsets, outputBuffer);

        ByteSampler sampler = new(relativeOffsets);
        byte[] buffer = new byte[bufferSize];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, ctoken).ConfigureAwait(false)) > 0)
        {
            sampler.Update(buffer.AsSpan(0, bytesRead));
            ctoken.ThrowIfCancellationRequested();
        }

        return sampler.FinalizeSampling(outputBuffer.AsSpan());
    }

    public static int CopySamples(
        Stream stream,
        ReadOnlySpan<long> relativeOffsets,
        Span<byte> outputBuffer)
    {
        stream.EnsureSeekableReadable();
        EnsureBufferSize(relativeOffsets, outputBuffer);

        long baseOffset = stream.Position;
        for (int i = 0; i < relativeOffsets.Length; i++)
        {
            long offset = baseOffset + relativeOffsets[i];

            EnsureOffsetInRange(offset, stream.Length);
            if (offset < 0 || offset >= stream.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(relativeOffsets),
                    $"Offset {relativeOffsets[i]} is out of range for the stream.");
            }

            stream.Position = offset; // Output requires reading from explicit positions
            int b = stream.ReadByte();
            if (b == -1)
                throw new EndOfStreamException($"Unexpected EOF at position {offset}.");

            outputBuffer[i] = (byte)b;
        }

        return relativeOffsets.Length;
    }

    public static int ComputeSpan(
        ReadOnlySpan<byte> source,
        ReadOnlySpan<long> relativeOffsets,
        Span<byte> outputBuffer)
    {
        EnsureBufferSize(relativeOffsets, outputBuffer);

        for (int i = 0; i < relativeOffsets.Length; i++)
        {
            long offset = relativeOffsets[i];
            EnsureOffsetInRange(offset, source.Length);
            outputBuffer[i] = source[(int)offset];
        }

        return relativeOffsets.Length;
    }

    // Internals

    private static void EnsureBufferSize(
        ReadOnlySpan<long> relativeOffsets,
        Span<byte> outputBuffer)
    {
        if (outputBuffer.Length < relativeOffsets.Length)
        {
            throw new ArgumentException(
                "Output buffer too small.",
                nameof(outputBuffer));
        }
    }

    private static void EnsureOffsetInRange(
        long offset,
        long length)
    {
        if (offset < 0 || offset >= length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset),
                $"Offset {offset} is out of range for length {length}.");
        }
    }
}