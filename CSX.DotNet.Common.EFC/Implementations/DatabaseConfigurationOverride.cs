using CSX.DotNet.Common.EFC.Abstractions;

namespace CSX.DotNet.Common.EFC.Implementations;

public class DatabaseConfigurationOverride
    : IDatabaseConfigurationOverride
{
    public DatabaseProviderType? ProviderType { get; set; } = null;

    public string? ConnectionStringTemplate { get; set; } = null;

    public bool? IsScoped { get; set; } = null;

    public bool? IsFactory { get; set; } = null;
}