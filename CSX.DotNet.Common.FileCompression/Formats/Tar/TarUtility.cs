using CSX.DotNet.Common.FileCompression.Abstractions;
using CSX.DotNet.Common.FileCompression.Shared;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace CSX.DotNet.Common.FileCompression.Formats.Tar;

public sealed class TarUtility : ArchiveUtilityBase
{
    protected override async Task ExtractInternalAsync(
        string archivePath,
        string decompressionPath,
        ExtractionOverwriteMode overwriteMode = ExtractionOverwriteMode.Abort,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        await Task.Run(() =>
        {
            ExtractInternal(
                archivePath: archivePath,
                decompressionPath: decompressionPath,
                overwriteMode: overwriteMode);
        }, ctoken).ConfigureAwait(false);
    }

    private static void ExtractInternal(
        string archivePath,
        string decompressionPath,
        ExtractionOverwriteMode overwriteMode)
    {
        using var archive = ArchiveFactory.Open(archivePath);

        foreach (var entry in archive.Entries.Where(e => !e.IsDirectory && !string.IsNullOrWhiteSpace(e.Key)))
        {
            string combinedPath = Path.Combine(
                decompressionPath,
                entry.Key!); // Safe due to check above

            // Prevent directory traversal attacks by ensuring the entry is within the target directory
            if (!combinedPath.StartsWith(Path.GetFullPath(decompressionPath), StringComparison.OrdinalIgnoreCase))
                throw new IOException($"Entry is outside the target directory: {entry.Key}");

            string originalExtractionPath = combinedPath;

            // Ensure directory exists before extraction
            Directory.CreateDirectory(Path.GetDirectoryName(originalExtractionPath)!);

            // Get the source file's last modified time if available
            DateTime? sourceLastWriteTimeUtc = entry.GetLastWriteTimeUtc();

            // Decide on final extraction path based on overwrite mode
            if (!FileSystemUtilities.TryGetExtractionPath(
                    originalExtractionPath,
                    overwriteMode,
                    sourceLastWriteTimeUtc,
                    out string finalExtractionPath))
            {
                // Skip extraction if TryGetExtractionPath says so
                continue;
            }

            // Safety-net in case TryGetExtractionPath fails to respect the overwrite policy
            bool overwrite =
                overwriteMode == ExtractionOverwriteMode.Always ||
                overwriteMode == ExtractionOverwriteMode.IfNewer;

            // Perform the actual extraction
            entry.WriteToFile(finalExtractionPath, new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = overwrite,
            });
        }
    }
}