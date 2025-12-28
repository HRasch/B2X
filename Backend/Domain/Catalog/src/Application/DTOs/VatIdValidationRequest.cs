using FluentValidation;

namespace B2Connect.Catalog.Application.DTOs;

/// <summary>
/// Request to validate a VAT-ID against VIES
/// </summary>
public class VatIdValidationRequest
{
    public required string CountryCode { get; set; }  // e.g., "DE"
    public required string VatNumber { get; set; }    // e.g., "123456789"
}

/// <summary>
/// Validator for VatIdValidationRequest
/// Enforces proper VAT-ID format per EU standards
/// </summary>
public class VatIdValidationRequestValidator : AbstractValidator<VatIdValidationRequest>
{
    public VatIdValidationRequestValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .Length(2)
            .Matches(@"^[A-Z]{2}$")
            .WithMessage("Country code must be 2 uppercase letters (e.g., 'DE')");

        RuleFor(x => x.VatNumber)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(12)
            .Matches(@"^[A-Z0-9]+$")
            .WithMessage("VAT number must be 2-12 alphanumeric characters");
    }
}

/// <summary>
/// Result of VAT-ID validation via VIES API
/// Immutable after creation
/// </summary>
public class VatValidationResult
{
    /// <summary>
    /// Whether the VAT-ID is valid and active in VIES
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// EU country code (e.g., "DE" for Germany)
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// The VAT number part (without country prefix)
    /// </summary>
    public string? VatNumber { get; set; }

    /// <summary>
    /// Company name as registered at tax authority
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Company address as registered at tax authority
    /// </summary>
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// When this validation was performed
    /// </summary>
    public DateTime ValidatedAt { get; set; }

    /// <summary>
    /// When this cached result expires (365 days per NIS2)
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether this result came from cache vs fresh VIES lookup
    /// </summary>
    public bool WasFromCache { get; set; }

    /// <summary>
    /// Error message if validation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Create a valid result
    /// </summary>
    public static VatValidationResult Valid(
        string countryCode,
        string vatNumber,
        string companyName,
        string companyAddress)
    {
        return new VatValidationResult
        {
            IsValid = true,
            CountryCode = countryCode,
            VatNumber = vatNumber,
            CompanyName = companyName,
            CompanyAddress = companyAddress,
            ValidatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(365),
            WasFromCache = false
        };
    }

    /// <summary>
    /// Create an invalid result
    /// </summary>
    public static VatValidationResult Invalid(string errorMessage)
    {
        return new VatValidationResult
        {
            IsValid = false,
            ErrorMessage = errorMessage,
            ValidatedAt = DateTime.UtcNow,
            WasFromCache = false
        };
    }
}
