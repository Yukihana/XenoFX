using CSX.DotNet.Modules.FileIndexing.Environment.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileIndexing.Environment;

public static partial class Factory
{
    public static async Task<IServiceCollection> AddFileIndexingAsync(
        this IServiceCollection services,
        Action<IFileIndexingOptions> configureOptions,          // Per runtime
        Func<IFileIndexingProfile, bool> configurePersistent,   // Persistent
        CancellationToken ctoken = default)
    {
        await Task.Yield(); // Placeholder/Simulate async work

        return services;
    }
}