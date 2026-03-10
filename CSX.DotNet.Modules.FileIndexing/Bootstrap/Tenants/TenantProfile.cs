using CSX.DotNet.Common.EFC.Implementations;
using CSX.DotNet.Common.IO.Paths;

namespace CSX.DotNet.Modules.FileIndexing.Bootstrap.Tenants;

internal sealed class TenantProfile : ITenantProfile
{
    // Metadata

    public string Id { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;

    // Pathing

    public string AssetsRootPath { get; set; } = string.Empty;
    public string IngressDirectory { get; set; } = "Ingress";
    public string[] AllowedFileExtensions { get; set; } = []; // No preceeding dots.
    public PathFilterConfiguration PathFilterConfiguration { get; set; } = new(); // aka Filter

    // Database

    public DatabaseConfigurationOverride DatabaseConfigurationOverride { get; set; } = new();
}