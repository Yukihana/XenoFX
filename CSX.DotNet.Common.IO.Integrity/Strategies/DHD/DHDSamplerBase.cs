using CSX.DotNet.Common.Data.Validations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.DHD;

/// <summary>
/// Deterministic Hierarchical Down-sampler:
/// Reduces sampling density gradually
/// while deterministically pruning older samples
/// to maintain a fixed sample buffer.
/// </summary>
public abstract partial class DHDSamplerBase
{
    private readonly byte[] _outputBuffer;

    private int _pruneIndex;
    private long _stepFactor;
    private long _stride;

    private long _readBytes;    // No of bytes read from the stream
    private long _nextOffset;

    // Lifecycle

    public DHDSamplerBase(int outputLength)
    {
        OutputLength = outputLength.EnsurePositive();
        _outputBuffer = new byte[outputLength];

        _pruneIndex = 0;
        _stepFactor = 0;
        _stride = 1; // 2 to power of _stepFactor

        _readBytes = 0;
        _nextOffset = -1;
    }

    // Public API

    public abstract string Name { get; }
    public int OutputLength { get; }

    public long Update(ReadOnlySpan<byte> buffer)
    {
        if (_readBytes < OutputLength)
        {
            // If outputBuffer isn't full yet, directly fill it from buffer
            int intReadBytes = (int)_readBytes;
            int initialRemaining = OutputLength - intReadBytes;
            int initialRead = Math.Min(initialRemaining, buffer.Length);
            buffer[..initialRead].CopyTo(_outputBuffer.AsSpan(intReadBytes));

            _readBytes += initialRead;

            // Return on incomplete filling
            if (_readBytes < _outputBuffer.Length)
                return _readBytes;

            // Assumes completion of initial phase:
            // Respan to exclude processed segment
            buffer = buffer[initialRead..];

            // Force prune index and stride to update
            _nextOffset = OutputLength - 1;            // Artificially update this for consumption
            _pruneIndex = (OutputLength + 1) / 2;      // +1: compensate for odd
            ProgressPointers();
        }

        // Actual sampling loop
        while (
            _nextOffset - _readBytes is long localOffset &&
            localOffset < buffer.Length) // Determine within block
        {
            int nextOffset = (int)localOffset;

            // Shift left: Progressively prune earliest samples (auto-alternating)
            var outputSpan = _outputBuffer.AsSpan();
            outputSpan[(_pruneIndex + 1)..].CopyTo(outputSpan[_pruneIndex..]);

            // Append current sample
            var test = buffer[nextOffset]; // Extra for debug purposes
            _outputBuffer[^1] = test;

            // Prepare for next iteration
            ProgressPointers();
        }

        // Inference: _nextOffset doest fall within this buffer.
        // => Update state and leave it for the next iteration.
        return _readBytes += buffer.Length;
    }

    public int FinalizeSampling(
        Span<byte> outputBuffer)
    {
        if (outputBuffer.Length < _outputBuffer.Length)
        {
            throw new ArgumentException(
                "Output buffer cannot be smaller than sampling count.",
                nameof(outputBuffer));
        }

        // Copy buffer's contents to outputbuffer
        _outputBuffer.CopyTo(outputBuffer);

        return (int)Math.Clamp(_readBytes, 0, _outputBuffer.Length); //
    }

    public byte[] FinalizeSampling()
        => [.. _outputBuffer];

    // One-off

    public async Task<int> ComputeAsync(
        Stream stream,
        byte[] outputBuffer,
        int bufferSize = 8192,
        CancellationToken ctoken = default)
    {
        return await ComputeAsync(
            stream: stream,
            outputBuffer: outputBuffer,
            outputLength: OutputLength,
            bufferSize: bufferSize, ctoken: ctoken);
    }

    public int ComputeSeekable(
        Stream stream,
        Span<byte> outputBuffer)
    {
        return ComputeSeekable(
            stream: stream,
            outputBuffer: outputBuffer,
            outputLength: OutputLength);
    }

    // Internals

    private void ProgressPointers() => ProgressPointers(
        OutputLength,
        ref _pruneIndex,
        ref _stepFactor,
        ref _stride,
        ref _nextOffset);

    private static void ProgressPointers(
        int outputLength,
        ref int pruneIndex,
        ref long stepFactor,
        ref long stride,
        ref long nextOffset)
    {
        pruneIndex++;

        // Odd: assign _nextOffset before upstep
        if (outputLength % 2 != 0)
            nextOffset += stride;

        // Upstep
        if (pruneIndex >= (outputLength + 1) / 2)
        {
            pruneIndex = 0;
            stride = (long)Math.Pow(2, ++stepFactor);
        }

        // Even: assign _nextOffset after upstep
        if (outputLength % 2 == 0)
            nextOffset += stride;
    }
}