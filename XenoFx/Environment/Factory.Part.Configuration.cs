using CSX.DotNet.Common.IO.Storage.AppProfiles;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment.Configuration;

namespace XenoFx.Environment;

public static partial class Factory
{
    public static async Task<XenoFxConfiguration> GetConfigAsync(
        this IXenoFxOptions options,
        CancellationToken ctoken)
    {
        ctoken.ThrowIfCancellationRequested();

        var profilePath = XenoFxProfile.GetFilePath(options.DataDirectory);
        var profile = await ProfileStore.ReadOrCreateAsync<XenoFxProfile>(profilePath, ctoken: ctoken);

        return new(profile, options);
    }
}