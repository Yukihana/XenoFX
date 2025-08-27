using System.IO;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Environment.Configuration;

public class ModuleProfile
{
    // Defaults

    public const string DefaultFilename = "ffmpegProvisioning.json";

    public static string GetFilePath(string dataDirectory) => Path.Combine(
        dataDirectory,
        DefaultFilename);

    // Parameters
}