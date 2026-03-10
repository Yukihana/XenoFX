using System.Collections.Generic;

namespace CSX.DotNet.Modules.FileIndexing.Bootstrap.Tenants;

internal sealed class TenantRegistryBuilder : ITenantRegistryBuilder
{
    public IReadOnlyDictionary<string, ITenantProfile> GetTenants()
    {
        // TODO
        return new Dictionary<string, ITenantProfile>().AsReadOnly();
    }
}