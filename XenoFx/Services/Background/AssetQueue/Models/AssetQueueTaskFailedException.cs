using System;

namespace XenoFx.Services.Background.AssetQueue.Models;

public class AssetQueueTaskFailedException : Exception
{
    public AssetQueueEventContextBase EventContext { get; init; }

    public AssetQueueTaskFailedException(AssetQueueEventContextBase context, Exception innerException)
        : base($"Task failed for {context.GetType().Name}.", innerException)
    {
        EventContext = context;
    }
}