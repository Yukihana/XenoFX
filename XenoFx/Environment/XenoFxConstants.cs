using System.Text.Json;

namespace XenoFx.Environment;

public static partial class XenoFxConstants
{
    // Value Constants

    public const string DefaultProfileExtension = ".xfp";

    public const string DefaultUploadExtension = ".xfu";
    public const int WriteBufferSize = 4 * 1024 * 1024;

    // Composite Constants

    public static string[] DefaultProfileNames => ["Default", "Index"];

    public static string[] AllowedAssetExtensions => [
        ".mp4", ".webm", ".mkv", ".flv", ".avi", ];           // Modern Video Formats

    public static JsonSerializerOptions HumanReadableJsonOptions => new()
    {
        WriteIndented = true
    };
}