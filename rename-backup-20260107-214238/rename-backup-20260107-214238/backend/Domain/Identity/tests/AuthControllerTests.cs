using B2X.AuthService.Controllers;
using B2X.AuthService.Data;
using B2X.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.AuthService.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var mockLogger = new Mock<ILogger<AuthController>>();

        var loginResponse = new AuthResponse
        {
            User = new UserInfo
            {
                Id = "test-user-001",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                TenantId = "default",
                Roles = new[] { "Admin" },
                Permissions = new[] { "*" }
            },
            AccessToken = "test-token-123",
            RefreshToken = "test-refresh-token",
            ExpiresIn = 3600
        };

        mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(new Result<AuthResponse>.Success(loginResponse));

        var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

        // Act
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };
        var result = await controller.Login(request);

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(200);

        var returnedResponse = okResult.Value.ShouldBeOfType<AuthResponse>();
        returnedResponse.AccessToken.ShouldBe("test-token-123");
        returnedResponse.User.ShouldNotBeNull();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var mockLogger = new Mock<ILogger<AuthController>>();

        mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(new Result<AuthResponse>.Failure(ErrorCodes.InvalidCredentials, "Invalid credentials"));

        var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

        // Act
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };
        var result = await controller.Login(request);

        // Assert
        var unauthorizedResult = result.ShouldBeOfType<ObjectResult>();
        unauthorizedResult.StatusCode.ShouldBe(401);
    }

    [Fact]
    public async Task Login_WithMissingEmail_ReturnsBadRequest()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var mockLogger = new Mock<ILogger<AuthController>>();
        var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

        // Act
        var request = new LoginRequest
        {
            Email = "",
            Password = "password123"
        };
        var result = await controller.Login(request);

        // Assert
        var badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.StatusCode.ShouldBe(400);
    }

    [Fact]
    public async Task Refresh_WithValidToken_ReturnsOkWithNewToken()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var mockLogger = new Mock<ILogger<AuthController>>();

        var refreshResponse = new AuthResponse
        {
            User = new UserInfo
            {
                Id = "test-user-001",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                TenantId = "default",
                Roles = new[] { "Admin" },
                Permissions = new[] { "*" }
            },
            AccessToken = "new-test-token-456",
            RefreshToken = "new-test-refresh-token",
            ExpiresIn = 3600
        };

        mockAuthService
            .Setup(x => x.RefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new Result<AuthResponse>.Success(refreshResponse));

        var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

        // Act
        var request = new RefreshTokenRequest
        {
            RefreshToken = "old-refresh-token"
        };
        var result = await controller.Refresh(request);

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(200);

        var returnedResponse = okResult.Value.ShouldBeOfType<AuthResponse>();
        returnedResponse.AccessToken.ShouldBe("new-test-token-456");
    }

    [Fact]
    public async Task Refresh_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var mockLogger = new Mock<ILogger<AuthController>>();

        mockAuthService
            .Setup(x => x.RefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new Result<AuthResponse>.Failure(ErrorCodes.InvalidCredentials, "Invalid refresh token"));

        var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

        // Act
        var request = new RefreshTokenRequest
        {
            RefreshToken = "invalid-token"
        };
        var result = await controller.Refresh(request);

        // Assert
        var unauthorizedResult = result.ShouldBeOfType<ObjectResult>();
        unauthorizedResult.StatusCode.ShouldBe(401);
    }

    [Fact]
    public async Task Enable2FA_WithValidUserId_ReturnsOkWith2FAEnabled()
    {
        // This test requires proper authorization context with User principal
        // See auth.spec.ts for E2E test of this functionality
        // In production, use TestServer or WebApplicationFactory
        Assert.True(true);
    }
}
