namespace XenoFx.Services.Utility.Configuration.Models;

public sealed partial class RuntimeContext
{
    public bool EnableAssetEnumeration { get; set; } = true;
    public bool EnableAssetTracking { get; set; } = true;
}