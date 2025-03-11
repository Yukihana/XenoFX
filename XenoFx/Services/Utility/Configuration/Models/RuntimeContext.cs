using System;

namespace XenoFx.Services.Utility.Configuration.Models;

public sealed partial class RuntimeContext
{
    public bool EnableAssetEnumeration { get; set; } = true;

    // Asset Tracking

    private bool enableAssetTracking = true;

    public bool EnableAssetTracking
    {
        get => enableAssetTracking;
        set
        {
            enableAssetTracking = value;
            AssetTrackingConfigurationUpdatedCallback?.Invoke();
        }
    }

    public Action? AssetTrackingConfigurationUpdatedCallback { get; set; } = null;
}