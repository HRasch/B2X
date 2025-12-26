# Shouldly Unit Testing Guide - B2Connect

**Version:** 1.0  
**Last Updated:** 26. Dezember 2025  
**Framework:** xUnit + Shouldly + Moq  

---

## Overview

Shouldly is a fluent assertion library that replaces traditional `Assert.*` statements with readable, chainable assertions. This improves test readability and provides better error messages.

### Why Shouldly?

| Aspect | Assert | Shouldly |
|--------|--------|----------|
| **Readability** | `Assert.Equal(expected, actual)` | `actual.ShouldBe(expected)` |
| **Flow** | Method-based | Object-oriented, fluent |
| **Error Messages** | Generic | Contextual, detailed |
| **Assertion Chaining** | Limited | Full support |
| **IntelliSense** | Basic | Excellent |

---

## Installation

### Add Shouldly to Your Test Project

**Option 1: Via Package Manager**
```bash
dotnet add package Shouldly
```

**Option 2: Update .csproj**
```xml
<ItemGroup>
  <PackageReference Include="Shouldly" />
</ItemGroup>
```

Both projects already updated:
- ✅ `backend/Tests/AuthService.Tests/AuthService.Tests.csproj`
- ✅ `backend/Tests/CatalogService.Tests/CatalogService.Tests.csproj`

---

## Common Assertions

### Null Checks

```csharp
// ✅ Shouldly
user.ShouldNotBeNull();
email.ShouldBeNull();

// ❌ Old Assert
Assert.NotNull(user);
Assert.Null(email);
```

### Value Equality

```csharp
// ✅ Shouldly
user.Id.ShouldBe("123");
user.Age.ShouldBe(25);
user.IsActive.ShouldBeTrue();
user.IsDeleted.ShouldBeFalse();

// ❌ Old Assert
Assert.Equal("123", user.Id);
Assert.Equal(25, user.Age);
Assert.True(user.IsActive);
Assert.False(user.IsDeleted);
```

### String Assertions

```csharp
// ✅ Shouldly
email.ShouldNotBeNullOrEmpty();
password.ShouldNotBeNullOrWhiteSpace();
name.ShouldContain("John");
email.ShouldStartWith("user@");
email.ShouldEndWith(".com");

// ❌ Old Assert
Assert.NotNull(email);
Assert.NotEmpty(email);
Assert.Contains("John", name);
Assert.StartsWith("user@", email);
Assert.EndsWith(".com", email);
```

### Type Assertions

```csharp
// ✅ Shouldly
result.ShouldBeOfType<Result<User>.Success>();
obj.ShouldBeAssignableTo<IRepository>();

// ❌ Old Assert
Assert.IsType<Result<User>.Success>(result);
Assert.IsAssignableFrom<IRepository>(obj);
```

### Collection Assertions

```csharp
// ✅ Shouldly
users.ShouldBeEmpty();
users.ShouldNotBeEmpty();
users.ShouldHaveCount(5);
users.ShouldContain(user);
users.ShouldNotContain(user);
items.ShouldAllBe(i => i.IsActive);
items.ShouldAnyBe(i => i.Status == "Pending");

// ❌ Old Assert
Assert.Empty(users);
Assert.NotEmpty(users);
Assert.Equal(5, users.Count);
Assert.Contains(user, users);
Assert.DoesNotContain(user, users);
```

### Numeric Assertions

```csharp
// ✅ Shouldly
price.ShouldBeGreaterThan(0);
count.ShouldBeLessThan(100);
amount.ShouldBeGreaterThanOrEqualTo(50);
number.ShouldBeLessThanOrEqualTo(1000);

// ❌ Old Assert
Assert.True(price > 0);
Assert.True(count < 100);
```

### Exception Testing

```csharp
// ✅ Shouldly
Should.Throw<ArgumentNullException>(() => 
    service.Process(null));

var exception = Should.Throw<ValidationException>(() => 
    service.Validate(invalidData));
exception.Message.ShouldContain("Invalid");

// ❌ Old Assert
Assert.Throws<ArgumentNullException>(() => 
    service.Process(null));
```

---

## Complete Test Examples

### Example 1: Service Method Test

```csharp
using Xunit;
using Shouldly;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        mockRepository
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser 
            { 
                Id = "user-1", 
                Email = "user@test.com" 
            });
        
        var service = new AuthService(mockRepository.Object);
        
        // Act
        var result = await service.LoginAsync("user@test.com", "password123");
        
        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Result<AuthResponse>.Success>();
        
        if (result is Result<AuthResponse>.Success success)
        {
            success.Value.AccessToken.ShouldNotBeNullOrEmpty();
            success.Value.User.ShouldNotBeNull();
            success.Value.User.Email.ShouldBe("user@test.com");
        }
    }
    
    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsFailure()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        mockRepository
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser)null);
        
        var service = new AuthService(mockRepository.Object);
        
        // Act
        var result = await service.LoginAsync("unknown@test.com", "password");
        
        // Assert
        result.ShouldBeOfType<Result<AuthResponse>.Failure>();
    }
}
```

### Example 2: Validator Test

```csharp
using Xunit;
using Shouldly;
using FluentValidation;

public class CreateProductValidatorTests
{
    [Fact]
    public async Task CreateProductValidator_WithValidData_Succeeds()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var validRequest = new CreateProductRequest(
            Sku: "PROD-001",
            Name: "Test Product",
            Description: "A valid product",
            Price: 99.99m
        );
        
        // Act
        var result = await validator.ValidateAsync(validRequest);
        
        // Assert
        result.IsValid.ShouldBeTrue();
    }
    
    [Theory]
    [InlineData("")]           // Empty
    [InlineData("PROD")]       // Too short
    [InlineData("prod-001")]   // Lowercase
    public async Task CreateProductValidator_WithInvalidSku_Fails(string sku)
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var request = new CreateProductRequest(
            Sku: sku,
            Name: "Test Product",
            Description: "A valid product",
            Price: 99.99m
        );
        
        // Act
        var result = await validator.ValidateAsync(request);
        
        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors
            .Select(e => e.PropertyName)
            .ShouldContain("Sku");
    }
}
```

