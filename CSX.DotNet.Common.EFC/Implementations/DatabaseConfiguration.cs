using CSX.DotNet.Common.EFC.Abstractions;

namespace CSX.DotNet.Common.EFC.Implementations;

// Dumb POCO if consumers don't want to implement the interface themselves.
public class DatabaseConfiguration : IDatabaseConfiguration
{
    public DatabaseProviderType ProviderType { get; set; } = DatabaseProviderType.Unknown;
    public string ConnectionStringTemplate { get; set; } = string.Empty;
    public bool IsScoped { get; set; } = true;
    public bool IsFactory { get; set; } = false;
}