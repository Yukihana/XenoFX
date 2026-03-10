using CSX.DotNet.Common.EFC.Implementations;
using CSX.DotNet.Common.IO.Paths;

namespace CSX.DotNet.Modules.FileIndexing.Bootstrap.Tenants;

public interface ITenantProfile
{
    // Metadata

    string Id { get; set; }
    string Comment { get; set; }

    // Pathing

    string AssetsRootPath { get; set; } // Path to monitor
    string IngressDirectory { get; set; }
    string[] AllowedFileExtensions { get; set; }
    PathFilterConfiguration PathFilterConfiguration { get; set; } // aka Filter

    // Database

    DatabaseConfigurationOverride DatabaseConfigurationOverride { get; set; }
}