using Xunit;
using Shouldly;
using B2Connect.AuthService.Data;
using B2Connect.Identity.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using B2Connect.Types;

namespace B2Connect.Identity.Tests.Services;

/// <summary>
/// Unit tests for AuthService.LoginAsync
/// Tests authentication, token generation, and error handling
/// </summary>
public class AuthServiceLoginTests : AuthServiceTestBase
{
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessResult()
    {
        // Arrange
        var email = "john.doe@example.com";
        var password = "TestPassword123";
        var user = await Fixture.CreateTestUserAsync(email, password);

        var request = new LoginRequest
        {
            Email = email,
            Password = password,
            RememberMe = false
        };

        // Act
        var result = await Fixture.AuthService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AuthResponse>.Success>();

        if (result is Result<AuthResponse>.Success success)
        {
            var response = success.Value;
            response.AccessToken.Should().NotBeNullOrEmpty();
            response.RefreshToken.Should().NotBeNullOrEmpty();
            response.ExpiresIn.Should().Be(3600); // 1 hour
            response.User.Should().NotBeNull();
            response.User.Email.Should().Be(email);
        }
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ReturnsFailureResult()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "AnyPassword123"
        };

        // Act
        var result = await Fixture.AuthService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AuthResponse>.Failure>();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsFailureResult()
    {
        // Arrange
        var email = "jane.doe@example.com";
        var correctPassword = "CorrectPassword123";
        var wrongPassword = "WrongPassword123";

        await Fixture.CreateTestUserAsync(email, correctPassword);

        var request = new LoginRequest
        {
            Email = email,
            Password = wrongPassword
        };

        // Act
        var result = await Fixture.AuthService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AuthResponse>.Failure>();
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ReturnsFailureResult()
    {
        // Arrange
        var email = "inactive@example.com";
        var password = "TestPassword123";
        var user = await Fixture.CreateTestUserAsync(email, password);
        user.IsActive = false;
        await Fixture.DbContext.SaveChangesAsync();

        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // Act
        var result = await Fixture.AuthService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AuthResponse>.Failure>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task LoginAsync_WithEmptyEmail_ReturnsFailureResult(string email)
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = email,
            Password = "TestPassword123"
        };

        // Act
        var result = await Fixture.AuthService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AuthResponse>.Failure>();
    }
}

