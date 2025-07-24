using CSX.DotNet.Modules.FileUploader.Environment.Configuration;
using XenoServe.Environment.Configuration;

namespace XenoServe.Environment.Modules;

public class FileUploaderOptions : IFileUploaderOptions
{
    // Defaults

    public const string DefaultDataDirectoryName = "FileUploader";

    // Infrastructure

    private readonly XenoServeConfiguration _config;

    // Lifecycle

    public FileUploaderOptions(
        XenoServeConfiguration config)
    {
        _config = config;
    }

    // Factory

    public static FileUploaderOptions CreateFrom(
        XenoServeConfiguration config)
        => new(config);

    // Module

    public string DataDirectory
        => _config.GetModuleDirectory(DefaultDataDirectoryName);

    // Shared

    public string UploadsDirectory
        => _config.UploadsDirectory;
}