using CSX.DotNet.Common.EFC.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment.Configuration;

namespace XenoFx.Database.AssetsDb;

public static class AssetsDbExtensions
{
    public static IServiceCollection AddAssetsDbContext(
        this IServiceCollection services,
        XenoFxConfiguration config)
    {
        if (config.AssetsDatabaseType.Equals("sqlite", StringComparison.OrdinalIgnoreCase))
            return services.AddAssetsDbContextUsingSqlite(config);

        throw new InvalidOperationException("Unsupported database type");
    }

    private static IServiceCollection AddAssetsDbContextUsingSqlite(
        this IServiceCollection services,
        XenoFxConfiguration config)
    {
        string dbpath = config.AssetsDatabasePath;
        string connectionString = $"Data Source={dbpath};Cache=Shared;";
        services.AddDbContext<AssetsDbContext>(
            options => options.UseSqlite(connectionString),
            optionsLifetime: ServiceLifetime.Singleton);
        services.AddDbContextFactory<AssetsDbContext>(
            options => options.UseSqlite(connectionString));
        return services;
    }

    public static async Task InitializeAssetsDbContextAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AssetsDbContext>();

        // Ensure pre-migration
        dbContext.EnsurePreMigration();

        // Ensures the database is created and ready for consumption.
        // dbContext.Database.EnsureCreated();

        // If using migrations instead, use this to update the database.
        await dbContext.Database.MigrateAsync(cancellationToken: ctoken);
    }
}