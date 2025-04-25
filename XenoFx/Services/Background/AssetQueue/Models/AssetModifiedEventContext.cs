using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetModifiedEventContext : AssetQueueEventContextBase
{
    // Request

    public FileSystemEventArgs EventArgs { get; }

    // Lifecycle

    public AssetModifiedEventContext(
        FileSystemEventArgs eventArgs)
    {
        EventArgs = eventArgs;
    }
}