using System.IO;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetResyncEventContext : AssetQueueEventContextBase
{
    // Request

    public string RelativePath { get; }

    // Lifecycle

    public AssetResyncEventContext(
        string relativePath)
    {
        RelativePath = relativePath;
    }
}