using CSX.Common.IO.Paths;
using System.IO;

namespace XenoFx.Environment;

public static partial class ConfigurationExtensions
{
    public static string GetBasePath(this XenoFxConfiguration configuration)
        => Path.GetDirectoryName(configuration.StartupPath)
        ?? Path.Combine(Directory.GetCurrentDirectory(), "DefaultProfile");

    public static string GetDatabasesDirectory(this XenoFxConfiguration configuration) => PathExtensions.ResolveCombine(
            configuration.GetBasePath(),
            configuration.Profile.DatabasesDirectory);

    public static string GetTempDbPath(this XenoFxConfiguration configuration) => PathExtensions.ResolveCombine(
        configuration.GetDatabasesDirectory(),
        configuration.Profile.TempDatabasePath);

    public static string GetAssetsDbPath(this XenoFxConfiguration configuration) => PathExtensions.ResolveCombine(
        configuration.GetDatabasesDirectory(),
        configuration.Profile.AssetsDatabasePath);

    public static string GetAuthDbPath(this XenoFxConfiguration configuration) => PathExtensions.ResolveCombine(
        configuration.GetDatabasesDirectory(),
        configuration.Profile.AuthDatabasePath);
}