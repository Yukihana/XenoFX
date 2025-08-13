using SharpCompress.Archives;

namespace CSX.DotNet.Common.FileCompression.Shared;

public static class SharpCompressExtensions
{
    // SharpCompress: IArchiveEntry -> DateTime? LastModifiedTime
    public static DateTime? GetLastWriteTimeUtc(
        this IArchiveEntry entry)
    {
        var t = entry.LastModifiedTime;
        if (!t.HasValue) return null;

        var v = t.Value;
        return v.Kind switch
        {
            DateTimeKind.Utc => v,
            DateTimeKind.Local => v.ToUniversalTime(),
            // Unspecified: play it safe for IfNewer comparisons -> treat as unknown
            DateTimeKind.Unspecified => null,
            _ => null
        };
    }
}