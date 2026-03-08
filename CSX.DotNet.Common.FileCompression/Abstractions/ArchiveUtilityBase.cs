using CSX.DotNet.Common.FileCompression.Shared;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.FileCompression.Abstractions;

public abstract class ArchiveUtilityBase
{
    public async Task<string> ExtractAsync(
        string archivePath,
        string decompressionPath,
        ExtractionOverwriteMode overwriteMode = ExtractionOverwriteMode.Abort,
        bool deleteArchiveAfterUse = false,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        if (!File.Exists(archivePath))
            throw new FileNotFoundException("Archive not found", archivePath);

        // Determine decompression path if not provided
        if (string.IsNullOrWhiteSpace(decompressionPath))
        {
            decompressionPath = FileSystemUtilities.DetermineDecompressDirectory(
                archivePath: archivePath,
                overwriteMode: overwriteMode);
        }

        // Allow decompressPath to throw early if it's bad or unwritable
        Directory.CreateDirectory(decompressionPath);

        // Let the subclass handle the actual extraction
        await ExtractInternalAsync(
            archivePath: archivePath,
            decompressionPath: decompressionPath,
            overwriteMode: overwriteMode,
            ctoken: ctoken);

        // Cleanup
        if (deleteArchiveAfterUse)
            File.Delete(archivePath);

        return decompressionPath;
    }

    // Abstract method forces each subclass to implement its own extraction logic
    protected abstract Task ExtractInternalAsync(
        string archivePath,
        string decompressionPath,
        ExtractionOverwriteMode overwriteMode = ExtractionOverwriteMode.Abort,
        CancellationToken ctoken = default);
}