using B2Connect.Tenancy.Repositories;
using B2Connect.Tenancy.Services;
using Wolverine;

namespace B2Connect.Tenancy.Handlers.Jobs;

/// <summary>
/// Background job that periodically verifies pending domain DNS configurations.
/// Runs every 5 minutes to check domains that are waiting for DNS verification.
/// </summary>
public class DnsVerificationJob
{
    /// <summary>
    /// Scheduled job that runs every 5 minutes to verify pending domain DNS configurations.
    /// </summary>
    [ScheduledJob("*/5 * * * *")] // Every 5 minutes
    public static async Task CheckPendingVerifications(
        ITenantDomainRepository domainRepository,
        IDnsVerificationService dnsVerificationService,
        IDomainLookupService domainLookupService,
        ILogger<DnsVerificationJob> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting DNS verification job for pending domains");

        try
        {
            // Get all domains with pending verification status
            var pendingDomains = await domainRepository.GetPendingVerificationAsync(cancellationToken);

            if (!pendingDomains.Any())
            {
                logger.LogDebug("No pending domains found for verification");
                return;
            }

            logger.LogInformation("Found {Count} pending domains for verification", pendingDomains.Count);

            var successCount = 0;
            var failureCount = 0;

            foreach (var domain in pendingDomains)
            {
                try
                {
                    logger.LogDebug("Verifying DNS for domain {Domain} (ID: {DomainId})",
                        domain.DomainName, domain.Id);

                    // Verify DNS configuration
                    var verificationResult = await dnsVerificationService.VerifyDomainAsync(
                        domain.DomainName, domain.VerificationToken, cancellationToken);

                    if (verificationResult.IsVerified)
                    {
                        // Mark domain as verified
                        domain.MarkAsVerified();
                        await domainRepository.UpdateAsync(domain, cancellationToken);

                        // Invalidate cache to ensure new domain is immediately available
                        await domainLookupService.InvalidateCacheAsync(domain.DomainName, cancellationToken);

                        logger.LogInformation("Domain {Domain} successfully verified", domain.DomainName);
                        successCount++;

                        // TODO: Send success notification to tenant admin
                    }
                    else if (domain.VerificationAttempts >= 10) // Max attempts reached
                    {
                        // Mark as failed after max attempts
                        domain.MarkVerificationFailed();
                        await domainRepository.UpdateAsync(domain, cancellationToken);

                        logger.LogWarning("Domain {Domain} verification failed after {Attempts} attempts",
                            domain.DomainName, domain.VerificationAttempts);
                        failureCount++;

                        // TODO: Send failure notification to tenant admin
                    }
                    else
                    {
                        // Increment attempt counter
                        domain.IncrementVerificationAttempt();
                        await domainRepository.UpdateAsync(domain, cancellationToken);

                        logger.LogDebug("Domain {Domain} verification attempt {Attempt} failed, will retry",
                            domain.DomainName, domain.VerificationAttempts);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error verifying domain {Domain}: {Message}",
                        domain.DomainName, ex.Message);
                    failureCount++;
                }
            }

            logger.LogInformation("DNS verification job completed. Success: {Success}, Failures: {Failures}",
                successCount, failureCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DNS verification job failed: {Message}", ex.Message);
            throw; // Let Wolverine handle retry logic
        }
    }
}