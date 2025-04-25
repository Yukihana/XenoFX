using CSX.Common.Data.Events;

namespace XenoFx.Services.Background.AssetQueue.Models;

public sealed class AssetUploadedEventContext : AssetQueueEventContextBase
{
    public FileUploadedEventArgs EventArgs { get; }

    // Lifecycle

    public AssetUploadedEventContext(
        FileUploadedEventArgs eventArgs)
    {
        EventArgs = eventArgs;
    }
}