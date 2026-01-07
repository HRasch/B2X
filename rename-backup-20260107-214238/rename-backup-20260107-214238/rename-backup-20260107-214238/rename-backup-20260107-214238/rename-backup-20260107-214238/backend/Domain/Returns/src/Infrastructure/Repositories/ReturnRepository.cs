using B2Connect.Returns.Core.Entities;
using B2Connect.Returns.Core.Interfaces;
using B2Connect.Returns.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Returns.Infrastructure.Repositories;

/// <summary>
/// EF Core-basierte Repository-Implementierung für Return-Entität
/// 
/// Verantwortung:
/// - Datenbank-Zugriff (CRUD)
/// - Multi-tenant Isolation auf allen Abfragen
/// - Lazy Loading Prevention (Select für Projektion)
/// - Async Operations mit CancellationToken
/// 
/// Konfiguration:
/// - DbContext: ReturnsDbContext (injiziert)
/// - Change Tracking: AutoDetectChangesEnabled für Updates
/// - Timestamps: UpdatedAt wird automatisch gesetzt (via DbContext)
/// </summary>
public class ReturnRepository : IReturnRepository
{
    private readonly ReturnsDbContext _dbContext;
    private readonly ILogger<ReturnRepository> _logger;

    public ReturnRepository(ReturnsDbContext dbContext, ILogger<ReturnRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Erstelle einen neuen Return
    /// </summary>
    public async Task AddAsync(Return returnEntity, CancellationToken cancellationToken = default)
    {
        if (returnEntity == null)
        {
            throw new ArgumentNullException(nameof(returnEntity));
        }

        _logger.LogInformation(
            "[RETURN] Adding new return for tenant {TenantId}, order {OrderId}",
            returnEntity.TenantId, returnEntity.OrderId);

        _dbContext.Returns.Add(returnEntity);
        await SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Hole einen Return nach ID (mit Tenant-Filter)
    /// </summary>
    public async Task<Return?> GetByIdAsync(Guid tenantId, Guid returnId, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Returns
            .Where(r => r.TenantId == tenantId && r.Id == returnId)
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null)
        {
            _logger.LogWarning(
                "[RETURN] Return not found: tenant {TenantId}, return {ReturnId}",
                tenantId, returnId);
        }

        return result;
    }

    /// <summary>
    /// Hole alle Returns einer Bestellung
    /// </summary>
    public async Task<List<Return>> GetByOrderIdAsync(Guid tenantId, Guid orderId, CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.Returns
            .Where(r => r.TenantId == tenantId && r.OrderId == orderId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "[RETURN] Found {Count} returns for tenant {TenantId}, order {OrderId}",
            results.Count, tenantId, orderId);

        return results;
    }

    /// <summary>
    /// Hole Returns eines Kunden mit Paginierung
    /// </summary>
    public async Task<List<Return>> GetByCustomerIdAsync(Guid tenantId, Guid customerId, int limit = 100, int offset = 0, CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.Returns
            .Where(r => r.TenantId == tenantId && r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "[RETURN] Found {Count} returns for tenant {TenantId}, customer {CustomerId} (offset: {Offset}, limit: {Limit})",
            results.Count, tenantId, customerId, offset, limit);

        return results;
    }

    /// <summary>
    /// Hole Returns mit bestimmtem Status
    /// </summary>
    public async Task<List<Return>> GetByStatusAsync(Guid tenantId, ReturnStatus status, int limit = 100, int offset = 0, CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.Returns
            .Where(r => r.TenantId == tenantId && r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "[RETURN] Found {Count} returns for tenant {TenantId} with status {Status}",
            results.Count, tenantId, status);

        return results;
    }

    /// <summary>
    /// Hole abgelaufene Returns (außerhalb der 14-Tage-Frist)
    /// 
    /// Wichtig: IsWithinDeadline ist eine Generated Column in der DB
    /// Diese Abfrage nutzt die Spalte direkt (keine Berechnung in C#)
    /// </summary>
    public async Task<List<Return>> GetExpiredReturnsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.Returns
            .Where(r => r.TenantId == tenantId && !r.IsWithinDeadline)
            .OrderByDescending(r => r.ReturnDeadline)
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "[RETURN] Found {Count} expired returns for tenant {TenantId}",
            results.Count, tenantId);

        return results;
    }

    /// <summary>
    /// Aktualisiere einen existierenden Return
    /// </summary>
    public async Task UpdateAsync(Return returnEntity, CancellationToken cancellationToken = default)
    {
        if (returnEntity == null)
        {
            throw new ArgumentNullException(nameof(returnEntity));
        }

        _logger.LogInformation(
            "[RETURN] Updating return {ReturnId} for tenant {TenantId}",
            returnEntity.Id, returnEntity.TenantId);

        // Setze UpdatedAt auf jetzt
        returnEntity.UpdatedAt = DateTime.UtcNow;

        _dbContext.Returns.Update(returnEntity);
        await SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Speichere alle Änderungen
    /// </summary>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("[RETURN] Changes saved to database");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "[RETURN] Database update error");
            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[RETURN] Save operation was cancelled");
            throw;
        }
    }

    /// <summary>
    /// Zähle Returns mit optionalem Status-Filter
    /// </summary>
    public async Task<int> CountAsync(Guid tenantId, ReturnStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Returns.Where(r => r.TenantId == tenantId);

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        var count = await query.CountAsync(cancellationToken);
        _logger.LogDebug("[RETURN] Count for tenant {TenantId}: {Count}", tenantId, count);

        return count;
    }

    /// <summary>
    /// Hole das abfrage-Interface für komplexe Filter
    /// 
    /// Beispiele:
    /// - var query = repo.Query(tenantId).Where(...).OrderBy(...)
    /// - var results = await query.ToListAsync()
    /// </summary>
    public IQueryable<Return> Query(Guid tenantId)
    {
        return _dbContext.Returns.Where(r => r.TenantId == tenantId);
    }
}
