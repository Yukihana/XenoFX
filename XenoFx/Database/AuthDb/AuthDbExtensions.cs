using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment;

namespace XenoFx.Database.AuthDb;

public static class AuthDbExtensions
{
    public static IServiceCollection AddAuthDbContextUsingSqlite(
        this IServiceCollection services,
        XenoFxConfiguration xfc)
    {
        string dbpath = xfc.GetAuthDbPath();
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

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        // Ensures the database is created and ready for consumption.
        // dbContext.Database.EnsureCreated();

        // If using migrations instead, use this to update the database.
        await context.Database.MigrateAsync(cancellationToken: ctoken);
    }
}