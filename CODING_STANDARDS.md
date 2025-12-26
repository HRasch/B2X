# Coding Standards & Guidelines - B2Connect

**Version:** 1.1  
**Last Updated:** 26. Dezember 2025  
**Status:** ACTIVE  
**Testing Update:** Shouldly assertions now required for all unit tests ✨

> ⭐ **NEW:** All unit tests now use **Shouldly** for fluent, readable assertions instead of traditional `Assert.*` statements. See [SHOULDLY_TESTING_GUIDE.md](SHOULDLY_TESTING_GUIDE.md) for complete reference and examples.

---

## 📋 Table of Contents

1. [C# Naming & Code Organization](#1-c-naming--code-organization)
2. [Error Handling & Result-Pattern](#2-error-handling--result-pattern)
3. [Type Safety & Null Handling](#3-type-safety--null-handling)
4. [Async/Await & Threading](#4-asyncawait--threading)
5. [Testing & Quality](#5-testing--quality) ⭐ *See [SHOULDLY_TESTING_GUIDE.md](SHOULDLY_TESTING_GUIDE.md) for detailed assertion examples*
6. [Documentation & Comments](#6-documentation--comments)
7. [Performance & Resource Management](#7-performance--resource-management)
8. [Frontend (Vue.js / TypeScript)](#8-frontend-vuejs--typescript)
9. [Architecture & Design Patterns](#9-architecture--design-patterns)

---

## 1. C# Naming & Code Organization

### 1.1 Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| **Classes** | PascalCase | `AuthenticationService`, `UserRepository` |
| **Interfaces** | PascalCase with `I` prefix | `IAuthenticationService`, `IRepository<T>` |
| **Methods** | PascalCase | `GetUserById()`, `CreateTenantAsync()` |
| **Properties** | PascalCase | `UserId`, `TenantName` |
| **Fields (private)** | camelCase with `_` prefix | `_userRepository`, `_logger` |
| **Local Variables** | camelCase | `userId`, `userName` |
| **Constants** | UPPER_SNAKE_CASE | `DEFAULT_PAGE_SIZE`, `MAX_RETRY_ATTEMPTS` |
| **Enums** | PascalCase (members: PascalCase) | `UserRole.Administrator` |
| **Error Codes** | PascalCase (via ErrorCodes class) | `ErrorCodes.InvalidCredentials` |

### 1.2 File Organization

```csharp
using System;                           // System namespaces
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;    // External namespaces
using Serilog;
using Wolverine;

using B2Connect.Shared.Types;           // Internal namespaces
using B2Connect.AuthService.Models;

#nullable enable

namespace B2Connect.AuthService.Services;

/// <summary>
/// Manages user authentication and token lifecycle.
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    #region Fields
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger _logger;
    #endregion

    #region Constructors
    public AuthenticationService(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    #endregion

    #region Public Methods
    public async Task<Result<AuthResponse>> LoginAsync(string email, string password)
    {
        // Implementation...
    }
    #endregion

    #region Private Methods
    private bool ValidatePassword(string hash, string password)
    {
        // Implementation...
    }
    #endregion
}
```

### 1.3 Class Size & Responsibility

- **Maximum 500 lines** per class (excluding auto-generated code)
- **Single Responsibility Principle**: One reason to change
- **Constructor Injection**: Always prefer DI over static methods
- **One public class** per file (exceptions: nested classes with private visibility)

### 1.4 Code Organization with Regions (Use Sparingly)

```csharp
#region Fields
// Private fields only
#endregion

#region Constructors
// Single or multiple constructors
#endregion

#region Public Methods
// Public API surface
#endregion

#region Private Methods
// Helper methods
#endregion
```

---

## 2. Error Handling & Result-Pattern

### 2.1 The Result-Pattern: Core Principle

**RULE: No exceptions for flow control - Use Result-Pattern instead**

```csharp
// ❌ WRONG: Exception for flow control
public User GetUser(int id)
{
    if (id <= 0)
        throw new ArgumentException("Invalid ID");  // ❌ Wrong
    
    try
    {
        var user = _repository.FindById(id);
        if (user == null)
            throw new NotFoundException("User not found");  // ❌ Wrong
        return user;
    }
    catch (Exception ex)
    {
        _logger.Error($"Failed to get user: {ex}");
        throw;
    }
}

// ✅ RIGHT: Result-Pattern for flow control
public Result<User> GetUser(int id)
{
    if (id <= 0)
        return new Result<User>.Failure(
            ErrorCodes.InvalidId,
            ErrorCodes.InvalidId.ToMessage()
        );
    
    var user = _repository.FindById(id);
    return user == null
        ? new Result<User>.Failure(
            ErrorCodes.NotFound,
            ErrorCodes.NotFound.ToMessage()
        )
        : new Result<User>.Success(user, "User loaded");
}

// Usage: Pattern matching
var result = await _userService.GetUser(userId);
return result.Match(
    onSuccess: (user, msg) => Ok(new { data = user, message = msg }),
    onFailure: (code, msg) =>
    {
        _logger.Warning($"User fetch failed: {code}");
        var statusCode = code.GetStatusCode();
        return StatusCode(statusCode, new { error = new { code, message = msg } });
    }
);
```

### 2.2 Type-Safe Error Codes (CRITICAL)

**RULE: Never use magic strings for error codes**

```csharp
// ❌ WRONG: Magic strings everywhere
return new Result<T>.Failure("InvalidCredentials", "Invalid credentials provided");
return new Result<T>.Failure("UserNotFound", "The user was not found");
return new Result<T>.Failure("AccessDenied", "You don't have permission");

// ✅ RIGHT: Type-safe ErrorCodes class
return new Result<T>.Failure(
    ErrorCodes.InvalidCredentials,
    ErrorCodes.InvalidCredentials.ToMessage()
);

return new Result<T>.Failure(
    ErrorCodes.UserNotFound,
    ErrorCodes.UserNotFound.ToMessage()
);

return new Result<T>.Failure(
    ErrorCodes.AccessDenied,
    ErrorCodes.AccessDenied.ToMessage()
);
```

**Benefits:**
- ✅ Compile-time type checking (no typos)
- ✅ IntelliSense support in IDE
- ✅ Easy refactoring (rename one constant, updates everywhere)
- ✅ Consistent error codes across entire application
- ✅ Single source of truth for HTTP status mapping

### 2.3 ErrorCodes Class Structure

**Location:** `backend/shared/types/ErrorCodes.cs`

```csharp
public static class ErrorCodes
{
    // Authentication & Authorization
    public const string InvalidCredentials = nameof(InvalidCredentials);
    public const string UserInactive = nameof(UserInactive);
    public const string InvalidToken = nameof(InvalidToken);
    public const string UserNotFound = nameof(UserNotFound);
    public const string Unauthorized = nameof(Unauthorized);
    public const string AccessDenied = nameof(AccessDenied);
    
    // Data Operations
    public const string NotFound = nameof(NotFound);
    public const string AlreadyExists = nameof(AlreadyExists);
    public const string Duplicate = nameof(Duplicate);
    
    // ... more error codes
}

public static class ErrorCodeStatusMap
{
    private static readonly Dictionary<string, int> StatusCodeMap = new()
    {
        { ErrorCodes.InvalidCredentials, 400 },
        { ErrorCodes.InvalidToken, 401 },
        { ErrorCodes.Unauthorized, 401 },
        { ErrorCodes.AccessDenied, 403 },
        { ErrorCodes.NotFound, 404 },
        { ErrorCodes.AlreadyExists, 409 },
        // ... more mappings
    };
}

public static class ErrorCodeExtensions
{
    /// <summary>Gets human-readable message for error code.</summary>
    public static string ToMessage(this string errorCode) => errorCode switch
    {
        ErrorCodes.InvalidCredentials => "Invalid email or password",
        ErrorCodes.UserInactive => "User account is inactive",
        ErrorCodes.NotFound => "Resource not found",
        // ... more messages
        _ => "An error occurred"
    };
    
    /// <summary>Gets HTTP status code for error code.</summary>
    public static int GetStatusCode(this string errorCode) =>
        ErrorCodeStatusMap.StatusCodeMap.TryGetValue(errorCode, out var statusCode)
            ? statusCode
            : 500;  // Default to Internal Server Error
}
```

### 2.4 Adding New Error Codes

**Step 1:** Define in ErrorCodes class
```csharp
public const string MyNewError = nameof(MyNewError);
```

**Step 2:** Add to ErrorCodeStatusMap
```csharp
{ ErrorCodes.MyNewError, 400 },  // Bad Request
```

**Step 3:** Add to ToMessage() switch
```csharp
ErrorCodes.MyNewError => "Your custom error message",
```

**Step 4:** Use in service
```csharp
return new Result<T>.Failure(ErrorCodes.MyNewError, ErrorCodes.MyNewError.ToMessage());
```

### 2.5 Result Composition in Controllers

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    var result = await _authService.LoginAsync(request.Email, request.Password);
    
    return result.Match(
        onSuccess: (response, msg) => Ok(new
        {
            success = true,
            data = response,
            message = msg
        }),
        onFailure: (code, msg) =>
        {
            var statusCode = code.GetStatusCode();
            return StatusCode(statusCode, new
            {
                success = false,
                error = new
                {
                    code,
                    message = code.ToMessage(),
                    timestamp = DateTime.UtcNow
                }
            });
        }
    );
}
```

---

## 3. Type Safety & Null Handling

### 3.1 Nullable Reference Types

**RULE: Enable `#nullable enable` in all files**

```csharp
#nullable enable

namespace B2Connect.AuthService.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }
    
    // ✅ Explicit non-nullable return
    public User GetUserOrThrow(int userId)
    {
        var user = _userRepository.FindById(userId);
        return user ?? throw new InvalidOperationException("User not found");
    }
    
    // ✅ Explicit nullable return
    public User? TryGetUser(int userId)
    {
        return _userRepository.FindById(userId);
    }
    
    // ✅ Use Result<T> for expected failures
    public Result<User> GetUser(int userId)
    {
        var user = _userRepository.FindById(userId);
        return user == null
            ? new Result<User>.Failure(ErrorCodes.NotFound, "User not found")
            : new Result<User>.Success(user, "User loaded");
    }
}
```

### 3.2 Null-Coalescing & Guard Clauses

```csharp
// ✅ Guard clauses at method entry
public Result<T> ProcessData(string? input, int userId)
{
    if (string.IsNullOrWhiteSpace(input))
        return new Result<T>.Failure(ErrorCodes.InvalidInput, "Input required");
    
    if (userId <= 0)
        return new Result<T>.Failure(ErrorCodes.InvalidId, "Valid ID required");
    
    // Safe to proceed - input is not null
    var processed = input.Trim();
    // ...
}

// ✅ Null-coalescing operator
var result = _cache.Get(key) ?? await _repository.GetAsync(key);

// ❌ AVOID: Nested null checks
if (user != null)
{
    if (user.Tenant != null)
    {
        if (user.Tenant.Settings != null)
        {
            // Code here
        }
    }
}
```

### 3.3 Using Records for Data Transfer

```csharp
// ✅ Immutable, type-safe DTOs
public record LoginRequest(string Email, string Password);

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);

// ✅ With validation
public record CreateUserRequest(string Email, string Name)
{
    public Result<CreateUserRequest> Validate()
    {
        if (string.IsNullOrWhiteSpace(Email))
            return new Result<CreateUserRequest>.Failure(ErrorCodes.InvalidInput, "Email required");
        
        if (!Email.Contains("@"))
            return new Result<CreateUserRequest>.Failure(ErrorCodes.InvalidFormat, "Invalid email format");
        
        return new Result<CreateUserRequest>.Success(this, "Valid request");
    }
}
```

---

## 4. Async/Await & Threading

### 4.1 Async Method Naming

```csharp
// ✅ Async methods end with "Async"
public async Task<User> GetUserAsync(int userId)
{
    return await _repository.GetUserAsync(userId);
}

// ✅ Don't use Async suffix for property getters
public IQueryable<User> Users => _repository.Users;

// ❌ AVOID: Mixing sync and async
public Task<User> GetUser(int userId)  // ❌ Missing Async suffix
{
    return _repository.GetUserAsync(userId);
}
```

### 4.2 ConfigureAwait Pattern

```csharp
// ✅ Use ConfigureAwait(false) in libraries to avoid context capture
public async Task<Result<User>> GetUserAsync(int userId)
{
    var user = await _repository.GetUserAsync(userId).ConfigureAwait(false);
    // ...
}

// ✅ Can use ConfigureAwait(true) in web apps
public async Task<IActionResult> GetUser(int userId)
{
    var result = await _userService.GetUserAsync(userId).ConfigureAwait(true);
    // ...
}
```

### 4.3 Cancellation Tokens

```csharp
// ✅ Accept CancellationToken parameter
public async Task<Result<List<User>>> GetUsersAsync(
    int tenantId,
    CancellationToken cancellationToken = default)
{
    var users = await _repository.GetUsersAsync(tenantId, cancellationToken).ConfigureAwait(false);
    return new Result<List<User>>.Success(users, "Users loaded");
}

// ✅ Use CancellationToken in loops
foreach (var user in users)
{
    cancellationToken.ThrowIfCancellationRequested();
    await ProcessUserAsync(user, cancellationToken).ConfigureAwait(false);
}
```

---

## 5. Testing & Quality

### 5.1 Unit Test Naming

```csharp
[TestClass]
public class AuthServiceTests
{
    // ✅ Pattern: MethodName_Condition_ExpectedResult
    [TestMethod]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var service = CreateAuthService();
        var request = new LoginRequest("user@example.com", "password123");
        
        // Act
        var result = await service.LoginAsync(request.Email, request.Password);
        
        // Assert
        Assert.IsTrue(result is Result<AuthResponse>.Success);
    }
    
    [TestMethod]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsFailureWithInvalidCredentialsCode()
    {
        // Arrange
        var service = CreateAuthService();
        
        // Act
        var result = await service.LoginAsync("wrong@example.com", "wrong");
        
        // Assert
        var failure = Assert.IsInstanceOfType<Result<AuthResponse>.Failure>(result);
        Assert.AreEqual(ErrorCodes.InvalidCredentials, failure.Code);
    }
}
```

### 5.2 Test Organization

```
Tests/
├── CatalogService.Tests/
│   ├── Services/
│   │   ├── ProductServiceTests.cs
│   │   ├── BrandServiceTests.cs
│   │   └── CategoryServiceTests.cs
│   ├── Controllers/
│   │   └── ProductControllerTests.cs
│   └── Fixtures/
│       └── TestDataBuilder.cs
├── AuthService.Tests/
│   ├── Services/
│   │   └── AuthServiceTests.cs
│   └── Integration/
│       └── AuthFlowTests.cs
└── Shared.Tests/
    └── Types/
        └── ResultTests.cs
```

### 5.3 Test Coverage Goals

- **Minimum 70%** code coverage for services
- **100% coverage** for Result<T> and error handling code
- **Happy path + 2-3 sad paths** per method
- **Edge cases** for boundary conditions

---

## 6. Documentation & Comments

### 6.1 XML Documentation

```csharp
/// <summary>
/// Authenticates a user with email and password.
/// </summary>
/// <param name="email">The user's email address.</param>
/// <param name="password">The user's password in plain text.</param>
/// <returns>
/// A <see cref="Result{AuthResponse}"/> containing:
/// - <see cref="Result{T}.Success"/> with an <see cref="AuthResponse"/>
/// - <see cref="Result{T}.Failure"/> with error code <see cref="ErrorCodes.InvalidCredentials"/>
/// </returns>
/// <remarks>
/// The password is hashed using BCrypt with 12 rounds.
/// Failed login attempts are logged for security auditing.
/// </remarks>
public async Task<Result<AuthResponse>> LoginAsync(string email, string password)
{
    // Implementation...
}
```

### 6.2 When to Comment

```csharp
// ✅ WHY (not WHAT)
// Use exponential backoff to avoid overwhelming the external API
await RetryWithBackoffAsync(() => _externalService.CallAsync(), maxRetries: 3);

// ✅ Complex logic explanation
// Group by tenant, then by status, to optimize the query plan
var grouped = orders
    .GroupBy(o => o.TenantId)
    .SelectMany(g => g.GroupBy(o => o.Status))
    .ToList();

// ❌ AVOID: Obvious comments
int userId = 42;  // Set userId to 42
var result = await _service.GetAsync(id);  // Get the result
```

---

## 7. Performance & Resource Management

### 7.1 LINQ & Query Optimization

```csharp
// ✅ Execute query on database, not in-memory
var activeUsers = await _context.Users
    .Where(u => u.IsActive && u.TenantId == tenantId)
    .Select(u => new { u.Id, u.Email })
    .ToListAsync();

// ❌ AVOID: Loading all data then filtering
var allUsers = await _context.Users.ToListAsync();
var activeUsers = allUsers.Where(u => u.IsActive).ToList();
```

### 7.2 Connection & Resource Management

```csharp
// ✅ Using statement for automatic cleanup
public async Task<Result<User>> GetUserAsync(int userId)
{
    using var connection = _database.CreateConnection();
    // Use connection
    return result;  // Connection automatically disposed
}

// ✅ Async disposal for async resources
await using var stream = File.OpenRead("data.txt");
var content = await stream.ReadToEndAsync();
```

### 7.3 Pagination

```csharp
// ✅ Always paginate large result sets
public async Task<Result<PaginatedResult<User>>> GetUsersAsync(
    int tenantId,
    int pageNumber = 1,
    int pageSize = 50)
{
    const int MaxPageSize = 100;
    pageSize = Math.Min(pageSize, MaxPageSize);
    
    var skip = (pageNumber - 1) * pageSize;
    var users = await _context.Users
        .Where(u => u.TenantId == tenantId)
        .OrderBy(u => u.Id)
        .Skip(skip)
        .Take(pageSize)
        .ToListAsync();
    
    var total = await _context.Users
        .Where(u => u.TenantId == tenantId)
        .CountAsync();
    
    return new Result<PaginatedResult<User>>.Success(
        new PaginatedResult<User>(users, pageNumber, pageSize, total)
    );
}
```

---

## 8. Frontend (Vue.js / TypeScript)

### 8.1 Component Naming & Structure

```vue
<!-- ✅ UserForm.vue -->
<script setup lang="ts">
import { ref, computed } from 'vue'
import type { User } from '@/types'

const props = defineProps<{
    user?: User
}>()

const emit = defineEmits<{
    (event: 'save', user: User): void
    (event: 'cancel'): void
}>()

const form = ref({
    email: props.user?.email ?? '',
    name: props.user?.name ?? '',
})

const isValid = computed(() => {
    return form.value.email && form.value.name
})

const handleSubmit = () => {
    if (!isValid.value) return
    emit('save', form.value as User)
}
</script>

<template>
    <form @submit.prevent="handleSubmit">
        <input v-model="form.email" type="email" />
        <input v-model="form.name" type="text" />
        <button :disabled="!isValid" type="submit">Save</button>
        <button @click="emit('cancel')" type="button">Cancel</button>
    </form>
</template>

<style scoped>
/* Component styles */
</style>
```

### 8.2 TypeScript Types & Interfaces

```typescript
// ✅ Use interfaces for data models
interface User {
    id: number
    email: string
    name: string
    tenantId: number
    isActive: boolean
}

// ✅ Use types for unions and constraints
type ApiResponse<T> = 
    | { success: true; data: T }
    | { success: false; error: { code: string; message: string } }

// ✅ Use enums for constants
enum UserRole {
    Admin = 'Admin',
    Manager = 'Manager',
    User = 'User',
}

// ✅ Use readonly for immutable data
interface Product {
    readonly id: number
    readonly name: string
    readonly price: number
}
```

### 8.3 API Client Pattern

```typescript
// services/api.ts
import type { ApiResponse } from '@/types'

export const apiClient = {
    async post<T>(url: string, data: unknown): Promise<ApiResponse<T>> {
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data),
            })

            if (!response.ok) {
                const error = await response.json()
                return {
                    success: false,
                    error: {
                        code: error.error?.code ?? 'UnknownError',
                        message: error.error?.message ?? 'An error occurred',
                    },
                }
            }

            const result = await response.json()
            return {
                success: true,
                data: result.data,
            }
        } catch (error) {
            return {
                success: false,
                error: {
                    code: 'NetworkError',
                    message: 'Failed to connect to server',
                },
            }
        }
    },
}
```

---

## 9. Architecture & Design Patterns

### 9.1 Dependency Injection

```csharp
// ✅ Register in Program.cs
builder.Services
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddScoped<ITokenService, TokenService>();

// ✅ Inject in constructor
public class AuthController(
    IAuthenticationService authService,
    ILogger logger)
{
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request.Email, request.Password);
        // Use result
    }
}
```

### 9.2 Service Layer Pattern

```csharp
// ❌ AVOID: Business logic in controller
[HttpPost("users")]
public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
{
    if (string.IsNullOrEmpty(request.Email))
        return BadRequest("Email required");
    
    var user = new User { Email = request.Email, ... };
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    return Ok(user);
}

// ✅ RIGHT: Business logic in service
public class UserService : IUserService
{
    public async Task<Result<User>> CreateUserAsync(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return new Result<User>.Failure(ErrorCodes.InvalidInput, "Email required");
        
        if (await _repository.UserExistsByEmailAsync(request.Email))
            return new Result<User>.Failure(ErrorCodes.Duplicate, "Email already exists");
        
        var user = new User { Email = request.Email, ... };
        await _repository.AddAsync(user);
        return new Result<User>.Success(user, "User created");
    }
}

// ✅ Controller delegates to service
[HttpPost("users")]
public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
{
    var result = await _userService.CreateUserAsync(request);
    return result.Match(
        onSuccess: (user, msg) => Created($"/api/users/{user.Id}", new { data = user }),
        onFailure: (code, msg) => StatusCode(code.GetStatusCode(), new { error = new { code, message = msg } })
    );
}
```

### 9.3 Repository Pattern

```csharp
public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
    }

    public async Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
```

---

## 📚 Quick Reference

### Error Handling Flow
```
User Input
    ↓
Validate (Guard Clauses)
    ↓
Result<T> Success/Failure
    ↓
Service Logic
    ↓
Return Result<T>
    ↓
Controller.Match()
    ↓
HTTP Response with proper status code
```

### Common ErrorCodes

| Code | HTTP Status | Use Case |
|------|-------------|----------|
| `InvalidCredentials` | 400 | Login with wrong password |
| `InvalidInput` | 400 | Validation failed |
| `InvalidToken` | 401 | Expired or malformed token |
| `Unauthorized` | 401 | Missing auth header |
| `AccessDenied` | 403 | Insufficient permissions |
| `NotFound` | 404 | Resource doesn't exist |
| `AlreadyExists` | 409 | Duplicate entry |
| `OperationFailed` | 500 | Unexpected error |

---

## 📖 References

- [ERROR_CODES_TYPE_SAFE.md](ERROR_CODES_TYPE_SAFE.md) - Detailed error code system
- [RESULT_PATTERN_GUIDE.md](RESULT_PATTERN_GUIDE.md) - Result pattern implementation
- [.copilot-specs.md](.copilot-specs.md) - Full specifications
- [DEVELOPMENT.md](DEVELOPMENT.md) - Development setup

---

**Status:** 🟢 ACTIVE AND ENFORCED  
**Last Review:** 26. Dezember 2025  
**Version:** 1.0
