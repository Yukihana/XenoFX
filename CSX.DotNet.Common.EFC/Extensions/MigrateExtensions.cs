using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace CSX.DotNet.Common.EFC.Extensions;

public static class MigrateExtensions
{
    public static void EnsurePreMigration(
        this DbContext dbContext)
    {
        // Sqlite
        if (dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            // Try to get the storage location from the DbContext
            var connection = dbContext.Database.GetDbConnection();
            var dataSource = connection.DataSource;

            if (string.IsNullOrWhiteSpace(dataSource))
                throw new IOException($"Invalid SQLite database path: {dataSource}");

            string fullPath = Path.IsPathFullyQualified(dataSource)
                ? dataSource
                : Path.GetFullPath(dataSource, AppContext.BaseDirectory);

            // Throw if the path is a directory (auto-correction can be exploited)
            if (Directory.Exists(fullPath))
                throw new IOException($"There's an existing directory at the database path: '{fullPath}'");

            // Resolve and create the database directory if it doesn't exist
            string? dir = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(dir))
                throw new IOException($"Could not resolve a directory from database path: '{fullPath}'");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}