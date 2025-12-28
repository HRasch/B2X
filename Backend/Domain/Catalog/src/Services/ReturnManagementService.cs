using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using B2Connect.Catalog.Models;

namespace B2Connect.Catalog.Services;

/// <summary>
/// ReturnManagementService - Handles 14-day withdrawal rights (VVVG §357 ff)
/// Issue #32: P0.6-US-003
/// </summary>
public interface IReturnManagementService
{
    Task<ReturnRequest> CreateReturnRequestAsync(
        Guid tenantId,
        Guid orderId,
        Guid userId,
        string reason,
        bool returnAllItems = true,
        CancellationToken ct = default);
    
    Task<ReturnRequest> GetReturnRequestAsync(Guid returnId, CancellationToken ct = default);
    Task<List<ReturnRequest>> GetOrderReturnsAsync(Guid orderId, CancellationToken ct = default);
    
    Task<bool> ProcessReturnAsync(Guid returnId, CancellationToken ct = default);
    Task<Refund> ProcessRefundAsync(Guid returnId, string refundMethod, CancellationToken ct = default);
    Task<string> GenerateReturnLabelAsync(Guid returnId, string carrierCode, CancellationToken ct = default);
    
    Task<ReturnValidationResult> ValidateReturnAsync(Guid orderId, CancellationToken ct = default);
}

public class ReturnManagementService : IReturnManagementService
{
    private readonly ILogger<ReturnManagementService> _logger;
    private readonly IReturnRepository _returnRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IRefundRepository _refundRepository;
    private readonly IAuditService _auditService;
    
    public ReturnManagementService(
        ILogger<ReturnManagementService> logger,
        IReturnRepository returnRepository,
        IOrderRepository orderRepository,
        IRefundRepository refundRepository,
        IAuditService auditService)
    {
        _logger = logger;
        _returnRepository = returnRepository;
        _orderRepository = orderRepository;
        _refundRepository = refundRepository;
        _auditService = auditService;
    }
    
    /// <summary>
    /// Create a return request (VVVG §357: 14-day withdrawal right)
    /// </summary>
    public async Task<ReturnRequest> CreateReturnRequestAsync(
        Guid tenantId,
        Guid orderId,
        Guid userId,
        string reason,
        bool returnAllItems = true,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Creating return request for Order {OrderId}, Tenant {TenantId}",
            orderId, tenantId);
        
        // 1. Validate order exists and belongs to tenant
        var order = await _orderRepository.GetByIdAsync(orderId, ct);
        if (order == null || order.TenantId != tenantId)
            throw new InvalidOperationException($"Order {orderId} not found for tenant {tenantId}");
        
        // 2. Validate withdrawal period (VVVG §357: 14 days from delivery)
        var validation = await ValidateReturnAsync(orderId, ct);
        if (!validation.IsValid)
        {
            _logger.LogWarning(
                "Return request rejected: {Reason}",
                validation.ErrorMessage);
            throw new InvalidOperationException(validation.ErrorMessage);
        }
        
        // 3. Check if return already exists
        var existingReturns = await _returnRepository.GetByOrderIdAsync(orderId, ct);
        if (existingReturns.Any(r => r.Status != "Rejected"))
            throw new InvalidOperationException($"Active return already exists for order {orderId}");
        
        // 4. Calculate refund amount
        var refundAmount = CalculateRefundAmount(order, returnAllItems);
        
