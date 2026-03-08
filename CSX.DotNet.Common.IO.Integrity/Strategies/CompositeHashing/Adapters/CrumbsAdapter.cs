using CSX.DotNet.Common.IO.Integrity.Strategies.CrumbsSampling;
using System;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing.Adapters;

public class CrumbsAdapter : IIncrementalHasher
{
    private readonly long _length;
    private readonly CrumbsSampler _sampler;

    public CrumbsAdapter(
        long length)
    {
        _length = length;
        _sampler = new CrumbsSampler(_length);
    }

    public string Name
        => CrumbsSampler.Name.ToString();

    public void Update(ReadOnlySpan<byte> buffer)
        => _sampler.Update(buffer);

    public byte[] FinalizeHash()
        => _sampler.FinalizeSampling();

    public IIncrementalHasher Clone()
        => new CrumbsAdapter(_length);
}