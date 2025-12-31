# Tech Lead / Architect - Zugeordnete Issues

**Status**: 0/1 Assigned (+ Code Review Oversight)  
**Effort**: 10-15 Story Points  
**Kritischer Pfad**: Sprint 1 (Epic Architecture)

---

## Epic Overview

| # | Titel | Type | Punkte |
|---|-------|------|--------|
| #4 | **Epic: Customer Registration Flow** | Epic | 15 |

---

## Epic #4: Customer Registration Flow (Breakdown)

**Epic Scope**: Complete customer onboarding from signup to verified user

**User Stories** (Sub-Issues):
- #5: User Registration Handler (Backend)
- #6: Email Verification Logic (Backend)
- #7: Password Reset Flow (Backend)
- #8: Duplicate Detection Service (Backend)
- #9: Multi-Tenant Context (Backend)
- #10: JWT Token Generation (Backend)
- #11: User Roles & Permissions (Backend)
- #12: Registration Endpoints (Backend HTTP Handlers)

**Acceptance Criteria** (Epic-Level):
```
✓ User can signup with email/password
✓ Tenant isolation enforced (no cross-tenant access)
✓ Email verification required before login
✓ Password reset available
✓ User roles & permissions assigned
✓ JWT tokens issued with proper claims
✓ Multi-tenant context propagated through stack
✓ Duplicate detection prevents email reuse
✓ All 8 stories completed + integrated
```

**Dependencies**:
- Frontend: #41 (AGB Checkbox), #19 (Vue Components)
- Security: JWT token validation
- Legal: #41 (AGB Requirements)
- Database: Multi-tenant schema

---

## Architecture Responsibilities

### Design Decisions

| Decision | Status | Owner |
|----------|--------|-------|
| Multi-Tenant Context Flow | 📋 Pending | Tech Lead |
| JWT Token Claims Structure | 📋 Pending | Tech Lead |
| User Role/Permission Model | 📋 Pending | Tech Lead |
| Email Verification Strategy | 📋 Pending | Tech Lead |
| Password Security (Argon2) | 📋 Pending | Tech Lead |
| Duplicate Detection Algorithm | 📋 Pending | Tech Lead |
| Error Handling Standards | 📋 Pending | Tech Lead |
| Logging & Audit Trail | 📋 Pending | Security Eng |

### Code Review Oversight

**Tech Lead Responsibilities**:
- ✅ Define architecture patterns for registration flow
- ✅ Review all PRs for #4-#12 (story implementation)
- ✅ Ensure Wolverine pattern consistency (NOT MediatR!)
- ✅ Validate multi-tenant isolation (no data leaks)
- ✅ Security code review (JWT, password handling)
- ✅ Performance validation (< 200ms response time)
- ✅ Integration testing coordination

---

## Architectural Patterns to Establish

### 1. Wolverine HTTP Handler Pattern

**For Story #12 (Registration Endpoints)**

```csharp
// CORRECT Pattern:
// Step 1: Plain POCO command
public class RegisterUserCommand
{
    public string Email { get; set; }
    public string Password { get; set; }
    public Guid TenantId { get; set; }
}

// Step 2: Service with public async method
public class RegistrationService
{
    public async Task<RegisterUserResponse> RegisterUser(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}

// Step 3: Simple DI registration
builder.Services.AddScoped<RegistrationService>();

// Step 4: Wolverine auto-discovers endpoint
// POST /registeruser
```

**Critical**: NO MediatR! NO IRequest<T>! NO IRequestHandler!

**Reference**: `backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs`

---

### 2. Multi-Tenant Context Propagation

**For Story #9 (Multi-Tenant Context)**

Architecture Requirement:
```csharp
// Every request includes X-Tenant-ID header
public class TenantMiddleware
{
    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        var tenantId = context.Request.Headers["X-Tenant-ID"];
        context.Items["TenantId"] = tenantId; // Available in all handlers
        await _next(context);
    }
}

// Every repository query includes tenant filter
public class UserRepository : IUserRepository
{
    public async Task<User?> GetByEmailAsync(Guid tenantId, string email)
    {
        // MUST filter by tenantId - no cross-tenant leaks!
        return await _context.Users
            .Where(u => u.TenantId == tenantId && u.Email == email)
            .FirstOrDefaultAsync();
    }
}
```

