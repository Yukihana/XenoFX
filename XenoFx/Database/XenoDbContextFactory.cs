using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using XenoFx.Database.Extensions;

namespace XenoFx.Database;

internal static class XenoDbContextFactory
{
    public static XenoDbContext OpenSqlite(this SqliteConnectionStringBuilder builder)
    {
        string connectionString = builder.ConnectionString;

        DbContextOptionsBuilder<XenoDbContext> optionsBuilder = new();

        DbContextOptions<XenoDbContext> options = optionsBuilder
            .UseSqlite(connectionString)
            .Options;

        XenoDbContext context = new(options);

        context.Database.EnsureCreated(); // TODO: Replace with Migrate in ver:Alpha

        return context;
    }

    public static XenoDbContext OpenSqlite(string databasePath)
    {
        var builder = databasePath.GetSqliteConnectionBuilder();
        return builder.OpenSqlite();
    }
}