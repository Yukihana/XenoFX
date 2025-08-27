namespace CSX.DotNet.Common.IO.Storage;

public enum ConflictResolution
{
    None,       // Fail if exists
    Overwrite,  // Replace if exists
    Rename,     // Retry with new GUID filename
}