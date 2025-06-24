using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment;

namespace XenoFx.Database.CacheDb;

public static partial class CacheDbExtensions
{
    public static IServiceCollection AddCacheDbContextUsingSqlite(
        this IServiceCollection services,
        XenoFxConfiguration xfc)
    {
        string dbpath = xfc.GetTempDbPath();
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
        var context = scope.ServiceProvider.GetRequiredService<CacheDbContext>();

        // Ensures the database is created and ready for consumption.
        // dbContext.Database.EnsureCreated();

        // If using migrations instead, use this to update the database.
        await context.Database.MigrateAsync(cancellationToken: ctoken);
    }
}