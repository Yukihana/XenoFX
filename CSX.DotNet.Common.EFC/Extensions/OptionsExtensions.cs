using CSX.DotNet.Common.EFC.Abstractions;
using CSX.DotNet.Common.EFC.Implementations;

namespace CSX.DotNet.Common.EFC.Extensions;

public static class OptionsExtensions
{
    public static IDatabaseConfiguration Composite(
        this IDatabaseConfiguration baseConfiguration,
        IDatabaseConfigurationOverride overrideConfiguration)
    {
        DatabaseConfiguration result = new()
        {
            ProviderType
                = overrideConfiguration.ProviderType
                ?? baseConfiguration.ProviderType,

            ConnectionStringTemplate
                = overrideConfiguration.ConnectionStringTemplate
                ?? baseConfiguration.ConnectionStringTemplate,

            IsScoped
                = overrideConfiguration.IsScoped
                ?? baseConfiguration.IsScoped,

            IsFactory
                = overrideConfiguration.IsFactory
                ?? baseConfiguration.IsFactory
        };

        return result;
    }
}