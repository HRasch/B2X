using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using B2Connect.Core.Interfaces;

namespace B2Connect.Data.Interceptors;

/// <summary>
/// EF Core interceptor for automatic audit logging
/// Automatically sets CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, DeletedAt, DeletedBy
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<AuditInterceptor> _logger;
    private readonly string _currentUserId;

    public AuditInterceptor(ILogger<AuditInterceptor> logger, string? currentUserId = null)
    {
        _logger = logger;
        _currentUserId = currentUserId ?? "System";
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var now = DateTime.UtcNow;

        // Process all changed entries
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditableEntity auditableEntity)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    auditableEntity.CreatedAt = now;
                    auditableEntity.CreatedBy = _currentUserId;
                    _logger.LogInformation(
                        "📝 Entity {EntityType} created by {UserId}",
                        entry.Entity.GetType().Name,
                        _currentUserId);
                    break;

                case EntityState.Modified:
                    // Only update ModifiedAt/ModifiedBy if the entity has actually changed
                    var hasChanges = entry.Properties.Any(p => p.IsModified && p.Metadata.Name != nameof(IAuditableEntity.ModifiedAt) && p.Metadata.Name != nameof(IAuditableEntity.ModifiedBy));

                    if (hasChanges)
                    {
                        auditableEntity.ModifiedAt = now;
                        auditableEntity.ModifiedBy = _currentUserId;
                        _logger.LogInformation(
                            "✏️ Entity {EntityType} modified by {UserId}",
                            entry.Entity.GetType().Name,
                            _currentUserId);
                    }
                    break;

                case EntityState.Deleted:
                    // Soft delete instead of hard delete
                    entry.State = EntityState.Modified;
                    auditableEntity.IsDeleted = true;
                    auditableEntity.DeletedAt = now;
                    auditableEntity.DeletedBy = _currentUserId;
                    _logger.LogWarning(
                        "🗑️ Entity {EntityType} soft-deleted by {UserId}",
                        entry.Entity.GetType().Name,
                        _currentUserId);
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null)
            return base.SavingChanges(eventData, result);

        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditableEntity auditableEntity)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    auditableEntity.CreatedAt = now;
                    auditableEntity.CreatedBy = _currentUserId;
                    _logger.LogInformation(
                        "📝 Entity {EntityType} created by {UserId}",
                        entry.Entity.GetType().Name,
                        _currentUserId);
                    break;

                case EntityState.Modified:
                    var hasChanges = entry.Properties.Any(p => p.IsModified && p.Metadata.Name != nameof(IAuditableEntity.ModifiedAt) && p.Metadata.Name != nameof(IAuditableEntity.ModifiedBy));

                    if (hasChanges)
                    {
                        auditableEntity.ModifiedAt = now;
                        auditableEntity.ModifiedBy = _currentUserId;
                        _logger.LogInformation(
                            "✏️ Entity {EntityType} modified by {UserId}",
                            entry.Entity.GetType().Name,
                            _currentUserId);
                    }
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    auditableEntity.IsDeleted = true;
                    auditableEntity.DeletedAt = now;
                    auditableEntity.DeletedBy = _currentUserId;
                    _logger.LogWarning(
                        "🗑️ Entity {EntityType} soft-deleted by {UserId}",
                        entry.Entity.GetType().Name,
                        _currentUserId);
                    break;
            }
        }

        return base.SavingChanges(eventData, result);
    }
}
