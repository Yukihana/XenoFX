using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using XenoFx.Database.AssetsDb;
using XenoFx.Database.Legacy.Extensions;

namespace XenoFx.Database.Legacy;

internal static class XenoDbContextFactory
{
    public static AssetsDbContext OpenSqlite(this SqliteConnectionStringBuilder builder)
    {
        string connectionString = builder.ConnectionString;

        DbContextOptionsBuilder<AssetsDbContext> optionsBuilder = new();

        DbContextOptions<AssetsDbContext> options = optionsBuilder
            .UseSqlite(connectionString)
            .Options;

        AssetsDbContext context = new(options);

        context.Database.EnsureCreated(); // TODO: Replace with Migrate in ver:Alpha

        return context;
    }

    public static AssetsDbContext OpenSqlite(string databasePath)
    {
        var builder = databasePath.GetSqliteConnectionBuilder();
        return builder.OpenSqlite();
    }
}