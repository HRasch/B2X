using FluentValidation;
using B2Connect.Catalog.Application.DTOs;

namespace B2Connect.Catalog.Application.Validators;

/// <summary>
/// Validator für CreateReturnRequestDto
/// VVVG §357 Compliance-Validierungen
/// </summary>
public class CreateReturnRequestValidator : AbstractValidator<CreateReturnRequestDto>
{
    public CreateReturnRequestValidator()
    {
        // TenantId required (multi-tenant safety)
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required");

        // OrderId required
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId is required");

        // WithdrawalReason required
        RuleFor(x => x.WithdrawalReason)
            .NotEmpty()
            .WithMessage("WithdrawalReason is required")
            .MaximumLength(100)
            .WithMessage("WithdrawalReason must not exceed 100 characters")
            .Must(x => IsValidReason(x))
            .WithMessage("WithdrawalReason must be one of: Defective, NotAsDescribed, ChangedMind, Other");

        // ReasonDetails optional, but max 1000 chars if provided
        RuleFor(x => x.ReasonDetails)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.ReasonDetails))
            .WithMessage("ReasonDetails must not exceed 1000 characters");
    }

    private static bool IsValidReason(string reason)
    {
        var validReasons = new[] { "Defective", "NotAsDescribed", "ChangedMind", "Other" };
        return validReasons.Contains(reason);
    }
}

/// <summary>
/// Validator für CreateRefundRequestDto
/// VVVG §359 (Rückerstattung) Compliance-Validierungen
/// </summary>
public class CreateRefundRequestValidator : AbstractValidator<CreateRefundRequestDto>
{
    public CreateRefundRequestValidator()
    {
        // TenantId required
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required");

        // ReturnRequestId required
        RuleFor(x => x.ReturnRequestId)
            .NotEmpty()
            .WithMessage("ReturnRequestId is required");

        // ApprovedRefundAmount must be positive
        RuleFor(x => x.ApprovedRefundAmount)
            .GreaterThan(0)
            .WithMessage("ApprovedRefundAmount must be greater than 0")
            .LessThanOrEqualTo(1000000)  // Max €1M per refund (sanity check)
            .WithMessage("ApprovedRefundAmount must not exceed €1,000,000");

        // InspectionNotes optional, max 2000 chars
        RuleFor(x => x.InspectionNotes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.InspectionNotes))
            .WithMessage("InspectionNotes must not exceed 2000 characters");
    }
}
