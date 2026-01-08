namespace B2X.Identity.Models;

/// <summary>
/// Registrierungsquelle (wo kommt der Registrierungsprozess her?)
/// </summary>
public enum RegistrationSource
{
    /// <summary>
    /// Öffentliche Website
    /// </summary>
    PublicWebsite = 0,

    /// <summary>
    /// Admin-Panel (Admin erstellt Benutzer)
    /// </summary>
    AdminPanel = 1,

    /// <summary>
    /// ERP-Import (Bestandskunden aus SAP)
    /// </summary>
    ErpImport = 2,

    /// <summary>
    /// CSV Import
    /// </summary>
    CsvImport = 3,

    /// <summary>
    /// API (externe Systeme)
    /// </summary>
    Api = 4
}
