using System;
using System.IO.Compression;

namespace CSX.DotNet.Common.FileCompression.Shared;

public static class ZipExtensions
{
    // System.IO.Compression: ZipArchiveEntry.LastWriteTime -> DateTimeOffset
    public static DateTime? GetLastWriteTimeUtc(
        this ZipArchiveEntry entry)
    {
        var dto = entry.LastWriteTime;
        if (dto == DateTimeOffset.MinValue) return null; // no timestamp present
        return dto.UtcDateTime;
    }
}