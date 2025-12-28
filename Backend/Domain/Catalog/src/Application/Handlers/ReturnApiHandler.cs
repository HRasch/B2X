using B2Connect.Catalog.Application.DTOs;
using B2Connect.Catalog.Core.Entities;
using B2Connect.Catalog.Core.Interfaces;
using B2Connect.Catalog.Core.Services;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace B2Connect.Catalog.Application.Handlers;

/// <summary>
/// Wolverine HTTP API Handler für Return/Refund Operationen
/// Story: VVVG §357 (14-Tage Widerrufsrecht), VVVG §359 (Rückerstattung)
/// 
/// Wolverine Pattern (NOT MediatR!):
/// - Handler ist ein einfacher Service
/// - Wird automatisch von Wolverine zu HTTP Endpoint konvertiert
/// - Methodenname wird zu Route: CreateReturn → POST /createreturn
/// 
/// Compliance:
/// - Multi-Tenant Isolation (X-Tenant-ID mandatory)
/// - Audit Logging für alle Return-Operationen
/// - 14-Tage Widerrufsrecht validation
/// - Refund Status Tracking (VVVG §359)
/// </summary>
public class ReturnApiHandler
{
    private readonly IReturnRepository _returnRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IRefundRepository _refundRepository;
    private readonly ReturnManagementService _returnManagementService;
    private readonly IValidator<CreateReturnRequestDto> _createReturnValidator;
    private readonly IValidator<CreateRefundRequestDto> _createRefundValidator;
    private readonly ILogger<ReturnApiHandler> _logger;

    public ReturnApiHandler(
        IReturnRepository returnRepository,
        IOrderRepository orderRepository,
        IRefundRepository refundRepository,
        ReturnManagementService returnManagementService,
        IValidator<CreateReturnRequestDto> createReturnValidator,
        IValidator<CreateRefundRequestDto> createRefundValidator,
        ILogger<ReturnApiHandler> logger)
    {
        _returnRepository = returnRepository;
        _orderRepository = orderRepository;
        _refundRepository = refundRepository;
        _returnManagementService = returnManagementService;
        _createReturnValidator = createReturnValidator;
        _createRefundValidator = createRefundValidator;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/returns/create
    /// Erstelle eine neue Return/Widerrufs-Anfrage (VVVG §357)
    /// 
    /// Anforderung:
    /// - Bestellung muss im Zeitraum (Lieferdatum, Lieferdatum + 14 Tage) liegen
    /// - Bestellung darf noch keinen anderen aktiven Return haben
    /// - Grund muss ausgewählt sein
    /// 
    /// Response:
    /// - ReturnRequestId zur Verfolgung
    /// - Withdrawal Days Remaining (z.B. 8 Tage verbleibend)
    /// - Nächste Schritte (Rückversand, etc.)
    /// </summary>
    public async Task<CreateReturnResponseDto> CreateReturn(
        CreateReturnRequestDto request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "CreateReturn started - Order: {OrderId}, Tenant: {TenantId}, Reason: {Reason}",
            request.OrderId, request.TenantId, request.WithdrawalReason);

        try
        {
            // 1. Validate input
            var validation = await _createReturnValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                _logger.LogWarning(
                    "CreateReturn validation failed - Order: {OrderId}: {Errors}",
                    request.OrderId, string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

                return new CreateReturnResponseDto
                {
                    Success = false,
                    Error = "VALIDATION_ERROR",
                    Message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))
                };
            }

            // 2. Get Order (verify exists, verify delivery date)
            var order = await _orderRepository.GetOrderAsync(
                request.TenantId,
                request.OrderId,
                cancellationToken);

            if (order == null)
            {
                _logger.LogWarning(
                    "CreateReturn - Order not found: {OrderId}, Tenant: {TenantId}",
                    request.OrderId, request.TenantId);

                return new CreateReturnResponseDto
                {
                    Success = false,
                    Error = "ORDER_NOT_FOUND",
                    Message = "Bestellung nicht gefunden."
                };
            }

            // 3. Validate withdrawal period (VVVG §357)
            var withdrawalDaysRemaining = await _orderRepository.GetWithdrawalDaysRemainingAsync(
                request.TenantId,
                request.OrderId,
                cancellationToken);

            if (withdrawalDaysRemaining <= 0)
            {
                _logger.LogInformation(
                    "CreateReturn - Withdrawal period expired: {OrderId}, DaysRemaining: {Days}",
                    request.OrderId, withdrawalDaysRemaining);

                return new CreateReturnResponseDto
                {
                    Success = false,
                    Error = "WITHDRAWAL_PERIOD_EXPIRED",
                    Message = $"Das 14-Tage Widerrufsrecht ist abgelaufen. Verbleibend: {withdrawalDaysRemaining} Tage."
                };
            }

