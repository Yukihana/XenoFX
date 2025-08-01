namespace CSX.DotNet.Common.Security.Integrity;

public class IntegrityDigestV1 : IIntegrityDigestV1
{
    // Data

    public byte[] SHA256 { get; }
    public byte[] Blake3 { get; }
    public byte[] Crumbs { get; }

    // Lifecycle
    public IntegrityDigestV1(
        byte[] sha256,
        byte[] blake3,
        byte[] crumbs)
    {
        SHA256 = sha256;
        Blake3 = blake3;
        Crumbs = crumbs;
    }
}