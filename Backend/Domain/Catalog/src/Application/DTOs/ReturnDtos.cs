namespace B2Connect.Catalog.Application.DTOs;

/// <summary>
/// DTO für Return-Anfrage (POST /api/returns/create)
/// </summary>
public class CreateReturnRequestDto
{
    public Guid TenantId { get; set; }
    public Guid OrderId { get; set; }
    public string WithdrawalReason { get; set; }  // "Defective", "NotAsDescribed", "Changed Mind", etc.
    public string ReasonDetails { get; set; }  // Optional: Zusätzliche Details
}

/// <summary>
/// DTO für erfolgreiche Return-Antwort
/// </summary>
public class CreateReturnResponseDto
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public string Message { get; set; }
    
    // Falls erfolgreich:
    public Guid? ReturnRequestId { get; set; }
    public Guid? OrderId { get; set; }
    public decimal? WithdrawalAmount { get; set; }
    public int? WithdrawalDaysRemaining { get; set; }
    public string Status { get; set; }  // "Requested"
    public string[] NextSteps { get; set; }
}

/// <summary>
/// DTO für Refund-Anfrage (POST /api/returns/refund/create)
/// </summary>
public class CreateRefundRequestDto
{
    public Guid TenantId { get; set; }
    public Guid ReturnRequestId { get; set; }
    public decimal ApprovedRefundAmount { get; set; }  // Einrichtung: volle Rückerstattung oder weniger?
    public string InspectionNotes { get; set; }  // Optional: Inspektionsergebnisse
}

/// <summary>
/// DTO für erfolgreiche Refund-Antwort
/// </summary>
public class CreateRefundResponseDto
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public string Message { get; set; }
    
    // Falls erfolgreich:
    public Guid? RefundId { get; set; }
    public Guid? ReturnRequestId { get; set; }
    public decimal? RefundAmount { get; set; }
    public string Status { get; set; }  // "Initiated"
    public DateTime? ExpectedRefundDate { get; set; }
    public string[] RefundInstructions { get; set; }
}

/// <summary>
/// DTO für Return-Status-Abfrage (GET /api/returns/{returnId})
/// </summary>
public class ReturnStatusResponseDto
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public string Message { get; set; }
    
    // Return-Informationen:
    public Guid? ReturnId { get; set; }
    public Guid? OrderId { get; set; }
    public string Status { get; set; }  // "Requested", "ReturnLabelSent", "InTransit", "Received", "Refunded"
    public decimal? WithdrawalAmount { get; set; }
    public string WithdrawalReason { get; set; }
    public string ReturnLabelNumber { get; set; }
    public int WithdrawalDaysRemaining { get; set; }
    
    // Zeitstempel:
    public DateTime? CreatedAt { get; set; }
    public DateTime? ReturnLabelIssuedAt { get; set; }
    public DateTime? ReturnReceivedAt { get; set; }
    
    // Timeline für Customer Portal:
    public List<TimelineItemDto> Timeline { get; set; }
}

/// <summary>
/// DTO für Order-Returns-Abfrage (GET /api/returns/order/{orderId})
/// </summary>
public class OrderReturnsResponseDto
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public string Message { get; set; }
    
    public Guid? OrderId { get; set; }
    public int TotalReturns { get; set; }
    public int WithdrawalDaysRemaining { get; set; }
    public List<ReturnSummaryDto> Returns { get; set; } = new();
}

/// <summary>
/// Summary eines Returns in Liste
/// </summary>
public class ReturnSummaryDto
{
    public Guid ReturnId { get; set; }
    public string Status { get; set; }
    public decimal WithdrawalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Timeline-Item für Return-Status-Anzeige
/// </summary>
public class TimelineItemDto
{
    public string Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}
