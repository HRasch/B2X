using B2X.Email.Infrastructure;
using B2X.Email.Models;
using B2X.Email.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.Email.Tests.Services;

public class EmailTemplateServiceTests : IAsyncLifetime
{
    private EmailDbContext _dbContext = null!;
    private EmailTemplateService _sut = null!;
    private Mock<ILogger<EmailTemplateService>> _loggerMock = null!;
    private readonly Guid _tenantId = Guid.NewGuid();

    public async ValueTask InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<EmailDbContext>()
            .UseInMemoryDatabase(databaseName: $"EmailTemplateTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new EmailDbContext(options);
        _loggerMock = new Mock<ILogger<EmailTemplateService>>();
        _sut = new EmailTemplateService(_dbContext, _loggerMock.Object);

        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
    }

    #region GetTemplatesAsync Tests

    [Fact]
    public async Task GetTemplatesAsync_WithExistingTemplates_ShouldReturnAll()
    {
        // Arrange
        var template1 = CreateTestTemplate("template-1");
        var template2 = CreateTestTemplate("template-2");
        _dbContext.EmailTemplates.AddRange(template1, template2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetTemplatesAsync(_tenantId);

        // Assert
        result.Count().ShouldBe(2);
    }

    [Fact]
    public async Task GetTemplatesAsync_ShouldFilterByTenant()
    {
        // Arrange
        var otherTenantId = Guid.NewGuid();
        var template1 = CreateTestTemplate("template-1");
        template1.TenantId = _tenantId;

        var template2 = CreateTestTemplate("template-2");
        template2.TenantId = otherTenantId;

        _dbContext.EmailTemplates.AddRange(template1, template2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetTemplatesAsync(_tenantId);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Key.ShouldBe("template-1");
    }

    [Fact]
    public async Task GetTemplatesAsync_ShouldOrderByUpdatedAtDescending()
    {
        // Arrange
        var oldTemplate = CreateTestTemplate("old-template");
        oldTemplate.UpdatedAt = DateTime.UtcNow.AddDays(-1);

        var newTemplate = CreateTestTemplate("new-template");
        newTemplate.UpdatedAt = DateTime.UtcNow;

        _dbContext.EmailTemplates.AddRange(oldTemplate, newTemplate);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = (await _sut.GetTemplatesAsync(_tenantId)).ToList();

        // Assert
        result[0].Key.ShouldBe("new-template");
        result[1].Key.ShouldBe("old-template");
    }

    #endregion

    #region GetTemplateAsync Tests

    [Fact]
    public async Task GetTemplateAsync_WithExistingTemplate_ShouldReturnTemplate()
    {
        // Arrange
        var template = CreateTestTemplate("test-template");
        _dbContext.EmailTemplates.Add(template);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetTemplateAsync(_tenantId, template.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Key.ShouldBe("test-template");
    }

    [Fact]
    public async Task GetTemplateAsync_WithNonExistentTemplate_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetTemplateAsync(_tenantId, nonExistentId);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetTemplateAsync_WithWrongTenant_ShouldReturnNull()
    {
        // Arrange
        var template = CreateTestTemplate("test-template");
        _dbContext.EmailTemplates.Add(template);
        await _dbContext.SaveChangesAsync();

        var otherTenantId = Guid.NewGuid();

        // Act
        var result = await _sut.GetTemplateAsync(otherTenantId, template.Id);

        // Assert
        result.ShouldBeNull();
    }

    #endregion

    #region GetTemplateByKeyAsync Tests

    [Fact]
    public async Task GetTemplateByKeyAsync_WithActiveTemplate_ShouldReturnTemplate()
    {
        // Arrange
        var template = CreateTestTemplate("order-confirmation");
        template.IsActive = true;
        _dbContext.EmailTemplates.Add(template);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetTemplateByKeyAsync(_tenantId, "order-confirmation");

        // Assert
        result.ShouldNotBeNull();
        result.Key.ShouldBe("order-confirmation");
    }

    [Fact]
    public async Task GetTemplateByKeyAsync_WithInactiveTemplate_ShouldReturnNull()
    {
        // Arrange
        var template = CreateTestTemplate("order-confirmation");
        template.IsActive = false;
        _dbContext.EmailTemplates.Add(template);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetTemplateByKeyAsync(_tenantId, "order-confirmation");

        // Assert
        result.ShouldBeNull();
    }

    #endregion

    #region CreateTemplateAsync Tests

    [Fact]
    public async Task CreateTemplateAsync_ValidDto_ShouldCreateTemplate()
    {
        // Arrange
        var dto = new CreateEmailTemplateDto
        {
            Name = "Order Confirmation",
            Key = "order-confirmation",
            Subject = "Your order {orderNumber} has been confirmed",
            Body = "<h1>Thank you for your order</h1>",
            IsHtml = true,
            Description = "Sent when order is confirmed"
        };

        // Act
        var result = await _sut.CreateTemplateAsync(_tenantId, dto, "admin@test.com");

        // Assert
        result.ShouldNotBeNull();
        result.TenantId.ShouldBe(_tenantId);
        result.Name.ShouldBe("Order Confirmation");
        result.Key.ShouldBe("order-confirmation");
        result.CreatedBy.ShouldBe("admin@test.com");
    }

    [Fact]
    public async Task CreateTemplateAsync_DuplicateKey_ShouldThrowException()
    {
        // Arrange
        var existingTemplate = CreateTestTemplate("order-confirmation");
        _dbContext.EmailTemplates.Add(existingTemplate);
        await _dbContext.SaveChangesAsync();

        var dto = new CreateEmailTemplateDto
        {
            Name = "Another Order Confirmation",
            Key = "order-confirmation",
            Subject = "Subject",
            Body = "Body"
        };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _sut.CreateTemplateAsync(_tenantId, dto));
    }

    #endregion

    #region UpdateTemplateAsync Tests

    [Fact]
    public async Task UpdateTemplateAsync_ExistingTemplate_ShouldUpdateFields()
    {
        // Arrange
        var template = CreateTestTemplate("test-template");
        _dbContext.EmailTemplates.Add(template);
        await _dbContext.SaveChangesAsync();

        var dto = new UpdateEmailTemplateDto
        {
            Name = "Updated Name",
            Subject = "Updated Subject",
            Body = "Updated Body",
            IsHtml = false,
            IsActive = true,
            Description = "Updated Description"
        };

        // Act
        var result = await _sut.UpdateTemplateAsync(_tenantId, template.Id, dto, "editor@test.com");

        // Assert
        result.Name.ShouldBe("Updated Name");
        result.Subject.ShouldBe("Updated Subject");
        result.Body.ShouldBe("Updated Body");
        result.IsHtml.ShouldBeFalse();
        result.UpdatedBy.ShouldBe("editor@test.com");
    }

    [Fact]
    public async Task UpdateTemplateAsync_NonExistentTemplate_ShouldThrowException()
    {
        // Arrange
        var dto = new UpdateEmailTemplateDto
        {
            Name = "Updated Name",
            Subject = "Subject",
            Body = "Body"
        };

        // Act & Assert
        await Should.ThrowAsync<KeyNotFoundException>(async () =>
            await _sut.UpdateTemplateAsync(_tenantId, Guid.NewGuid(), dto));
    }

    #endregion

    #region DeleteTemplateAsync Tests

    [Fact]
    public async Task DeleteTemplateAsync_ExistingTemplate_ShouldDelete()
    {
        // Arrange
        var template = CreateTestTemplate("test-template");
        _dbContext.EmailTemplates.Add(template);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.DeleteTemplateAsync(_tenantId, template.Id);

        // Assert
        var deletedTemplate = await _dbContext.EmailTemplates.FindAsync(template.Id);
        deletedTemplate.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteTemplateAsync_NonExistentTemplate_ShouldThrowException()
    {
        // Act & Assert
        await Should.ThrowAsync<KeyNotFoundException>(async () =>
            await _sut.DeleteTemplateAsync(_tenantId, Guid.NewGuid()));
    }

    #endregion

    #region ToggleTemplateStatusAsync Tests

    [Fact]
    public async Task ToggleTemplateStatusAsync_ShouldToggleActiveStatus()
    {
        // Arrange
        var template = CreateTestTemplate("test-template");
        template.IsActive = true;
        _dbContext.EmailTemplates.Add(template);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.ToggleTemplateStatusAsync(_tenantId, template.Id, false, "admin@test.com");

        // Assert
        result.IsActive.ShouldBeFalse();
        result.UpdatedBy.ShouldBe("admin@test.com");
    }

    #endregion

    #region TestTemplateAsync Tests

    [Fact]
    public async Task TestTemplateAsync_ShouldReplaceVariables()
    {
        // Arrange
        var template = CreateTestTemplate("test-template");
        template.Subject = "Hello {firstName}";
        template.Body = "<p>Dear {firstName} {lastName}</p>";
        template.Variables = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "firstName", "John" },
            { "lastName", "Doe" }
        };
        _dbContext.EmailTemplates.Add(template);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.TestTemplateAsync(
            _tenantId,
            template.Id,
            "test@example.com",
            new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { "firstName", "Jane" },
                { "lastName", "Smith" }
            });

        // Assert
        result.Subject.ShouldBe("Hello Jane");
        result.Body.ShouldBe("<p>Dear Jane Smith</p>");
        result.To.ShouldBe("test@example.com");
    }

    [Fact]
    public async Task TestTemplateAsync_NonExistentTemplate_ShouldThrowException()
    {
        // Act & Assert
        await Should.ThrowAsync<KeyNotFoundException>(async () =>
            await _sut.TestTemplateAsync(_tenantId, Guid.NewGuid(), "test@example.com"));
    }

    #endregion

    #region Helper Methods

    private EmailTemplate CreateTestTemplate(string key)
    {
        return new EmailTemplate
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Name = $"Test Template {key}",
            Key = key,
            Subject = "Test Subject",
            Body = "<p>Test Body</p>",
            IsHtml = true,
            IsActive = true
        };
    }

    #endregion
}
