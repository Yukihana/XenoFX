using CSX.DotNet.Common.EFC.Implementations;

namespace CSX.DotNet.Modules.FileIndexing.Bootstrap.Configuration;

public interface IFileIndexingProfile
{
    // Paths

    string TenantsDirectory { get; set; }
    string DatabaseDirectory { get; set; }

    // Database : Default setup (Tenants will override as required)

    DatabaseConfiguration DatabaseConfiguration { get; set; }
}