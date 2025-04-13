using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetRenamedEventContext : AssetQueueEventContextBase
{
    // Request

    public string OldRelativePath { get; }
    public string NewRelativePath { get; }
    public RenamedEventArgs EventArgs { get; }

    // Lifecycle

    public AssetRenamedEventContext(
        string oldRelativePath,
        string newRelativePath,
        RenamedEventArgs eventArgs)
    {
        OldRelativePath = oldRelativePath;
        NewRelativePath = newRelativePath;
        EventArgs = eventArgs;
    }
}