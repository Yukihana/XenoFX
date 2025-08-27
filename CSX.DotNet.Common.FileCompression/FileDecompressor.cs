namespace CSX.DotNet.Common.FileCompression;

public static partial class FileDecompressor
{
    public static async Task<string> DecompressAsync(
        string archiveFilePath,
        string decompressionPath,
        ExtractionOverwriteMode overwriteMode = ExtractionOverwriteMode.Abort,
        bool deleteArchiveAfterUse = false,
        CancellationToken ctoken = default)
    {
        // Instantiate the corresponding utility class based on the archive type
        var utility = ArchiveUtilityFactory.Create(archiveFilePath);

        return await utility.ExtractAsync(
            archiveFilePath,
            decompressionPath: decompressionPath,
            overwriteMode: overwriteMode,
            deleteArchiveAfterUse: deleteArchiveAfterUse,
            ctoken: ctoken);
    }
}