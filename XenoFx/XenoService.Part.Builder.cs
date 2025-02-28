using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.ProfileProvider;

namespace XenoFx;

public partial class XenoService
{
    private ServiceProvider? _serviceProvider = null;

    public ServiceProvider ServiceProvider
        => _serviceProvider
        ?? throw new InvalidOperationException($"{nameof(XenoService)} environment has not been initialized.");

    // Build

    private async Task BuildEnvironment(CancellationToken ctoken = default)
    {
        await Task.Yield();

        // Prebuild tasks
        ProfileProviderService prof = new(_profilePath);

        // Register services
        var services = new ServiceCollection();
        services.AddSingleton<IProfileProviderService>(prof);

        // Build
        ServiceProvider sp = services.BuildServiceProvider();

        // Post build

        // Finish
        _serviceProvider = sp;
    }
}