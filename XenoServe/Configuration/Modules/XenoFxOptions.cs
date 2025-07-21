using XenoFx.Environment.Configuration;

namespace XenoServe.Configuration.Modules;

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

    public static XenoFxOptions Create(
        XenoServeConfiguration config)
        => new(config);

    // Module

    public string DataDirectory
        => _config.GetModuleDirectory(DefaultDataDirectoryName);

    // Shared

    public string AssetsDirectory
        => _config.AssetsDirectory;

    public string SharedCacheDirectory
        => _config.SharedCacheDirectory;
}