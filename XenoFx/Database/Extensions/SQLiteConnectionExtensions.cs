using Microsoft.Data.Sqlite;
using System;

namespace XenoFx.Database.Extensions;

public static partial class SQLiteConnectionExtensions
{
    /// <summary>
    /// Opens an SQLite connection. Creates the database file if it doesn't exist.
    /// </summary>
    /// <param name="databasePath">Path to the sqlite file.</param>
    /// <param name="cacheSize">Size of the in-memory cache</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static SqliteConnectionStringBuilder GetSqliteConnectionBuilder(this string databasePath, int cacheSize = -16384)
    {
        if (string.IsNullOrWhiteSpace(databasePath))
            throw new ArgumentException("Database path cannot be null or empty.", nameof(databasePath));

        return new()
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
            ["Cache Size"] = cacheSize,
            ["Foreign Keys"] = "ON",
            ["Journal Mode"] = "WAL",
            ["Synchronous"] = "Normal",
        };
    }
}