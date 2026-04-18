namespace B2X.Catalog.Models;

/// <summary>
/// Represents a customer's acceptance of Terms & Conditions
/// Auditable entity: stored for legal compliance and dispute resolution
/// </summary>
public class TermsAcceptanceLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tenant ID for multi-tenancy isolation
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Customer ID who accepted the terms
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Version hash of Terms & Conditions accepted
    /// Allows tracking if customer accepted different version
    /// </summary>
    public string TermsVersion { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when Terms were accepted
    /// </summary>
    public DateTime AcceptedAt { get; set; }

    /// <summary>
    /// Flag: Customer accepted Terms & Conditions
    /// </summary>
    public bool AcceptedTermsAndConditions { get; set; }

    /// <summary>
    /// Flag: Customer accepted Privacy Policy
    /// </summary>
    public bool AcceptedPrivacyPolicy { get; set; }

    /// <summary>
    /// Flag: Customer (B2C only) understood 14-day withdrawal right
    /// </summary>
    public bool AcceptedWithdrawalRight { get; set; }

    /// <summary>
    /// IP address of accepting user (audit trail)
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// User agent of accepting browser (audit trail)
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// Order ID this acceptance is associated with (nullable, could accept terms outside checkout)
    /// </summary>
    public string? OrderId { get; set; }

    /// <summary>
    /// Audit: Who created this record and when
    /// </summary>
    public string CreatedBy { get; set; } = "system";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Soft delete flag for GDPR compliance
    /// </summary>
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

/// <summary>
/// Request to record terms acceptance during checkout
/// </summary>
public class RecordTermsAcceptanceRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public bool AcceptTermsAndConditions { get; set; }
    public bool AcceptPrivacyPolicy { get; set; }
    public bool AcceptWithdrawalRight { get; set; }
}

/// <summary>
/// Response confirming terms acceptance
/// </summary>
public class RecordTermsAcceptanceResponse
{
    public bool Success { get; set; }
    public Guid AcceptanceLogId { get; set; }
    public string? Error { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Represents available legal documents with versioning
/// </summary>
public class LegalDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Document type: TermsAndConditions, PrivacyPolicy, WithdrawalRights, etc.
    /// </summary>
    public LegalDocumentType Type { get; set; }

    /// <summary>
    /// Tenant ID
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Document version (e.g., "1.0", "2.1")
    /// Incremented when content changes
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Hash of document content for change detection
    /// </summary>
    public string ContentHash { get; set; } = string.Empty;

    /// <summary>
    /// Document content (HTML allowed for formatting)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Is this the current active version?
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When this version becomes effective
    /// </summary>
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last time document content was updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum LegalDocumentType
{
    TermsAndConditions,
    PrivacyPolicy,
    WithdrawalRights,
    Impressum,
    CookiePolicy
}
