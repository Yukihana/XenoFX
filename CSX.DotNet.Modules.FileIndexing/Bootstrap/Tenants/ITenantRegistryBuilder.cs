using System.Collections.Generic;

namespace CSX.DotNet.Modules.FileIndexing.Bootstrap.Tenants;

public interface ITenantRegistryBuilder
{
    IReadOnlyDictionary<string, ITenantProfile> GetTenants();
}