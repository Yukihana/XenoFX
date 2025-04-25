using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetDeletedEventContext : AssetQueueEventContextBase
{
    // Request

    public FileSystemEventArgs EventArgs { get; }

    // Lifecycle

    public AssetDeletedEventContext(
        FileSystemEventArgs eventArgs)
    {
        EventArgs = eventArgs;
    }
}