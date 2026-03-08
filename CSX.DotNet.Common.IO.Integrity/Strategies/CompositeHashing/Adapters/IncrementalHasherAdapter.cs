using System;
using System.Security.Cryptography;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing.Adapters;

public class IncrementalHasherAdapter : IIncrementalHasher
{
    private readonly IncrementalHash _hash;
    private readonly HashAlgorithmName _algorithm;

    public string Name { get; }

    public IncrementalHasherAdapter(
        HashAlgorithmName algorithm)
    {
        if (string.IsNullOrWhiteSpace(algorithm.Name))
        {
            throw new ArgumentException(
                "Invalid or unsupported hash algorithm.",
                nameof(algorithm));
        }

        Name = algorithm.Name;
        _algorithm = algorithm;
        _hash = IncrementalHash.CreateHash(algorithm);
    }

    public void Update(ReadOnlySpan<byte> buffer)
        => _hash.AppendData(buffer);

    public byte[] FinalizeHash()
        => _hash.GetHashAndReset();

    public IIncrementalHasher Clone()
        => new IncrementalHasherAdapter(_algorithm);
}