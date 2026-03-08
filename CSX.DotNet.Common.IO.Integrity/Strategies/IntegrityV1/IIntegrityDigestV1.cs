namespace CSX.DotNet.Common.IO.Integrity.Strategies.IntegrityV1;

public interface IIntegrityDigestV1
{
    byte[] SHA256 { get; }
    byte[] Blake3 { get; }
    byte[] Crumbs { get; }
}