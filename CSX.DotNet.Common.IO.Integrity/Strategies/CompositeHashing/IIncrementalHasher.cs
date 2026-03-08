using System;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing;

public interface IIncrementalHasher
{
    string Name { get; }

    void Update(ReadOnlySpan<byte> buffer);

    byte[] FinalizeHash();

    IIncrementalHasher Clone();
}