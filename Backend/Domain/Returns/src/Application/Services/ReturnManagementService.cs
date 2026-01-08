using FluentValidation;
using Microsoft.Extensions.Logging;
using B2X.Returns.Core.Entities;
using B2X.Returns.Application.Commands;
using B2X.Returns.Application.Validators;
using B2X.Shared.Core.Handlers;

namespace B2X.Returns.Application.Services;

/// <summary>
/// Wolverine Service Handler for return management (14-day VVVG compliance).
///
/// This is the public async service class pattern for Wolverine HTTP endpoints.
/// Method naming: CreateReturn → POST /createreturn (auto-discovered by Wolverine)
/// </summary>
public class ReturnManagementService : ValidatedBase
{
    private readonly CreateReturnValidator _validator;

    public ReturnManagementService(
        CreateReturnValidator validator,
        ILogger<ReturnManagementService> logger)
        : base(logger)
    {
        _validator = validator;
    }

    /// <summary>
    /// Wolverine handler: Create a new return request (14-day VVVG withdrawal).
    /// 
    /// Endpoint: POST /createreturn
    /// 
    /// Process:
    /// 1. Validate command (deadline, amounts, etc.)
    /// 2. Create Return entity
    /// 3. Check if within 14-day window
    /// 4. Calculate refund amount
    /// 5. Log audit trail
    /// 6. Return response with deadline info
    /// </summary>
    public async Task<CreateReturnResponse> CreateReturn(
        CreateReturnCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[RETURN] CreateReturn initiated for Order:{OrderId}, Customer:{CustomerId}, Tenant:{TenantId}",
            request.OrderId, request.CustomerId, request.TenantId
        );

        try
        {
            // ═══════════════════════════════════════════════════════════════════════════════
            // 1. INPUT VALIDATION
            // ═══════════════════════════════════════════════════════════════════════════════

            var validationError = await ValidateRequestAsync(
                request,
                _validator,
                cancellationToken,
                errorMessage => new CreateReturnResponse
                {
                    Success = false,
                    Status = "REJECTED",
                    Message = $"Validation failed: {errorMessage}",
                    ReturnId = Guid.Empty,
                    DaysRemaining = 0,
                    RefundAmount = 0,
                    ReturnDeadline = DateTime.MinValue
                });

            if (validationError != null)
            {
                return validationError;
            }

            // ═══════════════════════════════════════════════════════════════════════════════
            // 2. CRITICAL: 14-DAY DEADLINE CHECK
            // ═══════════════════════════════════════════════════════════════════════════════

            var returnDeadline = request.DeliveryDate.AddDays(14);
            var daysRemaining = (returnDeadline.Date - DateTime.UtcNow.Date).TotalDays;

            if (daysRemaining < 0)
            {
                _logger.LogWarning(
                    "[RETURN] Return rejected: Outside 14-day window. Order:{OrderId}, DeliveryDate:{DeliveryDate}, Deadline:{Deadline}",
                    request.OrderId, request.DeliveryDate, returnDeadline
                );

                return new CreateReturnResponse
                {
                    Success = false,
                    Status = "REJECTED",
                    Message = $"Return period expired on {returnDeadline:yyyy-MM-dd}. " +
                             $"VVVG requires returns within 14 days of delivery.",
                    ReturnId = Guid.Empty,
                    DaysRemaining = 0,
                    RefundAmount = 0,
                    ReturnDeadline = returnDeadline
                };
            }

            // ═══════════════════════════════════════════════════════════════════════════════
            // 3. CREATE RETURN ENTITY
            // ═══════════════════════════════════════════════════════════════════════════════

            var returnEntity = new Return
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                Status = ReturnStatus.Initiated,
                Reason = request.Reason,
                ItemsCount = request.ItemsCount,

                // CRITICAL: DeliveryDate used for deadline, not OrderDate
                DeliveryDate = request.DeliveryDate,
                ReturnDeadline = returnDeadline,
                RequestDate = DateTime.UtcNow,
                IsWithinDeadline = daysRemaining >= 0,

                // Refund calculation
                RefundAmount = request.RefundAmount,
                OriginalOrderAmount = request.OriginalOrderAmount,
                ShippingDeduction = request.ShippingDeduction,

                // Audit trail
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy
            };

            // ═══════════════════════════════════════════════════════════════════════════════
            // 4. VALIDATE ENTITY (Business Rules)
            // ═══════════════════════════════════════════════════════════════════════════════

            try
            {
                returnEntity.ValidateWithinDeadline();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("[RETURN] Business rule validation failed: {Message}", ex.Message);

                return new CreateReturnResponse
                {
                    Success = false,
                    Status = "REJECTED",
                    Message = ex.Message,
                    ReturnId = Guid.Empty,
                    DaysRemaining = 0,
                    RefundAmount = 0,
                    ReturnDeadline = returnDeadline
                };
            }

            // ═══════════════════════════════════════════════════════════════════════════════
            // 5. AUDIT LOGGING
            // ═══════════════════════════════════════════════════════════════════════════════

            _logger.LogInformation(
                "[RETURN] Return initiated successfully. ID:{ReturnId}, Order:{OrderId}, " +
                "Refund:{RefundAmount}, Deadline:{Deadline}, DaysRemaining:{DaysRemaining}",
                returnEntity.Id,
                request.OrderId,
                request.RefundAmount,
                returnDeadline,
                (int)daysRemaining
            );

            // ═══════════════════════════════════════════════════════════════════════════════
            // 6. BUILD RESPONSE
            // ═══════════════════════════════════════════════════════════════════════════════

            return new CreateReturnResponse
            {
                Success = true,
                Status = "INITIATED",
                ReturnId = returnEntity.Id,
                ReturnDeadline = returnDeadline,
                DaysRemaining = (int)daysRemaining,
                RefundAmount = request.RefundAmount,
                Message = $"Return initiated successfully. You have {(int)daysRemaining} days to ship the items back. " +
                         $"Return deadline: {returnDeadline:yyyy-MM-dd}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[RETURN] Unexpected error in CreateReturn. Order:{OrderId}, Customer:{CustomerId}",
                request.OrderId,
                request.CustomerId
            );

            return new CreateReturnResponse
            {
                Success = false,
                Status = "ERROR",
                Message = "An unexpected error occurred while processing your return. Please try again later.",
                ReturnId = Guid.Empty,
                DaysRemaining = 0,
                RefundAmount = 0,
                ReturnDeadline = DateTime.MinValue
            };
        }
    }
}
