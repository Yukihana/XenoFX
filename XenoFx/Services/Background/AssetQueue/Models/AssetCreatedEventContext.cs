using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetCreatedEventContext : AssetQueueEventContextBase
{
    // Request

    public string RelativePath { get; }
    public FileSystemEventArgs EventArgs { get; }

    // Lifecycle

    public AssetCreatedEventContext(
        string relativePath,
        FileSystemEventArgs eventArgs)
    {
        RelativePath = relativePath;
        EventArgs = eventArgs;
    }
}