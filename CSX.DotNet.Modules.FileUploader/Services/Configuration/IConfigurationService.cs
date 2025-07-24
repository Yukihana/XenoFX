namespace CSX.DotNet.Modules.FileUploader.Services.Configuration;

public interface IConfigurationService
{
    // Directories

    string UploadsDirectory { get; }

    // Filenames

    string IncompleteFilename { get; }
    string CompletedFilename { get; }
    string SidecarFilename { get; }

    // Parameters

    int WriteBufferSize { get; }
}