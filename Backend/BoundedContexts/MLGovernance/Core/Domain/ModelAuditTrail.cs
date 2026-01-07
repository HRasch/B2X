using B2Connect.Shared.Kernel;

namespace B2Connect.MLGovernance.Core.Domain;

public class ModelAuditTrail : Entity
{
    public Guid ModelId { get; private set; }
    public required string Action { get; set; }
    public required string UserId { get; set; }
    public required string Details { get; set; }
    public DateTime Timestamp { get; private set; }
    public required string IpAddress { get; set; }
    public required string UserAgent { get; set; }

    private ModelAuditTrail() { } // EF

    public ModelAuditTrail(Guid modelId, string action, string userId, string details)
    {
        ModelId = modelId;
        Action = action;
        UserId = userId;
        Details = details;
        Timestamp = DateTime.UtcNow;
        IpAddress = "System"; // Will be set by infrastructure
        UserAgent = "System"; // Will be set by infrastructure
    }
}