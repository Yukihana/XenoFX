using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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