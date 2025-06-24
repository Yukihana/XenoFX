using CSX.Common.IO;

namespace XenoFx.Environment;

public sealed partial class XenoFxProfile
{
    // Directories ----------------

    public string AssetsDirectory { get; set; } = "Assets";
    public string AssetUploadDirectory { get; set; } = "Uploads";
    public string DatabasesDirectory { get; set; } = "Databases";
    public string CacheDirectory { get; set; } = "Cache";
    public string ThumbsDirectory { get; set; } = "Thumbs";

    // Databases

    public string AssetsDatabasePath { get; set; } = "Assets.sqlite";
    public string AuthDatabasePath { get; set; } = "Auth.sqlite";
    public string TempDatabasePath { get; set; } = "Temp.sqlite";

    // Parameters ---------------

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