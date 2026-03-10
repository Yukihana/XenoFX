namespace CSX.DotNet.Common.EFC.Abstractions;

public interface IDatabaseOptions
{
    string ConnectionStringTemplate { get; }

    bool IsScoped { get; }

    bool IsFactory { get; }

    /*
     * Add as required:
     *
     * ConnectionString = configuration.IndexStoreConnectionString,
     * DatabaseProvider = configuration.IndexStoreDatabaseProvider,
     * EnableSensitiveDataLogging = configuration.EnableSensitiveDataLogging,
     * EnableDetailedErrors = configuration.EnableDetailedErrors,
     * CommandTimeout = configuration.CommandTimeout,
     * MaxRetryCount = configuration.MaxRetryCount,
     * MaxRetryDelay = configuration.MaxRetryDelay,
    */
}