using CSX.DotNet.Modules.FFMpeg.Provisioning.Environment.Configuration;
using XenoServe.Environment.Configuration;

namespace XenoServe.Environment.Modules;

public class FFMpegProvisioningOptions : IFFMpegProvisioningOptions
{
    // Defaults

    public const string DefaultDataDirectoryName = "FFMpegProvisioning";

    // Infrastructure

    private readonly XenoServeConfiguration _config;

    // Lifecycle

    public FFMpegProvisioningOptions(
        XenoServeConfiguration config)
    {
        _config = config;
    }

    // Factory

    public static FFMpegProvisioningOptions CreateFrom(
        XenoServeConfiguration config)
        => new(config);

    // Module

    public string DataDirectory
        => _config.GetModuleDirectory(DefaultDataDirectoryName);

    // Shared

    public string BinariesDirectory
        => _config.BinariesDirectory;

    public string SharedCacheDirectory
        => _config.SharedCacheDirectory;
}