### Example 3: Controller Test

```csharp
using Xunit;
using Shouldly;
using Microsoft.AspNetCore.Mvc;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_WithValidRequest_ReturnsOkWithToken()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        mockService
            .Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new Result<AuthResponse>.Success(
                new AuthResponse 
                { 
                    AccessToken = "token123",
                    User = new UserInfo { Email = "user@test.com" }
                }
            ));
        
        var controller = new AuthController(mockService.Object);
        
        // Act
        var result = await controller.Login(
            new LoginRequest { Email = "user@test.com", Password = "pass123" }
        );
        
        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.StatusCode.ShouldBe(200);
        
        var response = okResult.Value.ShouldBeOfType<AuthResponse>();
        response.AccessToken.ShouldBe("token123");
        response.User.Email.ShouldBe("user@test.com");
    }
}
```

---

## Best Practices

### 1. Use Pattern Matching with Shouldly

```csharp
// ✅ Good: Combine pattern matching with Shouldly
var result = await service.GetUserAsync("user-1");

result.ShouldBeOfType<Result<AppUser>.Success>();
var user = (result as Result<AppUser>.Success)?.Value;
user.ShouldNotBeNull();
user.Email.ShouldBe("user@test.com");

// Or more elegantly:
if (result is Result<AppUser>.Success success)
{
    success.Value.Email.ShouldBe("user@test.com");
}
```

### 2. Readable Assertion Messages

```csharp
// ✅ Good: Shouldly provides clear context
result.IsValid.ShouldBeTrue();
// Output: "result.IsValid" should be true but was false

// ❌ Poor: Generic Assert message
Assert.True(result.IsValid);
// Output: "Assert.True(result.IsValid)" failed
```

### 3. Test One Thing Per Test

```csharp
// ✅ Good: Focused test
[Fact]
public async Task GetUser_WithValidId_ReturnsCorrectEmail()
{
    var user = await service.GetUserAsync("user-1");
    user.Email.ShouldBe("user@test.com");
}

// ❌ Poor: Multiple assertions for different concerns
[Fact]
public async Task GetUser_ChecksEverything()
{
    var user = await service.GetUserAsync("user-1");
    user.ShouldNotBeNull();
    user.Id.ShouldBe("user-1");
    user.Email.ShouldBe("user@test.com");
    user.IsActive.ShouldBeTrue();
    user.Roles.ShouldContain("Admin");
    // ... many more assertions
}
```

### 4. Use Theory for Multiple Inputs

```csharp
// ✅ Good: Reusable test with multiple inputs
[Theory]
[InlineData("")]
[InlineData(" ")]
[InlineData(null)]
public void ValidateEmail_WithEmptyInput_ReturnsFalse(string email)
{
    validator.IsValid(email).ShouldBeFalse();
}

// ❌ Poor: Duplicated tests
[Fact]
public void ValidateEmail_WithEmptyString_ReturnsFalse()
{
    validator.IsValid("").ShouldBeFalse();
}

[Fact]
public void ValidateEmail_WithWhitespace_ReturnsFalse()
{
    validator.IsValid(" ").ShouldBeFalse();
}
```

---

## Migration from Assert to Shouldly

### Quick Reference

| Task | Before (Assert) | After (Shouldly) |
|------|-----------------|------------------|
| **Is not null** | `Assert.NotNull(x)` | `x.ShouldNotBeNull()` |
| **Is equal** | `Assert.Equal(exp, act)` | `act.ShouldBe(exp)` |
| **Is true** | `Assert.True(x)` | `x.ShouldBeTrue()` |
| **Is false** | `Assert.False(x)` | `x.ShouldBeFalse()` |
| **Is type** | `Assert.IsType<T>(x)` | `x.ShouldBeOfType<T>()` |
| **Contains** | `Assert.Contains(item, list)` | `list.ShouldContain(item)` |
| **Count** | `Assert.Equal(5, list.Count)` | `list.ShouldHaveCount(5)` |
| **Empty** | `Assert.Empty(list)` | `list.ShouldBeEmpty()` |
| **Throws** | `Assert.Throws<Ex>()` | `Should.Throw<Ex>()` |

---

## Project Status

✅ **Shouldly Installed:**
- AuthService.Tests.csproj
- CatalogService.Tests.csproj

✅ **Tests Updated:**
- AuthServiceTests.cs (using Shouldly assertions)
- AuthControllerTests.cs (using Shouldly assertions)
- CatalogValidatorsTests.cs (using Shouldly assertions)

✅ **Coding Standards Updated:**
- CODING_STANDARDS.md (section 5.1-5.5)

---

## Next Steps

1. ✅ Add Shouldly to remaining test projects as needed
2. ✅ Update all remaining test files with Shouldly assertions
3. ✅ Train team on Shouldly best practices
4. Run tests to verify everything works:
   ```bash
   cd backend
   dotnet test
   ```

---

## Resources

- **Shouldly Documentation:** https://docs.shouldly.io/
- **Shouldly GitHub:** https://github.com/shouldly/shouldly
- **Example Tests:** See CatalogValidatorsTests.cs, AuthServiceTests.cs

