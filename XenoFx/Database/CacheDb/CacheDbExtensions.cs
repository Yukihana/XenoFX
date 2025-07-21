using CSX.DotNet.EFC.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment.Configuration;

namespace XenoFx.Database.CacheDb;

public static partial class CacheDbExtensions
{
    public static IServiceCollection AddCacheDbContext(
        this IServiceCollection services,
        XenoFxConfiguration config)
    {
        if (config.CacheDatabaseType.Equals("sqlite", StringComparison.OrdinalIgnoreCase))
            return services.AddCacheDbContextUsingSqlite(config);

        throw new InvalidOperationException("Unsupported database type");
    }

    private static IServiceCollection AddCacheDbContextUsingSqlite(
        this IServiceCollection services,
        XenoFxConfiguration config)
    {
        string dbpath = config.CacheDatabasePath;
        string connectionString = $"Data Source={dbpath};Cache=Shared;";
        services.AddDbContext<CacheDbContext>(
            options => options.UseSqlite(connectionString),
            optionsLifetime: ServiceLifetime.Singleton);
        services.AddDbContextFactory<CacheDbContext>(
            options => options.UseSqlite(connectionString));
        return services;
    }

    public static async Task InitializeCacheDbContextAsync(
        this IServiceProvider serviceProvider,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CacheDbContext>();

        // Ensure pre-migration
        dbContext.EnsurePreMigration();

        // Ensures the database is created and ready for consumption.
        // dbContext.Database.EnsureCreated();

        // If using migrations instead, use this to update the database.
        await dbContext.Database.MigrateAsync(cancellationToken: ctoken);
    }
}