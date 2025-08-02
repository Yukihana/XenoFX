using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.EFC.Common.Abstractions;

public abstract partial class BaseDbContext : DbContext
{
    public BaseDbContext(DbContextOptions options) : base(options)
    { }

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

    private void ApplyAuditRules()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.RecordCreatedAt = now;
                entry.Entity.RecordUpdatedAt = now;
                entry.Entity.IsRecordDeleted = false; // Ensure consistency
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.RecordUpdatedAt = now;
            }
            else if (entry.State == EntityState.Deleted)
            {
                // Soft delete logic
                entry.State = EntityState.Modified;
                entry.Entity.IsRecordDeleted = true;
                entry.Entity.RecordDeletedAt = now;
                entry.Entity.RecordUpdatedAt = now;
            }
        }
    }

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