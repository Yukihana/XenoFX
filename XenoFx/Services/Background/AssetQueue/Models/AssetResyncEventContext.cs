using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetResyncEventContext : AssetQueueEventContextBase
{
    // Request

    public FileSystemEventArgs EventArgs { get; }

    // Lifecycle

    public AssetResyncEventContext(
        FileSystemEventArgs eventArgs)
    {
        EventArgs = eventArgs;
    }
}