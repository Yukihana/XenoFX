using Blake3;
using CSX.Common.Cryptography;
using CSX.Common.IO.Streams;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.Common.Security.Integrity;

public static class FileIntegrity
{
    public static async Task<IntegrityDigestV1> GetInfoV1Async(
        Stream stream,
        long length = 0,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Guard and prepare for hashing
        stream.EnsureSeekableReadable();

        long startPosition = stream.Position;
        long endPosition = startPosition + length;
        if (endPosition > stream.Length)
            throw new ArgumentOutOfRangeException(nameof(length), "Specified length exceeds stream size.");

        using SHA256 sha256 = SHA256.Create();
        Hasher blake3 = Hasher.New();

        byte[] buffer = new byte[64 * 1024]; // 64 KB
        int bytesRead;

        // primary hash logic
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
        _ = FileCrumbs.SampleBytes(
            stream: stream,
            length: length,
            sampleBuffer: sampleBuffer);

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
        return await GetInfoV1Async(
            stream: stream,
            length: stream.Length,
            ctoken: ctoken);
    }
}