            // 4. Check if order has existing active return
            var existingReturns = await _returnRepository.GetOrderReturnRequestsAsync(
                request.TenantId,
                request.OrderId,
                cancellationToken);

            var activeReturn = existingReturns
                .FirstOrDefault(r => new[] { "Requested", "ReturnLabelSent", "InTransit", "Received" }.Contains(r.Status));

            if (activeReturn != null)
            {
                _logger.LogInformation(
                    "CreateReturn - Active return already exists: {ReturnId} (Status: {Status})",
                    activeReturn.Id, activeReturn.Status);

                return new CreateReturnResponseDto
                {
                    Success = false,
                    Error = "ACTIVE_RETURN_EXISTS",
                    Message = $"Für diese Bestellung existiert bereits ein aktiver Rückgabeprozess (ID: {activeReturn.Id})."
                };
            }

            // 5. Create Return Request (validates amount, etc.)
            var returnRequest = await _returnManagementService.CreateReturnRequestAsync(
                request.TenantId,
                order,
                request.WithdrawalReason,
                request.ReasonDetails,
                cancellationToken);

            // 6. Persist Return Request
            await _returnRepository.CreateReturnRequestAsync(
                request.TenantId,
                returnRequest,
                cancellationToken);

            _logger.LogInformation(
                "CreateReturn successful - ReturnId: {ReturnId}, OrderId: {OrderId}, Amount: {Amount}, DaysRemaining: {Days}",
                returnRequest.Id, order.Id, order.TotalAmount, withdrawalDaysRemaining);

