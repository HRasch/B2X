namespace B2Connect.Shared.User.Models;

/// <summary>
/// User profile with additional information
/// </summary>
public class Profile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; } // Male, Female, Other
    public string? Nationality { get; set; }
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public string? PreferredLanguage { get; set; } = "en";
    public string? Timezone { get; set; }
    public bool ReceiveNewsletter { get; set; }
    public bool ReceivePromotionalEmails { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual User? User { get; set; }

    public int GetAge()
    {
        if (!DateOfBirth.HasValue)
            return 0;

        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Value.Year;
        if (DateOfBirth.Value.Date > today.AddYears(-age))
            age--;
        return age;
    }
}
