using System.IO;

namespace XenoServe.Environment.Configuration;

/// <summary>
/// Model for configuration specifications in the data directory.
/// </summary>
public partial class XenoServeProfile
{
    // Defaults

    public static string DefaultPath => Path.Combine(
        "Workspaces",
        "DefaultWorkspace",
        "Default.xsp");

    // Lifecycle

    public XenoServeProfile()
    { }

    // Partitioning directories

    public string ModulesDirectory { get; set; } = "Modules";

    // Shared directories

    public string AssetsDirectory { get; set; } = "Assets";

    public string UploadsDirectory { get; set; } = "Uploads";

    public string MetadataDirectory { get; set; } = "Metadata";

    public string SharedCacheDirectory { get; set; } = "Cache";
}