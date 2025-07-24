using System;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Background.AssetQueue.Models;

public sealed class AssetUploadedEventContext : AssetQueueEventContextBase
{
    // Request

    public string UploadMetadataPath { get; }
    public Func<string, CancellationToken, Task>? CleanupCallback { get; }

    // Lifecycle

    public AssetUploadedEventContext(
        string uploadMetadataPath,
        Func<string, CancellationToken, Task>? cleanupCallback = null)
    {
        UploadMetadataPath = uploadMetadataPath;
        CleanupCallback = cleanupCallback;
    }
}