        // 5. Create return request
        var returnRequest = new ReturnRequest
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            OrderId = orderId,
            ReturnNumber = GenerateReturnNumber(tenantId),
            Reason = reason,
            ReturnAllItems = returnAllItems,
            Status = "Requested",
            RefundAmount = refundAmount,
            RefundStatus = "Pending",
            IsWithinWithdrawalPeriod = validation.IsWithinPeriod,
            DaysAfterDelivery = validation.DaysAfterDelivery,
            CreatedBy = userId,
            AuditNotes = $"Return requested by customer. Reason: {reason}. Days after delivery: {validation.DaysAfterDelivery}",
            RequestedAt = DateTime.UtcNow
        };
        
        await _returnRepository.AddAsync(returnRequest, ct);
        
        // 6. Audit log
        await _auditService.LogAsync(
            tenantId: tenantId,
            userId: userId,
            action: "ReturnRequested",
            entityType: "ReturnRequest",
            entityId: returnRequest.Id,
            changes: new
            {
                OrderId = orderId,
                Reason = reason,
                RefundAmount = refundAmount,
                DaysAfterDelivery = validation.DaysAfterDelivery
            },
            ct: ct);
        
        _logger.LogInformation(
            "Return request created: {ReturnNumber}, Refund: {RefundAmount}",
            returnRequest.ReturnNumber, refundAmount);
        
        return returnRequest;
    }
    
    /// <summary>
    /// Validate if return is within VVVG §357 withdrawal period
    /// </summary>
    public async Task<ReturnValidationResult> ValidateReturnAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, ct);
        
        if (order == null)
            return new ReturnValidationResult
            {
                IsValid = false,
                ErrorMessage = "Order not found"
            };
        
        if (!order.DeliveredAt.HasValue)
            return new ReturnValidationResult
            {
                IsValid = false,
                ErrorMessage = "Order not yet delivered"
            };
        
        var daysAfterDelivery = (int)(DateTime.UtcNow - order.DeliveredAt.Value).TotalDays;
        var withdrawalDeadline = order.DeliveredAt.Value.AddDays(14);
        var isWithinPeriod = DateTime.UtcNow <= withdrawalDeadline;
        
        return new ReturnValidationResult
        {
            IsValid = isWithinPeriod,
            IsWithinPeriod = isWithinPeriod,
            DaysAfterDelivery = daysAfterDelivery,
            DaysRemaining = isWithinPeriod ? (int)(withdrawalDeadline - DateTime.UtcNow).TotalDays : 0,
            DeliveryDate = order.DeliveredAt.Value,
            WithdrawalDeadline = withdrawalDeadline,
            ErrorMessage = isWithinPeriod ? null : $"Return period expired (delivered {daysAfterDelivery} days ago)"
        };
    }
    
    /// <summary>
    /// Get return request by ID
    /// </summary>
    public async Task<ReturnRequest> GetReturnRequestAsync(Guid returnId, CancellationToken ct = default)
    {
        return await _returnRepository.GetByIdAsync(returnId, ct);
    }
    
    /// <summary>
    /// Get all returns for an order
    /// </summary>
    public async Task<List<ReturnRequest>> GetOrderReturnsAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _returnRepository.GetByOrderIdAsync(orderId, ct);
    }
    
    /// <summary>
    /// Process return (mark as received)
    /// </summary>
    public async Task<ReturnRequest> ProcessReturnAsync(Guid returnId, CancellationToken ct = default)
    {
        var returnRequest = await _returnRepository.GetByIdAsync(returnId, ct);
        if (returnRequest == null)
            throw new InvalidOperationException($"Return {returnId} not found");
        
        returnRequest.Status = "Received";
        returnRequest.ReturnReceivedAt = DateTime.UtcNow;
        returnRequest.ModifiedAt = DateTime.UtcNow;
        
        await _returnRepository.UpdateAsync(returnRequest, ct);
        
        _logger.LogInformation(
            "Return processed: {ReturnNumber}, Amount: {RefundAmount}",
            returnRequest.ReturnNumber, returnRequest.RefundAmount);
        
        return returnRequest;
    }
    
    /// <summary>
    /// Process refund (VVVG §357: within 14 days of withdrawal)
    /// </summary>
    public async Task<Refund> ProcessRefundAsync(Guid returnId, string refundMethod, CancellationToken ct = default)
    {
        var returnRequest = await _returnRepository.GetByIdAsync(returnId, ct);
        if (returnRequest == null)
            throw new InvalidOperationException($"Return {returnId} not found");
        
        // Create refund record
        var refund = new Refund
        {
            Id = Guid.NewGuid(),
            TenantId = returnRequest.TenantId,
            ReturnRequestId = returnId,
            OrderId = returnRequest.OrderId,
            RefundAmount = returnRequest.RefundAmount,
            RefundMethod = refundMethod,
            Reason = "Withdrawal",
            Status = "Pending",
            ProcessedAt = DateTime.UtcNow,
            ProcessedBy = returnRequest.ModifiedBy ?? returnRequest.CreatedBy,
            AuditLog = System.Text.Json.JsonSerializer.Serialize(new
            {
                CreatedAt = DateTime.UtcNow,
                Method = refundMethod,
                Amount = returnRequest.RefundAmount
            })
        };
        
        await _refundRepository.AddAsync(refund, ct);
        
        // Update return status
        returnRequest.Status = "Refunded";
        returnRequest.RefundStatus = "Processed";
        returnRequest.RefundProcessedAt = DateTime.UtcNow;
        returnRequest.RefundTransactionId = refund.Id.ToString();
        returnRequest.ModifiedAt = DateTime.UtcNow;
        
        await _returnRepository.UpdateAsync(returnRequest, ct);
        
        // Audit log
        await _auditService.LogAsync(
            tenantId: returnRequest.TenantId,
            userId: returnRequest.ModifiedBy ?? returnRequest.CreatedBy,
            action: "RefundProcessed",
            entityType: "Refund",
            entityId: refund.Id,
            changes: new { Amount = refund.RefundAmount, Method = refundMethod },
            ct: ct);
        
        _logger.LogInformation(
            "Refund processed: {RefundNumber}, Amount: {RefundAmount}, Method: {RefundMethod}",
            returnRequest.ReturnNumber, refund.RefundAmount, refundMethod);
        
        return refund;
    }
    
    /// <summary>
    /// Generate return shipping label (placeholder - integrates with carrier API)
    /// </summary>
    public async Task<string> GenerateReturnLabelAsync(Guid returnId, string carrierCode, CancellationToken ct = default)
    {
        var returnRequest = await _returnRepository.GetByIdAsync(returnId, ct);
        if (returnRequest == null)
            throw new InvalidOperationException($"Return {returnId} not found");
        
        // TODO: Integrate with carrier API (DHL, DPD, Deutsche Post)
        // For now, generate a mock label URL
        var labelUrl = $"https://returns.b2connect.local/{returnId}/label.pdf";
        
        returnRequest.ReturnCarrier = carrierCode;
        returnRequest.ReturnLabelUrl = labelUrl;
        returnRequest.ReturnLabelGeneratedAt = DateTime.UtcNow;
        returnRequest.Status = "ReturnLabelSent";
        returnRequest.ModifiedAt = DateTime.UtcNow;
        
        await _returnRepository.UpdateAsync(returnRequest, ct);
        
        _logger.LogInformation(
            "Return label generated: {ReturnNumber}, Carrier: {Carrier}",
            returnRequest.ReturnNumber, carrierCode);
        
        return labelUrl;
    }
    
    // ========== Private Helpers ==========
    
    private decimal CalculateRefundAmount(Order order, bool returnAllItems)
    {
        if (returnAllItems)
        {
            // Full refund: order total (including tax and shipping)
            return order.TotalPrice;
        }
        else
        {
            // Partial refund: calculated based on returned items
            // TODO: Implement partial return logic
            return order.TotalPrice;
        }
    }
    
    private string GenerateReturnNumber(Guid tenantId)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        return $"RET-{timestamp}-{random}";
    }
}

/// <summary>
/// Return validation result
/// </summary>
public class ReturnValidationResult
{
    public bool IsValid { get; set; }
    public bool IsWithinPeriod { get; set; }
    public int DaysAfterDelivery { get; set; }
    public int DaysRemaining { get; set; }
    public DateTime DeliveryDate { get; set; }
    public DateTime WithdrawalDeadline { get; set; }
    public string ErrorMessage { get; set; }
}

// Repository interfaces (implementation in Infrastructure layer)
public interface IReturnRepository
{
    Task<ReturnRequest> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<ReturnRequest>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task AddAsync(ReturnRequest returnRequest, CancellationToken ct = default);
    Task UpdateAsync(ReturnRequest returnRequest, CancellationToken ct = default);
}

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
}

public interface IRefundRepository
{
    Task<Refund> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Refund refund, CancellationToken ct = default);
    Task UpdateAsync(Refund refund, CancellationToken ct = default);
}

public interface IAuditService
{
    Task LogAsync(
        Guid tenantId,
        Guid userId,
        string action,
        string entityType,
        Guid entityId,
        object changes,
        CancellationToken ct = default);
}
