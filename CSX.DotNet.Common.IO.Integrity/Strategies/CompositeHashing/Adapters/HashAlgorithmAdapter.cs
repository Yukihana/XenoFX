using System;
using System.Buffers;
using System.Security.Cryptography;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing.Adapters;

[Obsolete("Requires workaround for Span based update. Use IncrementalHasherAdapter instead.")]
public class HashAlgorithmAdapter : IIncrementalHasher
{
    // Infrastructure

    private readonly Func<HashAlgorithm> _factory;
    private readonly HashAlgorithm _hashAlgorithm;
    private readonly HashAlgorithmName _name;

    // Lifecycle

    public HashAlgorithmAdapter(
        Func<HashAlgorithm> factory,
        HashAlgorithmName name)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _hashAlgorithm = _factory();
        _name = name;
    }

    // Interface : IIncrementalHasher

    public string Name
        => _name.Name ?? string.Empty;

    public void Update(ReadOnlySpan<byte> buffer)
    {
        // TransformBlock requires byte[]. Rent a temporary array from the pool.
        byte[]? tmp = null;
        try
        {
            tmp = ArrayPool<byte>.Shared.Rent(buffer.Length);
            buffer.CopyTo(tmp);
            _hashAlgorithm.TransformBlock(
                inputBuffer: tmp,
                inputOffset: 0,
                inputCount: tmp.Length,
                outputBuffer: null,
                outputOffset: 0);
        }
        finally
        {
            if (tmp != null)
                ArrayPool<byte>.Shared.Return(tmp);
        }
    }

    public byte[] FinalizeHash()
    {
        _hashAlgorithm.TransformFinalBlock(
            inputBuffer: [],
            inputOffset: 0,
            inputCount: 0);

        return _hashAlgorithm.Hash ?? [];
    }

    public IIncrementalHasher Clone()
        => new HashAlgorithmAdapter(_factory, _name);
}