using B2X.Identity.Models;

namespace B2X.Identity.Interfaces;

/// <summary>
/// Service-Interface für die Kommunikation mit dem ERP-System
/// </summary>
public interface IErpCustomerService
{
    /// <summary>
    /// Sucht einen Kunden nach Kundennummer im ERP
    /// </summary>
    Task<ErpCustomerDto?> GetCustomerByNumberAsync(string customerNumber, CancellationToken ct = default);

    /// <summary>
    /// Sucht einen Kunden nach E-Mail-Adresse im ERP
    /// </summary>
    Task<ErpCustomerDto?> GetCustomerByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Sucht einen Kunden nach Firmenname im ERP (für B2B)
    /// </summary>
    Task<ErpCustomerDto?> GetCustomerByCompanyNameAsync(string companyName, CancellationToken ct = default);

    /// <summary>
    /// Überprüft, ob die Verbindung zum ERP verfügbar ist
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken ct = default);

    /// <summary>
    /// Holt den Status der letzten ERP-Synchronisierung
    /// </summary>
    Task<ErpSyncStatusDto> GetSyncStatusAsync(CancellationToken ct = default);
}

/// <summary>
/// ERP-Kundendaten
/// </summary>
public class ErpCustomerDto
{
    /// <summary>
    /// Eindeutige Kundennummer im ERP
    /// </summary>
    public string CustomerNumber { get; set; } = string.Empty;

    /// <summary>
    /// Name des Kunden (Privat: Firstname + Lastname, Business: Company)
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// E-Mail-Adresse
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefonnummer
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Lieferadresse
    /// </summary>
    public string? ShippingAddress { get; set; }

    /// <summary>
    /// Rechnungsadresse
    /// </summary>
    public string? BillingAddress { get; set; }

    /// <summary>
    /// Land/Region
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Geschäftstyp (PRIVATE, BUSINESS)
    /// </summary>
    public string BusinessType { get; set; } = "PRIVATE";

    /// <summary>
    /// Datum der letzten Änderung im ERP
    /// </summary>
    public DateTime LastModifiedDate { get; set; }

    /// <summary>
    /// Ist der Kunde im ERP aktiv?
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Kundenstatus im ERP
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Kreditlimit
    /// </summary>
    public decimal? CreditLimit { get; set; }

    /// <summary>
    /// Hinzugefügte Metadaten
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new(StringComparer.Ordinal);
}

/// <summary>
/// Status der ERP-Synchronisierung
/// </summary>
public class ErpSyncStatusDto
{
    /// <summary>
    /// Ist die ERP-Verbindung aktiv?
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Zeitpunkt der letzten erfolgreichen Synchronisierung
    /// </summary>
    public DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// Anzahl der Kunden im Cache
    /// </summary>
    public int CachedCustomerCount { get; set; }

    /// <summary>
    /// ERP-Systemtyp (SAP, Oracle, etc.)
    /// </summary>
    public string? ErpSystemType { get; set; }

    /// <summary>
    /// Status-Nachricht
    /// </summary>
    public string? Message { get; set; }
}
