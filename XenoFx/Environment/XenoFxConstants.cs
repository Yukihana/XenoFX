using System.Text.Json;

namespace XenoFx.Environment;

public static partial class XenoFxConstants
{
    // Value Constants

    public const string DefaultProfileExtension = ".xfp";

    // Composite Constants

    public static string[] DefaultProfileNames => ["Default", "Index"];

    public static JsonSerializerOptions HumanReadableJsonOptions => new()
    {
        WriteIndented = true
    };
}