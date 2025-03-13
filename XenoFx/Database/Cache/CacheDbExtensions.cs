using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using XenoFx.Environment;

namespace XenoFx.Database.Cache;

public static partial class CacheDbExtensions
{
    public static IServiceCollection AddCacheDbContextUsingSqlite(this IServiceCollection services, XenoFxConfiguration xfc)
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
}