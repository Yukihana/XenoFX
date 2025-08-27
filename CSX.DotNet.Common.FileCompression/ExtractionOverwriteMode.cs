namespace CSX.DotNet.Common.FileCompression;

public enum ExtractionOverwriteMode
{
    Abort,
    Always,
    IfNewer,
    SkipExisting,
    RenameIfExists,
}