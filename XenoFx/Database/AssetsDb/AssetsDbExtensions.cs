using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment;

namespace XenoFx.Database.AssetsDb;

public static class AssetsDbExtensions
{
    public static IServiceCollection AddAssetsDbContextUsingSqlite(this IServiceCollection services, XenoFxConfiguration xfc)
    {
        string dbpath = xfc.GetAssetsDbPath();
        string connectionString = $"Data Source={dbpath};Cache=Shared;";
        services.AddDbContext<AssetsDbContext>(
            options => options.UseSqlite(connectionString),
            optionsLifetime: ServiceLifetime.Singleton);
        services.AddDbContextFactory<AssetsDbContext>(
            options => options.UseSqlite(connectionString));

        return services;
    }

    public static async Task InitializeAssetsDbContextAsync(this IServiceProvider serviceProvider, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AssetsDbContext>();

        // Ensures the database is created and ready for consumption.
        // dbContext.Database.EnsureCreated();

        // If using migrations instead, use this to update the database.
        await context.Database.MigrateAsync(cancellationToken: ctoken);
    }
}