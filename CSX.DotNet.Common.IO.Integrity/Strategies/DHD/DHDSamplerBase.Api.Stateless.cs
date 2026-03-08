using CSX.DotNet.Common.IO.Integrity.Strategies.ByteSampling;
using CSX.DotNet.Common.IO.Streams;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.DHD;

public abstract partial class DHDSamplerBase
{
    private class DHDSamplerImpl(int outputLength)
        : DHDSamplerBase(outputLength)
    {
        public override string Name => "";
    }

    // Public API: One Off

    public static async Task<int> ComputeAsync(
        Stream stream,
        byte[] outputBuffer,
        int outputLength,
        int bufferSize = 8192,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        if (stream.CanSeek)
        {
            return ComputeSeekable(
                stream: stream,
                outputBuffer: outputBuffer,
                outputLength: outputLength);
        }

        stream.EnsureReadable();

        // Create instance of local implementation
        DHDSamplerImpl sampler = new(outputLength);
        if (outputBuffer.Length < sampler.OutputLength)
            throw new ArgumentException("Output buffer is too small.");

        // Process incrementally
        byte[] buffer = new byte[bufferSize];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, ctoken).ConfigureAwait(false)) > 0)
        {
            sampler.Update(buffer.AsSpan(0, bytesRead));
            ctoken.ThrowIfCancellationRequested();
        }

        return sampler.FinalizeSampling(outputBuffer.AsSpan());
    }

    public static int ComputeSeekable(
        Stream stream,
        Span<byte> outputBuffer,
        int outputLength)
    {
        if (outputBuffer.Length < outputLength)
        {
            throw new ArgumentException(
                "Output buffer is too small.",
                nameof(outputBuffer));
        }

        stream.EnsureSeekableReadable();

        // Initial fill
        long[] offsets = new long[outputLength];
        for (int i = 0; i < outputLength; i++)
            offsets[i] = i;

        int sampleCount = (int)Math.Min(outputLength, stream.Length);
        long stepFactor = 0;
        long stride = 1;
        long nextOffset = outputLength - 1;           // Artificially update this for consumption
        int pruneIndex = (outputLength + 1) / 2;      // +1: compensate for odd

        // Simulate state update after initial fill
        ProgressPointers(
            outputLength,
            ref pruneIndex,
            ref stepFactor,
            ref stride,
            ref nextOffset);

        // 3. Simulate mapping for remaining entries
        while (nextOffset < stream.Length) // Seekable; Use full view
        {
            // Shift left: Progressively prune earliest samples (auto-alternating)
            var offsetSpan = offsets.AsSpan();
            offsetSpan[(pruneIndex + 1)..].CopyTo(offsetSpan[pruneIndex..]);

            // Append current sample
            offsets[^1] = nextOffset;

            // Advance pointers
            ProgressPointers(
                outputLength,
                ref pruneIndex,
                ref stepFactor,
                ref stride,
                ref nextOffset);
        }

        // finally copy the bytes
        return ByteSampler.CopySamples(
            stream: stream,
            relativeOffsets: offsets.AsSpan()[..sampleCount],
            outputBuffer: outputBuffer);
    }
}