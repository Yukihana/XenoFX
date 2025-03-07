using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Environment;

public static partial class FactoryExtensions
{
    public static XenoFxOptions GetXenoFxOptions(this IConfiguration configuration)
    {
        IConfigurationSection configurationSection = configuration.GetSection(XenoFxOptions.SectionTitle);
        return configurationSection.Get<XenoFxOptions>() ?? new();
    }

    public async static Task<XenoFxConfiguration> GetXenoFxConfiguration(this IConfiguration configuration, CancellationToken ctoken = default)
    {
        XenoFxOptions options = configuration.GetXenoFxOptions();
        (XenoFxProfile profile, string startupPath) = await options.GetProfile(ctoken: ctoken);
        return new(
            startupPath: startupPath,
            profile: profile,
            options: options);
    }

    public static string GetProfilePath(this XenoFxOptions options)
    {
        if (options.UseCommandLine &&
            System.Environment.GetCommandLineArgs() is string[] args &&
            args.Length > 1)
        {
            return args[1];
        }
        return options.StartupPath;
    }

    public async static Task<(XenoFxProfile, string)> GetProfile(this XenoFxOptions options, CancellationToken ctoken = default)
    {
        string providedPath = options.GetProfilePath();

        // File assumed: if the path is not the directory and has the designated extension, .
        if (!Directory.Exists(providedPath) && providedPath.EndsWith(XenoFxConstants.DefaultProfileExtension))
        {
            // try load if exists, else create new.
            XenoFxProfile profile
                = File.Exists(providedPath)
                ? await LoadProfile(providedPath, ctoken)
                : await CreateProfile(providedPath, ctoken);

            return (profile, providedPath);
        }

        // Directory assumed by default: If it doesn't exist, create it
        if (!Directory.Exists(providedPath))
            Directory.CreateDirectory(providedPath);

        // Prepare default filenames to be tested, preceeded with provided path
        string[] testPaths = [.. XenoFxConstants.DefaultProfileNames
                .Select(x => x + XenoFxConstants.DefaultProfileExtension)
                .Select(x => Path.Combine(providedPath, x))];

        // Scan for existing files (Note: Unknown formats will throw errors. This is intended.)
        foreach (string path in testPaths)
        {
            if (File.Exists(path))
                return (await LoadProfile(path, ctoken), path);
        }

        // If none of those files exist, create a profile based on the first default name
        string fallbackPath = testPaths.First();
        XenoFxProfile fallbackProfile = await CreateProfile(fallbackPath, ctoken);
        return (fallbackProfile, fallbackPath);
    }

    public static async Task<XenoFxProfile> LoadProfile(string profilePath, CancellationToken ctoken = default)
    {
        using FileStream fs = File.OpenRead(profilePath);
        return await JsonSerializer.DeserializeAsync<XenoFxProfile>(
            utf8Json: fs,
            options: XenoFxConstants.HumanReadableJsonOptions,
            cancellationToken: ctoken)
            ?? throw new InvalidDataException("Provided path isn't a valid XenoFx Profile.");
    }

    public static async Task<XenoFxProfile> CreateProfile(string profilePath, CancellationToken ctoken = default)
    {
        XenoFxProfile profile = new();
        using FileStream fs = File.Create(profilePath);
        await JsonSerializer.SerializeAsync(
            utf8Json: fs,
            value: profile,
            options: XenoFxConstants.HumanReadableJsonOptions,
            cancellationToken: ctoken);
        return profile;
    }
}