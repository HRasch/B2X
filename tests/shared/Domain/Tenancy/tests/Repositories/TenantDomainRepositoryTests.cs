using B2X.Tenancy.Models;
using B2X.Tenancy.Repositories;
using B2X.Types.Domain;
using Moq;
using Xunit;

namespace B2X.Tenancy.Tests.Repositories;

public class TenantDomainRepositoryTests
{
    private readonly Mock<ITenantDomainRepository> _mockRepo;

    public TenantDomainRepositoryTests()
    {
        _mockRepo = new Mock<ITenantDomainRepository>();
    }

    [Fact]
    public async Task ResolveTenantIdAsync_WithVerifiedDomain_ReturnsTenantId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var domainName = "test.B2X.de";

        _mockRepo.Setup(r => r.ResolveTenantIdAsync(domainName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenantId);

        // Act
        var result = await _mockRepo.Object.ResolveTenantIdAsync(domainName);

        // Assert
        Assert.Equal(tenantId, result);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_WithUnknownDomain_ReturnsNull()
    {
        // Arrange
        var domainName = "unknown.example.com";

        _mockRepo.Setup(r => r.ResolveTenantIdAsync(domainName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        // Act
        var result = await _mockRepo.Object.ResolveTenantIdAsync(domainName);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTenantIdAsync_ReturnsDomainsSortedByPrimary()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var domains = new List<TenantDomain>
        {
            new() { Id = Guid.NewGuid(), TenantId = tenantId, DomainName = "primary.B2X.de", IsPrimary = true },
            new() { Id = Guid.NewGuid(), TenantId = tenantId, DomainName = "secondary.example.com", IsPrimary = false }
        };

        _mockRepo.Setup(r => r.GetByTenantIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(domains);

        // Act
        var result = await _mockRepo.Object.GetByTenantIdAsync(tenantId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.True(result[0].IsPrimary);
    }

    [Fact]
    public async Task DomainExistsAsync_WithExistingDomain_ReturnsTrue()
    {
        // Arrange
        var domainName = "existing.B2X.de";

        _mockRepo.Setup(r => r.DomainExistsAsync(domainName, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _mockRepo.Object.DomainExistsAsync(domainName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DomainExistsAsync_WithExcludedId_ExcludesFromCheck()
    {
        // Arrange
        var domainName = "existing.B2X.de";
        var excludeId = Guid.NewGuid();

        _mockRepo.Setup(r => r.DomainExistsAsync(domainName, excludeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _mockRepo.Object.DomainExistsAsync(domainName, excludeId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetPendingVerificationDomainsAsync_ReturnsOnlyPendingCustomDomains()
    {
        // Arrange
        var domains = new List<TenantDomain>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                DomainName = "pending.example.com",
                Type = DomainType.CustomDomain,
                VerificationStatus = DomainVerificationStatus.Pending,
                VerificationExpiresAt = DateTime.UtcNow.AddHours(48)
            }
        };

        _mockRepo.Setup(r => r.GetPendingVerificationDomainsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(domains);

        // Act
        var result = await _mockRepo.Object.GetPendingVerificationDomainsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(DomainVerificationStatus.Pending, result[0].VerificationStatus);
    }
}
