using B2X.Catalog.Models;
using B2X.Catalog.Services;
using B2X.Shared.Core.Handlers;
using FluentValidation;

namespace B2X.Catalog.Handlers;

/// <summary>
/// Wolverine HTTP Handler for B2B VAT ID Validation
///
/// Wolverine automatically creates: POST /validatevatid
///
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// </summary>
public class VatIdValidationHandler : ValidatedHandlerBase
{
    private readonly IVatIdValidationService _vatService;
    private readonly IValidator<ValidateVatIdRequest> _validator;

    public VatIdValidationHandler(
        IVatIdValidationService vatService,
        IValidator<ValidateVatIdRequest> validator,
        ILogger<VatIdValidationHandler> logger)
        : base(logger)
    {
        _vatService = vatService ?? throw new ArgumentNullException(nameof(vatService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
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
        Logger.LogInformation("VAT validation request: {CountryCode}{VatNumber}",
            request.CountryCode, request.VatNumber);

        try
        {
            // Validate request
            var validationError = await ValidateRequestAsync(
                request,
                _validator,
                cancellationToken,
                errorMessage => CreateInvalidRequestResponse(errorMessage));

            if (validationError != null)
            {
                return validationError;
            }

            // Validate VAT ID via VIES
            var validation = await _vatService.ValidateVatIdAsync(
                request.CountryCode,
                request.VatNumber,
                cancellationToken).ConfigureAwait(false);

            // Determine reverse charge
            var reverseChargeApplies = DetermineReverseCharge(validation, request);

            return CreateValidationResponse(validation, reverseChargeApplies);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error during VAT validation");
            return CreateErrorResponse();
        }
    }

    private ValidateVatIdResponse CreateInvalidRequestResponse(string errorMessage)
    {
        Logger.LogWarning("VAT validation request invalid: {Errors}", errorMessage);

        return new ValidateVatIdResponse
        {
            IsValid = false,
            Message = "Invalid request: " + errorMessage,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
    }

    private bool DetermineReverseCharge(VatValidationResult validation, ValidateVatIdRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BuyerCountry) || string.IsNullOrWhiteSpace(request.SellerCountry))
        {
            return false;
        }

        return _vatService.ShouldApplyReverseCharge(
            validation,
            request.BuyerCountry,
            request.SellerCountry);
    }

    private ValidateVatIdResponse CreateValidationResponse(VatValidationResult validation, bool reverseChargeApplies)
    {
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

        Logger.LogInformation("VAT validation response: IsValid={IsValid}, ReverseCharge={ReverseCharge}",
            response.IsValid, response.ReverseChargeApplies);

        return response;
    }

    private static ValidateVatIdResponse CreateErrorResponse()
    {
        return new ValidateVatIdResponse
        {
            IsValid = false,
            Message = "An error occurred during validation. Please try again later.",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}
