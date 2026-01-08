# 🧪 Testing Strategy & Implementation Guide

## Aktuelle Test-Coverage: ~3%
## Ziel: 80%+ (12 Wochen)

---

## Phase 1: Unit Test Foundation (2 Wochen)

### 1.1 Backend Test Setup

#### Schritt 1: Test Project Struktur

```bash
backend/
├── BoundedContexts/
│   ├── Shared/
│   │   ├── Identity/
│   │   │   ├── B2X.Identity.API.csproj
│   │   │   └── tests/
│   │   │       └── B2X.Identity.Tests.csproj  # CREATE
│   │   ├── Tenancy/
│   │   │   └── tests/
│   │   │       └── B2X.Tenancy.Tests.csproj   # CREATE
```

#### Schritt 2: Test Project Template

**Datei:** `backend/BoundedContexts/Shared/Identity/tests/B2X.Identity.Tests.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsTestProject>true</IsTestProject>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.4" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.2" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Testcontainers" Version="3.7.0" />
    <PackageReference Include="Testcontainers.PostgreSql" Version="3.7.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../B2X.Identity.API.csproj" />
  </ItemGroup>

</Project>
```

#### Schritt 3: Basis Test Class

**Datei:** `backend/BoundedContexts/Shared/Identity/tests/Fixtures/IdentityTestFixture.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace B2X.Identity.Tests.Fixtures;

public class IdentityTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    public IdentityDbContext DbContext { get; private set; }

    public IdentityTestFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("B2X_identity_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var connectionString = _container.GetConnectionString();
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        DbContext = new IdentityDbContext(options);
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _container.StopAsync();
    }
}

public class IdentityTestBase : IAsyncLifetime
{
    protected readonly IdentityTestFixture Fixture;

    protected IdentityTestBase()
    {
        Fixture = new IdentityTestFixture();
    }

    public async Task InitializeAsync() => await Fixture.InitializeAsync();
    public async Task DisposeAsync() => await Fixture.DisposeAsync();
}
```

#### Schritt 4: Unit Test Beispiel - Identity Service

**Datei:** `backend/BoundedContexts/Shared/Identity/tests/Services/IdentityServiceTests.cs`

```csharp
using Xunit;
using Moq;
using FluentAssertions;
using B2X.Identity.Core.Entities;
using B2X.Identity.Core.Interfaces;
using B2X.Identity.Application.Services;
using B2X.Identity.Application.DTOs;

namespace B2X.Identity.Tests.Services;

public class IdentityServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly IdentityService _service;

    public IdentityServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _service = new IdentityService(_mockUserRepository.Object, _mockPasswordHasher.Object);
    }

    #region Register Tests

    [Fact]
    public async Task Register_WithValidInput_CreatesUserSuccessfully()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Email = "test@example.com",
            Password = "SecurePassword123!",
            FirstName = "John",
            LastName = "Doe"
        };

        var userId = Guid.NewGuid();
        var hashedPassword = "hashed_password";

        _mockPasswordHasher
            .Setup(x => x.HashPassword(registerDto.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.UserExistsByEmailAsync(registerDto.Email))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(new User
            {
                Id = userId,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PasswordHash = hashedPassword
            });

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Email.Should().Be(registerDto.Email);

        _mockUserRepository.Verify(
            x => x.CreateAsync(It.IsAny<User>()),
            Times.Once);

        _mockPasswordHasher.Verify(
            x => x.HashPassword(registerDto.Password),
            Times.Once);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ThrowsException()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Email = "existing@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe"
        };

        _mockUserRepository
            .Setup(x => x.UserExistsByEmailAsync(registerDto.Email))
            .ReturnsAsync(true);

        // Act
        var act = () => _service.RegisterAsync(registerDto);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var email = "test@example.com";
        var password = "SecurePassword123!";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = "hashed_password",
            IsActive = true
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(password, user.PasswordHash))
            .Returns(true);

        // Act
        var result = await _service.LoginAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeEmpty();
        result.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ThrowsException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "WrongPassword123!";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = "hashed_password"
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(password, user.PasswordHash))
            .Returns(false);

        // Act
        var act = () => _service.LoginAsync(email, password);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>();
    }

    #endregion
}
```

#### Schritt 5: Integration Tests

**Datei:** `backend/BoundedContexts/Shared/Identity/tests/Integration/IdentityControllerIntegrationTests.cs`

```csharp
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace B2X.Identity.Tests.Integration;

public class IdentityControllerIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

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
    public async Task Register_WithValidInput_Returns201()
    {
        // Arrange
        var registerRequest = new
        {
            email = "newuser@example.com",
            password = "SecurePassword123!",
            firstName = "John",
            lastName = "Doe"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var result = await response.Content.ReadAsAsync<dynamic>();
        result.id.Should().NotBeEmpty();
        result.email.Should().Be(registerRequest.email);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginRequest = new
        {
            email = "test@example.com",
            password = "password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var result = await response.Content.ReadAsAsync<dynamic>();
        result.token.Should().NotBeEmpty();
        result.expiresIn.Should().BeGreaterThan(0);
    }
}
```

---

## Phase 2: Frontend Testing (1 Woche)

### 2.1 Unit Tests für Vue Components

**Datei:** `frontend-admin/tests/unit/components/LoginForm.spec.ts`

