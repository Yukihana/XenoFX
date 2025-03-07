using CSX.Common.IO;

namespace XenoFx.Environment;

public sealed partial class XenoFxProfile
{
    // Paths

    public string AssetsDatabasePath { get; set; } = "XenoAssets.sqlite";
    public string AssetsDirectory { get; set; } = "Assets";
    public string CacheDirectory { get; set; } = "Cache";
    public string ThumbsDirectory { get; set; } = "Thumbs";

    // Parameters

    public PathFilterConfiguration AssetFilterConfig { get; set; } = new();
}