/// <summary>
/// Unit tests for AuthService.RefreshTokenAsync
/// Tests token refresh functionality and validation
/// </summary>
public class AuthServiceRefreshTokenTests : AuthServiceTestBase
{
    [Fact]
    public async Task RefreshTokenAsync_WithValidRefreshToken_ReturnsNewAccessToken()
    {
        // Arrange
        var email = "refresh.test@example.com";
        var password = "TestPassword123";
        await Fixture.CreateTestUserAsync(email, password);

        // First, login to get tokens
        var loginRequest = new LoginRequest { Email = email, Password = password };
        var loginResult = await Fixture.AuthService.LoginAsync(loginRequest);

        loginResult.Should().BeOfType<Result<AuthResponse>.Success>();
        var loginResponse = ((Result<AuthResponse>.Success)loginResult).Value;
        var refreshToken = loginResponse.RefreshToken;

        // Act
        var refreshResult = await Fixture.AuthService.RefreshTokenAsync(refreshToken);

        // Assert
        refreshResult.Should().NotBeNull();
        refreshResult.Should().BeOfType<Result<AuthResponse>.Success>();

        if (refreshResult is Result<AuthResponse>.Success success)
        {
            success.Value.AccessToken.Should().NotBeNullOrEmpty();
            success.Value.RefreshToken.Should().NotBeNullOrEmpty();
            success.Value.ExpiresIn.Should().Be(3600);
        }
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidRefreshToken_ReturnsFailureResult()
    {
        // Arrange
        var invalidToken = "invalid-refresh-token-xyz";

        // Act
        var result = await Fixture.AuthService.RefreshTokenAsync(invalidToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AuthResponse>.Failure>();
    }

    [Fact]
    public async Task RefreshTokenAsync_WithEmptyRefreshToken_ReturnsFailureResult()
    {
        // Act
        var result = await Fixture.AuthService.RefreshTokenAsync("");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AuthResponse>.Failure>();
    }
}

/// <summary>
/// Unit tests for AuthService.GetUserByIdAsync
/// Tests user retrieval by ID
/// </summary>
public class AuthServiceGetUserTests : AuthServiceTestBase
{
    [Fact]
    public async Task GetUserByIdAsync_WithValidUserId_ReturnsUserSuccessfully()
    {
        // Arrange
        var email = "getuser.test@example.com";
        var firstName = "Get";
        var lastName = "User";
        var user = await Fixture.CreateTestUserAsync(email, "Password123", firstName, lastName);

        // Act
        var result = await Fixture.AuthService.GetUserByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AppUser>.Success>();

        if (result is Result<AppUser>.Success success)
        {
            success.Value.Email.Should().Be(email);
            success.Value.FirstName.Should().Be(firstName);
            success.Value.LastName.Should().Be(lastName);
        }
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidUserId_ReturnsFailureResult()
    {
        // Act
        var result = await Fixture.AuthService.GetUserByIdAsync("invalid-user-id");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AppUser>.Failure>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task GetUserByIdAsync_WithEmptyUserId_ReturnsFailureResult(string? userId)
    {
        // Act
        var result = await Fixture.AuthService.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AppUser>.Failure>();
    }
}

/// <summary>
/// Unit tests for AuthService.EnableTwoFactorAsync
/// Tests 2FA setup
/// </summary>
public class AuthServiceTwoFactorTests : AuthServiceTestBase
{
    [Fact]
    public async Task EnableTwoFactorAsync_WithValidUser_ReturnsSuccessResult()
    {
        // Arrange
        var user = await Fixture.CreateTestUserAsync();
        user.IsTwoFactorRequired = false;
        await Fixture.DbContext.SaveChangesAsync();

        // Act
        var result = await Fixture.AuthService.EnableTwoFactorAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AuthResponse>.Success>();

        // Verify user 2FA is enabled
        var updatedUser = await Fixture.UserManager.FindByIdAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.IsTwoFactorRequired.Should().BeTrue();
    }

    [Fact]
    public async Task EnableTwoFactorAsync_WithInvalidUser_ReturnsFailureResult()
    {
        // Act
        var result = await Fixture.AuthService.EnableTwoFactorAsync("invalid-user-id");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<AuthResponse>.Failure>();
    }
}

/// <summary>
/// Unit tests for AuthService.GetAllUsersAsync
/// Tests pagination and user listing
/// </summary>
public class AuthServiceGetAllUsersTests : AuthServiceTestBase
{
    [Fact]
    public async Task GetAllUsersAsync_WithMultipleUsers_ReturnsPaginatedResults()
    {
        // Arrange
        await Fixture.CreateTestUserAsync("user1@example.com", "Pass123");
        await Fixture.CreateTestUserAsync("user2@example.com", "Pass123");
        await Fixture.CreateTestUserAsync("user3@example.com", "Pass123");

        // Act
        var result = await Fixture.AuthService.GetAllUsersAsync(1, 10, null);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<IEnumerable<UserDto>>.Success>();

        if (result is Result<IEnumerable<UserDto>>.Success success)
        {
            success.Value.Should().HaveCountGreaterThanOrEqualTo(3);
        }
    }

    [Fact]
    public async Task GetAllUsersAsync_WithSearchFilter_ReturnsFilteredResults()
    {
        // Arrange
        await Fixture.CreateTestUserAsync("john.doe@example.com", "Pass123");
        await Fixture.CreateTestUserAsync("jane.doe@example.com", "Pass123");
        await Fixture.CreateTestUserAsync("bob.smith@example.com", "Pass123");

        // Act
        var result = await Fixture.AuthService.GetAllUsersAsync(1, 10, "doe");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<IEnumerable<UserDto>>.Success>();

        if (result is Result<IEnumerable<UserDto>>.Success success)
        {
            var users = success.Value.ToList();
            users.Should().HaveCount(2);
            users.All(u => u.Email.Contains("doe")).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetAllUsersAsync_WithInvalidPage_ReturnsEmptyResult()
    {
        // Arrange
        await Fixture.CreateTestUserAsync("user@example.com", "Pass123");

        // Act
        var result = await Fixture.AuthService.GetAllUsersAsync(99, 10, null);

        // Assert
        result.Should().NotBeNull();
        if (result is Result<IEnumerable<UserDto>>.Success success)
        {
            success.Value.Should().BeEmpty();
        }
    }
}
