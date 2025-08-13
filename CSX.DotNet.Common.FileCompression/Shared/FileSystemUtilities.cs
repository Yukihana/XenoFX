using CSX.DotNet.Common.Data.Guids;

namespace CSX.DotNet.Common.FileCompression.Shared;

public static partial class FileSystemUtilities
{
    public static string DetermineDecompressDirectory(
        string archivePath,
        OverwriteMode overwriteMode = OverwriteMode.Abort)
    {
        if (string.IsNullOrWhiteSpace(archivePath))
            throw new ArgumentException("Archive path cannot be null or empty.", nameof(archivePath));

        archivePath = Path.GetFullPath(archivePath);

        string parentDir
            = Path.GetDirectoryName(archivePath)
            ?? Directory.GetCurrentDirectory();

        // Strip the archive name to get a clean target directory name
        string fileName = Path.GetFileName(archivePath);
        string targetDirName = StripKnownExtensions(fileName);

        // Use a generic prefix if still empty
        // edge case: file named ".tar.gz" without prefix
        if (string.IsNullOrWhiteSpace(targetDirName))
            targetDirName = "extracted";

        // Ideal decompress path
        string decompressPath = Path.Combine(
            parentDir,
            targetDirName);

        // Handle collisions based on overwrite mode
        bool dirExists = Directory.Exists(decompressPath) || File.Exists(decompressPath);

        switch (overwriteMode)
        {
            case OverwriteMode.Abort:
                if (dirExists)
                    throw new IOException($"Decompression directory already exists: {decompressPath}");
                break;

            case OverwriteMode.Always:
            case OverwriteMode.IfNewer:
            case OverwriteMode.SkipExisting:
                // Just use the existing directory if present
                break;

            case OverwriteMode.RenameIfExists:
                do
                {
                    decompressPath = Path.Combine(
                        parentDir,
                        $"{targetDirName}_{DMC212710Guid.FromUtcNow():N}");
                }
                while (File.Exists(decompressPath) || Directory.Exists(decompressPath));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(overwriteMode), overwriteMode, null);
        }

        return decompressPath;
    }

    private static readonly string[] ArchiveExtensions
        = [".zip", ".tar", ".gz", ".bz2", ".xz", ".zst", ".7z", ".rar", ".tgz"];

    private static string StripKnownExtensions(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        string result = fileName;
        bool stripped = true;

        while (stripped)
        {
            stripped = false;
            foreach (var ext in ArchiveExtensions)
            {
                if (result.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                {
                    result = result[..^ext.Length];
                    stripped = true;
                    break; // Restart checking from the beginning
                }
            }
        }

        // Fallback to default extension strip if nothing was removed
        if (result == fileName)
            result = Path.GetFileNameWithoutExtension(fileName);

        return result;
    }

    public static bool TryGetExtractionPath(
        string originalExtractionPath,
        OverwriteMode overwriteMode,
        DateTime? sourceLastWriteTime,
        out string extractionPath)
    {
        if (string.IsNullOrWhiteSpace(originalExtractionPath))
            throw new ArgumentException("Extraction path cannot be null or empty.", nameof(originalExtractionPath));

        extractionPath = originalExtractionPath;

        // Early resolve: If file doesn't exist or Always mode, just overwrite
        if (!File.Exists(originalExtractionPath) || overwriteMode == OverwriteMode.Always)
            return true;

        // Route by mode
        switch (overwriteMode)
        {
            case OverwriteMode.Abort:
                throw new IOException($"File already exists: {originalExtractionPath}");

            case OverwriteMode.SkipExisting:
                return false; // skip

            case OverwriteMode.IfNewer:
                var existingLastWrite = File.GetLastWriteTimeUtc(originalExtractionPath);
                // Overwrite only if timestamp exists and is newer
                return sourceLastWriteTime.HasValue && sourceLastWriteTime > existingLastWrite;

            case OverwriteMode.RenameIfExists:
                string directory = Path.GetDirectoryName(originalExtractionPath)!;
                string nameWithoutExt = Path.GetFileNameWithoutExtension(originalExtractionPath);
                string ext = Path.GetExtension(originalExtractionPath);

                do
                {
                    extractionPath = Path.Combine(
                        directory,
                        $"{nameWithoutExt}_{DMC212710Guid.FromUtcNow():N}{ext}");
                }
                while (File.Exists(extractionPath));

                return true;

            default:
                throw new ArgumentOutOfRangeException(nameof(overwriteMode), overwriteMode, null);
        }
    }
}