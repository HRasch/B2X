using B2X.Types.Domain;

namespace B2X.Tenancy.Models;

/// <summary>
/// Represents a domain associated with a tenant.
/// Each tenant can have multiple domains (one primary, rest secondary).
/// Supports both subdomains (*.B2X.de) and custom domains.
/// </summary>
public class TenantDomain : Entity
{
    /// <summary>
    /// Reference to the owning tenant.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// The full domain name (e.g., "shop.B2X.de" or "store.example.com").
    /// Must be unique across all tenants.
    /// </summary>
    public required string DomainName { get; set; }

    /// <summary>
    /// Type of domain: Subdomain (automatic) or CustomDomain (requires verification).
    /// </summary>
    public DomainType Type { get; set; } = DomainType.Subdomain;

    /// <summary>
    /// Whether this is the primary domain for the tenant.
    /// Only one domain per tenant can be primary.
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Verification status for custom domains.
    /// Subdomains are automatically verified.
    /// </summary>
    public DomainVerificationStatus VerificationStatus { get; set; } = DomainVerificationStatus.Pending;

    /// <summary>
    /// Token for DNS TXT record verification.
    /// Format: "b2c-verify-{random-hex}"
    /// </summary>
    public string? VerificationToken { get; set; }

    /// <summary>
    /// When the verification token expires.
    /// Default: 72 hours from creation.
    /// </summary>
    public DateTime? VerificationExpiresAt { get; set; }

    /// <summary>
    /// When the domain was successfully verified.
    /// </summary>
    public DateTime? VerifiedAt { get; set; }

    /// <summary>
    /// SSL certificate status for the domain.
    /// </summary>
    public SslStatus SslStatus { get; set; } = SslStatus.None;

    /// <summary>
    /// Number of verification attempts made.
    /// Used to limit retry attempts and prevent abuse.
    /// </summary>
    public int VerificationAttempts { get; set; }

    /// <summary>
    /// When the domain was last checked for verification.
    /// </summary>
    public DateTime? LastVerificationCheck { get; set; }

    /// <summary>
    /// Navigation property to the tenant.
    /// </summary>
    public virtual Tenant? Tenant { get; set; }

    /// <summary>
    /// Checks if the domain is ready to serve traffic.
    /// </summary>
    public bool IsActive => VerificationStatus == DomainVerificationStatus.Verified
                            && SslStatus == SslStatus.Active;

    /// <summary>
    /// Generates a new verification token for custom domain verification.
    /// </summary>
    public void GenerateVerificationToken(int expirationHours = 72)
    {
        var randomBytes = new byte[32];
        System.Security.Cryptography.RandomNumberGenerator.Fill(randomBytes);
        VerificationToken = $"b2c-verify-{Convert.ToHexString(randomBytes).ToLowerInvariant()}";
        VerificationExpiresAt = DateTime.UtcNow.AddHours(expirationHours);
        VerificationStatus = DomainVerificationStatus.Pending;
    }

    /// <summary>
    /// Marks the domain as verified.
    /// </summary>
    public void MarkAsVerified()
    {
        VerificationStatus = DomainVerificationStatus.Verified;
        VerifiedAt = DateTime.UtcNow;
        VerificationToken = null; // Clear token after successful verification
        VerificationExpiresAt = null;
        VerificationAttempts = 0; // Reset attempts on success
        LastVerificationCheck = DateTime.UtcNow;
    }

    /// <summary>
    /// Increments the verification attempt counter.
    /// </summary>
    public void IncrementVerificationAttempt()
    {
        VerificationAttempts++;
        LastVerificationCheck = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the domain verification as failed.
    /// </summary>
    public void MarkVerificationFailed()
    {
        VerificationStatus = DomainVerificationStatus.Failed;
    }
}

/// <summary>
/// Type of domain configuration.
/// </summary>
public enum DomainType
{
    /// <summary>
    /// Automatic subdomain under *.B2X.de.
    /// No verification required.
    /// </summary>
    Subdomain = 0,

    /// <summary>
    /// Custom domain owned by the tenant.
    /// Requires DNS verification.
    /// </summary>
    CustomDomain = 1
}

/// <summary>
/// Status of domain ownership verification.
/// </summary>
public enum DomainVerificationStatus
{
    /// <summary>
    /// Verification not yet attempted or in progress.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Domain ownership verified via DNS TXT record.
    /// </summary>
    Verified = 1,

    /// <summary>
    /// Verification failed (DNS record not found or mismatch).
    /// </summary>
    Failed = 2
}

/// <summary>
/// Status of SSL certificate for the domain.
/// </summary>
public enum SslStatus
{
    /// <summary>
    /// No SSL certificate configured.
    /// </summary>
    None = 0,

    /// <summary>
    /// SSL certificate is being provisioned (Let's Encrypt).
    /// </summary>
    Provisioning = 1,

    /// <summary>
    /// SSL certificate is active and valid.
    /// </summary>
    Active = 2,

    /// <summary>
    /// SSL certificate has expired.
    /// </summary>
    Expired = 3
}
