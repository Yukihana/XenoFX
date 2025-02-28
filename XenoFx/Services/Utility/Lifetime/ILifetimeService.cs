using System.Threading;

namespace XenoFx.Services.Utility.Lifetime;

internal interface ILifetimeService
{
    CancellationToken ShutdownToken { get; }

    void RequestShutdown();
}