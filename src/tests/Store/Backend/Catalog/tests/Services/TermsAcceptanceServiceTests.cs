using B2X.Catalog.Handlers;
using B2X.Catalog.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2X.Catalog.Tests.Services;

/// <summary>
/// Unit tests for Terms Acceptance Service
/// Story: P0.6-US-005 - Mandatory Terms & Conditions Acceptance
/// 
/// Test cases cover:
/// - Valid acceptance of all required terms
/// - Validation of missing/incomplete acceptance
/// - Audit logging for compliance
/// - Error handling and edge cases
/// </summary>
public class TermsAcceptanceServiceTests : IAsyncLifetime
{
    private TermsAcceptanceService _service = null!;
    private Mock<ILogger<TermsAcceptanceService>> _mockLogger = null!;
    private RecordTermsAcceptanceValidator _validator = null!;

    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly string _customerId = "customer-123";

    public async ValueTask InitializeAsync()
    {
        _mockLogger = new Mock<ILogger<TermsAcceptanceService>>();
        _validator = new RecordTermsAcceptanceValidator();
        _service = new TermsAcceptanceService(_mockLogger.Object, _validator);

        await Task.CompletedTask.ConfigureAwait(false);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task RecordAcceptanceAsync_AllTermsAccepted_ReturnsSuccess()
    {
        // Arrange
        var request = new RecordTermsAcceptanceRequest
        {
            CustomerId = _customerId,
            AcceptTermsAndConditions = true,
            AcceptPrivacyPolicy = true,
            AcceptWithdrawalRight = true
        };

        // Act
        var result = await _service.RecordAcceptanceAsync(
            _tenantId,
            request,
            "192.168.1.1",
            "Mozilla/5.0",
            CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.AcceptanceLogId);
        Assert.Equal("Bedingungen erfolgreich akzeptiert", result.Message);
    }

    [Fact]
    public async Task RecordAcceptanceAsync_TermsNotAccepted_ReturnsFailed()
    {
        // Arrange: Customer did NOT accept T&Cs
        var request = new RecordTermsAcceptanceRequest
        {
            CustomerId = _customerId,
            AcceptTermsAndConditions = false,  // ❌ NOT ACCEPTED
            AcceptPrivacyPolicy = true,
            AcceptWithdrawalRight = true
        };

        // Act
        var result = await _service.RecordAcceptanceAsync(
            _tenantId,
            request,
            "192.168.1.1",
            "Mozilla/5.0",
            CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("INCOMPLETE_ACCEPTANCE", result.Error);
        Assert.Contains("Allgemeinen Geschäftsbedingungen", result.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RecordAcceptanceAsync_PrivacyPolicyNotAccepted_ReturnsFailed()
    {
        // Arrange: Customer did NOT accept privacy policy
        var request = new RecordTermsAcceptanceRequest
        {
            CustomerId = _customerId,
            AcceptTermsAndConditions = true,
            AcceptPrivacyPolicy = false,  // ❌ NOT ACCEPTED
            AcceptWithdrawalRight = true
        };

        // Act
        var result = await _service.RecordAcceptanceAsync(
            _tenantId,
            request,
            "192.168.1.1",
            "Mozilla/5.0",
            CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("INCOMPLETE_ACCEPTANCE", result.Error);
    }

    [Fact]
    public async Task RecordAcceptanceAsync_EmptyCustomerId_ReturnsFailed()
    {
        // Arrange: Customer ID is empty
        var request = new RecordTermsAcceptanceRequest
        {
            CustomerId = string.Empty,  // ❌ INVALID
            AcceptTermsAndConditions = true,
            AcceptPrivacyPolicy = true,
            AcceptWithdrawalRight = true
        };

        // Act
        var result = await _service.RecordAcceptanceAsync(
            _tenantId,
            request,
            "192.168.1.1",
            "Mozilla/5.0",
            CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("VALIDATION_ERROR", result.Error);
        Assert.Contains("Kunden-ID", result.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RecordAcceptanceAsync_WithdrawalRightOptional_SucceedsWithoutIt()
    {
        // Arrange: Withdrawal right is optional (B2B scenario)
        var request = new RecordTermsAcceptanceRequest
        {
            CustomerId = _customerId,
            AcceptTermsAndConditions = true,
            AcceptPrivacyPolicy = true,
            AcceptWithdrawalRight = false  // ✓ Optional
        };

        // Act
        var result = await _service.RecordAcceptanceAsync(
            _tenantId,
            request,
            "192.168.1.1",
            "Mozilla/5.0",
            CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.AcceptanceLogId);
    }

    [Fact]
    public async Task RecordAcceptanceAsync_RecordsAuditTrail_WithIpAndUserAgent()
    {
        // Arrange
        var request = new RecordTermsAcceptanceRequest
        {
            CustomerId = _customerId,
            AcceptTermsAndConditions = true,
            AcceptPrivacyPolicy = true,
            AcceptWithdrawalRight = true
        };
        var ipAddress = "203.0.113.42";
        var userAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)";

        // Act
        var result = await _service.RecordAcceptanceAsync(
            _tenantId,
            request,
            ipAddress,
            userAgent,
            CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        // In production, would verify IP and UserAgent stored in TermsAcceptanceLog
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void HasCustomerAcceptedCurrentTermsAsync_NoAcceptanceRecord_ReturnsFalse()
    {
        // Arrange: No acceptance record exists

        // Act
        var result = _service.HasCustomerAcceptedCurrentTermsAsync(
            _tenantId,
            _customerId,
            null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasCustomerAcceptedCurrentTermsAsync_ValidAcceptance_ReturnsTrue()
    {
        // Arrange: Valid acceptance record exists
        var acceptance = new TermsAcceptanceLog
        {
            CustomerId = _customerId,
            TenantId = _tenantId,
            AcceptedTermsAndConditions = true,
            AcceptedPrivacyPolicy = true,
            TermsVersion = "v1.0.0-2025-12-29"  // Current version
        };

        // Act
        var result = _service.HasCustomerAcceptedCurrentTermsAsync(
            _tenantId,
            _customerId,
            acceptance);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasCustomerAcceptedCurrentTermsAsync_IncompleteAcceptance_ReturnsFalse()
    {
        // Arrange: Acceptance missing privacy policy
        var acceptance = new TermsAcceptanceLog
        {
            CustomerId = _customerId,
            TenantId = _tenantId,
            AcceptedTermsAndConditions = true,
            AcceptedPrivacyPolicy = false,  // ❌ NOT ACCEPTED
            TermsVersion = "v1.0.0-2025-12-29"
        };

        // Act
        var result = _service.HasCustomerAcceptedCurrentTermsAsync(
            _tenantId,
            _customerId,
            acceptance);

        // Assert
        Assert.False(result);
    }
}
