using System.IO;
using XenoServe.Configuration.Options;

namespace XenoServe.Configuration;

/// <summary>
/// Serves as the primary compounding wrapper for runtime specifications.
/// </summary>
public class XenoServeConfiguration
{
    // Infrastructure

    private readonly XenoServeOptions _options; // Configuration section from appsettings.json
    private readonly XenoServeProfile _profile; // User config from the data directory
    private readonly string _fullProfilePath;   // Final profile path

    // Lifecycle

    public XenoServeConfiguration(
        XenoServeOptions options,
        XenoServeProfile profile,
        string fullProfilePath)
    {
        _options = options;
        _profile = profile;
        _fullProfilePath = fullProfilePath;

        // Cache this ahead to prevent repeated processing
        ProfileDirectoryFullPath = Path.GetDirectoryName(fullProfilePath)
            ?? throw new IOException("Unable to determine the data directory");
    }

    // Profile

    private string ProfileDirectoryFullPath { get; }

    // Modules

    public string GetModuleDirectory(string moduleName) => Path.Combine(
        ProfileDirectoryFullPath,
        _profile.ModulesDirectory,
        moduleName);

    // Shared

    public string AssetsDirectory => Path.Combine(
        ProfileDirectoryFullPath,
        _profile.AssetsDirectory);

    public string SharedCacheDirectory => Path.Combine(
        ProfileDirectoryFullPath,
        _profile.SharedCacheDirectory);
}