using B2X.Email.Infrastructure;
using B2X.Email.Models;
using B2X.Email.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.Email.Tests.Services;

public class EmailQueueServiceTests : IAsyncLifetime
{
    private EmailDbContext _dbContext = null!;
    private EmailQueueService _sut = null!;
    private Mock<ILogger<EmailQueueService>> _loggerMock = null!;
    private readonly Guid _tenantId = Guid.NewGuid();

    public async ValueTask InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<EmailDbContext>()
            .UseInMemoryDatabase(databaseName: $"EmailTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new EmailDbContext(options);
        _loggerMock = new Mock<ILogger<EmailQueueService>>();
        _sut = new EmailQueueService(_dbContext, _loggerMock.Object);

        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
    }

    #region QueueEmailAsync Tests

    [Fact]
    public async Task QueueEmailAsync_ValidEmail_ShouldAddToDatabase()
    {
        // Arrange
        var email = CreateTestEmail();

        // Act
        await _sut.QueueEmailAsync(email);

        // Assert
        var savedEmail = await _dbContext.EmailMessages.FindAsync(email.Id);
        savedEmail.ShouldNotBeNull();
        savedEmail.To.ShouldBe(email.To);
        savedEmail.Subject.ShouldBe(email.Subject);
    }

    [Fact]
    public async Task QueueEmailAsync_WithoutScheduledFor_ShouldSetToNow()
    {
        // Arrange
        var email = CreateTestEmail();
        email.ScheduledFor = null;
        var beforeQueue = DateTime.UtcNow;

        // Act
        await _sut.QueueEmailAsync(email);

        // Assert
        var savedEmail = await _dbContext.EmailMessages.FindAsync(email.Id);
        savedEmail.ShouldNotBeNull();
        savedEmail.ScheduledFor.ShouldNotBeNull();
        savedEmail.ScheduledFor.Value.ShouldBeGreaterThanOrEqualTo(beforeQueue);
        savedEmail.Status.ShouldBe(EmailStatus.Queued);
    }

    [Fact]
    public async Task QueueEmailAsync_WithFutureScheduledFor_ShouldSetStatusToScheduled()
    {
        // Arrange
        var email = CreateTestEmail();
        email.ScheduledFor = DateTime.UtcNow.AddHours(1);

        // Act
        await _sut.QueueEmailAsync(email);

        // Assert
        var savedEmail = await _dbContext.EmailMessages.FindAsync(email.Id);
        savedEmail.ShouldNotBeNull();
        savedEmail.Status.ShouldBe(EmailStatus.Scheduled);
    }

    #endregion

    #region GetPendingEmailsAsync Tests

    [Fact]
    public async Task GetPendingEmailsAsync_WithQueuedEmails_ShouldReturnAndMarkAsProcessing()
    {
        // Arrange
        var email1 = CreateTestEmail();
        email1.Status = EmailStatus.Queued;
        email1.ScheduledFor = DateTime.UtcNow.AddMinutes(-5);

        var email2 = CreateTestEmail();
        email2.Status = EmailStatus.Queued;
        email2.ScheduledFor = DateTime.UtcNow.AddMinutes(-10);

        _dbContext.EmailMessages.AddRange(email1, email2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetPendingEmailsAsync(batchSize: 10);

        // Assert
        result.Count.ShouldBe(2);
        result.All(e => e.Status == EmailStatus.Processing).ShouldBeTrue();
    }

    [Fact]
    public async Task GetPendingEmailsAsync_WithFailedEmailsDueForRetry_ShouldReturnThem()
    {
        // Arrange
        var email = CreateTestEmail();
        email.Status = EmailStatus.Failed;
        email.RetryCount = 1;
        email.MaxRetries = 3;
        email.NextRetryAt = DateTime.UtcNow.AddMinutes(-1);

        _dbContext.EmailMessages.Add(email);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetPendingEmailsAsync(batchSize: 10);

        // Assert
        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(email.Id);
    }

    [Fact]
    public async Task GetPendingEmailsAsync_WithFutureScheduledEmails_ShouldNotReturn()
    {
        // Arrange
        var email = CreateTestEmail();
        email.Status = EmailStatus.Scheduled;
        email.ScheduledFor = DateTime.UtcNow.AddHours(1);

        _dbContext.EmailMessages.Add(email);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetPendingEmailsAsync(batchSize: 10);

        // Assert
        result.Count.ShouldBe(0);
    }

    [Fact]
    public async Task GetPendingEmailsAsync_ShouldRespectBatchSize()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            var email = CreateTestEmail();
            email.Status = EmailStatus.Queued;
            email.ScheduledFor = DateTime.UtcNow.AddMinutes(-5);
            _dbContext.EmailMessages.Add(email);
        }
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetPendingEmailsAsync(batchSize: 5);

