using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;
using FluentValidation;

namespace B2Connect.CatalogService.Handlers;

/// <summary>
/// Wolverine HTTP Handler for B2B VAT ID Validation
///
/// Wolverine automatically creates: POST /validatevatid
///
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// </summary>
public class VatIdValidationHandler
{
    private readonly IVatIdValidationService _vatService;
    private readonly IValidator<ValidateVatIdRequest> _validator;
    private readonly ILogger<VatIdValidationHandler> _logger;

    public VatIdValidationHandler(
        IVatIdValidationService vatService,
        IValidator<ValidateVatIdRequest> validator,
        ILogger<VatIdValidationHandler> logger)
    {
        _vatService = vatService ?? throw new ArgumentNullException(nameof(vatService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validate B2B customer VAT ID
    ///
    /// Wolverine Endpoint: POST /validatevatid
    ///
    /// Request:
    /// {
    ///   "countryCode": "DE",
    ///   "vatNumber": "123456789",
    ///   "buyerCountry": "AT",
    ///   "sellerCountry": "DE"
    /// }
    ///
    /// Response:
    /// {
    ///   "isValid": true,
    ///   "vatId": "DE123456789",
    ///   "companyName": "Example GmbH",
    ///   "companyAddress": "123 Main St, 10115 Berlin",
    ///   "reverseChargeApplies": true,
    ///   "message": "Reverse charge applies - 0% VAT",
    ///   "expiresAt": "2026-12-29T12:34:56Z"
    /// }
    /// </summary>
    public async Task<ValidateVatIdResponse> ValidateVatId(
        ValidateVatIdRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("VAT validation request: {CountryCode}{VatNumber}",
            request.CountryCode, request.VatNumber);

        try
        {
            // Validate request
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("VAT validation request invalid: {Errors}",
                    string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

                return new ValidateVatIdResponse
                {
                    IsValid = false,
                    Message = "Invalid request: " + string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)),
                    ExpiresAt = DateTime.UtcNow.AddDays(1)
                };
            }

            // Validate VAT ID via VIES
            var validation = await _vatService.ValidateVatIdAsync(
                request.CountryCode,
                request.VatNumber,
                cancellationToken);

            // Determine reverse charge
            var reverseChargeApplies = false;
            if (!string.IsNullOrWhiteSpace(request.BuyerCountry) && !string.IsNullOrWhiteSpace(request.SellerCountry))
            {
                reverseChargeApplies = _vatService.ShouldApplyReverseCharge(
                    validation,
                    request.BuyerCountry,
                    request.SellerCountry);
            }

            var response = new ValidateVatIdResponse
            {
                IsValid = validation.IsValid,
                VatId = validation.VatId,
                CompanyName = validation.CompanyName,
                CompanyAddress = validation.CompanyAddress,
                ReverseChargeApplies = reverseChargeApplies,
                Message = validation.IsValid
                    ? (reverseChargeApplies
                        ? "Reverse charge applies - 0% VAT"
                        : "Valid VAT ID - standard VAT applies")
                    : "Invalid VAT ID",
                ExpiresAt = validation.ExpiresAt
            };

            _logger.LogInformation("VAT validation response: IsValid={IsValid}, ReverseCharge={ReverseCharge}",
                response.IsValid, response.ReverseChargeApplies);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during VAT validation");

            return new ValidateVatIdResponse
            {
                IsValid = false,
                Message = "An error occurred during validation. Please try again later.",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }
    }
}
