using CSX.DotNet.Common.EFC.Abstractions;
using CSX.DotNet.Common.EFC.Implementations;
using CSX.DotNet.Modules.FileIndexing.Shared.Tenancy.Internal;
using System.IO;

namespace CSX.DotNet.Modules.FileIndexing.Bootstrap.Configuration;

internal class FileIndexingProfile : IFileIndexingProfile
{
    // Defaults

    public const string DefaultFilename = "fileIndexing.json";

    public static string GetFilePath(string dataDirectory) => Path.Combine(
        dataDirectory,
        DefaultFilename);

    // Paths

    public string TenantsDirectory { get; set; } = "Tenants"; //(Tenant configs separate)
    public string DatabaseDirectory { get; set; } = "Database";

    // Database : Default setup (Tenants will override as required)

    public DatabaseConfiguration DatabaseConfiguration { get; set; } = new()
    {
        ProviderType = DatabaseProviderType.Sqlite,
        ConnectionStringTemplate = "Data Source={databaseDirectory}/{tenantId}.sqlite;Cache=Shared;",
        IsScoped = true,
        IsFactory = false
    };
}