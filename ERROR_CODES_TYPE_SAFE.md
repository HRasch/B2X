# Error Codes - Type-Safe Error Handling

**Status:** ✅ COMPLETE & INTEGRATED  
**Date:** 26. Dezember 2025  
**Build:** Successful (0 errors)

---

## 🎯 Overview

Instead of using magic strings like `"InvalidCredentials"` throughout the codebase, we now use the `ErrorCodes` static class with strongly-typed constants. This provides:

- ✅ **Type Safety** - Compile-time checking
- ✅ **DRY (Don't Repeat Yourself)** - Single source of truth
- ✅ **IntelliSense** - IDE autocomplete support
- ✅ **Refactoring** - Easy to rename codes everywhere
- ✅ **Documentation** - Clear error code meanings

---

## 📦 Implementation

### ErrorCodes.cs
**Location:** `backend/shared/types/ErrorCodes.cs`

**Contains:**
- ✅ `ErrorCodes` - Static class with all error code constants
- ✅ `ErrorCodeStatusMap` - HTTP status code mapping
- ✅ `ErrorCodeExtensions` - Helper methods (ToMessage(), GetStatusCode())

---

## 📋 Error Code Categories

### Authentication & Authorization
```csharp
ErrorCodes.InvalidCredentials        // Invalid email or password
ErrorCodes.UserInactive              // User account is inactive
ErrorCodes.InvalidToken              // Invalid or expired token
ErrorCodes.UserNotFound              // User not found
ErrorCodes.Unauthorized              // Not authorized to perform action
ErrorCodes.AccessDenied              // Access denied
ErrorCodes.TokenExpired              // Token has expired
ErrorCodes.TwoFactorRequired         // 2FA required
ErrorCodes.TwoFactorEnabled          // 2FA enabled
```

### Data Operations
```csharp
ErrorCodes.NotFound                  // Resource not found
ErrorCodes.AlreadyExists             // Resource already exists
ErrorCodes.InvalidId                 // Invalid ID format
ErrorCodes.Duplicate                 // Duplicate entry
ErrorCodes.InvalidFormat             // Invalid format
```

### Validation
```csharp
ErrorCodes.EmptyKey                  // Key cannot be empty
ErrorCodes.EmptyValue                // Value cannot be empty
ErrorCodes.EmptyCategory             // Category cannot be empty
ErrorCodes.OutOfRange                // Value out of range
ErrorCodes.InvalidInput              // Invalid input
```

### Operations
```csharp
ErrorCodes.OperationFailed           // Operation failed
ErrorCodes.ProviderError             // External provider error
ErrorCodes.SyncFailed                // Synchronization failed
ErrorCodes.TimeoutError              // Operation timed out
ErrorCodes.ConflictError             // Conflict error
ErrorCodes.NoProvidersFound          // No providers found
```

---

## 💻 Usage Examples

### In Services

**Before (Magic String):**
```csharp
return new Result<AuthResponse>.Failure("InvalidCredentials", "Invalid email or password");
```

**After (Type-Safe):**
```csharp
return new Result<AuthResponse>.Failure(ErrorCodes.InvalidCredentials, ErrorCodes.InvalidCredentials.ToMessage());
```

### In Controllers

**Before (Switch on String):**
```csharp
return result.Match(
    onSuccess: (response, msg) => Ok(new { data = response }),
    onFailure: (code, msg) => code switch
    {
        "InvalidCredentials" => Unauthorized(...),
        "UserInactive" => BadRequest(...),
        _ => StatusCode(500, ...)
    }
);
```

**After (Using ErrorCodeStatusMap):**
```csharp
return result.Match(
    onSuccess: (response, msg) => Ok(new { data = response }),
    onFailure: (code, msg) =>
    {
        var statusCode = code.GetStatusCode();
        return StatusCode(statusCode, new { error = new { code, message = code.ToMessage() } });
    }
);
```

---

## 🔗 Extension Methods

### ToMessage()
Converts error code to human-readable message:
```csharp
ErrorCodes.InvalidCredentials.ToMessage()
// Returns: "Invalid email or password"

ErrorCodes.UserInactive.ToMessage()
// Returns: "User account is inactive"
```

### GetStatusCode()
Returns HTTP status code for error:
```csharp
ErrorCodes.InvalidCredentials.GetStatusCode()
// Returns: 400 (Bad Request)

ErrorCodes.NotFound.GetStatusCode()
// Returns: 404 (Not Found)

ErrorCodes.AccessDenied.GetStatusCode()
// Returns: 403 (Forbidden)
```

---

## 📊 HTTP Status Code Mapping

| Error Code | HTTP Status | Meaning |
|-----------|-----------|---------|
| InvalidCredentials | 400 | Bad Request |
| InvalidToken | 401 | Unauthorized |
| UserInactive | 400 | Bad Request |
| TokenExpired | 401 | Unauthorized |
| Unauthorized | 401 | Unauthorized |
| AccessDenied | 403 | Forbidden |
| NotFound | 404 | Not Found |
| AlreadyExists | 409 | Conflict |
| Duplicate | 409 | Conflict |
| InvalidInput | 400 | Bad Request |
| OperationFailed | 500 | Server Error |
| SyncFailed | 500 | Server Error |
| TimeoutError | 500 | Server Error |

---

## ✅ Current Implementation Status

### AuthService - ✅ UPDATED
```csharp
// All error codes now use ErrorCodes constants
LoginAsync()           → ErrorCodes.InvalidCredentials
                      → ErrorCodes.UserInactive
                      → ErrorCodes.TwoFactorRequired

RefreshTokenAsync()    → ErrorCodes.InvalidToken
                      → ErrorCodes.UserNotFound

GetUserByIdAsync()     → ErrorCodes.NotFound

EnableTwoFactorAsync() → ErrorCodes.NotFound
                      → ErrorCodes.TwoFactorEnabled
```

### AuthController - ✅ UPDATED
```csharp
// All endpoints use code.GetStatusCode() and code.ToMessage()
Login()                → Dynamic status code mapping
Refresh()              → Dynamic status code mapping
GetCurrentUser()       → Dynamic status code mapping
Enable2FA()            → Dynamic status code mapping
```

### CatalogService - ✅ UPDATED
```csharp
// ProductService updated to use ErrorCodes
UpdateProductAsync()   → ErrorCodes.NotFound
```

---

## 🎯 Adding New Error Codes

To add a new error code:

**Step 1:** Add constant to `ErrorCodes` class
```csharp
public const string MyNewError = nameof(MyNewError);
```

**Step 2:** Add to `ErrorCodeStatusMap` (optional, defaults to 500)
```csharp
{ ErrorCodes.MyNewError, 400 },  // Bad Request
```

**Step 3:** Add to `ToMessage()` switch (optional, defaults to "An error occurred")
```csharp
ErrorCodes.MyNewError => "Your custom error message",
```

**Step 4:** Use in service
```csharp
return new Result<T>.Failure(ErrorCodes.MyNewError, ErrorCodes.MyNewError.ToMessage());
```

---

## 🧪 Testing Impact

### Unit Tests
```csharp
// Easy assertion on error code
var result = await _service.DoSomethingAsync();
Assert.AreEqual(ErrorCodes.NotFound, ((Result<T>.Failure)result).Code);

// No string comparison
Assert.AreEqual("NotFound", ...);  // ❌ Magic string
Assert.AreEqual(ErrorCodes.NotFound, ...);  // ✅ Type-safe
```

### Integration Tests
```csharp
// Check HTTP status code
var response = await _client.PostAsync("/api/auth/login", ...);
Assert.AreEqual(ErrorCodes.InvalidCredentials.GetStatusCode(), (int)response.StatusCode);
```

---

## 🌍 Frontend Integration

Frontend developers can now:

1. **Use error codes for specific handling:**
```typescript
if (error.code === 'InvalidCredentials') {
    // Show login-specific error message
}
if (error.code === 'TokenExpired') {
    // Redirect to login
}
```

2. **Consistent error responses:**
```json
{
    "error": {
        "code": "InvalidCredentials",
        "message": "Invalid email or password"
    }
}
```

3. **Type-safe error handling:**
Create an enum on frontend matching backend:
```typescript
enum ErrorCode {
    InvalidCredentials = 'InvalidCredentials',
    UserInactive = 'UserInactive',
    NotFound = 'NotFound',
    // ... etc
}
```

---

## 📈 Benefits Summary

| Aspect | Before | After |
|--------|--------|-------|
| **Type Safety** | Magic strings | Constants |
| **Refactoring** | Find & replace strings | Compiler helps |
| **IDE Support** | Manual typing | IntelliSense |
| **Message Mapping** | Scattered throughout | Single class |
| **HTTP Status** | Manual mapping | Automatic |
| **Testing** | String assertions | Constant assertions |
| **Documentation** | Implicit | Explicit in code |

---

## ✅ Verification

All error codes are:
- ✅ Defined in one place (`ErrorCodes.cs`)
- ✅ Mapped to HTTP status codes
- ✅ Mapped to human messages
- ✅ Available via extension methods
- ✅ Type-safe and refactoring-safe

---

## 📚 References

- **ErrorCodes Implementation:** `backend/shared/types/ErrorCodes.cs`
- **AuthService Usage:** `backend/services/auth-service/src/Services/AuthService.cs`
- **AuthController Usage:** `backend/services/auth-service/src/Controllers/AuthController.cs`
- **Result Types:** `backend/shared/types/Result.cs`
- **Result Extensions:** `backend/shared/types/ResultExtensions.cs`

---

## 🚀 Next Steps

1. **Update all remaining services** to use ErrorCodes
2. **Create frontend error code enum** matching backend
3. **Add Roslyn analyzer** to warn on magic error strings
4. **Document error codes** in API documentation
5. **Add error code logging** for debugging

---

**Status:** 🟢 COMPLETE AND DEPLOYED  
**Build Status:** ✅ All projects compile successfully
