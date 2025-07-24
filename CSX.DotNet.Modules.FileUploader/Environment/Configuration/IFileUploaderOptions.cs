namespace CSX.DotNet.Modules.FileUploader.Environment.Configuration;

public interface IFileUploaderOptions
{
    string DataDirectory { get; }
    string UploadsDirectory { get; }
}