using DnsClient;
using Microsoft.Extensions.Logging;

namespace B2X.Tenancy.Services;

/// <summary>
/// Service for verifying domain ownership via DNS TXT records.
/// Uses DnsClient library for DNS lookups.
/// </summary>
public class DnsVerificationService : IDnsVerificationService
{
    private readonly ILookupClient _dnsClient;
    private readonly ILogger<DnsVerificationService> _logger;

    private const string TxtRecordPrefix = "_B2X.";

    public DnsVerificationService(
        ILookupClient dnsClient,
        ILogger<DnsVerificationService> logger)
    {
        _dnsClient = dnsClient;
        _logger = logger;
    }

    public async Task<DnsVerificationResult> VerifyDomainAsync(
        string domainName,
        string expectedToken,
        CancellationToken cancellationToken = default)
    {
        var txtRecordName = $"{TxtRecordPrefix}{domainName}";

        _logger.LogInformation("Verifying DNS TXT record for {RecordName}", txtRecordName);

        try
        {
            var result = await _dnsClient.QueryAsync(txtRecordName, QueryType.TXT, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (result.HasError)
            {
                _logger.LogWarning("DNS query error for {RecordName}: {Error}", txtRecordName, result.ErrorMessage);
                return new DnsVerificationResult(
                    false,
                    $"DNS lookup failed: {result.ErrorMessage}");
            }

            var txtRecords = result.Answers.TxtRecords().ToList();

            if (txtRecords.Count == 0)
            {
                _logger.LogWarning("No TXT records found for {RecordName}", txtRecordName);
                return new DnsVerificationResult(
                    false,
                    $"No TXT record found at {txtRecordName}. Please add the verification record to your DNS configuration.");
            }

            // Check each TXT record for the expected token
            foreach (var record in txtRecords)
            {
                var values = record.Text.ToList();
                foreach (var value in values)
                {
                    var trimmedValue = value.Trim();
                    if (string.Equals(trimmedValue, expectedToken, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("DNS verification successful for {Domain}", domainName);
                        return new DnsVerificationResult(true, null, trimmedValue);
                    }
                }
            }

            // Token not found in any TXT record
            var foundValues = string.Join(", ", txtRecords.SelectMany(r => r.Text));
            _logger.LogWarning("TXT record found for {RecordName} but token mismatch. Found: {FoundValues}",
                txtRecordName, foundValues);

            return new DnsVerificationResult(
                false,
                $"TXT record found but verification token does not match. Found: \"{foundValues}\". Expected: \"{expectedToken}\"",
                foundValues);
        }
        catch (DnsResponseException ex)
        {
            _logger.LogError(ex, "DNS response exception for {RecordName}", txtRecordName);
            return new DnsVerificationResult(
                false,
                $"DNS lookup failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during DNS verification for {Domain}", domainName);
            return new DnsVerificationResult(
                false,
                "An unexpected error occurred during DNS verification. Please try again later.");
        }
    }
}
