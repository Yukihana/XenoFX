using CSX.DotNet.Common.IO.Storage.AppProfiles;
using CSX.DotNet.Modules.FileUploader.Environment.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileUploader.Environment;

public static partial class Factory
{
    public static async Task<FileUploaderConfiguration> GetConfigAsync(
        this IFileUploaderOptions options,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        var profilePath = FileUploaderProfile.GetFilePath(options.DataDirectory);
        var profile = await ProfileStore.ReadOrCreateAsync<FileUploaderProfile>(profilePath, ctoken: ctoken);

        return new(profile, options);
    }
}