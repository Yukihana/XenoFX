using CSX.DotNet.Common.IO.Integrity.Strategies.IntegrityV1;

namespace CSX.DotNet.Common.IO.Integrity.Files;

public static partial class FileSignatureExtensions
{
    public static byte[] Concat(this IntegrityDigestV1 info)
    => [.. info.Crumbs, .. info.SHA256, .. info.Blake3];
}