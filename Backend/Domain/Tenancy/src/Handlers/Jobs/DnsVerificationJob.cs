using B2Connect.Tenancy.Models;
using B2Connect.Tenancy.Repositories;
using B2Connect.Tenancy.Services;
using Wolverine;

namespace B2Connect.Tenancy.Handlers.Jobs;

/// <summary>
/// Background job that periodically verifies pending domain DNS configurations.
/// Runs every 5 minutes to check domains that are waiting for DNS verification.
/// </summary>
public class DnsVerificationJob(
    ITenantDomainRepository domainRepository,
    IDnsVerificationService dnsVerificationService,
    IDomainLookupService domainLookupService,
    ILogger<DnsVerificationJob> logger)
{
    private readonly ITenantDomainRepository _domainRepository = domainRepository;
    private readonly IDnsVerificationService _dnsVerificationService = dnsVerificationService;
    private readonly IDomainLookupService _domainLookupService = domainLookupService;
    private readonly ILogger<DnsVerificationJob> _logger = logger;

    /// <summary>
    /// Handles the DNS verification job execution.
    /// This method processes all pending domain verifications.
    /// </summary>
    public async Task HandleAsync()
    {
        _logger.LogInformation("Starting DNS verification job for pending domains");

        try
        {
            var pendingDomains = await _domainRepository.GetPendingVerificationDomainsAsync().ConfigureAwait(false);

            if (!pendingDomains.Any())
            {
                _logger.LogInformation("No pending domains found for verification");
                return;
            }

            _logger.LogInformation("Found {Count} pending domains for verification", pendingDomains.Count);

            foreach (var domain in pendingDomains)
            {
                await ProcessDomainVerificationAsync(domain).ConfigureAwait(false);
            }

            _logger.LogInformation("Completed DNS verification job");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during DNS verification job");
            throw new InvalidOperationException("DNS verification job failed", ex);
        }
    }

    private async Task ProcessDomainVerificationAsync(TenantDomain domain)
    {
        try
        {
            _logger.LogDebug("Verifying DNS for domain {DomainName} (ID: {DomainId})",
                domain.DomainName, domain.Id);

            // Attempt DNS verification
            var verificationResult = await _dnsVerificationService.VerifyDomainAsync(
                domain.DomainName, domain.VerificationToken!).ConfigureAwait(false);

            if (verificationResult.IsVerified)
            {
                // Domain verification successful
                domain.MarkAsVerified();
                await _domainRepository.UpdateAsync(domain).ConfigureAwait(false);

                // Invalidate cache to ensure new routing takes effect
                await _domainLookupService.InvalidateCacheAsync(domain.DomainName).ConfigureAwait(false);

                _logger.LogInformation("Domain {DomainName} verification successful",
                    domain.DomainName);
            }
            else
            {
                // Domain verification failed - increment attempt counter
                domain.IncrementVerificationAttempt();
                await _domainRepository.UpdateAsync(domain).ConfigureAwait(false);

                _logger.LogWarning("Domain {DomainName} verification failed (attempt {Attempt})",
                    domain.DomainName, domain.VerificationAttempts);

                // Check if max attempts reached
                if (domain.VerificationAttempts >= 10)
                {
                    domain.VerificationStatus = DomainVerificationStatus.Failed;
                    await _domainRepository.UpdateAsync(domain).ConfigureAwait(false);

                    _logger.LogWarning("Domain {DomainName} marked as failed after {Attempts} attempts",
                        domain.DomainName, domain.VerificationAttempts);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying domain {DomainName}: {Message}",
                domain.DomainName, ex.Message);

            // Increment attempt counter even on exception
            domain.IncrementVerificationAttempt();
            await _domainRepository.UpdateAsync(domain).ConfigureAwait(false);
        }
    }
}
