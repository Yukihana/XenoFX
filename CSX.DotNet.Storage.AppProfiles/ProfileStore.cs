using CSX.DotNet.Common.Data.Text.Json;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Storage.AppProfiles;

public static class ProfileStore
{
    public static async Task<T> ReadOrCreateAsync<T>(
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
            using var file = File.OpenRead(fullPath);
            var profile = await JsonSerializer.DeserializeAsync<T>(
                utf8Json: file,
                cancellationToken: ctoken);

            return profile ?? throw new InvalidDataException(
                $"Failed to deserialize profile at '{fullPath}'. The format is invalid.");
        }

        // Abort if the path is a directory
        if (Directory.Exists(fullPath))
            throw new IOException($"A directory already exists at '{fullPath}'.");

        // Ensure parent directory exists
        var dirName = Path.GetDirectoryName(fullPath)
            ?? throw new InvalidDataException("Invalid directory name.");
        Directory.CreateDirectory(dirName);

        // File doesn't exist — create a new profile
        T newProfile = new();
        using (var fileStream = File.Create(fullPath))
        {
            await JsonSerializer.SerializeAsync(
                utf8Json: fileStream,
                value: newProfile,
                options: JsonOptionsUtilities.HumanReadableJsonOptions,
                cancellationToken: ctoken);
        }
        return newProfile;
    }
}