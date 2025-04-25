using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetRenamedEventContext : AssetQueueEventContextBase
{
    // Request

    public RenamedEventArgs EventArgs { get; }

    // Lifecycle

    public AssetRenamedEventContext(
        RenamedEventArgs eventArgs)
    {
        EventArgs = eventArgs;
    }
}