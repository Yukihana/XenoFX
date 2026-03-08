using CSX.DotNet.Common.IO.Integrity.Strategies.ByteSampling;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.CrumbsSampling;

public sealed class CrumbsSampler : ByteSampler
{
    public CrumbsSampler(long length)
        : base(CrumbsCartography.CreateOffsets(length))
    {
    }

    // Public API

    public const string Name = "CRUMBS";

    public static async Task<int> FromFile(
        string filePath,
        byte[] outputBuffer,
        CancellationToken ctoken = default)
    {
        if (outputBuffer.Length < CrumbsCartography.OutputSize)
        {
            throw new ArgumentException(
                "Output buffer is too small",
                nameof(outputBuffer));
        }

        await using FileStream stream = new(
            path: filePath,
            mode: FileMode.Open,
            access: FileAccess.Read,
            share: FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        var offsets = CrumbsCartography.CreateOffsets(
            sourceLength: stream.Length);

        return await ComputeAsync(
            stream: stream,
            relativeOffsets: offsets,
            outputBuffer: outputBuffer,
            bufferSize: 81920,
            ctoken: ctoken);
    }

    public static async Task<byte[]> FromFile(
        string filePath,
        CancellationToken ctoken = default)
    {
        byte[] outputBuffer = new byte[CrumbsCartography.OutputSize];
        await FromFile(filePath, outputBuffer, ctoken);
        return outputBuffer;
    }
}