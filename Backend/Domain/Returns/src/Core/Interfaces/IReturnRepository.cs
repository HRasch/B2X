using B2Connect.Returns.Core.Entities;

namespace B2Connect.Returns.Core.Interfaces;

/// <summary>
/// Repository interface für Return-Entität (Withdrawal Rights)
/// 
/// Standardisiertes CRUD-Interface mit:
/// - Multi-tenant isolation: Alle Methoden filtern nach TenantId
/// - Async/await: Alle Methoden sind async
/// - CancellationToken: Alle Operationen können abgebrochen werden
/// - LINQ-basiert: Verwendung von IQueryable für komplexe Filter
/// </summary>
public interface IReturnRepository
{
    /// <summary>
    /// Erstelle einen neuen Return und speichere ihn
    /// </summary>
    Task AddAsync(Return returnEntity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rufe einen Return nach ID ab
    /// 
    /// Filtert nach TenantId für Sicherheit
    /// </summary>
    Task<Return?> GetByIdAsync(Guid tenantId, Guid returnId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rufe alle Returns einer Bestellung ab
    /// 
    /// Häufiger Fall: Kunde hat mehrere Returns für eine Bestellung
    /// </summary>
    Task<List<Return>> GetByOrderIdAsync(Guid tenantId, Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rufe alle Returns eines Kunden ab
    /// 
    /// Paginierung mit Limit und Offset für große Datenmengen
    /// </summary>
    Task<List<Return>> GetByCustomerIdAsync(Guid tenantId, Guid customerId, int limit = 100, int offset = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finde alle Returns mit bestimmtem Status
    /// 
    /// Verwendet für: Dashboard-Übersichten, Batch-Verarbeitung, Reporting
    /// </summary>
    Task<List<Return>> GetByStatusAsync(Guid tenantId, ReturnStatus status, int limit = 100, int offset = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finde alle abgelaufenen Returns (außerhalb der 14-Tage-Frist)
    /// 
    /// Verwendet für: Automatische Ablehnung, Audit, Reporting
    /// </summary>
    Task<List<Return>> GetExpiredReturnsAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualisiere einen existierenden Return
    /// 
    /// Ruft automatisch UpdatedAt auf
    /// </summary>
    Task UpdateAsync(Return returnEntity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Speichere alle Änderungen (Unit of Work Pattern)
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Zähle Returns mit optionalem Status-Filter
    /// </summary>
    Task<int> CountAsync(Guid tenantId, ReturnStatus? status = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Abfrage-Interface für komplexe Filter
    /// 
    /// Ermöglicht: Where(), OrderBy(), Skip(), Take(), etc.
    /// </summary>
    IQueryable<Return> Query(Guid tenantId);
}
