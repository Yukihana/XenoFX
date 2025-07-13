using System.Collections.Generic;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetResyncEventContext : AssetQueueEventContextBase
{
    // Request

    public List<string> FileList { get; }

    // Lifecycle

    public AssetResyncEventContext(
        List<string> fileList)
    {
        FileList = fileList;
    }
}