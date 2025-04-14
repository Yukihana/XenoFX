using CSX.Common.IO;

namespace XenoFx.Services.Utility.Profile.Models;

public sealed partial class ParameterCache
{
    public string BaseDirectory { get; set; } = string.Empty;

    // SubPaths

    public string AssetsDatabasePath { get; set; } = "Assets.sqlite";
    public string AssetsDirectory { get; set; } = "Assets";
    public string CacheDirectory { get; set; } = "Cache";
    public string ThumbsDirectory { get; set; } = "Thumbs";
    public string AssetUploadDirectory { get; set; } = "Uploads";

    // Parameters

    public PathFilterConfiguration FilterConfig { get; set; } = new();
}