**Critical**: Tenant ID is **NOT** from user input, it's from JWT claims!

---

### 3. JWT Token Structure

**For Story #10 (JWT Generation)**

Design Specification:
```json
{
  "sub": "user-id-uuid",
  "tenant_id": "tenant-uuid",
  "email": "user@example.com",
  "roles": ["customer", "store-admin"],
  "iat": 1703755200,
  "exp": 1703758800,
  "iss": "b2connect-identity",
  "aud": "b2connect-api"
}
```

**Requirements**:
- `sub`: User ID (immutable)
- `tenant_id`: Tenant ID (for multi-tenancy)
- `roles`: Array of role strings
- `exp`: 1 hour (3600 seconds)
- Refresh token: 7 days
- Signed with HS256 (shared secret in KeyVault)

---

### 4. Validation Strategy

**For All Stories**

Pattern to enforce:
```csharp
// Use FluentValidation for all commands
public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254);
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]", "Must contain uppercase")
            .Matches(@"[0-9]", "Must contain digit");
    }
}
```

---

### 5. Error Handling & Response Format

**Standardized API Response**

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public string[]? ValidationErrors { get; set; }
}

// Usage:
public async Task<ApiResponse<RegisterUserResponse>> RegisterUser(...)
{
    try
    {
        // Business logic
        return ApiResponse<RegisterUserResponse>.Success(data);
    }
    catch (ValidationException ex)
    {
        return ApiResponse<RegisterUserResponse>.ValidationError(ex.Errors);
    }
    catch (DuplicateEmailException ex)
    {
        return ApiResponse<RegisterUserResponse>.Error("Email already registered");
    }
}
```

---

## Code Review Checklist

**Tech Lead must verify before story completion**:

```
Story #5 (User Registration Handler):
  ☑ Wolverine pattern used (NOT MediatR)
  ☑ FluentValidation implemented
  ☑ Tenant ID from JWT (not request)
  ☑ Password hashed with Argon2
  ☑ Audit logging included
  ☑ Unit tests > 80% coverage
  ☑ Error handling standardized
  ☑ No sensitive data in logs

Story #6 (Email Verification):
  ☑ Token generated securely (random, expires)
  ☑ Token stored hashed (not plain)
  ☑ Email delivery confirmed
  ☑ Retry logic for failures
  ☑ Test email functionality

Story #9 (Multi-Tenant Context):
  ☑ X-Tenant-ID header validated
  ☑ All queries filter by tenant
  ☑ Cross-tenant test case
  ☑ No data leaks possible
  ☑ Performance acceptable (< 200ms)

Story #10 (JWT Token Generation):
  ☑ Token format matches spec
  ☑ Expiry set correctly
  ☑ Refresh token separate
  ☑ Claims include tenant_id
  ☑ Signed with KeyVault secret
```

---

## Integration Points

| Story | Depends On | Enables |
|-------|-----------|---------|
| #5 | Database schema | #6, #8, #9 |
| #6 | #5 | Email sending |
| #7 | #5 | Password recovery |
| #8 | #5 | Duplicate prevention |
| #9 | Middleware setup | All other stories |
| #10 | #5, #9 | #11 (Permissions) |
| #11 | #10 | Role-based access |
| #12 | #5-#11 | API consumers |

---

## Nächste Schritte

1. **Tech Lead zuweisen** (Epic oversight, code review)
2. **Sprint 1 Start**: Define architecture patterns (#4)
3. **Story Kickoff**: Explain patterns to Backend Developers (#5-#12)
4. **Code Review**: Each story completion
5. **Integration Testing**: All 8 stories work together

---

## Reference Documents

- **Wolverine Pattern**: `backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs`
- **Multi-Tenancy**: See `copilot-instructions.md` section "Multi-Tenancy & Context Propagation"
- **Security**: See `copilot-instructions.md` section "P0.1 - JWT & Secrets Management"
- **Testing**: See `TESTING_STRATEGY.md`

