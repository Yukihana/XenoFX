using CSX.Common.IO;

namespace XenoFx.Environment;

public sealed partial class XenoFxProfile
{
    // Databases

    public string AssetsDatabasePath { get; set; } = "XenoAssets.sqlite";
    public string TempDatabasePath { get; set; } = "Temp.sqlite";

    // Directories

    public string AssetsDirectory { get; set; } = "Assets";
    public string AssetUploadDirectory { get; set; } = "Uploads";
    public string CacheDirectory { get; set; } = "Cache";
    public string ThumbsDirectory { get; set; } = "Thumbs";

    // Parameters

    public PathFilterConfiguration AssetFilterConfig { get; set; } = new()
    {
        Greylist = ["**/*.*"],
        Blacklist = [
            "**/*.x",                                           // Text Metadata
            $"**/*{XenoFxConstants.DefaultUploadExtension}"]    // Uploads
    };

    // Hosted

    public ulong AssetEnumerationIntervalSeconds { get; set; } = 3600;
}