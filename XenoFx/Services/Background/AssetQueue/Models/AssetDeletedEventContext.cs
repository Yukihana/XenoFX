using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetDeletedEventContext : AssetQueueEventContextBase
{
    // Request

    public string FullPath { get; }

    // Lifecycle

    public AssetDeletedEventContext(
        string fullPath)
    {
        FullPath = fullPath;
    }
}