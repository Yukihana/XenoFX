using CSX.DotNet.Common.EFC.Extensions;
using CSX.DotNet.Modules.AuthIpPin.Environment.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.AuthIpPin.Storage.AuthDb;

public static class AuthDbExtensions
{
    public static IServiceCollection AddAuthDbContext(
        this IServiceCollection services,
        AuthIpPinConfiguration config)
    {
        if (config.AuthDatabaseType.Equals("sqlite", StringComparison.OrdinalIgnoreCase))
            return services.AddAuthDbContextUsingSqlite(config);

        throw new InvalidOperationException("Unsupported database type");
    }

    private static IServiceCollection AddAuthDbContextUsingSqlite(
        this IServiceCollection services,
        AuthIpPinConfiguration config)
    {
        string dbpath = config.AuthDatabasePath;
        string connectionString = $"Data Source={dbpath};Cache=Shared;";

        services.AddDbContext<AuthDbContext>(
            options => options.UseSqlite(connectionString),
            optionsLifetime: ServiceLifetime.Singleton);
        services.AddDbContextFactory<AuthDbContext>(
            options => options.UseSqlite(connectionString));

        return services;
    }

    public static async Task InitializeAuthDbContextAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        // Ensure pre-migration
        dbContext.EnsurePreMigration();

        // Ensures the database is created and ready for consumption.
        // dbContext.Database.EnsureCreated();

        // If using migrations instead, use this to update the database.
        await dbContext.Database.MigrateAsync(cancellationToken: ctoken);
    }
}