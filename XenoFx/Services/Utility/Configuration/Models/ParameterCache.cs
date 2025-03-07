using CSX.Common.IO;
using System;

namespace XenoFx.Services.Utility.Profile.Models;

public sealed partial class ParameterCache
{
    public string BaseDirectory { get; set; } = string.Empty;

    // SubPaths

    public string AssetsDatabasePath { get; set; } = "XenoAssets.sqlite";
    public string AssetsDirectory { get; set; } = "Assets";
    public string CacheDirectory { get; set; } = "Cache";
    public string ThumbsDirectory { get; set; } = "Thumbs";

    // Parameters

    public PathFilterConfiguration FilterConfig { get; set; } = new();
}