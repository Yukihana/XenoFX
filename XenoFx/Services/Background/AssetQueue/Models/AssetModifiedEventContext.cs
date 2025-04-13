using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetModifiedEventContext : AssetQueueEventContextBase
{
    // Request

    public string RelativePath { get; }
    public FileSystemEventArgs EventArgs { get; }

    // Lifecycle

    public AssetModifiedEventContext(
        string relativePath,
        FileSystemEventArgs eventArgs)
    {
        RelativePath = relativePath;
        EventArgs = eventArgs;
    }
}