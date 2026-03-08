using CSX.DotNet.Common.Data.Collections;
using CSX.DotNet.Common.Data.Validations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing;

public class MultiHasher
{
    private readonly int _bufferSize;
    private readonly int _channelCapacity;

    public List<IIncrementalHasher> Hashers { get; }

    // Lifecycle

    public MultiHasher(
        int bufferSize = 64 * 1024,
        int channelCapacity = 4,
        params IEnumerable<IIncrementalHasher> hashers)
    {
        Hashers = [.. hashers];
        _bufferSize = bufferSize;
        _channelCapacity = channelCapacity;
    }

    // Internals: Producer and consumer tasks

    private static async Task ReadToChannelAsync(
        Stream stream,
        ChannelWriter<PooledArray<byte>> writer,
        long? length,
        int bufferSize,
        CancellationToken ctoken)
    {
        byte[] buffer = new byte[bufferSize];

        try
        {
            // Read until stream ends
            if (length is null)
            {
                while (
                    await stream.ReadAsync(buffer, ctoken).ConfigureAwait(false) is var bytesRead
                    && bytesRead > 0)
                {
                    ctoken.ThrowIfCancellationRequested();

                    await writer.WriteAsync(
                        item: new PooledArray<byte>(buffer.AsSpan(0, bytesRead)), // chunk
                        cancellationToken: ctoken
                    ).ConfigureAwait(false);
                }
            }
            // Respect explicit length
            else
            {
                long remaining = length.Value.EnsureNotNegative(
                    errorMessage: "Length cannot be negative. Did you forget to use 'Math.Abs()'?");

                while (remaining > 0)
                {
                    ctoken.ThrowIfCancellationRequested();

                    int toRead = (int)Math.Min(buffer.Length, remaining);
                    int bytesRead = await stream
                        .ReadAsync(buffer.AsMemory(0, toRead), ctoken)
                        .ConfigureAwait(false);

                    if (bytesRead == 0)
                        break;

                    await writer.WriteAsync(
                        item: new PooledArray<byte>(buffer.AsSpan(0, bytesRead)), // chunk
                        cancellationToken: ctoken
                    ).ConfigureAwait(false);

                    remaining -= bytesRead;
                }
            }
        }
        finally { writer.Complete(); }
    }

    private static async Task HashFromChannelAsync(
        ChannelReader<PooledArray<byte>> reader,
        IEnumerable<IIncrementalHasher> hashers,
        bool parallelHashing,
        CancellationToken ctoken)
    {
        await foreach (var chunk in reader.ReadAllAsync(ctoken).ConfigureAwait(false))
        {
            try
            {
                if (parallelHashing)
                {
                    // update all hashes in parallel
                    Parallel.ForEach(
                        hashers,
                        hasher => hasher.Update(chunk.AsSpan()));
                }
                else
                {
                    // update sequentially
                    foreach (var hasher in hashers)
                        hasher.Update(chunk.AsSpan());
                }
            }
            finally { chunk.Dispose(); } // Return buffer to pool
        }
    }

    // Public API

    public async Task<Dictionary<string, byte[]>> HashStreamAsync(
        Stream stream,
        long? length = null,
        bool parallelHashing = true,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        if (!stream.CanRead)
            throw new ArgumentException("Stream must be readable.", nameof(stream));

        // Snapshot parameters for thread safety
        var hashers = Hashers.Select(h => h.Clone()).ToArray();
        int bufferSize = _bufferSize;
        int channelCapacity = _channelCapacity;

        // Create a bounded channel to read and hash concurrently
        var channel = Channel.CreateBounded<PooledArray<byte>>(
            options: new BoundedChannelOptions(channelCapacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = true
            });

        var readTask = ReadToChannelAsync(
            stream: stream,
            writer: channel.Writer,
            length: length,
            bufferSize: bufferSize,
            ctoken: ctoken);

        var hashTask = HashFromChannelAsync(
            reader: channel.Reader,
            hashers: hashers,
            parallelHashing: parallelHashing,
            ctoken: ctoken);

        // Run both tasks concurrently
        await Task
            .WhenAll(readTask, hashTask)
            .ConfigureAwait(false);

        return hashers.ToDictionary(
            h => h.Name,
            h => h.FinalizeHash());
    }

    public async Task<Dictionary<string, byte[]>> HashFileAsync(
        string filePath,
        bool parallelHashing = false,
        CancellationToken ctoken = default)
    {
        await using FileStream stream = new(
            path: filePath,
            mode: FileMode.Open,
            access: FileAccess.Read,
            share: FileShare.Read,
            bufferSize: 81920,
            useAsync: true);
        return await HashStreamAsync(
            stream: stream,
            length: stream.Length,
            parallelHashing: parallelHashing,
            ctoken: ctoken);
    }
}