using CSX.DotNet.Common.IO.Integrity.Strategies.DHD;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSX.DotNet.Common.IO.Integrity.Tests;

public class DHD256Tests
{
    private class DHDImpl : DHDSamplerBase
    {
        public DHDImpl(int size) : base(size)
        {
        }

        public override string Name
            => nameof(DHDImpl);
    }

    private readonly ITestOutputHelper _output;

    public DHD256Tests(ITestOutputHelper output) => _output = output;

    [Theory]
    [InlineData(8, 50)]   // small buffer, moderate iterations
    [InlineData(16, 100)] // bigger buffer, more iterations
    [InlineData(32, 40)]  // DHD256, shorter run
    [InlineData(8, 5)]    // edge case: very short run

    // --- Odd buffer length cases ---
    [InlineData(7, 50)]    // small odd buffer, longer run
    [InlineData(1, 10)]    // trivial case, should just hold latest
    [InlineData(3, 10)]    // tiny odd buffer, stress edge case
    [InlineData(5, 20)]    // very small odd buffer
    [InlineData(9, 100)]   // odd buffer, more iterations
    [InlineData(15, 60)]   // half of 30, checks midpoint resets
    [InlineData(31, 40)]   // just under 32, borderline with power-of-two
    public void DumpIndices(int bufferLength, int iterations)
    {
        _output.WriteLine($"Case: buffer size = {bufferLength}, iteration = {iterations}");

        var enumerator = GetTestStream().GetEnumerator();
        var sampler = new DHDImpl(bufferLength);

        // Act: feed sampler in blocks of 16 bytes
        var temp = new byte[16];
        for (int i = 0; i < iterations; i++)
        {
            if (!FillBlock(enumerator, temp))
                break;

            sampler.Update(temp);

            var snap = new byte[bufferLength];
            sampler.FinalizeSampling(snap);

            // log snapshot
            _output.WriteLine($"[{string.Join(",", temp)}] => [{string.Join(",", snap)}]");
        }
    }

    [Theory]
    [InlineData(8, 50)]   // small buffer, moderate iterations
    [InlineData(16, 100)] // bigger buffer, more iterations
    [InlineData(32, 40)]  // DHD256, shorter run
    [InlineData(8, 5)]    // edge case: very short run

    // --- Odd buffer length cases ---
    [InlineData(7, 50)]    // small odd buffer, longer run
    [InlineData(1, 10)]    // trivial case, should just hold latest
    [InlineData(3, 10)]    // tiny odd buffer, stress edge case
    [InlineData(5, 20)]    // very small odd buffer
    [InlineData(9, 100)]   // odd buffer, more iterations
    [InlineData(15, 60)]   // half of 30, checks midpoint resets
    [InlineData(31, 40)]   // just under 32, borderline with power-of-two
    public void CompareIncrementalAndCompute(int bufferLength, int iterations)
    {
        _output.WriteLine($"Case: buffer size = {bufferLength}, iterations = {iterations}");

        const int blockSize = 16;
        var sampleData = PrepareData(blockSize * iterations);

        // --- Incremental sampler ---
        var sampler = new DHDImpl(bufferLength);

        for (int i = 0; i < iterations; i++)
            sampler.Update(sampleData.AsSpan(i * blockSize, blockSize));

        var snapIncremental = new byte[bufferLength];
        sampler.FinalizeSampling(snapIncremental);

        // --- Compute sampler ---
        using var stream = new MemoryStream(sampleData);
        var snapCompute = new byte[bufferLength];
        DHDSamplerBase.ComputeSeekable(stream, snapCompute, bufferLength);

        // --- Log results ---
        _output.WriteLine($"Incremental: [{string.Join(",", snapIncremental)}]");
        _output.WriteLine($"Compute    : [{string.Join(",", snapCompute)}]");

        // --- Verify equality ---
        Assert.Equal(snapIncremental, snapCompute);
    }

    private static byte[] PrepareData(int length)
    {
        var result = new byte[length];
        for (int i = 0; i < length; i++)
            result[i] = (byte)(i % 256);
        return result;
    }

    private static IEnumerable<byte> GetTestStream()

    {
        while (true)
        {
            for (int i = 0; i < 256; i++)
                yield return (byte)i;
        }
    }

    private static bool FillBlock(IEnumerator<byte> enumerator, byte[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (!enumerator.MoveNext())
                return false;

            buffer[i] = enumerator.Current;
        }
        return true;
    }
}