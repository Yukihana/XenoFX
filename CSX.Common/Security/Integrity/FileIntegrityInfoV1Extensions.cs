namespace CSX.Common.Security.Integrity;

public static class FileIntegrityInfoV1Extensions
{
    public static byte[] Concat(this IntegrityDigestV1 info)
        => [.. info.SHA256, .. info.Blake3, .. info.Crumbs];
}