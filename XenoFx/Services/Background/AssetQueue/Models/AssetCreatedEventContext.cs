using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetCreatedEventContext : AssetQueueEventContextBase
{
    // Request

    public FileSystemEventArgs EventArgs { get; }

    // Lifecycle

    public AssetCreatedEventContext(
        FileSystemEventArgs eventArgs)
    {
        EventArgs = eventArgs;
    }
}