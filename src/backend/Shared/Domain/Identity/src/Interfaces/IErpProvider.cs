using B2X.Identity.Models;

namespace B2X.Identity.Interfaces;

/// <summary>
/// Provider-Abstraction für verschiedene ERP-Systeme
/// Ermöglicht einfachen Wechsel zwischen SAP, Oracle, Fake, etc.
/// </summary>
public interface IErpProvider
{
    /// <summary>
    /// Eindeutiger Name des Providers (z.B. "SAP", "Oracle", "Fake")
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Sucht einen Kunden nach Kundennummer
    /// </summary>
    Task<ErpCustomerDto?> GetCustomerByNumberAsync(string customerNumber, CancellationToken ct = default);

    /// <summary>
    /// Sucht einen Kunden nach E-Mail-Adresse
    /// </summary>
    Task<ErpCustomerDto?> GetCustomerByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Sucht einen Kunden nach Firmenname (für B2B)
    /// </summary>
    Task<ErpCustomerDto?> GetCustomerByCompanyNameAsync(string companyName, CancellationToken ct = default);

    /// <summary>
    /// Überprüft die Verbindung zum ERP-System
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken ct = default);

    /// <summary>
    /// Holt den Synchronisierungsstatus vom ERP
    /// </summary>
    Task<ErpSyncStatusDto> GetSyncStatusAsync(CancellationToken ct = default);
}
