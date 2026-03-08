using CSX.DotNet.Common.IO.Integrity.Strategies.DHD;
using System;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing.Adapters;

public sealed class DHD256Adapter : IIncrementalHasher
{
    private readonly DHD256Sampler _sampler;

    public DHD256Adapter()
        => _sampler = new DHD256Sampler();

    public string Name
        => _sampler.Name;

    public void Update(ReadOnlySpan<byte> buffer)
        => _sampler.Update(buffer);

    public byte[] FinalizeHash()
        => _sampler.FinalizeSampling();

    public IIncrementalHasher Clone()
        => new DHD256Adapter();
}