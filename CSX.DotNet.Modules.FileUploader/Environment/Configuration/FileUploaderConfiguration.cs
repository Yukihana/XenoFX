namespace CSX.DotNet.Modules.FileUploader.Environment.Configuration;

public class FileUploaderConfiguration
{
    // Infrastructure

    private readonly FileUploaderProfile _profile;
    private readonly IFileUploaderOptions _options;

    // Lifecycle

    public FileUploaderConfiguration(
        FileUploaderProfile profile,
        IFileUploaderOptions options)
    {
        _profile = profile;
        _options = options;
    }

    // Profile; Not needed but keep anyway

    public string ProfilePath
        => FileUploaderProfile.GetFilePath(_options.DataDirectory);

    // Directories

    public string UploadsDirectory
        => _options.UploadsDirectory;

    // Filenames

    public string IncompleteFilename
        => _profile.IncompleteFilename;

    public string CompletedFilename
        => _profile.CompletedFilename;

    public string SidecarFilename
        => _profile.SidecarFilename;

    // Parameters

    public int WriteBufferSize
        => _profile.WriteBufferSize;
}