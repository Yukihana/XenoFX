using CSX.DotNet.Common.Data.Text.Json;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Storage.AppProfiles;

public static class ProfileStore
{
    // Internal

    private static async Task<(FileStream, T)> OpenOrCreateDefaultAsync<T>(
        string path,
        CancellationToken ctoken = default)
        where T : new()
    {
        ctoken.ThrowIfCancellationRequested();

        // Ensure the path is fully qualified
        string fullPath = Path.GetFullPath(path);

        // If path is a file
        if (File.Exists(fullPath))
        {
            FileStream existingStream = new(
                path: fullPath,
                mode: FileMode.Open,
                access: FileAccess.ReadWrite,
                share: FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            T deserialized;
            try
            {
                deserialized = await JsonSerializer.DeserializeAsync<T>(
                    utf8Json: existingStream,
                    cancellationToken: ctoken)

                    ?? throw new JsonException();
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException(
                    $"Failed to deserialize profile at '{fullPath}'. The JSON format is invalid.",
                    ex);
            }

            return (existingStream, deserialized);
        }

        // Abort if the path is a directory
        if (Directory.Exists(fullPath))
            throw new IOException($"A directory already exists at '{fullPath}'.");

        // Ensure parent directory exists
        var dirName = Path.GetDirectoryName(fullPath)
            ?? throw new InvalidDataException("Invalid directory name.");
        Directory.CreateDirectory(dirName);

        // File doesn't exist — create new
        T newInstance = new();
        FileStream? newStream = null;
        try
        {
            newStream = new(
                path: fullPath,
                mode: FileMode.CreateNew,
                access: FileAccess.ReadWrite,
                share: FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            // Write ahead so stream can be closed if no changes are needed
            await JsonSerializer.SerializeAsync(
                utf8Json: newStream,
                value: newInstance,
                options: JsonOptionsUtilities.HumanReadableJsonOptions,
                cancellationToken: ctoken);

            // Flush and return stream without closing
            await newStream.FlushAsync(ctoken);
            return (newStream, newInstance);
        }
        catch
        {
            if (newStream is not null)
                await newStream.DisposeAsync();
            throw;
        }
    }

    // Public API

    public static async Task<T> ReadOrCreateAsync<T>(
        string path,
        Func<T, bool>? configure = null,
        CancellationToken ctoken = default)
        where T : new()
    {
        (FileStream fs, T instance) = await OpenOrCreateDefaultAsync<T>(
            path: path,
            ctoken: ctoken);

        // Ensure stream gets disposed after
        await using FileStream stream = fs;

        // Apply action and update if return is true
        if (configure is not null && configure(instance))
        {
            // Rewind and truncate before writing
            stream.Position = 0;
            stream.SetLength(0);

            // Write the updated content
            await JsonSerializer.SerializeAsync(
                utf8Json: stream,
                value: instance,
                options: JsonOptionsUtilities.HumanReadableJsonOptions,
                cancellationToken: ctoken);

            // Ensure write
            await stream.FlushAsync(ctoken);
        }

        return instance;
    }
}