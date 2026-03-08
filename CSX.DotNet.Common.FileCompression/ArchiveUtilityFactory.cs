using CSX.DotNet.Common.FileCompression.Abstractions;
using CSX.DotNet.Common.FileCompression.Formats.Rar;
using CSX.DotNet.Common.FileCompression.Formats.SevenZip;
using CSX.DotNet.Common.FileCompression.Formats.Tar;
using CSX.DotNet.Common.FileCompression.Formats.Zip;
using System;
using System.IO;

namespace CSX.DotNet.Common.FileCompression;

public static class ArchiveUtilityFactory
{
    public static ArchiveUtilityBase Create(string archiveFilePath)
    {
        if (string.IsNullOrWhiteSpace(archiveFilePath))
            throw new ArgumentException("Archive file path cannot be null or empty.", nameof(archiveFilePath));

        string ext = Path.GetExtension(archiveFilePath).ToLowerInvariant();

        return ext switch
        {
            ".zip" => new ZipUtility(),
            ".tar" or ".tgz" => new TarUtility(),
            ".7z" => new SevenZipUtility(),
            ".rar" => new RarUtility(),
            _ => CreateFromCompoundExtension(archiveFilePath)
        };
    }

    private static TarUtility CreateFromCompoundExtension(string path)
    {
        if (path.EndsWith(".tar.xz", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
        {
            return new TarUtility();
        }

        throw new NotSupportedException($"Unsupported archive type: {path}");
    }
}