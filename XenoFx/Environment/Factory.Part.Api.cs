using System;

namespace XenoFx.Environment;

public static partial class Factory
{
    // Legacy
    /*
    public async static Task<XenoFxConfiguration> GetConfiguration(IConfiguration configuration, CancellationToken ctoken = default)
    {
        // Config
        IConfigurationSection configurationSection = configuration.GetSection(XenoFxOptions.SectionTitle);
        XenoFxOptions config = configurationSection.Get<XenoFxOptions>() ?? new();

        // ProfilePath
        string profilePath = config.StartupPath;
        if (config.UseCommandLine &&
            System.Environment.GetCommandLineArgs() is string[] args &&
            args.Length > 1)
        {
            profilePath = args[1];
        }

        // Profile

        XenoFxProfile profile = await LoadProfile(profilePath, ctoken);
        XenoFxConfiguration config = new(profilePath, profile, config);
        return config;
    }
    */

    // TODO Documentation:  Handles pre-initialization for the framework before consumption.
    public static IServiceProvider PreInitializeXenoFx(this IServiceProvider provider)
    {
        // Start database connections

        // Warm up the file tracker

        // Detect assets and run quick-mode integrity tests

        // Register available assets for consumption

        return provider;
    }

    // TODO Documentation: Activates parallel subroutines
    public static IServiceProvider Activate(this IServiceProvider provider)
    {
        // Start database connections

        // Warm up the file tracker

        // Detect assets and run quick-mode integrity tests

        // Register available assets for consumption

        return provider;
    }
}