using System.Threading;

namespace XenoFx.Services.Utility.Lifetime;

public sealed partial class LifetimeService : ILifetimeService
{
    private readonly CancellationTokenSource _cts = new();

    public CancellationToken ShutdownToken => _cts.Token;

    public void RequestShutdown()
        => _cts.Cancel();
}