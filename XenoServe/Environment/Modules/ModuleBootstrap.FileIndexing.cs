namespace XenoServe.Environment.Modules;

public static partial class ModuleBootstrap
{
    private const string AssetsIndexerTenantId = "Assets";
    /*
    public static async Task<IServiceCollection> AddFileIndexingModuleAsync(
        this IServiceCollection services,
        XenoServeConfiguration configuration,
        string moduleDirectoryName = "",
        CancellationToken ctoken = default)
    {
        await services.AddFileIndexingAsync(
            configureOptions: options =>
            {
                options.DataDirectory = configuration.GetModuleDirectory(moduleDirectoryName);
                options.MonitoredRootPath = configuration.AssetsDirectory;
            },
            configureProfile: profile =>
            {
                bool updated = false;

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
                }

                // Persist if changes were made
                return updated;
            },
            ctoken: ctoken);

        return services;
    }
    */
}