using B2Connect.Tenancy.Models;
using Xunit;

namespace B2Connect.Tenancy.Tests.Models;

public class TenantDomainTests
{
    [Fact]
    public void GenerateVerificationToken_CreatesValidToken()
    {
        // Arrange
        var domain = new TenantDomain
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            DomainName = "test.example.com",
            Type = DomainType.CustomDomain
        };

        // Act
        domain.GenerateVerificationToken();

        // Assert
        Assert.NotNull(domain.VerificationToken);
        Assert.StartsWith("b2c-verify-", domain.VerificationToken);
        Assert.Equal(75, domain.VerificationToken.Length); // "b2c-verify-" + 64 hex chars
        Assert.NotNull(domain.VerificationExpiresAt);
        Assert.True(domain.VerificationExpiresAt > DateTime.UtcNow);
        Assert.Equal(DomainVerificationStatus.Pending, domain.VerificationStatus);
    }

    [Fact]
    public void GenerateVerificationToken_WithCustomExpiration_SetsCorrectExpiry()
    {
        // Arrange
        var domain = new TenantDomain
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            DomainName = "test.example.com",
            Type = DomainType.CustomDomain
        };
        var expirationHours = 24;

        // Act
        domain.GenerateVerificationToken(expirationHours);

        // Assert
        Assert.NotNull(domain.VerificationExpiresAt);
        var expectedExpiry = DateTime.UtcNow.AddHours(expirationHours);
        Assert.InRange(domain.VerificationExpiresAt.Value,
            expectedExpiry.AddMinutes(-1),
            expectedExpiry.AddMinutes(1));
    }

    [Fact]
    public void MarkAsVerified_SetsCorrectStatus()
    {
        // Arrange
        var domain = new TenantDomain
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            DomainName = "test.example.com",
            Type = DomainType.CustomDomain
        };
        domain.GenerateVerificationToken();

        // Act
        domain.MarkAsVerified();

        // Assert
        Assert.Equal(DomainVerificationStatus.Verified, domain.VerificationStatus);
        Assert.NotNull(domain.VerifiedAt);
        Assert.Null(domain.VerificationToken); // Token should be cleared
        Assert.Null(domain.VerificationExpiresAt);
    }

    [Fact]
    public void MarkVerificationFailed_SetsCorrectStatus()
    {
        // Arrange
        var domain = new TenantDomain
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            DomainName = "test.example.com",
            Type = DomainType.CustomDomain,
            VerificationStatus = DomainVerificationStatus.Pending
        };

        // Act
        domain.MarkVerificationFailed();

        // Assert
        Assert.Equal(DomainVerificationStatus.Failed, domain.VerificationStatus);
    }

    [Theory]
    [InlineData(DomainVerificationStatus.Verified, SslStatus.Active, true)]
    [InlineData(DomainVerificationStatus.Verified, SslStatus.Provisioning, false)]
    [InlineData(DomainVerificationStatus.Verified, SslStatus.None, false)]
    [InlineData(DomainVerificationStatus.Pending, SslStatus.Active, false)]
    [InlineData(DomainVerificationStatus.Failed, SslStatus.Active, false)]
    public void IsActive_ReturnsCorrectValue(
        DomainVerificationStatus verificationStatus,
        SslStatus sslStatus,
        bool expectedIsActive)
    {
        // Arrange
        var domain = new TenantDomain
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            DomainName = "test.example.com",
            Type = DomainType.CustomDomain,
            VerificationStatus = verificationStatus,
            SslStatus = sslStatus
        };

        // Act & Assert
        Assert.Equal(expectedIsActive, domain.IsActive);
    }

    [Fact]
    public void NewSubdomain_HasCorrectDefaults()
    {
        // Arrange & Act
        var domain = new TenantDomain
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            DomainName = "tenant.b2connect.de",
            Type = DomainType.Subdomain,
            VerificationStatus = DomainVerificationStatus.Verified,
            SslStatus = SslStatus.Active
        };

        // Assert
        Assert.Equal(DomainType.Subdomain, domain.Type);
        Assert.True(domain.IsActive);
    }
}
