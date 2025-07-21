using CSX.DotNet.Modules.AuthIpPin.Environment.Configuration;
using CSX.DotNet.Modules.AuthIpPin.Middlewares;
using CSX.DotNet.Modules.AuthIpPin.Services.AuthApi;
using CSX.DotNet.Modules.AuthIpPin.Services.AuthDatabase;
using CSX.DotNet.Modules.AuthIpPin.Storage.AuthDb;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace CSX.DotNet.Modules.AuthIpPin.Environment;

public static partial class Factory
{
    // TODO Documentation: Registers the database contexts with the provided IServiceCollection.
    public static IServiceCollection AddAuthIpPinDatabases(
        this IServiceCollection services,
        AuthIpPinConfiguration config,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        services.AddAuthDbContext(config);

        return services;
    }

    public static IServiceCollection AddAuthIpPinServices(
        this IServiceCollection services,
        AuthIpPinConfiguration config,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Configuration

        // Storage
        services.AddSingleton<IAuthDbWorkerService, AuthDbWorkerService>();
        services.AddScoped<IAuthDbScopedService, AuthDbScopedService>();

        // Api
        services.AddSingleton<IAuthApiService, AuthApiService>();

        // Middleware
        services.AddSingleton<AuthIpPinMiddleware>();

        return services;
    }
}