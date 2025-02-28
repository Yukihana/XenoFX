using System;
using System.Threading;

namespace XenoFx.Services.ProfileProvider;

public partial class ProfileProviderService(string profilePath) : IProfileProviderService
{
    // Mutex

    private readonly Mutex _mutex = GetMutex(profilePath);

    private static Mutex GetMutex(string profilePath)
    {
        string identifier = $"XenoDB_Profile-{profilePath}".Replace(":", "_").Replace('/', '_');
        Mutex mutex = new(true, $"Global\\{identifier}");
        if (!mutex.WaitOne(0))
            throw new InvalidOperationException("Unable to proceed. This profile is currently active in a separate service instance.");
        return mutex;
    }

    public void Dispose()
    {
        _mutex.Dispose();
        GC.SuppressFinalize(this);
    }

    // Api
}