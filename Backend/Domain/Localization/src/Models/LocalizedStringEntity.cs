namespace B2Connect.LocalizationService.Models;

/// <summary>
/// Entity that stores localized strings for a specific tenant.
/// Uses LocalizedString as a value object.
/// </summary>
public class LocalizedStringEntity : B2Connect.Shared.Core.ITenantEntity
{
    /// <summary>Gets or sets the unique identifier</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the tenant ID this entity belongs to</summary>
    public Guid TenantId { get; set; }

    /// <summary>Gets or sets the localized string value object</summary>
    public LocalizedString LocalizedString { get; set; } = null!;

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the last modification timestamp</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the LocalizedStringEntity class.
    /// </summary>
    public LocalizedStringEntity(Guid tenantId, LocalizedString localizedString)
    {
        TenantId = tenantId;
        LocalizedString = localizedString ?? throw new ArgumentNullException(nameof(localizedString));
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    private LocalizedStringEntity() { }
}