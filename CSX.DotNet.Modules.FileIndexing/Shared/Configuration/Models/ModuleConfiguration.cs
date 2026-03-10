using CSX.DotNet.Modules.FileIndexing.Bootstrap.Configuration;
using CSX.DotNet.Modules.FileIndexing.Bootstrap.Tenants;
using System.Collections.Generic;

namespace CSX.DotNet.Modules.FileIndexing.Shared.Configuration.Models;

internal class ModuleConfiguration : IModuleConfiguration
{
    // Infrastructure

    private readonly IFileIndexingProfile _profile;
    private IReadOnlyDictionary<string, ITenantProfile> _tenantProfiles;
    private readonly IFileIndexingOptions _options;

    // Lifecycle

    public ModuleConfiguration(
        IFileIndexingProfile profile,
        IReadOnlyDictionary<string, ITenantProfile> tenantProfiles,
        IFileIndexingOptions options)
    {
        _profile = profile;
        _tenantProfiles = tenantProfiles;
        _options = options;
    }
}