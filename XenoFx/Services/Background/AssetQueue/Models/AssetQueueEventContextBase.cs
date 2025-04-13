namespace XenoFx.Services.Background.AssetQueue.Models;

public abstract class AssetQueueEventContextBase
{
    public bool ReevaluationRequired { get; set; } = false;
    public ulong ReevaluationCount { get; set; } = 0;
}