            // 7. Return response with next steps
            return new CreateReturnResponseDto
            {
                Success = true,
                ReturnRequestId = returnRequest.Id,
                OrderId = order.Id,
                WithdrawalAmount = order.TotalAmount,
                WithdrawalDaysRemaining = withdrawalDaysRemaining,
                Status = "Requested",
                Message = $"Widerruf erfolgreich eingeleitet. Rückerstattungsbetrag: €{order.TotalAmount:F2}. " +
                          $"Verbleibend: {withdrawalDaysRemaining} Tage.",
                NextSteps = new[]
                {
                    "Bitte senden Sie das Produkt an unsere Rückgabeadresse.",
                    "Eine Rückgabeetikette wird in Kürze per E-Mail versendet.",
                    "Nach Erhalt Ihrer Rückgabe: Prüfung + Rückerstattung innerhalb von 14 Tagen (VVVG §359)."
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "CreateReturn error - Order: {OrderId}, Tenant: {TenantId}",
                request.OrderId, request.TenantId);

            return new CreateReturnResponseDto
            {
                Success = false,
                Error = "INTERNAL_ERROR",
                Message = "Ein Fehler ist bei der Widerrufsanfrage aufgetreten. Bitte versuchen Sie es später erneut."
            };
        }
    }

    /// <summary>
    /// POST /api/returns/refund/create
    /// Erstelle eine Rückerstattung nachdem Rückgabe erhalten wurde
    /// 
    /// Anforderung:
    /// - Return Request muss im Status "Received" sein
    /// - Artikel-Inspektion abgeschlossen
    /// - Rückerstattungsbetrag validiert
    /// 
    /// VVVG §359: Rückerstattung innerhalb 14 Tage nach Erhalt der Rückgabe
    /// </summary>
    public async Task<CreateRefundResponseDto> CreateRefund(
        CreateRefundRequestDto request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "CreateRefund started - ReturnId: {ReturnId}, Tenant: {TenantId}",
            request.ReturnRequestId, request.TenantId);

        try
        {
            // 1. Validate input
            var validation = await _createRefundValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                _logger.LogWarning(
                    "CreateRefund validation failed: {Errors}",
                    string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

                return new CreateRefundResponseDto
                {
                    Success = false,
                    Error = "VALIDATION_ERROR",
                    Message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))
                };
            }

            // 2. Get Return Request
            var returnRequest = await _returnRepository.GetReturnRequestAsync(
                request.TenantId,
                request.ReturnRequestId,
                cancellationToken);

            if (returnRequest == null)
            {
                _logger.LogWarning(
                    "CreateRefund - ReturnRequest not found: {ReturnId}",
                    request.ReturnRequestId);

                return new CreateRefundResponseDto
                {
                    Success = false,
                    Error = "RETURN_NOT_FOUND",
                    Message = "Rückgabeanfrage nicht gefunden."
                };
            }

            // 3. Validate status (must be "Received")
            if (returnRequest.Status != "Received")
            {
                _logger.LogInformation(
                    "CreateRefund - Invalid return status: {Status} (expected: Received)",
                    returnRequest.Status);

                return new CreateRefundResponseDto
                {
                    Success = false,
                    Error = "INVALID_RETURN_STATUS",
                    Message = $"Rückgabe befindet sich im Status '{returnRequest.Status}', kann aber nur im Status 'Received' rückerstellt werden."
                };
            }

            // 4. Get associated Order (verify VVVG §359 timeline)
            var order = await _orderRepository.GetOrderAsync(
                request.TenantId,
                returnRequest.OrderId,
                cancellationToken);

            if (order == null)
            {
                _logger.LogError(
                    "CreateRefund - Order not found for Return: {ReturnId}",
                    request.ReturnRequestId);

                return new CreateRefundResponseDto
                {
                    Success = false,
                    Error = "ORDER_NOT_FOUND",
                    Message = "Zugehörige Bestellung nicht gefunden."
                };
            }

            // 5. Check for existing refund (prevent duplicates)
            var hasExistingRefund = await _refundRepository.HasRefundForReturnAsync(
                request.TenantId,
                request.ReturnRequestId,
                cancellationToken);

            if (hasExistingRefund)
            {
                _logger.LogInformation(
                    "CreateRefund - Refund already exists for ReturnRequest: {ReturnId}",
                    request.ReturnRequestId);

                return new CreateRefundResponseDto
                {
                    Success = false,
                    Error = "REFUND_ALREADY_EXISTS",
                    Message = "Für diese Rückgabe existiert bereits eine Rückerstattung."
                };
            }

            // 6. Create Refund via service (validates amount, initiates processing)
            var refund = await _returnManagementService.InitializeRefundAsync(
                request.TenantId,
                returnRequest,
                order,
                request.ApprovedRefundAmount,
                request.InspectionNotes,
                cancellationToken);

            // 7. Persist Refund
            await _refundRepository.CreateRefundAsync(
                request.TenantId,
                refund,
                cancellationToken);

            // 8. Update Return status to "Refunded" (optional, depends on workflow)
            await _returnRepository.UpdateReturnRequestStatusAsync(
                request.TenantId,
                request.ReturnRequestId,
                "Refunded",
                cancellationToken);

            _logger.LogInformation(
                "CreateRefund successful - RefundId: {RefundId}, ReturnId: {ReturnId}, Amount: {Amount}",
                refund.Id, request.ReturnRequestId, refund.RefundAmount);

            // 9. Return response with payment instructions
            return new CreateRefundResponseDto
            {
                Success = true,
                RefundId = refund.Id,
                ReturnRequestId = request.ReturnRequestId,
                RefundAmount = refund.RefundAmount,
                Status = "Initiated",
                Message = $"Rückerstattung eingeleitet: €{refund.RefundAmount:F2}. " +
                          $"Überweisung erfolgt in Kürze.",
                ExpectedRefundDate = DateTime.UtcNow.AddDays(3),  // Typically 2-3 business days
                RefundInstructions = new[]
                {
                    "Rückerstattung wird auf das ursprüngliche Zahlungsmittel übertragen.",
                    "Bearbeitungszeit: 2-3 Arbeitstage.",
                    "Referenz-ID: " + refund.Id.ToString().Substring(0, 8).ToUpper()
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "CreateRefund error - ReturnId: {ReturnId}, Tenant: {TenantId}",
                request.ReturnRequestId, request.TenantId);

            return new CreateRefundResponseDto
            {
                Success = false,
                Error = "INTERNAL_ERROR",
                Message = "Ein Fehler bei der Rückerstattung ist aufgetreten. Bitte versuchen Sie es später erneut."
            };
        }
    }

    /// <summary>
    /// GET /api/returns/{returnId}
    /// Abrufen des Return-Status und Refund-Informationen
    /// </summary>
    public async Task<ReturnStatusResponseDto> GetReturnStatus(
        Guid tenantId,
        Guid returnId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetReturnStatus - ReturnId: {ReturnId}", returnId);

        try
        {
            var returnRequest = await _returnRepository.GetReturnRequestAsync(
                tenantId,
                returnId,
                cancellationToken);

            if (returnRequest == null)
            {
                _logger.LogWarning("GetReturnStatus - ReturnRequest not found: {ReturnId}", returnId);
                return new ReturnStatusResponseDto
                {
                    Success = false,
                    Error = "NOT_FOUND",
                    Message = "Rückgabeanfrage nicht gefunden."
                };
            }

            // Get associated refund (if exists)
            var order = await _orderRepository.GetOrderAsync(
                tenantId,
                returnRequest.OrderId,
                cancellationToken);

            var withdrawalDaysRemaining = order != null
                ? await _orderRepository.GetWithdrawalDaysRemainingAsync(tenantId, order.Id, cancellationToken)
                : 0;

            return new ReturnStatusResponseDto
            {
                Success = true,
                ReturnId = returnRequest.Id,
                OrderId = returnRequest.OrderId,
                Status = returnRequest.Status,
                WithdrawalAmount = returnRequest.WithdrawalAmount,
                WithdrawalReason = returnRequest.WithdrawalReason,
                ReturnLabelNumber = returnRequest.ReturnLabelNumber,
                WithdrawalDaysRemaining = Math.Max(0, withdrawalDaysRemaining),
                CreatedAt = returnRequest.CreatedAt,
                ReturnLabelIssuedAt = returnRequest.ReturnLabelIssuedAt,
                ReturnReceivedAt = returnRequest.ReturnReceivedAt,
                Timeline = GenerateTimeline(returnRequest),
                Message = $"Status: {returnRequest.Status}. Verbleibende Tage: {Math.Max(0, withdrawalDaysRemaining)}."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetReturnStatus error - ReturnId: {ReturnId}", returnId);
            return new ReturnStatusResponseDto
            {
                Success = false,
                Error = "INTERNAL_ERROR",
                Message = "Ein Fehler ist aufgetreten."
            };
        }
    }

    /// <summary>
    /// GET /api/returns/order/{orderId}
    /// Abrufen aller Returns für eine Bestellung
    /// </summary>
    public async Task<OrderReturnsResponseDto> GetOrderReturns(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetOrderReturns - OrderId: {OrderId}", orderId);

        try
        {
            var order = await _orderRepository.GetOrderAsync(tenantId, orderId, cancellationToken);
            if (order == null)
            {
                return new OrderReturnsResponseDto
                {
                    Success = false,
                    Error = "ORDER_NOT_FOUND",
                    Message = "Bestellung nicht gefunden."
                };
            }

            var returns = await _returnRepository.GetOrderReturnRequestsAsync(
                tenantId,
                orderId,
                cancellationToken);

            return new OrderReturnsResponseDto
            {
                Success = true,
                OrderId = orderId,
                TotalReturns = returns.Count,
                WithdrawalDaysRemaining = await _orderRepository.GetWithdrawalDaysRemainingAsync(tenantId, orderId, cancellationToken),
                Returns = returns.Select(r => new ReturnSummaryDto
                {
                    ReturnId = r.Id,
                    Status = r.Status,
                    WithdrawalAmount = r.WithdrawalAmount,
                    CreatedAt = r.CreatedAt
                }).ToList(),
                Message = $"{returns.Count} Rückgabe(n) für diese Bestellung."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOrderReturns error - OrderId: {OrderId}", orderId);
            return new OrderReturnsResponseDto
            {
                Success = false,
                Error = "INTERNAL_ERROR",
                Message = "Ein Fehler ist aufgetreten."
            };
        }
    }

    /// <summary>
    /// Helper: Generiere Timeline für Return-Status
    /// </summary>
    private static List<TimelineItemDto> GenerateTimeline(ReturnRequest returnRequest)
    {
        var timeline = new List<TimelineItemDto>
        {
            new TimelineItemDto
            {
                Status = "Requested",
                Timestamp = returnRequest.CreatedAt,
                Description = "Widerruf eingeleitet",
                IsCompleted = true
            }
        };

        if (returnRequest.ReturnLabelIssuedAt.HasValue)
        {
            timeline.Add(new TimelineItemDto
            {
                Status = "ReturnLabelSent",
                Timestamp = returnRequest.ReturnLabelIssuedAt.Value,
                Description = "Rückgabeetikette erstellt",
                IsCompleted = true
            });
        }

        if (!string.IsNullOrEmpty(returnRequest.ReturnLabelNumber))
        {
            timeline.Add(new TimelineItemDto
            {
                Status = "InTransit",
                Timestamp = returnRequest.ReturnLabelIssuedAt ?? DateTime.UtcNow,
                Description = $"In Rückversand (#{returnRequest.ReturnLabelNumber})",
                IsCompleted = returnRequest.ReturnReceivedAt.HasValue
            });
        }

        if (returnRequest.ReturnReceivedAt.HasValue)
        {
            timeline.Add(new TimelineItemDto
            {
                Status = "Received",
                Timestamp = returnRequest.ReturnReceivedAt.Value,
                Description = "Rückgabe empfangen",
                IsCompleted = true
            });
        }

        if (returnRequest.Status == "Refunded")
        {
            timeline.Add(new TimelineItemDto
            {
                Status = "Refunded",
                Timestamp = DateTime.UtcNow,
                Description = "Rückerstattung verarbeitet",
                IsCompleted = true
            });
        }

        return timeline;
    }
}
