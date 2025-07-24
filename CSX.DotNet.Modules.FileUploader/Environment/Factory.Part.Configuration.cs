using CSX.DotNet.Modules.FileUploader.Environment.Configuration;
using CSX.DotNet.Storage.AppProfiles;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Environment;

public static partial class Factory
{
    public static async Task<FileUploaderConfiguration> GetConfigAsync(
        this IFileUploaderOptions options,
        CancellationToken ctoken)
    {
        ctoken.ThrowIfCancellationRequested();

        var profilePath = FileUploaderProfile.GetFilePath(options.DataDirectory);
        var profile = await ProfileStore.ReadOrCreateAsync<FileUploaderProfile>(profilePath, ctoken);

        return new(profile, options);
    }
}