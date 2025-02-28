using System;
using System.Collections.Generic;

namespace XenoFx.Database;

/// <summary>
/// Grouping service for database connections.
/// </summary>
public sealed partial class DataKeeperService : IDisposable
{
    private readonly Dictionary<string, XenoDbContext> _contexts = [];

    public XenoDbContext ConnectSqlite(string databasePath)
    {
        if (!_contexts.TryGetValue(databasePath, out XenoDbContext? value))
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