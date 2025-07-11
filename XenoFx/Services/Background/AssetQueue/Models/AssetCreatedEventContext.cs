using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetCreatedEventContext : AssetQueueEventContextBase
{
    // Request

    public string FullPath { get; }

    // Lifecycle

    public AssetCreatedEventContext(
        string fullPath)
    {
        FullPath = fullPath;
    }
}