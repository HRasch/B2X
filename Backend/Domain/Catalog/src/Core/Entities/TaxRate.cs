namespace B2Connect.Catalog.Core.Entities;

/// <summary>
/// TaxRate entity - Domain model for VAT rates per country
/// Issue #30: B2C Price Transparency (PAngV)
/// </summary>
public class TaxRate
{
    public Guid Id { get; set; }
    public string CountryCode { get; set; } = null!; // "DE", "AT", "FR", etc.
    public string CountryName { get; set; } = null!;
    public decimal StandardVatRate { get; set; } // 19.00 for Germany
    public decimal? ReducedVatRate { get; set; } // Optional: 7.00 for Germany
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public TaxRate() { }

    public TaxRate(
        string countryCode,
        string countryName,
        decimal standardVatRate,
        DateTime effectiveDate,
        decimal? reducedVatRate = null)
    {
        Id = Guid.NewGuid();
        CountryCode = countryCode ?? throw new ArgumentNullException(nameof(countryCode));
        CountryName = countryName ?? throw new ArgumentNullException(nameof(countryName));
        StandardVatRate = standardVatRate;
        ReducedVatRate = reducedVatRate;
        EffectiveDate = effectiveDate;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsActive() => EffectiveDate <= DateTime.UtcNow && (!EndDate.HasValue || EndDate > DateTime.UtcNow);
}
