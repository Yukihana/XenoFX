using CSX.DotNet.Common.IO.Storage.AppProfiles;
using CSX.DotNet.Modules.FFMpeg.Provisioning.Environment.Configuration;
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
        var profile = await ProfileStore.ReadOrCreateAsync<ModuleProfile>(profilePath, ctoken: ctoken);

        return new(profile, options);
    }
}