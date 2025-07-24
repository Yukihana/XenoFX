using XenoFx.Environment.Configuration;
using XenoServe.Environment.Configuration;

namespace XenoServe.Environment.Modules;

public class XenoFxOptions : IXenoFxOptions
{
    // Defaults

    public const string DefaultDataDirectoryName = "XenoFx";

    // Infrastructure

    private readonly XenoServeConfiguration _config;

    // Lifecycle

    public XenoFxOptions(
        XenoServeConfiguration config)
    {
        _config = config;
    }

    // Factory

    public static XenoFxOptions CreateFrom(
        XenoServeConfiguration config)
        => new(config);

    // Module

    public string DataDirectory
        => _config.GetModuleDirectory(DefaultDataDirectoryName);

    // Shared

    public string AssetsDirectory
        => _config.AssetsDirectory;

    public string UploadsDirectory
        => _config.UploadsDirectory;

    public string MetadataDirectory
        => _config.MetadataDirectory;

    public string SharedCacheDirectory
        => _config.SharedCacheDirectory;
}