        // Assert
        result.Count.ShouldBe(5);
    }

    [Fact]
    public async Task GetPendingEmailsAsync_ShouldOrderByPriorityThenCreatedAt()
    {
        // Arrange
        var highPriorityEmail = CreateTestEmail();
        highPriorityEmail.Priority = EmailPriority.High;
        highPriorityEmail.Status = EmailStatus.Queued;
        highPriorityEmail.ScheduledFor = DateTime.UtcNow.AddMinutes(-5);
        highPriorityEmail.CreatedAt = DateTime.UtcNow;

        var normalPriorityEmail = CreateTestEmail();
        normalPriorityEmail.Priority = EmailPriority.Normal;
        normalPriorityEmail.Status = EmailStatus.Queued;
        normalPriorityEmail.ScheduledFor = DateTime.UtcNow.AddMinutes(-5);
        normalPriorityEmail.CreatedAt = DateTime.UtcNow.AddMinutes(-10); // Created earlier

        _dbContext.EmailMessages.AddRange(highPriorityEmail, normalPriorityEmail);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetPendingEmailsAsync(batchSize: 10);

        // Assert
        result.Count.ShouldBe(2);
        result[0].Priority.ShouldBe(EmailPriority.High);
    }

    #endregion

    #region MarkEmailAsSentAsync Tests

    [Fact]
    public async Task MarkEmailAsSentAsync_ValidEmail_ShouldUpdateStatusAndSetSentAt()
    {
        // Arrange
        var email = CreateTestEmail();
        email.Status = EmailStatus.Processing;
        _dbContext.EmailMessages.Add(email);
        await _dbContext.SaveChangesAsync();

        var messageId = "message-123";

        // Act
        await _sut.MarkEmailAsSentAsync(email.Id, messageId);

        // Assert
        var updatedEmail = await _dbContext.EmailMessages.FindAsync(email.Id);
        updatedEmail.ShouldNotBeNull();
        updatedEmail.Status.ShouldBe(EmailStatus.Sent);
        updatedEmail.SentAt.ShouldNotBeNull();
        updatedEmail.MessageId.ShouldBe(messageId);
    }

    [Fact]
    public async Task MarkEmailAsSentAsync_NonExistentEmail_ShouldNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert - should not throw
        await Should.NotThrowAsync(async () =>
            await _sut.MarkEmailAsSentAsync(nonExistentId, "message-123"));
    }

    #endregion

    #region MarkEmailAsFailedAsync Tests

    [Fact]
    public async Task MarkEmailAsFailedAsync_WithRetriesRemaining_ShouldScheduleRetry()
    {
        // Arrange
        var email = CreateTestEmail();
        email.Status = EmailStatus.Processing;
        email.RetryCount = 0;
        email.MaxRetries = 3;
        _dbContext.EmailMessages.Add(email);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.MarkEmailAsFailedAsync(email.Id, "SMTP connection failed");

        // Assert
        var updatedEmail = await _dbContext.EmailMessages.FindAsync(email.Id);
        updatedEmail.ShouldNotBeNull();
        updatedEmail.RetryCount.ShouldBe(1);
        updatedEmail.NextRetryAt.ShouldNotBeNull();
        updatedEmail.LastError.ShouldBe("SMTP connection failed");
    }

    [Fact]
    public async Task MarkEmailAsFailedAsync_MaxRetriesReached_ShouldMarkAsPermanentlyFailed()
    {
        // Arrange
        var email = CreateTestEmail();
        email.Status = EmailStatus.Processing;
        email.RetryCount = 2;
        email.MaxRetries = 3;
        _dbContext.EmailMessages.Add(email);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.MarkEmailAsFailedAsync(email.Id, "Final failure");

        // Assert
        var updatedEmail = await _dbContext.EmailMessages.FindAsync(email.Id);
        updatedEmail.ShouldNotBeNull();
        updatedEmail.RetryCount.ShouldBe(3);
        updatedEmail.NextRetryAt.ShouldBeNull();
        updatedEmail.Status.ShouldBe(EmailStatus.Failed);
    }

    #endregion

    #region RetryEmailAsync Tests

    [Fact]
    public async Task RetryEmailAsync_FailedEmail_ShouldRequeueAndClearError()
    {
        // Arrange
        var email = CreateTestEmail();
        email.Status = EmailStatus.Failed;
        email.LastError = "Previous error";
        _dbContext.EmailMessages.Add(email);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.RetryEmailAsync(email.Id);

        // Assert
        var updatedEmail = await _dbContext.EmailMessages.FindAsync(email.Id);
        updatedEmail.ShouldNotBeNull();
        updatedEmail.Status.ShouldBe(EmailStatus.Queued);
        updatedEmail.LastError.ShouldBeNull();
        updatedEmail.NextRetryAt.ShouldNotBeNull();
    }

    #endregion

    #region CancelEmailAsync Tests

    [Fact]
    public async Task CancelEmailAsync_QueuedEmail_ShouldCancel()
    {
        // Arrange
        var email = CreateTestEmail();
        email.Status = EmailStatus.Queued;
        _dbContext.EmailMessages.Add(email);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.CancelEmailAsync(email.Id);

        // Assert
        var updatedEmail = await _dbContext.EmailMessages.FindAsync(email.Id);
        updatedEmail.ShouldNotBeNull();
        updatedEmail.Status.ShouldBe(EmailStatus.Cancelled);
    }

    [Fact]
    public async Task CancelEmailAsync_AlreadySentEmail_ShouldNotCancel()
    {
        // Arrange
        var email = CreateTestEmail();
        email.Status = EmailStatus.Sent;
        _dbContext.EmailMessages.Add(email);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.CancelEmailAsync(email.Id);

        // Assert
        var updatedEmail = await _dbContext.EmailMessages.FindAsync(email.Id);
        updatedEmail.ShouldNotBeNull();
        updatedEmail.Status.ShouldBe(EmailStatus.Sent); // Should remain sent
    }

    #endregion

    #region GetEmailsForManagementAsync Tests

    [Fact]
    public async Task GetEmailsForManagementAsync_ShouldFilterByTenant()
    {
        // Arrange
        var otherTenantId = Guid.NewGuid();

        var email1 = CreateTestEmail();
        email1.TenantId = _tenantId;

        var email2 = CreateTestEmail();
        email2.TenantId = otherTenantId;

        _dbContext.EmailMessages.AddRange(email1, email2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetEmailsForManagementAsync(_tenantId);

        // Assert
        result.Count.ShouldBe(1);
        result[0].TenantId.ShouldBe(_tenantId);
    }

    [Fact]
    public async Task GetEmailsForManagementAsync_WithStatusFilter_ShouldFilterByStatus()
    {
        // Arrange
        var email1 = CreateTestEmail();
        email1.TenantId = _tenantId;
        email1.Status = EmailStatus.Sent;

        var email2 = CreateTestEmail();
        email2.TenantId = _tenantId;
        email2.Status = EmailStatus.Failed;

        _dbContext.EmailMessages.AddRange(email1, email2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetEmailsForManagementAsync(_tenantId, EmailStatus.Sent);

        // Assert
        result.Count.ShouldBe(1);
        result[0].Status.ShouldBe(EmailStatus.Sent);
    }

    [Fact]
    public async Task GetEmailsForManagementAsync_ShouldSupportPagination()
    {
        // Arrange
        for (int i = 0; i < 20; i++)
        {
            var email = CreateTestEmail();
            email.TenantId = _tenantId;
            _dbContext.EmailMessages.Add(email);
        }
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetEmailsForManagementAsync(_tenantId, skip: 5, take: 10);

        // Assert
        result.Count.ShouldBe(10);
    }

    #endregion

    #region Helper Methods

    private EmailMessage CreateTestEmail()
    {
        return new EmailMessage
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            To = "test@example.com",
            Subject = "Test Subject",
            Body = "<p>Test Body</p>",
            IsHtml = true,
            Priority = EmailPriority.Normal,
            MaxRetries = 3
        };
    }

    #endregion
}
