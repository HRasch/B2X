using B2Connect.Email.Interfaces;
using B2Connect.Email.Models;
using B2Connect.Email.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Email.Tests.Services;

public class EmailServiceTests
{
    private readonly Mock<IEmailRepository> _repositoryMock;
    private readonly Mock<IEmailProvider> _providerMock;
    private readonly Mock<ILogger<EmailService>> _loggerMock;
    private readonly EmailService _service;

    public EmailServiceTests()
    {
        _repositoryMock = new Mock<IEmailRepository>();
        _providerMock = new Mock<IEmailProvider>();
        _loggerMock = new Mock<ILogger<EmailService>>();
        _service = new EmailService(_repositoryMock.Object, _providerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SendEmailAsync_Success()
    {
        // Arrange
        var message = TestData.CreateValidEmailMessage();
        var providerResult = new EmailProviderResult
        {
            Success = true,
            ExternalMessageId = "ext-123"
        };

        _providerMock.Setup(p => p.IsAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _providerMock.Setup(p => p.SendAsync(message, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResult);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SendEmailAsync(message, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        message.Status.Should().Be(EmailStatus.Sent);
        message.SentAt.Should().NotBeNull();
        message.ExternalMessageId.Should().Be("ext-123");
        message.SendAttempts.Should().Be(1);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _providerMock.Verify(p => p.SendAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ProviderUnavailable()
    {
        // Arrange
        var message = TestData.CreateValidEmailMessage();

        _providerMock.Setup(p => p.IsAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SendEmailAsync(message, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        message.Status.Should().Be(EmailStatus.Deferred);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        _providerMock.Verify(p => p.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SendEmailAsync_RetryableFailure()
    {
        // Arrange
        var message = TestData.CreateValidEmailMessage();
        var providerResult = new EmailProviderResult
        {
            Success = false,
            ErrorMessage = "Temporary failure",
            IsRetryable = true
        };

        _providerMock.Setup(p => p.IsAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _providerMock.Setup(p => p.SendAsync(message, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResult);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SendEmailAsync(message, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        message.Status.Should().Be(EmailStatus.Deferred);
        message.LastError.Should().Be("Temporary failure");
        message.SendAttempts.Should().Be(1);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task SendEmailAsync_PermanentFailure()
    {
        // Arrange
        var message = TestData.CreateValidEmailMessage();
        message.SendAttempts = 3; // Max attempts reached
        var providerResult = new EmailProviderResult
        {
            Success = false,
            ErrorMessage = "Permanent failure",
            IsRetryable = false
        };

        _providerMock.Setup(p => p.IsAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _providerMock.Setup(p => p.SendAsync(message, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResult);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SendEmailAsync(message, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        message.Status.Should().Be(EmailStatus.Failed);
        message.LastError.Should().Be("Permanent failure");
        message.SendAttempts.Should().Be(4);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetEmailAsync_ExistingMessage()
    {
        // Arrange
        var messageId = "msg-123";
        var expectedMessage = TestData.CreateValidEmailMessage();
        expectedMessage.Id = messageId;

        _repositoryMock.Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMessage);

        // Act
        var result = await _service.GetEmailAsync(messageId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(messageId);

        _repositoryMock.Verify(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetEmailAsync_NonExistingMessage()
    {
        // Arrange
        var messageId = "non-existing";

        _repositoryMock.Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmailMessage?)null);

        // Act
        var result = await _service.GetEmailAsync(messageId, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _repositoryMock.Verify(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPendingEmailsAsync_WithLimit()
    {
        // Arrange
        var tenantId = "tenant-123";
        var limit = 50;
        var expectedMessages = new List<EmailMessage>
        {
            TestData.CreateValidEmailMessage(),
            TestData.CreateValidEmailMessage()
        };

        _repositoryMock.Setup(r => r.GetPendingByTenantAsync(tenantId, limit, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMessages);

        // Act
        var result = await _service.GetPendingEmailsAsync(tenantId, limit, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);

        _repositoryMock.Verify(r => r.GetPendingByTenantAsync(tenantId, limit, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RetryFailedEmailsAsync_Success()
    {
        // Arrange
        var tenantId = "tenant-123";
        var failedEmails = new List<EmailMessage>
        {
            TestData.CreateValidEmailMessage(),
            TestData.CreateValidEmailMessage()
        };

        _repositoryMock.Setup(r => r.GetFailedForRetryAsync(tenantId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedEmails);
        _providerMock.Setup(p => p.IsAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _providerMock.Setup(p => p.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailProviderResult { Success = true });
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.RetryFailedEmailsAsync(tenantId, CancellationToken.None);

        // Assert
        result.Should().Be(2);

        _repositoryMock.Verify(r => r.GetFailedForRetryAsync(tenantId, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _providerMock.Verify(p => p.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task UpdateEmailStatusAsync_Success()
    {
        // Arrange
        var messageId = "msg-123";
        var message = TestData.CreateValidEmailMessage();
        var newStatus = EmailStatus.Cancelled;
        var errorMessage = "User cancelled";

        _repositoryMock.Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        _repositoryMock.Setup(r => r.UpdateAsync(message, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateEmailStatusAsync(messageId, newStatus, errorMessage, CancellationToken.None);

        // Assert
        message.Status.Should().Be(newStatus);
        message.LastError.Should().Be(errorMessage);

        _repositoryMock.Verify(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelEmailAsync_PendingEmail()
    {
        // Arrange
        var messageId = "msg-123";
        var message = TestData.CreateValidEmailMessage();
        message.Status = EmailStatus.Pending;

        _repositoryMock.Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        _repositoryMock.Setup(r => r.UpdateAsync(message, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CancelEmailAsync(messageId, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        message.Status.Should().Be(EmailStatus.Cancelled);

        _repositoryMock.Verify(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelEmailAsync_SentEmail()
    {
        // Arrange
        var messageId = "msg-123";
        var message = TestData.CreateValidEmailMessage();
        message.Status = EmailStatus.Sent;

        _repositoryMock.Setup(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);

        // Act
        var result = await _service.CancelEmailAsync(messageId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        message.Status.Should().Be(EmailStatus.Sent); // Should not change

        _repositoryMock.Verify(r => r.GetByIdAsync(messageId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SendEmailAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.SendEmailAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task GetEmailAsync_EmptyMessageId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.GetEmailAsync("", CancellationToken.None));
    }

    [Fact]
    public async Task GetPendingEmailsAsync_EmptyTenantId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.GetPendingEmailsAsync("", cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task UpdateEmailStatusAsync_EmptyMessageId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateEmailStatusAsync("", EmailStatus.Sent, null, CancellationToken.None));
    }

    [Fact]
    public async Task CancelEmailAsync_EmptyMessageId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CancelEmailAsync("", CancellationToken.None));
    }

    [Fact]
    public async Task RetryFailedEmailsAsync_EmptyTenantId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RetryFailedEmailsAsync("", CancellationToken.None));
    }
}