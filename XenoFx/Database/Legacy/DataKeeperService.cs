using System;
using System.Collections.Generic;
using XenoFx.Database.AssetsDb;

namespace XenoFx.Database.Legacy;

/// <summary>
/// Grouping service for database connections.
/// </summary>
public sealed partial class DataKeeperService : IDisposable
{
    private readonly Dictionary<string, AssetsDbContext> _contexts = [];

    public AssetsDbContext ConnectSqlite(string databasePath)
    {
        if (!_contexts.TryGetValue(databasePath, out AssetsDbContext? value))
            _contexts[databasePath] = value = XenoDbContextFactory.OpenSqlite(databasePath);
        return value;
    }

    public void DisconnectSqlite(string databasePath)
    {
        if (_contexts.TryGetValue(databasePath, out var context))
        {
            context.Dispose();
            _contexts.Remove(databasePath);
        }
    }

    public void Dispose()
    {
        foreach (var context in _contexts.Values)
        {
            context.Dispose();
        }
        _contexts.Clear();
    }
}