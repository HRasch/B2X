namespace B2X.Identity.Models;

/// <summary>
/// Registrierungstyp zur Unterscheidung zwischen Neu- und Bestandskunden
/// </summary>
public enum RegistrationType
{
    /// <summary>
    /// Neuer Kunde (keine ERP-Erfassung vorhanden)
    /// </summary>
    NewCustomer = 0,

    /// <summary>
    /// Bestandskunde (bereits im ERP erfasst)
    /// </summary>
    ExistingCustomer = 1,

    /// <summary>
    /// Kunde mit Geschäftskonto (B2B)
    /// </summary>
    BusinessCustomer = 2
}