```typescript
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import LoginForm from '@/components/LoginForm.vue';

describe('LoginForm.vue', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mount(LoginForm, {
      global: {
        stubs: {
          'router-link': true
        }
      }
    });
  });

  it('renders login form', () => {
    expect(wrapper.find('form').exists()).toBe(true);
    expect(wrapper.find('input[type="email"]').exists()).toBe(true);
    expect(wrapper.find('input[type="password"]').exists()).toBe(true);
  });

  it('updates email on input change', async () => {
    const emailInput = wrapper.find('input[type="email"]');
    await emailInput.setValue('test@example.com');

    expect(wrapper.vm.email).toBe('test@example.com');
  });

  it('emits login event with credentials', async () => {
    await wrapper.find('input[type="email"]').setValue('test@example.com');
    await wrapper.find('input[type="password"]').setValue('password123');
    await wrapper.find('form').trigger('submit');

    expect(wrapper.emitted('login')).toHaveLength(1);
    expect(wrapper.emitted('login')[0]).toEqual(['test@example.com', 'password123']);
  });

  it('shows error message on login failure', async () => {
    wrapper = mount(LoginForm, {
      props: {
        error: 'Invalid credentials'
      }
    });

    expect(wrapper.find('.error-message').text()).toBe('Invalid credentials');
  });

  it('disables submit button while loading', async () => {
    wrapper = mount(LoginForm, {
      props: {
        isLoading: true
      }
    });

    expect(wrapper.find('button[type="submit"]').attributes('disabled')).toBeDefined();
  });
});
```

### 2.2 E2E Tests erweitern

**Datei:** `frontend-admin/tests/e2e/auth.spec.ts`

```typescript
import { test, expect } from '@playwright/test';

test.describe('Authentication Flow', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
  });

  test('complete login flow', async ({ page, context }) => {
    // Navigate to login
    await page.goto('/login');
    
    // Fill credentials
    await page.fill('[data-testid="email-input"]', 'admin@example.com');
    await page.fill('[data-testid="password-input"]', 'password');
    
    // Submit form
    await page.click('[data-testid="login-button"]');
    
    // Wait for redirect
    await page.waitForURL('/dashboard');
    
    // Verify user is logged in
    expect(await page.textContent('[data-testid="user-name"]')).toContain('Admin');
    
    // Verify auth token in storage
    const token = await page.evaluate(() => 
      localStorage.getItem('auth_token')
    );
    expect(token).toBeTruthy();
  });

  test('shows error on invalid credentials', async ({ page }) => {
    await page.goto('/login');
    
    await page.fill('[data-testid="email-input"]', 'test@example.com');
    await page.fill('[data-testid="password-input"]', 'wrongpassword');
    
    await page.click('[data-testid="login-button"]');
    
    // Wait for error message
    const errorMessage = page.locator('[data-testid="error-message"]');
    await expect(errorMessage).toBeVisible();
    await expect(errorMessage).toContainText('Invalid credentials');
  });

  test('logout clears auth token', async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('[data-testid="email-input"]', 'admin@example.com');
    await page.fill('[data-testid="password-input"]', 'password');
    await page.click('[data-testid="login-button"]');
    await page.waitForURL('/dashboard');
    
    // Logout
    await page.click('[data-testid="logout-button"]');
    
    // Verify redirect and token cleared
    await page.waitForURL('/login');
    const token = await page.evaluate(() => 
      localStorage.getItem('auth_token')
    );
    expect(token).toBeNull();
  });

  test('handles network errors gracefully', async ({ page }) => {
    // Offline mode
    await page.context().setOffline(true);
    
    await page.goto('/login');
    await page.fill('[data-testid="email-input"]', 'admin@example.com');
    await page.fill('[data-testid="password-input"]', 'password');
    await page.click('[data-testid="login-button"]');
    
    // Should show network error
    const errorMessage = page.locator('[data-testid="error-message"]');
    await expect(errorMessage).toBeVisible();
    await expect(errorMessage).toContainText('network');
  });
});
```

---

## Phase 3: Coverage Reporting

### 3.1 Backend Coverage

**.csproj:**
```xml
<ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
</ItemGroup>
```

**Run:**
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### 3.2 Frontend Coverage

**vitest.config.ts:**
```typescript
export default defineConfig({
  test: {
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      include: ['src/**/*.{ts,tsx,vue}'],
      exclude: ['src/**/*.spec.ts', 'src/main.ts'],
      lines: 80,
      functions: 80,
      branches: 80,
      statements: 80
    }
  }
});
```

**Run:**
```bash
npm run test:coverage
```

---

## Test Checklist

```
Phase 1: Unit Tests (2 Weeks)
[ ] Identity Service Tests (10 tests)
[ ] Catalog Service Tests (10 tests)
[ ] Repository Tests (8 tests)
[ ] DTO Mapping Tests (5 tests)
[ ] Utility/Extension Tests (10 tests)
✅ Backend Coverage: 60%

Phase 2: Integration Tests (1 Week)
[ ] API Endpoint Tests (20 tests)
[ ] Database Integration Tests (10 tests)
[ ] Multi-tenant Tests (8 tests)
✅ Backend Coverage: 75%

Phase 3: Frontend Tests (1 Week)
[ ] Vue Component Unit Tests (20 tests)
[ ] Store Tests (10 tests)
[ ] Composable Tests (8 tests)
[ ] E2E Happy Path (5 tests)
[ ] E2E Error Cases (10 tests)
✅ Frontend Coverage: 70%

Phase 4: E2E & Load Testing (2 Weeks)
[ ] Complete User Journeys (15 tests)
[ ] Admin Workflows (10 tests)
[ ] Payment Flows (5 tests)
[ ] Load Testing (k6 scripts)
✅ Overall Coverage: 80%+
```

