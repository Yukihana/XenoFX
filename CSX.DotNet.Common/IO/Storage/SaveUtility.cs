using CSX.DotNet.Common.Data.Guids;
using CSX.DotNet.Common.IO.DirectoryUtilities;
using CSX.DotNet.Common.IO.Paths;
using CSX.DotNet.Common.IO.Streams;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Storage;

public static partial class SaveUtility
{
    private static StreamWrapper<FileStream, string> OpenStream(
        string savePath,
        int bufferSize = 81920,
        ConflictResolution resolution = ConflictResolution.None,
        bool useAsync = false,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        string dir = PathExtensions.GetDirectoryOrThrow(savePath);
        Directory.CreateDirectory(dir);

        string baseFileName = Path.GetFileNameWithoutExtension(savePath);
        string extension = Path.GetExtension(savePath);

        FileMode fileMode
            = resolution == ConflictResolution.Overwrite
            ? FileMode.Create
            : FileMode.CreateNew;

        string finalPath = savePath;

        while (true)
        {
            try
            {
                FileStream fileStream = new(
                    path: finalPath,
                    mode: fileMode,
                    access: FileAccess.Write,
                    share: FileShare.None,
                    bufferSize: bufferSize,
                    useAsync: useAsync);

                return new(
                    Stream: fileStream,
                    Attachment: finalPath);
            }
            catch (IOException ex)
            {
                // If not set to rename, fail on exists
                if (ex.IsFileAlreadyExistsError() &&
                    resolution != ConflictResolution.Rename)
                    throw;

                // Guid based rename allows uncapped number of attempts
                finalPath = $"{baseFileName}_{DMC212710Guid.FromUtcNow()}{extension}";
            }
        }
    }

    // Public : Core API

    public static async Task<string> SaveToDiskAsync(
        this Stream stream,
        string savePath,
        ConflictResolution resolution = ConflictResolution.None,
        int bufferSize = 81920,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        if (Directory.Exists(savePath))
            throw new IOException($"A directory already exists at the provided save path: {savePath}");

        var streamWrapper = OpenStream(
            savePath: savePath,
            bufferSize: bufferSize,
            resolution: resolution,
            useAsync: true,
            ctoken: ctoken);

        await using FileStream fileStream = streamWrapper.Stream;

        await stream.CopyToAsync(
            destination: fileStream,
            bufferSize: bufferSize,
            cancellationToken: ctoken)
            .ConfigureAwait(false);

        return streamWrapper.Attachment;
    }

    public static async Task<string> SaveToWorkspaceAsync(
        this Stream stream,
        IWorkspace workspace,
        string relativePath,
        bool overwrite = false,
        int bufferSize = 81920,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(workspace);

        string finalPath = await workspace.ExecuteWithLockAsync(async (rootPath, ct) =>
        {
            var pathInfo = workspace.AnalyzePath(relativePath); // Will throw if not valid path

            if (!pathInfo.IsInsideWorkspace)
                throw new InvalidDataException("Path is not within workspace.");

            string savePath = pathInfo.AbsolutePath;

            return await SaveToDiskAsync(
                stream: stream,
                savePath: savePath,
                resolution: overwrite ? ConflictResolution.Overwrite : ConflictResolution.None,
                bufferSize: bufferSize,
                ctoken: ct);
        }, ctoken);

        return finalPath;
    }

    // Thin wrappers

    public static async Task<string> SaveToUniqueDirectoryAsync(
        this Stream stream,
        string fileName,
        string parentDirectory = "",
        int bufferSize = 81920,
        CancellationToken ctoken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        await using UniqueWorkspace workspace = UniqueWorkspace.Create(
            parentDirectory: parentDirectory,   // working directory if empty
            clearContentsOnDispose: false);     // caller owns cleanup

        return await stream.SaveToWorkspaceAsync(
            workspace: workspace,
            relativePath: fileName,
            bufferSize: bufferSize,
            ctoken: ctoken);
    }
}