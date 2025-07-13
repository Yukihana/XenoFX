namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetModifiedEventContext : AssetQueueEventContextBase
{
    // Request

    public string FullPath { get; }

    // Lifecycle

    public AssetModifiedEventContext(
        string fullPath)
    {
        FullPath = fullPath;
    }
}