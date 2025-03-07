using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Environment;

public static partial class Factory
{
    public async static Task<XenoFxConfiguration> GetDefaultConfiguration(CancellationToken ctoken = default)
    {
        await Task.Yield();
        // Skip actual reading and generate defaults
        // TODO Create the profile though
        XenoFxOptions options = new();
        XenoFxProfile profile = new();
        string startupPath = options.GetProfilePath();
        return new(startupPath, profile, options);
    }
}