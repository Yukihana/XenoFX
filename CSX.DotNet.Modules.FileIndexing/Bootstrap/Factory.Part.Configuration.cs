using CSX.DotNet.Common.IO.Storage.AppProfiles;
using CSX.DotNet.Modules.FileIndexing.Bootstrap.Configuration;
using CSX.DotNet.Modules.FileIndexing.Bootstrap.Tenants;
using CSX.DotNet.Modules.FileIndexing.Shared.Configuration.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileIndexing.Bootstrap;

public static partial class Factory
{
    internal static async Task<ModuleConfiguration> GetConfigAsync(
        this IServiceCollection services,
        Action<IFileIndexingOptions> configureOptions,
        Func<IFileIndexingProfile, bool> configureProfile,
        Func<ITenantRegistryBuilder, bool> configureTenants,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Placeholder for IOptions usage
        _ = services;

        // Spin up a default options instance and apply the runtime updates
        FileIndexingOptions options = new();
        configureOptions(options);

        // Load (or create) the module's profile and patch it if applicable
        var profilePath = FileIndexingProfile.GetFilePath(options.DataDirectory);
        var profile = await ProfileStore.ReadOrCreateAsync<FileIndexingProfile>(
            path: profilePath,
            configure: configureProfile,
            ctoken: ctoken);

        // In case tenancy is used in this module, build it here:
        TenantRegistryBuilder builder = new();
        // TODO: Read disk and populate persistence first.
        _ = configureTenants;
        // TODO: If save is consented, make updates to the disk as needed.
        // TODO: Perhaps set this up as library code, just like ProfileStore.
        return new(profile, builder.GetTenants(), options);
    }
}