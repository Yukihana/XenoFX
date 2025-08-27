using System.IO;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Environment.Configuration;

public class ModuleConfiguration
{
    private readonly ModuleProfile _profile;
    private readonly IFFMpegProvisioningOptions _options;

    public ModuleConfiguration(
        ModuleProfile profile,
        IFFMpegProvisioningOptions options)
    {
        _profile = profile;
        _options = options;
    }

    // Shared, Pre-resolved

    public string BinariesDirectory
        => _options.BinariesDirectory;

    public string SharedCacheDirectory
        => _options.SharedCacheDirectory;
}