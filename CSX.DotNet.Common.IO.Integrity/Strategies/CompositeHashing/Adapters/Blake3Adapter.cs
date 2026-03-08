using Blake3;
using System;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing.Adapters;

public class Blake3Adapter : IIncrementalHasher
{
    // Infrastructure

    private readonly Hasher _blake3 = Hasher.New();

    // Interface : IIncrementalHasher

    public string Name => "BLAKE3";

    public void Update(ReadOnlySpan<byte> buffer)
        => _blake3.Update(buffer);

    public byte[] FinalizeHash()
    {
        return _blake3.Finalize()
            .AsSpan().ToArray();
    }

    public IIncrementalHasher Clone()
        => new Blake3Adapter();
}