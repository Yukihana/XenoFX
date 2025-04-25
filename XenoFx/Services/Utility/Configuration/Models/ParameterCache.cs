using CSX.Common.IO;

namespace XenoFx.Services.Utility.Configuration.Models;

public sealed partial class ParameterCache
{
    public string BaseDirectory { get; set; } = string.Empty;

    // Resolved SubPaths : Base

    public string AssetsDirectory { get; set; } = "Assets";
    public string CacheDirectory { get; set; } = "Cache";
    public string ThumbsDirectory { get; set; } = "Thumbs";

    // Resolved SubPaths : Assets

    public string AssetUploadDirectory { get; set; } = "Uploads";

    // Resolved SubPaths : Cache

    public string UploadDirectory { get; set; } = "Uploads";

    // Parameters

    public PathFilterConfiguration FilterConfig { get; set; } = new();
}