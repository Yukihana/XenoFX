using Blake3;
using CSX.DotNet.Common.IO.Integrity.Strategies.ByteSampling;
using CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing;
using CSX.DotNet.Common.IO.Integrity.Strategies.CrumbsSampling;
using CSX.DotNet.Common.IO.Streams;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.IntegrityV1;

[Obsolete("IntegrityV1 is deprecated. Use FileIntegrityAnalysis or FileSignatureBuilder instead.")]
public static class FileIntegrityV1
{
    public static async Task<IntegrityDigestV1> GetInfoV1Async(
        Stream stream,
        long? length = null,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();
        stream.EnsureSeekableReadable();

        long startPosition = stream.Position;
        // Determine effective length
        long effectiveLength;

        // read until end of stream
        if (length is null)
            effectiveLength = stream.Length - startPosition;
        // explicitly hash zero bytes
        else if (length == 0)
            effectiveLength = 0;
        else
            effectiveLength = length.Value;

        // prepare multi-hasher
        var multiHasher = new MultiHasher(bufferSize: 64 * 1024)
            .WithSHA256()
            .WithBlake3()
            .WithCrumbs(effectiveLength);

        // run hashing
        var results = await multiHasher.HashStreamAsync(
            stream: stream,
            length: length,
            parallelHashing: true,
            ctoken: ctoken
            ).ConfigureAwait(false);

        return new IntegrityDigestV1(
            sha256: results.TryGetValue("SHA256", out var sha256) ? sha256 : [],
            blake3: results.TryGetValue("BLAKE3", out var blake3) ? blake3 : [],
            crumbs: results.TryGetValue("CRUMBS", out var crumbs) ? crumbs : []);
    }

    public static async Task<IntegrityDigestV1> LegacyGetInfoV1Async(
        Stream stream,
        long? length = null,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        if (length is < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");

        // Guard and prepare for hashing
        stream.EnsureSeekableReadable();

        long startPosition = stream.Position;

        // Determine effective length
        long effectiveLength;

        // read until end of stream
        if (length is null)
            effectiveLength = stream.Length - startPosition;
        // explicitly hash zero bytes
        else if (length == 0)
            effectiveLength = 0;
        else
            effectiveLength = length.Value;

        long endPosition = startPosition + effectiveLength;
        if (endPosition > stream.Length)
            throw new ArgumentOutOfRangeException(nameof(length), "Specified length exceeds stream size.");

        using SHA256 sha256 = SHA256.Create();
        Hasher blake3 = Hasher.New();

        byte[] buffer = new byte[64 * 1024]; // 64 KB
        int bytesRead;

        // Hashing loop
        while (stream.Position < endPosition)
        {
            int toRead = (int)Math.Min(buffer.Length, endPosition - stream.Position);
            bytesRead = await stream.ReadAsync(buffer.AsMemory(0, toRead), ctoken).ConfigureAwait(false);

            if (bytesRead == 0)
                break;

            sha256.TransformBlock(buffer, 0, bytesRead, null, 0);
            blake3.Update(buffer.AsSpan(0, bytesRead));

            ctoken.ThrowIfCancellationRequested();
        }

        // final block needs to be handled for sha256
        sha256.TransformFinalBlock([], 0, 0);
        var sha256hash = sha256.Hash ?? [];

        // add crumbs sampling
        byte[] sampleBuffer = new byte[32];
        stream.Position = startPosition;

        _ = ByteSampler.CopySamples(
            stream: stream,
            relativeOffsets: CrumbsCartography.CreateOffsets(effectiveLength),
            outputBuffer: sampleBuffer);

        // restore position before finishing
        stream.Position = startPosition;

        return new(
            sha256: sha256hash,
            blake3: blake3.Finalize().AsSpan().ToArray(),
            crumbs: sampleBuffer
        );
    }

    public static async Task<IntegrityDigestV1> GetFullInfoV1Async(
        Stream stream,
        CancellationToken ctoken = default)
    {
        stream.EnsureSeekableReadable();
        stream.Position = 0;
        return await LegacyGetInfoV1Async(
            stream: stream,
            length: stream.Length,
            ctoken: ctoken);
    }
}