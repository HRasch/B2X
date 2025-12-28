using B2Connect.Identity.Models;

namespace B2Connect.Identity.Interfaces;

/// <summary>
/// Service für die Erkennung von Duplikaten in Kundenregistrierungen
/// </summary>
public interface IDuplicateDetectionService
{
    /// <summary>
    /// Sucht potenzielle Duplikate anhand von E-Mail und Personendaten
    /// </summary>
    Task<DuplicateCheckResultDto> CheckForDuplicatesAsync(
        string email,
        string? firstName = null,
        string? lastName = null,
        string? companyName = null,
        string? phone = null,
        CancellationToken ct = default);

    /// <summary>
    /// Gibt ein Ähnlichkeits-Score (0-100) zwischen zwei E-Mail-Adressen zurück
    /// </summary>
    int CalculateEmailSimilarity(string email1, string email2);

    /// <summary>
    /// Gibt ein Ähnlichkeits-Score (0-100) zwischen zwei Namen zurück (Levenshtein)
    /// </summary>
    int CalculateNameSimilarity(string name1, string name2);
}

/// <summary>
/// Ergebnis der Duplikat-Prüfung
/// </summary>
public class DuplicateCheckResultDto
{
    /// <summary>
    /// Wurden potenzielle Duplikate gefunden?
    /// </summary>
    public bool DuplicatesFound { get; set; }

    /// <summary>
    /// Konfidenz-Score des Duplikat-Matches (0-100)
    /// </summary>
    public int ConfidenceScore { get; set; }

    /// <summary>
    /// Liste der gefundenen Duplikate
    /// </summary>
    public List<PotentialDuplicateDto> PotentialDuplicates { get; set; } = new();

    /// <summary>
    /// Empfehlung für den Benutzer
    /// </summary>
    public string? Recommendation { get; set; }

    /// <summary>
    /// Soll die Registrierung blockiert werden?
    /// </summary>
    public bool ShouldBlock { get; set; }
}

/// <summary>
/// Potentieller Duplikat-Treffer
/// </summary>
public class PotentialDuplicateDto
{
    /// <summary>
    /// Benutzer-ID des bestehenden Accounts
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Name des bestehenden Accounts
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// E-Mail des bestehenden Accounts
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Ähnlichkeits-Score (0-100)
    /// </summary>
    public int SimilarityScore { get; set; }

    /// <summary>
    /// Matching-Quelle (EMAIL, NAME, PHONE, etc.)
    /// </summary>
    public string MatchingSource { get; set; } = string.Empty;

    /// <summary>
    /// Datum der Registrierung des bestehenden Accounts
    /// </summary>
    public DateTime RegisteredDate { get; set; }

    /// <summary>
    /// Ist dieser Account bereits aktiviert?
    /// </summary>
    public bool IsActive { get; set; }
}
