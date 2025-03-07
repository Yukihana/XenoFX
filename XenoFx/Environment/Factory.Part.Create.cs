using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Environment;

public static partial class Factory
{
    // TODO Documentation: Creates a default container for the framework's services.
    public async static Task<IServiceProvider> Create(CancellationToken ctoken = default)
    {
        XenoFxConfiguration configuration = await GetDefaultConfiguration(ctoken);
        return Create(configuration);
    }

    public async static Task<IServiceProvider> Create(string path, bool useCommandLine = false, CancellationToken ctoken = default)
    {
        XenoFxOptions options = new()
        {
            StartupPath = path,
            UseCommandLine = useCommandLine
        };

        return await Create(options, ctoken);
    }

    public async static Task<IServiceProvider> Create(XenoFxOptions options, CancellationToken ctoken = default)
    {
        (XenoFxProfile profile, string startupPath) = await options.GetProfile(ctoken);

        return Create(startupPath, profile, options);
    }

    private static IServiceProvider Create(string startupPath, XenoFxProfile profile, XenoFxOptions options)
    {
        XenoFxConfiguration configuration = new(startupPath, profile, options);

        return Create(configuration);
    }

    public static IServiceProvider Create(XenoFxConfiguration configuration)
    {
        ServiceCollection services = new();
        services.AddXenoFx(configuration);

        ServiceProvider sp = services.BuildServiceProvider();
        return sp;
    }
}