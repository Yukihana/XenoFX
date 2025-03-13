using CSX.Common.IO.Paths;
using System.IO;

namespace XenoFx.Environment;

public static partial class ConfigurationExtensions
{
    public static string GetBasePath(this XenoFxConfiguration configuration)
        => Path.GetDirectoryName(configuration.StartupPath)
        ?? Directory.GetCurrentDirectory();

    public static string GetTempDbPath(this XenoFxConfiguration configuration) => PathExtensions.ResolveCombine(
        configuration.GetBasePath(),
        configuration.Profile.TempDatabasePath);
}