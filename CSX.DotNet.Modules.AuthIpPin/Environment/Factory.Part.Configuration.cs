using CSX.DotNet.Common.IO.Storage.AppProfiles;
using CSX.DotNet.Modules.AuthIpPin.Environment.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.AuthIpPin.Environment;

public static partial class Factory
{
    public static async Task<AuthIpPinConfiguration> GetConfigAsync(
        this IAuthIpPinOptions options,
        CancellationToken ctoken)
    {
        ctoken.ThrowIfCancellationRequested();

        var profilePath = AuthIpPinProfile.GetFilePath(options.DataDirectory);
        var profile = await ProfileStore.ReadOrCreateAsync<AuthIpPinProfile>(profilePath, ctoken: ctoken);

        return new(profile, options);
    }
}