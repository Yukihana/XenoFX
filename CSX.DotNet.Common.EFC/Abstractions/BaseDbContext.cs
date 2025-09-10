using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.EFC.Abstractions;

public abstract partial class BaseDbContext<T> : DbContext
{
    // Infrastructure

    protected readonly ILogger<T>? _logger = null;

    // Lifecycle

    public BaseDbContext(
        DbContextOptions options,
        ILogger<T> logger)
        : base(options)
    {
        _logger = logger;
    }

    // Save

    public override int SaveChanges(
        bool acceptAllChangesOnSuccess)
    {
        ApplyAuditRules();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditRules();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    // Rules

    private void ApplyAuditRules()
    {
        var now = DateTimeOffset.UtcNow;
        string user = string.Empty; // Users to be implemented later
        HashSet<string> nonAuditableTypes = [];

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditable auditable)
            {
                _ = nonAuditableTypes.Add(entry.Entity.GetType().Name);
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    auditable.RecordCreatedAt = now;
                    auditable.RecordCreatedBy = user;

                    auditable.IsRecordDeleted = false; // Ensure consistency
                    break;

                case EntityState.Modified:
                    auditable.RecordUpdatedAt = now;
                    auditable.RecordUpdatedBy = user;

                    // Log inconsistency
                    if (!string.IsNullOrEmpty(auditable.RecordCreatedBy) &&
                        auditable.RecordCreatedBy != user &&
                        _logger != null)
                    {
                        _logger.LogWarning(
                            "Entity {Entity} created by {CreatedBy} is being updated by {UpdatedBy} at {Time}",
                            entry.Entity.GetType().Name,
                            auditable.RecordCreatedBy,
                            user,
                            now
                        );
                    }
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified; // Bypass actual deletion
                    auditable.IsRecordDeleted = true;   // Soft delete

                    auditable.RecordDeletedAt = now;
                    auditable.RecordDeletedBy = user;

                    // Log inconsistency
                    if (!string.IsNullOrEmpty(auditable.RecordCreatedBy) &&
                        auditable.RecordCreatedBy != user &&
                        _logger != null)
                    {
                        _logger.LogWarning(
                            "Entity {Entity} created by {CreatedBy} is being deleted by {DeletedBy} at {Time}",
                            entry.Entity.GetType().Name,
                            auditable.RecordCreatedBy,
                            user,
                            now
                        );
                    }

                    break;
            }
        }

        if (nonAuditableTypes.Count != 0 &&
            _logger != null)
            _logger.LogWarning("Non-auditable types detected: {@types}", nonAuditableTypes);
    }

    // Load and Filter

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplyGlobalSoftDeleteFilter(modelBuilder);
    }

    private static void ApplyGlobalSoftDeleteFilter(
        ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IAuditable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var isDeletedProperty = Expression.Property(parameter, nameof(IAuditable.IsRecordDeleted));
                var filter = Expression.Lambda(
                    Expression.Equal(isDeletedProperty, Expression.Constant(false)),
                    parameter);

                entityType.SetQueryFilter(filter);
            }
        }
    }
}