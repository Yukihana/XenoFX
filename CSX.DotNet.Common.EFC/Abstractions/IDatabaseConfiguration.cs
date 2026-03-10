namespace CSX.DotNet.Common.EFC.Abstractions;

public interface IDatabaseConfiguration
    : IDatabaseOptions
{
    DatabaseProviderType ProviderType { get; }
}