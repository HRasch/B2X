using B2Connect.Identity.Interfaces;
using Microsoft.Extensions.Logging;

namespace B2Connect.Identity.Services;

/// <summary>
/// Service für Duplikat-Erkennung in Kundenregistrierungen
/// Nutzt Multi-Field-Matching: Email → Levenshtein Distance für Namen → Phonetic Matching
/// </summary>
public partial class DuplicateDetectionService : IDuplicateDetectionService
{
    private readonly ILogger<DuplicateDetectionService> _logger;

    // Dieser Service würde in der Realität gegen die Datenbank prüfen
    // Für Demoversion verwenden wir in-memory Liste
    private readonly List<(Guid UserId, string UserName, string Email, string? Phone, DateTime RegisteredDate, bool IsActive)>
        _registeredUsers = new();

    private const int EmailSimilarityThreshold = 85;
    private const int NameSimilarityThreshold = 80;

    public DuplicateDetectionService(ILogger<DuplicateDetectionService> logger)
    {
        _logger = logger;
        // Initialisierung mit Demo-Daten
        InitializeDemoData();
    }

    public async Task<DuplicateCheckResultDto> CheckForDuplicatesAsync(
        string email,
        string? firstName = null,
        string? lastName = null,
        string? companyName = null,
        string? phone = null,
        CancellationToken ct = default)
    {
        await Task.Delay(10, ct).ConfigureAwait(false); // Simulate async work

        var result = new DuplicateCheckResultDto();

        if (string.IsNullOrWhiteSpace(email))
        {
            return result;
        }

        var potentialDuplicates = CheckForExactEmailMatch(email);
        if (potentialDuplicates.Count > 0)
        {
            result.PotentialDuplicates = potentialDuplicates;
            result.DuplicatesFound = true;
            result.ConfidenceScore = 100;
            result.ShouldBlock = true;
            result.Recommendation = "Diese E-Mail-Adresse ist bereits registriert. Bitte verwenden Sie eine andere oder setzen Sie Ihr Passwort zurück.";
            return result;
        }

        // Check for fuzzy matches
        potentialDuplicates.AddRange(CheckForFuzzyEmailMatches(email));
        potentialDuplicates.AddRange(CheckForNameMatches(firstName, lastName));
        potentialDuplicates.AddRange(CheckForPhoneMatches(phone));

        result.PotentialDuplicates = potentialDuplicates.DistinctBy(p => p.UserId).ToList();
        result.DuplicatesFound = potentialDuplicates.Count > 0;

        if (result.DuplicatesFound)
        {
            result.ConfidenceScore = potentialDuplicates.Max(p => p.SimilarityScore);
            result.ShouldBlock = result.ConfidenceScore >= 90;

            result.Recommendation = result.ShouldBlock
                ? "Diese E-Mail-Adresse ist bereits registriert. Bitte verwenden Sie eine andere oder setzen Sie Ihr Passwort zurück."
                : "Ähnliche Konten gefunden. Bitte überprüfen Sie, ob einer der aufgelisteten Konten Ihnen gehört.";
        }

        return result;
    }

    private List<PotentialDuplicateDto> CheckForExactEmailMatch(string email)
    {
        var exactEmailMatch = _registeredUsers.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (exactEmailMatch != default)
        {
            _logger.LogWarning("Exakte Email-Übereinstimmung gefunden: {Email}", email);
            return new List<PotentialDuplicateDto>
            {
                new PotentialDuplicateDto
                {
                    UserId = exactEmailMatch.UserId,
                    UserName = exactEmailMatch.UserName,
                    Email = exactEmailMatch.Email,
                    SimilarityScore = 100,
                    MatchingSource = "EXACT_EMAIL",
                    RegisteredDate = exactEmailMatch.RegisteredDate,
                    IsActive = exactEmailMatch.IsActive
                }
            };
        }

        return new List<PotentialDuplicateDto>();
    }

    private List<PotentialDuplicateDto> CheckForFuzzyEmailMatches(string email)
    {
        var matches = new List<PotentialDuplicateDto>();

        foreach (var user in _registeredUsers)
        {
            var similarity = CalculateEmailSimilarity(email, user.Email);
            if (similarity >= EmailSimilarityThreshold)
            {
                _logger.LogWarning("Ähnliche Email gefunden: {Email} vs {ExistingEmail} ({Score}%)",
                    email, user.Email, similarity);
                matches.Add(new PotentialDuplicateDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    SimilarityScore = similarity,
                    MatchingSource = "FUZZY_EMAIL",
                    RegisteredDate = user.RegisteredDate,
                    IsActive = user.IsActive
                });
            }
        }

