using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetDeletedEventContext : AssetQueueEventContextBase
{
    // Request

    public string RelativePath { get; }
    public FileSystemEventArgs EventArgs { get; }

    // Lifecycle

    public AssetDeletedEventContext(
        string relativePath,
        FileSystemEventArgs eventArgs)
    {
        RelativePath = relativePath;
        EventArgs = eventArgs;
    }
}