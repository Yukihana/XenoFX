namespace CSX.DotNet.Common.EFC.Abstractions;

public interface IDatabaseConfigurationOverride
    : IDatabaseOptionsOverride
{
    DatabaseProviderType? ProviderType { get; }
}