        return matches;
    }

    private List<PotentialDuplicateDto> CheckForNameMatches(string? firstName, string? lastName)
    {
        var matches = new List<PotentialDuplicateDto>();

        if (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName))
        {
            var fullName = $"{firstName ?? ""} {lastName ?? ""}".Trim();

            foreach (var user in _registeredUsers)
            {
                var similarity = CalculateNameSimilarity(fullName, user.UserName);
                if (similarity >= NameSimilarityThreshold)
                {
                    _logger.LogWarning("Ähnlicher Name gefunden: {Name} vs {ExistingName} ({Score}%)",
                        fullName, user.UserName, similarity);
                    matches.Add(new PotentialDuplicateDto
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Email = user.Email,
                        SimilarityScore = similarity,
                        MatchingSource = "FUZZY_NAME",
                        RegisteredDate = user.RegisteredDate,
                        IsActive = user.IsActive
                    });
                }
            }
        }

        return matches;
    }

    private List<PotentialDuplicateDto> CheckForPhoneMatches(string? phone)
    {
        var matches = new List<PotentialDuplicateDto>();

        if (!string.IsNullOrWhiteSpace(phone))
        {
            var cleanPhone = CleanPhoneNumber(phone);
            foreach (var user in _registeredUsers)
            {
                if (user.Phone != null && string.Equals(CleanPhoneNumber(user.Phone), cleanPhone, StringComparison.Ordinal))
                {
                    _logger.LogWarning("Gleiche Telefonnummer gefunden: {Phone}", phone);
                    matches.Add(new PotentialDuplicateDto
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Email = user.Email,
                        SimilarityScore = 95,
                        MatchingSource = "EXACT_PHONE",
                        RegisteredDate = user.RegisteredDate,
                        IsActive = user.IsActive
                    });
                }
            }
        }

        return matches;
    }

    public int CalculateEmailSimilarity(string email1, string email2)
    {
        if (email1.Equals(email2, StringComparison.OrdinalIgnoreCase))
        {
            return 100;
        }

        // Domain-Matching: z.B. john@gmail.com vs john@gmial.com (Domain-Typo)
        var parts1 = email1.ToLower(System.Globalization.CultureInfo.CurrentCulture).Split('@');
        var parts2 = email2.ToLower(System.Globalization.CultureInfo.CurrentCulture).Split('@');

        if (parts1.Length == 2 && parts2.Length == 2)
        {
            var localSimilarity = LevenshteinDistance(parts1[0], parts2[0]);
            var domainSimilarity = LevenshteinDistance(parts1[1], parts2[1]);

            var avgSimilarity = (localSimilarity + domainSimilarity) / 2;
            return Math.Min(100, avgSimilarity);
        }

        return LevenshteinDistance(email1.ToLower(System.Globalization.CultureInfo.CurrentCulture), email2.ToLower(System.Globalization.CultureInfo.CurrentCulture));
    }

    public int CalculateNameSimilarity(string name1, string name2)
    {
        if (name1.Equals(name2, StringComparison.OrdinalIgnoreCase))
        {
            return 100;
        }

        // Normalisierung: Leerzeichen, Bindestriche, etc.
        name1 = WhitespaceRegex().Replace(name1.ToLower(System.Globalization.CultureInfo.CurrentCulture), " ").Trim();
        name2 = WhitespaceRegex().Replace(name2.ToLower(System.Globalization.CultureInfo.CurrentCulture), " ").Trim();

        return LevenshteinDistance(name1, name2);
    }

    /// <summary>
    /// Berechnet Levenshtein Distance (Edit Distance) zwischen zwei Strings
    /// Score 100 = identisch, 0 = völlig unterschiedlich
    /// </summary>
    private static int LevenshteinDistance(string s1, string s2)
    {
        var length1 = s1.Length;
        var length2 = s2.Length;
        var distances = new int[length1 + 1, length2 + 1];

        for (var i = 0; i <= length1; distances[i, 0] = i++)
        {
            // Initialize first column
        }
        for (var j = 0; j <= length2; distances[0, j] = j++)
        {
            // Initialize first row
        }

        for (var i = 1; i <= length1; i++)
        {
            for (var j = 1; j <= length2; j++)
            {
                var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                distances[i, j] = Math.Min(
                    Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost);
            }
        }

        var maxLength = Math.Max(length1, length2);
        if (maxLength == 0)
        {
            return 100;
        }

        var distance = distances[length1, length2];
        var similarity = 100 - (distance * 100 / maxLength);
        return Math.Max(0, similarity);
    }

    private static string CleanPhoneNumber(string phone)
    {
        return new string(phone.Where(char.IsDigit).ToArray());
    }

    private void InitializeDemoData()
    {
        // Demo-Benutzer für Testing
        _registeredUsers.Add((Guid.NewGuid(), "John Doe", "john@example.com", "+49 123 456789", DateTime.UtcNow.AddDays(-30), true));
        _registeredUsers.Add((Guid.NewGuid(), "Jane Smith", "jane.smith@example.com", "+49 987 654321", DateTime.UtcNow.AddDays(-15), true));
        _registeredUsers.Add((Guid.NewGuid(), "Max Mustermann", "max.mustermann@gmx.de", "+49 111 222333", DateTime.UtcNow.AddDays(-7), true));
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"\s+")]
    private static partial System.Text.RegularExpressions.Regex WhitespaceRegex();
}
