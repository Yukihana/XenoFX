using CSX.DotNet.Common.IO;
using System.IO;

namespace XenoFx.Environment.Configuration;

public sealed partial class XenoFxProfile
{
    // Defaults

    public const string DefaultFilename = "xenoFx.json";

    public static string GetFilePath(string dataDirectory) => Path.Combine(
        dataDirectory,
        DefaultFilename);

    // Directories

    public string DatabaseDirectory { get; set; } = "Database";
    public string AssetsUploadDirectory { get; set; } = "Uploaded";
    public string ThumbsDirectory { get; set; } = "Thumbs";

    // Assets database

    public string AssetsDatabasePath { get; set; } = "Assets.sqlite";
    public string AssetsDatabaseType { get; set; } = "sqlite";

    // Cache database

    public string CacheDatabasePath { get; set; } = "Cache.sqlite";
    public string CacheDatabaseType { get; set; } = "sqlite";

    // Parameters

    public string[] AllowedAssetExtensions { get; set; } = [
        ".mp4", ".webm", ".mkv", ".flv", ".avi", ];

    public PathFilterConfiguration AssetFilterConfig { get; set; } = new()
    {
        Greylist = ["**/*.*"],
        Blacklist = [
            "**/*.x",        // Text Metadata
            $"**/*.json"]    // Uploads
    };

    public ulong AssetEnumerationIntervalSeconds { get; set; } = 3600;
}