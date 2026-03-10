using CSX.DotNet.Common.Data.Text.Sanitization;
using CSX.DotNet.Modules.FileIndexing.Bootstrap;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoServe.Environment.Configuration;

namespace XenoServe.Environment.Modules;

public static partial class ModuleBootstrap
{
    private const string AssetsIndexerTenantId = "Assets";
    private const string DefaultModuleDirectoryName = "FileIndexing";

    public static async Task<IServiceCollection> AddFileIndexingModuleAsync(
        this IServiceCollection services,
        XenoServeConfiguration configuration,
        string moduleDirectoryName = DefaultModuleDirectoryName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Ensure module's data directory name is valid to prevent issues when used in file paths
        if (string.IsNullOrWhiteSpace(moduleDirectoryName) ||
            moduleDirectoryName is "." or ".." ||
            !FileNameSanitizer.IsStrictFileName(moduleDirectoryName))
        {
            throw new ArgumentException(
                "The value must be a valid sanitized file name.",
                nameof(moduleDirectoryName));
        }

        await services.AddFileIndexingAsync(
            configureOptions: options =>
            {
                options.DataDirectory = configuration.GetModuleDirectory(moduleDirectoryName);
            },
            configureProfile: profile =>
            {
                bool saveConfirmation = false;

                // Profile settings are usually for modules to handle themselves.
                // For any required changes however, set saveConfirmation to true.

                // Persist if changes were made
                return saveConfirmation;
            },
            configureTenants: builder =>
            {
                bool saveConfirmation = false;

                // builder.Tenants.AddOrUpdate(AssetsIndexerTenantId, tenantBuilder => {}); etc
                // The dictionary should not be exposed here,
                // instead the builder should have the necessary APIs to add or update tenants
                // , to avoid dealing with the dictionary directly here

                // avoid unused variable warning for now,
                // until we have actual tenant configuration to add here
                _ = AssetsIndexerTenantId;

                /* Old code sample:
                // Set up startup tenants if missing
                if (!profile.StartupTenants.ContainsKey(AssetsIndexerTenantId))
                {
                    // if Tenants['Assets'] is missing, create default
                    profile.StartupTenants[AssetsIndexerTenantId] = new();
                    // Let the implementation be internal to the module
                    // FileIndexing...CreateTenant();
                    // Then apply property updates here
                    // Or add a AddOrUpdate api directly,
                    // so we dont need to deal with the implementation or dictionary here

                    updated = true;
                }*/

                // Persist if changes were made
                return saveConfirmation;
            },
            ctoken: cancellationToken);

        return services;
    }
}