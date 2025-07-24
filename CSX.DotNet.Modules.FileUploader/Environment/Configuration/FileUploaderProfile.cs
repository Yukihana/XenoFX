using System.IO;

namespace CSX.DotNet.Modules.FileUploader.Environment.Configuration;

public class FileUploaderProfile
{
    // Defaults

    public const string DefaultFilename = "fileUploader.json";

    public static string GetFilePath(
        string dataDirectory)
    {
        return Path.Combine(
            dataDirectory,
            DefaultFilename);
    }

    // Filenames

    public string IncompleteFilename { get; set; } = "file.caching";
    public string CompletedFilename { get; set; } = "file.cached";
    public string SidecarFilename { get; set; } = "metadata.json";

    // Parameters

    public int WriteBufferSize { get; internal set; }
}