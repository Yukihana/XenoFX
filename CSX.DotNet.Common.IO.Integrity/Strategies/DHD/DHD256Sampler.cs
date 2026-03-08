namespace CSX.DotNet.Common.IO.Integrity.Strategies.DHD;

/// <summary>
/// Deterministic Hierarchical Down-sampler:
/// Reduces sampling density gradually
/// while deterministically pruning older samples
/// to maintain a fixed sample buffer.
/// </summary>
public sealed class DHD256Sampler : DHDSamplerBase
{
    public override string Name
        => "DHD256";

    public DHD256Sampler() : base(32)
    {
    }
}