using CSX.DotNet.Modules.FFMpeg.Provisioning.Environment.Configuration;
using CSX.DotNet.Storage.AppProfiles;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Environment;

public static partial class Factory
{
    public static async Task<ModuleConfiguration> GetConfigAsync(
        this IFFMpegProvisioningOptions options,
        CancellationToken ctoken)
    {
        ctoken.ThrowIfCancellationRequested();

        var profilePath = ModuleProfile.GetFilePath(options.DataDirectory);
        var profile = await ProfileStore.ReadOrCreateAsync<ModuleProfile>(profilePath, ctoken);

        return new(profile, options);
    }
}