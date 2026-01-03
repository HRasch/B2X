using B2Connect.Tenancy.Handlers.Jobs;
using B2Connect.Tenancy.Models;
using B2Connect.Tenancy.Repositories;
using B2Connect.Tenancy.Services;
using B2Connect.Types.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Wolverine;
using Xunit;

namespace B2Connect.Tenancy.Tests.Jobs;

/// <summary>
/// Unit tests for DnsVerificationJob.
/// </summary>
public class DnsVerificationJobTests
{
    private readonly Mock<ITenantDomainRepository> _domainRepositoryMock;
    private readonly Mock<IDnsVerificationService> _dnsServiceMock;
    private readonly Mock<IDomainLookupService> _lookupServiceMock;
    private readonly Mock<ILogger<DnsVerificationJob>> _loggerMock;
    private readonly DnsVerificationJob _job;

    public DnsVerificationJobTests()
    {
        _domainRepositoryMock = new Mock<ITenantDomainRepository>();
        _dnsServiceMock = new Mock<IDnsVerificationService>();
        _lookupServiceMock = new Mock<IDomainLookupService>();
        _loggerMock = new Mock<ILogger<DnsVerificationJob>>();

        _job = new DnsVerificationJob(
            _domainRepositoryMock.Object,
            _dnsServiceMock.Object,
            _lookupServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_NoPendingDomains_DoesNothing()
    {
        // Arrange
        _domainRepositoryMock
            .Setup(r => r.GetPendingVerificationDomainsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TenantDomain>());

        // Act
        await _job.HandleAsync();

        // Assert
        _dnsServiceMock.Verify(s => s.VerifyDomainAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _lookupServiceMock.Verify(s => s.InvalidateCacheAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_DomainVerificationSucceeds_UpdatesDomainAndInvalidatesCache()
    {
        // Arrange
        var domain = CreatePendingDomain();
        var domains = new List<TenantDomain> { domain };

        _domainRepositoryMock
            .Setup(r => r.GetPendingVerificationDomainsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(domains);

        _dnsServiceMock
            .Setup(s => s.VerifyDomainAsync(domain.DomainName, domain.VerificationToken!))
            .ReturnsAsync(new DnsVerificationResult(true));

        _domainRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TenantDomain>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantDomain d, CancellationToken _) => d);

        // Act
        await _job.HandleAsync();

        // Assert
        domain.VerificationStatus.ShouldBe(DomainVerificationStatus.Verified);
        domain.VerifiedAt.ShouldNotBeNull();
        domain.VerificationToken.ShouldBeNull();
        domain.VerificationExpiresAt.ShouldBeNull();
        domain.VerificationAttempts.ShouldBe(0);
        domain.LastVerificationCheck.ShouldNotBeNull();

        _domainRepositoryMock.Verify(r => r.UpdateAsync(domain, It.IsAny<CancellationToken>()), Times.Once);
        _lookupServiceMock.Verify(s => s.InvalidateCacheAsync(domain.DomainName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DomainVerificationFails_IncrementsAttempts()
    {
        // Arrange
        var domain = CreatePendingDomain();
        var initialAttempts = domain.VerificationAttempts;
        var domains = new List<TenantDomain> { domain };

        _domainRepositoryMock
            .Setup(r => r.GetPendingVerificationDomainsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(domains);

        _dnsServiceMock
            .Setup(s => s.VerifyDomainAsync(domain.DomainName, domain.VerificationToken!))
            .ReturnsAsync(new DnsVerificationResult(false, "Verification failed"));

        _domainRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TenantDomain>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantDomain d, CancellationToken _) => d);

        // Act
        await _job.HandleAsync();

        // Assert
        domain.VerificationStatus.ShouldBe(DomainVerificationStatus.Pending);
        domain.VerificationAttempts.ShouldBe(initialAttempts + 1);
        domain.LastVerificationCheck.ShouldNotBeNull();

        _domainRepositoryMock.Verify(r => r.UpdateAsync(domain, It.IsAny<CancellationToken>()), Times.Once);
        _lookupServiceMock.Verify(s => s.InvalidateCacheAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_DomainVerificationFailsMaxAttempts_MarksAsFailed()
    {
        // Arrange
        var domain = CreatePendingDomain();
        domain.VerificationAttempts = 9; // One attempt away from max
        var domains = new List<TenantDomain> { domain };

        _domainRepositoryMock
            .Setup(r => r.GetPendingVerificationDomainsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(domains);

        _dnsServiceMock
            .Setup(s => s.VerifyDomainAsync(domain.DomainName, domain.VerificationToken!))
            .ReturnsAsync(new DnsVerificationResult(false, "Verification failed"));

        _domainRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TenantDomain>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantDomain d, CancellationToken _) => d);

        // Act
        await _job.HandleAsync();

        // Assert
        domain.VerificationStatus.ShouldBe(DomainVerificationStatus.Failed);
        domain.VerificationAttempts.ShouldBe(10);
        domain.LastVerificationCheck.ShouldNotBeNull();

        _domainRepositoryMock.Verify(r => r.UpdateAsync(domain, It.IsAny<CancellationToken>()), Times.Exactly(2));
        _lookupServiceMock.Verify(s => s.InvalidateCacheAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_DnsServiceThrowsException_ContinuesProcessing()
    {
        // Arrange
        var domain1 = CreatePendingDomain();
        var domain2 = CreatePendingDomain();
        var domains = new List<TenantDomain> { domain1, domain2 };

        _domainRepositoryMock
            .Setup(r => r.GetPendingVerificationDomainsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(domains);

        _dnsServiceMock
            .Setup(s => s.VerifyDomainAsync(domain1.DomainName, domain1.VerificationToken!))
#pragma warning disable CA2201 // Do not raise reserved exception types
            .ThrowsAsync(new Exception("DNS lookup failed"));
#pragma warning restore CA2201 // Do not raise reserved exception types

        _dnsServiceMock
            .Setup(s => s.VerifyDomainAsync(domain2.DomainName, domain2.VerificationToken!))
            .ReturnsAsync(new DnsVerificationResult(true));

        _domainRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TenantDomain>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantDomain d, CancellationToken _) => d);

        // Act
        await _job.HandleAsync();

        // Assert
        // domain1 should have incremented attempts due to exception
        domain1.VerificationAttempts.ShouldBe(1);
        domain1.LastVerificationCheck.ShouldNotBeNull();

        // domain2 should be verified
        domain2.VerificationStatus.ShouldBe(DomainVerificationStatus.Verified);
        domain2.VerificationAttempts.ShouldBe(0);

        _domainRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TenantDomain>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task HandleAsync_ExpiredDomain_StillAttemptsVerification()
    {
        // Arrange - Note: In reality, expired domains should be filtered by repository
        // This test verifies job behavior if expired domain is somehow returned
        var domain = CreatePendingDomain();
        domain.VerificationExpiresAt = DateTime.UtcNow.AddMinutes(-1); // Expired
        var domains = new List<TenantDomain> { domain };

        _domainRepositoryMock
            .Setup(r => r.GetPendingVerificationDomainsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(domains);

        _dnsServiceMock
            .Setup(s => s.VerifyDomainAsync(domain.DomainName, domain.VerificationToken!))
            .ReturnsAsync(new DnsVerificationResult(false, "Verification failed"));

        _domainRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TenantDomain>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantDomain d, CancellationToken _) => d);

        // Act
        await _job.HandleAsync();

        // Assert - Job still processes domains returned by repository
        _dnsServiceMock.Verify(s => s.VerifyDomainAsync(domain.DomainName, domain.VerificationToken!), Times.Once);
        _domainRepositoryMock.Verify(r => r.UpdateAsync(domain, It.IsAny<CancellationToken>()), Times.Once);
    }

    private TenantDomain CreatePendingDomain()
    {
        return new TenantDomain
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            DomainName = $"test-{Guid.NewGuid().ToString().Substring(0, 8)}.example.com",
            Type = DomainType.CustomDomain,
            VerificationStatus = DomainVerificationStatus.Pending,
            VerificationToken = $"token-{Guid.NewGuid().ToString().Substring(0, 8)}",
            VerificationExpiresAt = DateTime.UtcNow.AddDays(7),
            VerificationAttempts = 0,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
    }
}