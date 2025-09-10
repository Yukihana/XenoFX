using CSX.DotNet.Modules.FileIndexing.Environment.Configuration;

namespace CSX.DotNet.Modules.FileIndexing.Shared.Configuration.Models;

internal class ModuleConfiguration : IModuleConfiguration
{
    // Infrastructure

    private readonly IFileIndexingProfile _profile;
    private readonly IFileIndexingOptions _options;

    // Lifecycle

    public ModuleConfiguration(
        IFileIndexingProfile profile,
        IFileIndexingOptions options)
    {
        _profile = profile;
        _options = options;
    }
}