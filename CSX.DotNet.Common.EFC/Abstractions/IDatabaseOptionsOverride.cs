namespace CSX.DotNet.Common.EFC.Abstractions;

public interface IDatabaseOptionsOverride
{
    string? ConnectionStringTemplate { get; }

    bool? IsScoped { get; }

    bool? IsFactory { get; }
}