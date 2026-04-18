namespace B2X.Identity.Models;

/// <summary>
/// DTO für die Überprüfung des Registrierungstyps
/// </summary>
public class CheckRegistrationTypeDto
{
    /// <summary>
    /// Kundennummer/CustomerID aus dem ERP-System
    /// </summary>
    public string? CustomerNumber { get; set; }

    /// <summary>
    /// E-Mail-Adresse für Lookup
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Firmenname (für Business-Kunden)
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Telefonnummer für Validierung
    /// </summary>
    public string? Phone { get; set; }
}

/// <summary>
/// Response-DTO für Registrierungstyp-Check
/// </summary>
public class RegistrationTypeResponseDto
{
    /// <summary>
    /// Der ermittelte Registrierungstyp
    /// </summary>
    public RegistrationType RegistrationType { get; set; }

    /// <summary>
    /// Wird Bestandskunde erkannt?
    /// </summary>
    public bool IsExistingCustomer { get; set; }

    /// <summary>
    /// Kundennummer aus ERP (wenn gefunden)
    /// </summary>
    public string? ErpCustomerId { get; set; }

    /// <summary>
    /// Kundenname aus ERP
    /// </summary>
    public string? ErpCustomerName { get; set; }

    /// <summary>
    /// Kundenadresse aus ERP
    /// </summary>
    public string? ErpCustomerAddress { get; set; }

    /// <summary>
    /// Gebiet/Land aus ERP
    /// </summary>
    public string? ErpCustomerCountry { get; set; }

    /// <summary>
    /// Rechnungsadresse aus ERP
    /// </summary>
    public string? ErpBillingAddress { get; set; }

    /// <summary>
    /// Lieferadresse aus ERP
    /// </summary>
    public string? ErpShippingAddress { get; set; }

    /// <summary>
    /// Geschäftstyp (B2C, B2B)
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// Konfidenz-Score für Matching (0-100)
    /// </summary>
    public int MatchConfidenceScore { get; set; }

    /// <summary>
    /// Fehlermeldung (falls vorhanden)
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Nachricht für den Benutzer
    /// </summary>
    public string? Message { get; set; }
}
