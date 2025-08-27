using CSX.DotNet.Common.IO.Streams;
using System;
using System.IO;

namespace CSX.DotNet.Common.IO.Generators;

public static class FileGenerator
{
    public static IStreamWithAttachment<FileStream, string> CreateUniqueFile(
        string parentDirectory,
        int bufferSize = 1024,
        string prefix = "",
        string suffix = ".bin")
    {
        while (true)
        {
            string fileName = $"{prefix}{Guid.NewGuid()}{suffix}";
            string path = Path.Combine(parentDirectory, fileName);

            try
            {
                FileStream stream = new(
                    path: path,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: bufferSize,
                    useAsync: true);

                return new StreamWrapper<FileStream, string>(stream, path);
            }
            catch (IOException ex) when (ex.IsFileAlreadyExistsError())
            { } // Retry
        }
    }
}