using CSX.DotNet.Common.Data.Guids;
using CSX.DotNet.Common.EFC.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace CSX.DotNet.Common.EFC.Extensions;

public static partial class RegistrationExtensions
{
    /// <summary>
    /// Builds the actual connection string from the template, replacing placeholders with runtime parameters.
    /// </summary>
    public static string BuildConnectionString(
        IDatabaseConfiguration config,
        Func<Dictionary<string, string>>? getTemplateParameters = null)
    {
        if (string.IsNullOrWhiteSpace(config.ConnectionStringTemplate))
            throw new InvalidOperationException("ConnectionStringTemplate cannot be null or empty.");

        var template = config.ConnectionStringTemplate;

        if (getTemplateParameters != null)
        {
            var parameters = getTemplateParameters();
            foreach (var kv in parameters)
            {
                template = template.Replace("{" + kv.Key + "}", kv.Value);
            }
        }

        return template;
    }

    private static Action<DbContextOptionsBuilder> GetOptionsAction<T>(
        IDatabaseConfiguration config,
        Func<Dictionary<string, string>>? getTemplateParameters)
        where T : DbContext
    {
        return config.ProviderType switch
        {
            DatabaseProviderType.Sqlite => options =>
            {
                // Commented line is not longer valid
                // Reason: config.ConnectionStringTemplate is now the full template.
                // var connStr = $"Data Source={config.ConnectionStringTemplate};Cache=Shared;";
                // Remove this comments section after full integration.
                var connStr = BuildConnectionString(config, getTemplateParameters);
                options.UseSqlite(connStr);
            }
            ,
            DatabaseProviderType.InMemory => options =>
            {
                var storeName = string.IsNullOrWhiteSpace(config.ConnectionStringTemplate)
                    ? DMC212710Guid.FromUtcNow().ToString("N")
                    : config.ConnectionStringTemplate;
                options.UseInMemoryDatabase(storeName);
            }
            ,
            _ => throw new InvalidOperationException($"Unsupported database type: {config.ProviderType}")
        };
    }

    public static IServiceCollection AddStandardDbContext<T>(
        this IServiceCollection services,
        IDatabaseConfiguration config,
        Func<Dictionary<string, string>>? getTemplateParameters = null)
        where T : DbContext
    {
        // Handle provider specific options
        Action<DbContextOptionsBuilder> optionsAction
            = GetOptionsAction<T>(config, getTemplateParameters);

        // Handle registration for scoped and factory
        if (config.IsScoped)
        {
            // Prevent options DI lifetime conflicts:
            // Use Singleton if both scoped and factory are used
            var optionsLifetime
                = config.IsFactory
                ? ServiceLifetime.Singleton
                : ServiceLifetime.Scoped;

            services.AddDbContext<T>(optionsAction,
                optionsLifetime: optionsLifetime);
        }
        if (config.IsFactory)
        {
            services.AddDbContextFactory<T>(optionsAction);
        }

        return services;
    }
}