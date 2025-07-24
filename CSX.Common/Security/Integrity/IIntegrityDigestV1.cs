namespace CSX.Common.Security.Integrity;

public interface IIntegrityDigestV1
{
    byte[] SHA256 { get; }
    byte[] Blake3 { get; }
    byte[] Crumbs { get; }
}