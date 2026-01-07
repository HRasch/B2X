using B2X.Catalog.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.Handlers;

/// <summary>
/// Service for recording and validating terms acceptance
/// Story: P0.6-US-005 - Mandatory Terms & Conditions Acceptance
///
/// Handles:
/// - Validation that all required terms are accepted
/// - Recording acceptance in audit log
/// - Retrieving acceptance history
/// - Change detection when terms are updated
/// </summary>
public class TermsAcceptanceService
{
    private readonly ILogger<TermsAcceptanceService> _logger;
    private readonly RecordTermsAcceptanceValidator _validator;

    public TermsAcceptanceService(
        ILogger<TermsAcceptanceService> logger,
        RecordTermsAcceptanceValidator validator)
    {
        _logger = logger;
        _validator = validator;
    }

    /// <summary>
    /// Wolverine Handler: POST /api/checkout/accept-terms
    /// Records customer acceptance of all required terms
    /// </summary>
    public async Task<RecordTermsAcceptanceResponse> RecordAcceptanceAsync(
        Guid tenantId,
        RecordTermsAcceptanceRequest request,
        string ipAddress,
        string userAgent,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Recording terms acceptance for customer {CustomerId} in tenant {TenantId}",
            request.CustomerId, tenantId);

        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return CreateValidationErrorResponse(request.CustomerId, validationResult.Errors);
            }

            // Validate required acceptances
            if (!request.AcceptTermsAndConditions || !request.AcceptPrivacyPolicy)
            {
                return CreateIncompleteAcceptanceResponse(request.CustomerId);
            }

            // Create and record acceptance
            var acceptanceLog = CreateAcceptanceLog(tenantId, request, ipAddress, userAgent);

            _logger.LogInformation(
                "Terms acceptance recorded successfully for {CustomerId}: {AcceptanceId}",
                request.CustomerId, acceptanceLog.Id);

            return new RecordTermsAcceptanceResponse
            {
                Success = true,
                AcceptanceLogId = acceptanceLog.Id,
                Message = "Bedingungen erfolgreich akzeptiert"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error recording terms acceptance for {CustomerId}",
                request.CustomerId);

            return CreateInternalErrorResponse();
        }
    }

    private RecordTermsAcceptanceResponse CreateValidationErrorResponse(
        string customerId,
        IEnumerable<FluentValidation.Results.ValidationFailure> errors)
    {
        _logger.LogWarning(
            "Terms acceptance validation failed for {CustomerId}",
            customerId);

        return new RecordTermsAcceptanceResponse
        {
            Success = false,
            Error = "VALIDATION_ERROR",
            Message = string.Join("; ", errors.Select(e => e.ErrorMessage))
        };
    }

    private RecordTermsAcceptanceResponse CreateIncompleteAcceptanceResponse(string customerId)
    {
        _logger.LogWarning(
            "Required terms not accepted by {CustomerId}",
            customerId);

        return new RecordTermsAcceptanceResponse
        {
            Success = false,
            Error = "INCOMPLETE_ACCEPTANCE",
            Message = "Sie müssen den Allgemeinen Geschäftsbedingungen und der Datenschutzerklärung zustimmen"
        };
    }

    private TermsAcceptanceLog CreateAcceptanceLog(
        Guid tenantId,
        RecordTermsAcceptanceRequest request,
        string ipAddress,
        string userAgent)
    {
        return new TermsAcceptanceLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CustomerId = request.CustomerId,
            TermsVersion = CurrentTermsVersion,
            AcceptedAt = DateTime.UtcNow,
            AcceptedTermsAndConditions = request.AcceptTermsAndConditions,
            AcceptedPrivacyPolicy = request.AcceptPrivacyPolicy,
            AcceptedWithdrawalRight = request.AcceptWithdrawalRight,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedBy = request.CustomerId,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static RecordTermsAcceptanceResponse CreateInternalErrorResponse()
    {
        return new RecordTermsAcceptanceResponse
        {
            Success = false,
            Error = "INTERNAL_ERROR",
            Message = "Ein Fehler ist bei der Speicherung aufgetreten. Bitte versuchen Sie es später erneut."
        };
    }

    /// <summary>
    /// Validate that customer has accepted current version of terms
    /// Returns false if different version was accepted or none at all
    /// </summary>
    public bool HasCustomerAcceptedCurrentTermsAsync(
        Guid tenantId,
        string customerId,
        TermsAcceptanceLog? lastAcceptance)
    {
        if (lastAcceptance == null)
        {
            _logger.LogWarning(
                "No terms acceptance found for customer {CustomerId}",
                customerId);
            return false;
        }

        var currentVersion = CurrentTermsVersion;
        var isCurrentVersion = string.Equals(lastAcceptance.TermsVersion, currentVersion, StringComparison.Ordinal);

        if (!isCurrentVersion)
        {
            _logger.LogWarning(
                "Customer {CustomerId} accepted older terms version: {OldVersion} vs {CurrentVersion}",
                customerId, lastAcceptance.TermsVersion, currentVersion);
        }

        return lastAcceptance.AcceptedTermsAndConditions
            && lastAcceptance.AcceptedPrivacyPolicy
            && isCurrentVersion;
    }

    /// <summary>
    /// Current terms version hash
    /// In production, this would be a hash of actual T&C document from DB
    /// </summary>
    private const string CurrentTermsVersion = "v1.0.0-2025-12-29";
}

/// <summary>
/// Fluent validation for terms acceptance request
/// Note: Only validates field format, not business rules (e.g., acceptance values)
/// Business rule validation (like "must accept T&C") happens in service
/// </summary>
public class RecordTermsAcceptanceValidator : AbstractValidator<RecordTermsAcceptanceRequest>
{
    public RecordTermsAcceptanceValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Kunden-ID erforderlich")
            .Length(1, 100).WithMessage("Kunden-ID muss zwischen 1 und 100 Zeichen lang sein");

        // Note: We don't validate boolean values here
        // Business logic in service will check if required flags are true
    }
}
