using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;

namespace B2Connect.Shared.User.Tests.ModelBinding;

public class ExtensibleEntityModelBinderTests
{
    private readonly Mock<IEntityExtensionService> _mockExtensionService;
    private readonly Mock<ILogger<ExtensibleEntityModelBinder>> _mockLogger;
    private readonly ExtensibleEntityModelBinder _binder;

    public ExtensibleEntityModelBinderTests()
    {
        _mockExtensionService = new Mock<IEntityExtensionService>();
        _mockLogger = new Mock<ILogger<ExtensibleEntityModelBinder>>();
        _binder = new ExtensibleEntityModelBinder(_mockExtensionService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task BindModelAsync_WithValidUserJson_BindsStandardAndCustomProperties()
    {
        // Arrange
        var json = """
        {
            "email": "john@example.com",
            "firstName": "John",
            "lastName": "Doe",
            "phoneNumber": "+49 123 456789",
            "customProperties": {
                "erp_customer_id": "CUST-12345",
                "warehouse_code": "WH-001"
            }
        }
        """;

        var tenantId = Guid.NewGuid();
        var bindingContext = CreateBindingContext(json, typeof(User), tenantId);

        // Mock validation
        _mockExtensionService
            .Setup(x => x.ValidateCustomPropertyAsync(
                tenantId, "User", "erp_customer_id", "CUST-12345"))
            .ReturnsAsync(true);

        _mockExtensionService
            .Setup(x => x.ValidateCustomPropertyAsync(
                tenantId, "User", "warehouse_code", "WH-001"))
            .ReturnsAsync(true);

        // Act
        await _binder.BindModelAsync(bindingContext);

        // Assert
        bindingContext.Result.IsModelSet.Should().BeTrue();
        var user = bindingContext.Result.Model as User;

        user.Should().NotBeNull();
        user!.Email.Should().Be("john@example.com");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.PhoneNumber.Should().Be("+49 123 456789");

        // Verify custom properties were set
        _mockExtensionService.Verify(
            x => x.SetCustomProperty(
                It.IsAny<IExtensibleEntity>(), "erp_customer_id", "CUST-12345"),
            Times.Once);

        _mockExtensionService.Verify(
            x => x.SetCustomProperty(
                It.IsAny<IExtensibleEntity>(), "warehouse_code", "WH-001"),
            Times.Once);
    }

    [Fact]
    public async Task BindModelAsync_WithInvalidCustomProperty_AddsModelError()
    {
        // Arrange
        var json = """
        {
            "email": "john@example.com",
            "firstName": "John",
            "lastName": "Doe",
            "customProperties": {
                "warehouse_code": "INVALID"
            }
        }
        """;

        var tenantId = Guid.NewGuid();
        var bindingContext = CreateBindingContext(json, typeof(User), tenantId);

        // Mock invalid validation
        _mockExtensionService
            .Setup(x => x.ValidateCustomPropertyAsync(
                tenantId, "User", "warehouse_code", "INVALID"))
            .ReturnsAsync(false);

        // Act
        await _binder.BindModelAsync(bindingContext);

        // Assert
        bindingContext.ModelState.IsValid.Should().BeFalse();
        bindingContext.ModelState["customProperties.warehouse_code"].Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task BindModelAsync_WithoutCustomProperties_BindsOnlyStandardProperties()
    {
        // Arrange
        var json = """
        {
            "email": "john@example.com",
            "firstName": "John",
            "lastName": "Doe"
        }
        """;

        var bindingContext = CreateBindingContext(json, typeof(User), Guid.NewGuid());

        // Act
        await _binder.BindModelAsync(bindingContext);

        // Assert
        bindingContext.Result.IsModelSet.Should().BeTrue();
        var user = bindingContext.Result.Model as User;

        user.Should().NotBeNull();
        user!.Email.Should().Be("john@example.com");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");

        // Custom properties should not be set
        _mockExtensionService.Verify(
            x => x.SetCustomProperty(It.IsAny<IExtensibleEntity>(), It.IsAny<string>(), It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public async Task BindModelAsync_WithInvalidJson_ReturnsFailed()
    {
        // Arrange
        var json = "{ invalid json }";
        var bindingContext = CreateBindingContext(json, typeof(User), Guid.NewGuid());

        // Act
        await _binder.BindModelAsync(bindingContext);

        // Assert
        bindingContext.Result.IsModelSet.Should().BeFalse();
        bindingContext.ModelState.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task BindModelAsync_WithNonExtensibleEntity_ReturnsFailed()
    {
        // Arrange
        var json = "{}";
        var bindingContext = CreateBindingContext(json, typeof(string), Guid.NewGuid());

        // Act
        await _binder.BindModelAsync(bindingContext);

        // Assert
        bindingContext.Result.IsModelSet.Should().BeFalse();
    }

    [Fact]
    public async Task BindModelAsync_WithMissingTenantHeader_SkipsCustomProperties()
    {
        // Arrange
        var json = """
        {
            "email": "john@example.com",
            "firstName": "John",
            "lastName": "Doe",
            "customProperties": {
                "erp_customer_id": "CUST-12345"
            }
        }
        """;

        var bindingContext = CreateBindingContext(json, typeof(User), null);

        // Act
        await _binder.BindModelAsync(bindingContext);

        // Assert
        bindingContext.Result.IsModelSet.Should().BeTrue();
        var user = bindingContext.Result.Model as User;
        user.Should().NotBeNull();

        // Custom properties should not be validated because no tenant ID
        _mockExtensionService.Verify(
            x => x.ValidateCustomPropertyAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<object>()),
            Times.Never);
    }

    // Helper Method
    private ModelBindingContext CreateBindingContext(
        string json,
        Type modelType,
        Guid? tenantId = null)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var httpContext = new DefaultHttpContext
        {
            Request = { Body = stream, ContentType = "application/json" }
        };

        if (tenantId.HasValue)
        {
            httpContext.Request.Headers["X-Tenant-ID"] = tenantId.Value.ToString();
        }

        return new DefaultModelBindingContext
        {
            HttpContext = httpContext,
            ModelType = modelType,
            ModelName = modelType.Name,
            FieldName = modelType.Name
        };
    }
}

/// <summary>
/// Integration Tests für Model Binding in echtem Controller
/// </summary>
public class ModelBindingIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Fact]
    public async Task PostUser_WithCustomProperties_BindsAndValidatesAutomatically()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new
        {
            email = "john@example.com",
            firstName = "John",
            lastName = "Doe",
            customProperties = new
            {
                erp_customer_id = "CUST-12345",
                warehouse_code = "WH-001"
            }
        };

        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        _client.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());

        // Act
        var response = await _client.PostAsync("/api/users", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var responseBody = await response.Content.ReadAsAsync<dynamic>();
        responseBody.email.Should().Be("john@example.com");
        responseBody.customProperties.erp_customer_id.Should().Be("CUST-12345");
    }

    [Fact]
    public async Task PatchCustomProperty_WithValidValue_UpdatesAndReturns204()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize("NEW-VALUE"),
            Encoding.UTF8,
            "application/json");

        _client.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());

        // Act
        var response = await _client.PatchAsync(
            $"/api/users/{userId}/custom-properties/erp_customer_number",
            content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
}
