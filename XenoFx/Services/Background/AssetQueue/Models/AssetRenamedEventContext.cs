namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetRenamedEventContext : AssetQueueEventContextBase
{
    // Request

    public string FullPath { get; }
    public string OldFullPath { get; }

    // Lifecycle

    public AssetRenamedEventContext(
        string fullPath,
        string oldFullPath)
    {
        FullPath = fullPath;
        OldFullPath = oldFullPath;
    }
}