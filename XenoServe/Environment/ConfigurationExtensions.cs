using CSX.DotNet.Common.Data.Text;
using CSX.DotNet.Common.IO.Storage.AppProfiles;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoServe.Environment.Configuration;

namespace XenoServe.Environment;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Serves as the main entry point for retrieving the core configuration
    /// </summary>
    public async static Task<XenoServeConfiguration> GetXenoServeConfigAsync(
        this IConfiguration configuration,
        string[] args,
        CancellationToken ctoken = default)
    {
        // Read options
        IConfigurationSection configurationSection = configuration.GetSection(XenoServeOptions.SectionTitle);
        XenoServeOptions options = configurationSection.Get<XenoServeOptions>() ?? new();

        // Retrieve profile from the path
        var profilePath = options.DetermineProfilePath(
            args: args,
            defaultPath: XenoServeProfile.DefaultPath);
        string fullProfilePath = Path.GetFullPath(profilePath);
        var profile = await ProfileStore.ReadOrCreateAsync<XenoServeProfile>(fullProfilePath, ctoken: ctoken);

        // Create the scaffold and set the profile path
        XenoServeConfiguration xsConfig = new(
            options: options,
            profile: profile,
            fullProfilePath: fullProfilePath);

        // Return the scaffold
        return xsConfig;
    }

    public static string DetermineProfilePath(
        this XenoServeOptions options,
        string[] args,
        string defaultPath)
    {
        // Note: Fallback if not provided, throw on invalid.

        // Check if args provided a profile path
        if (options.UseCommandLine)
        {
            ArgsArray argsArray = new(args);
            if (argsArray.TryGet("-profile", out string? profilePath) && !string.IsNullOrEmpty(profilePath))
                return profilePath;
        }

        // Check if the configuration provides a startup profile path
        if (!string.IsNullOrWhiteSpace(options.StartupProfilePath))
            return options.StartupProfilePath;

        // Fall back to preset default profile path
        return defaultPath;
    }
}