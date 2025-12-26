# Coding Standards & Guidelines Index

**Status:** ✅ CENTRALIZED & ACTIVE  
**Last Updated:** 26. Dezember 2025

---

## 📚 Central Standards Documentation

Alle Code-Standards für das B2Connect-Projekt sind zentral definiert. Diese Dokumentation ist die **Single Source of Truth** für alle Entwickler.

---

## 🎯 Core Documents

### 1. **CODING_STANDARDS.md** (PRIMARY)
**Umfassendes Regelwerk für alle Entwickler**

**Inhalt:**
- ✅ C# Naming Conventions (PascalCase, camelCase, etc.)
- ✅ File Organization & Class Structure
- ✅ **Error Handling & Result-Pattern (Section 2)**
- ✅ **Type-Safe Error Codes (Section 2.2 - 2.4)**
- ✅ Type Safety & Null Handling
- ✅ Async/Await & Threading Patterns
- ✅ Testing & Quality Standards
- ✅ Documentation & Comments
- ✅ Performance & Resource Management
- ✅ Frontend (Vue.js) Standards
- ✅ Architecture & Design Patterns

**Key Takeaways:**
- No exceptions for flow control → Use Result-Pattern
- No magic error strings → Use ErrorCodes class
- Enable `#nullable enable` in all files
- Always use async/await with ConfigureAwait()
- Paginate large result sets (default: 50-100 items)

---

### 2. **.copilot-specs.md** (SPECIFICATIONS)
**Offizielle GitHub Copilot Specifications**

**Sections relevant to Standards:**
- Section 1.2: Naming Conventions
- Section 1.3: Code Organization
- **Section 3.3: Error Handling & Result-Pattern**
- **Section 3.3a: Type-Safe Error Codes (NEW)**
- Section 4: Type Safety & Static Analysis
- Section 5: Performance Considerations

**Key Reference:**
```csharp
// From .copilot-specs.md Section 3.3a
❌ WRONG: return new Result<T>.Failure("InvalidCredentials", "Invalid");
✅ RIGHT: return new Result<T>.Failure(
    ErrorCodes.InvalidCredentials,
    ErrorCodes.InvalidCredentials.ToMessage()
);
```

---

### 3. **ERROR_CODES_TYPE_SAFE.md** (IMPLEMENTATION)
**Detailed Error Code System Documentation**

**Inhalt:**
- Overview of ErrorCodes system
- **27 predefined error codes** with categories
- HTTP status code mapping
- Extension methods (ToMessage(), GetStatusCode())
- Usage examples in services and controllers
- Frontend integration patterns
- How to add new error codes
- Testing impact

**Error Code Categories:**
- Authentication & Authorization (9 codes)
- Data Operations (5 codes)
- Validation (5 codes)
- Operations (6 codes)

---

### 4. **RESULT_PATTERN_GUIDE.md** (DEEP DIVE)
**Comprehensive Result-Pattern Implementation**

**Inhalt:**
- Result<T> type system
- ResultExtensions with functional methods
- Pattern matching with Match() and Fold()
- Composition and chaining
- Async patterns with ResultAsync
- Testing strategies
- Real-world examples

---

## 🔄 Standards Hierarchy

```
.copilot-specs.md (Official Specifications)
    ↓
CODING_STANDARDS.md (Detailed Guidelines)
    ↓
ERROR_CODES_TYPE_SAFE.md (Error Code System)
RESULT_PATTERN_GUIDE.md (Pattern Details)
```

**Resolution Order:**
1. Check `.copilot-specs.md` for official specifications
2. Refer to `CODING_STANDARDS.md` for detailed guidelines
3. Use `ERROR_CODES_TYPE_SAFE.md` for error code reference
4. Consult `RESULT_PATTERN_GUIDE.md` for advanced patterns

---

## 📋 Standards by Topic

### Error Handling
| Document | Section | Focus |
|----------|---------|-------|
| CODING_STANDARDS.md | 2.0 | Comprehensive rules |
| .copilot-specs.md | 3.3 - 3.3a | Official policy |
| ERROR_CODES_TYPE_SAFE.md | Full | Error codes reference |
| RESULT_PATTERN_GUIDE.md | Full | Pattern implementation |

### Type Safety
| Document | Section | Focus |
|----------|---------|-------|
| CODING_STANDARDS.md | 3.0 | Null handling, records |
| .copilot-specs.md | 4.0 | Static analysis |
| CODING_STANDARDS.md | 3.1 | Nullable references |

### Naming Conventions
| Document | Section | Focus |
|----------|---------|-------|
| CODING_STANDARDS.md | 1.1 | Full naming table |
| .copilot-specs.md | 1.2 | Official standards |

### Frontend (Vue.js/TypeScript)
| Document | Section | Focus |
|----------|---------|-------|
| CODING_STANDARDS.md | 8.0 | Vue & TypeScript rules |
| .copilot-specs.md | 1.5 - 1.7 | Vue/Pinia standards |

### Async & Threading
| Document | Section | Focus |
|----------|---------|-------|
| CODING_STANDARDS.md | 4.0 | ConfigureAwait, tokens |
| .copilot-specs.md | 1.1 | Async naming |

### Testing
| Document | Section | Focus |
|----------|---------|-------|
| CODING_STANDARDS.md | 5.0 | Unit testing patterns |

### Performance
| Document | Section | Focus |
|----------|---------|-------|
| CODING_STANDARDS.md | 7.0 | Optimization & pagination |
| .copilot-specs.md | 5.0 | Performance considerations |

---

## ⚡ Quick Reference: Top Rules

### 1. Error Handling (CRITICAL)
```csharp
// ❌ Never exception for flow control
if (user == null) throw new NotFoundException();

// ✅ Use Result-Pattern with type-safe error codes
return user == null
    ? new Result<User>.Failure(ErrorCodes.NotFound, ErrorCodes.NotFound.ToMessage())
    : new Result<User>.Success(user, "User loaded");
```

**Reference:** CODING_STANDARDS.md Section 2, .copilot-specs.md Section 3.3a

### 2. Error Code Usage (CRITICAL)
```csharp
// ❌ Never use magic error strings
return new Result<T>.Failure("InvalidInput", "Invalid input");

// ✅ Use ErrorCodes constants
return new Result<T>.Failure(ErrorCodes.InvalidInput, ErrorCodes.InvalidInput.ToMessage());
```

**Reference:** CODING_STANDARDS.md Section 2.2, ERROR_CODES_TYPE_SAFE.md

### 3. Naming Conventions
```csharp
public class UserService        // ✅ PascalCase
{
    private readonly IUserRepository _userRepository;  // ✅ camelCase with _
    
    public async Task<Result<User>> GetUserAsync(int userId)  // ✅ Async suffix
    {
        var user = await _userRepository.GetAsync(userId);  // ✅ camelCase
        return ...;
    }
}
```

**Reference:** CODING_STANDARDS.md Section 1.1

### 4. Nullable Reference Types
```csharp
#nullable enable  // ✅ Enable in all files

public User? TryGetUser(int id)  // ✅ Nullable return
{
    return _repository.Find(id);
}

public User GetUserOrThrow(int id)  // ✅ Non-nullable return
{
    return _repository.Find(id) ?? throw new InvalidOperationException();
}
```

**Reference:** CODING_STANDARDS.md Section 3.1

### 5. Async/Await Pattern
```csharp
// ✅ Use ConfigureAwait(false) in services
public async Task<Result<User>> GetUserAsync(int userId)
{
    var user = await _repository.GetAsync(userId).ConfigureAwait(false);
    return new Result<User>.Success(user, "User loaded");
}

// ✅ Accept CancellationToken
public async Task<T> ProcessAsync(T item, CancellationToken cancellationToken = default)
{
    return await _service.ProcessAsync(item, cancellationToken).ConfigureAwait(false);
}
```

**Reference:** CODING_STANDARDS.md Section 4

### 6. Controller Error Response
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    var result = await _authService.LoginAsync(request.Email, request.Password);
    
    return result.Match(
        onSuccess: (response, msg) => Ok(new { data = response, message = msg }),
        onFailure: (code, msg) =>
        {
            var statusCode = code.GetStatusCode();  // ✅ Automatic mapping
            return StatusCode(statusCode, new { error = new { code, message = code.ToMessage() } });
        }
    );
}
```

**Reference:** CODING_STANDARDS.md Section 2.5

---

## 🚀 Implementation Checklist

When starting a new service or feature, ensure:

- [ ] **Error Handling:**
  - [ ] Use Result<T> pattern (not exceptions for flow control)
  - [ ] All error codes use ErrorCodes class constants
  - [ ] Controllers use result.Match() with proper status codes

- [ ] **Type Safety:**
  - [ ] Add `#nullable enable` at top of file
  - [ ] Use nullable (T?) and non-nullable (T) intentionally
  - [ ] Enable StrictNullChecks in project file

- [ ] **Naming:**
  - [ ] Classes: PascalCase (UserService, IAuthRepository)
  - [ ] Fields: camelCase with underscore (_userRepository)
  - [ ] Methods: PascalCase, Async suffix if async
  - [ ] Error codes: ErrorCodes.InvalidInput (not "InvalidInput")

- [ ] **Async Patterns:**
  - [ ] Use ConfigureAwait(false) in services
  - [ ] Accept CancellationToken parameter
  - [ ] Propagate cancellation tokens to called methods

- [ ] **Documentation:**
  - [ ] XML doc comments on public methods
  - [ ] Include return type documentation
  - [ ] Document Result<T> success/failure cases

- [ ] **Testing:**
  - [ ] Test naming: MethodName_Condition_ExpectedResult
  - [ ] Happy path + 2-3 sad paths per method
  - [ ] Verify error codes using ErrorCodes constants

---

## 🔗 Cross-References

**Error Handling:**
- Implementation: `backend/shared/types/ErrorCodes.cs`
- Implementation: `backend/shared/types/Result.cs`
- Example: `backend/services/auth-service/src/Services/AuthService.cs`
- Example: `backend/services/auth-service/src/Controllers/AuthController.cs`

**Standards:**
- Central: [CODING_STANDARDS.md](CODING_STANDARDS.md)
- Official: [.copilot-specs.md](.copilot-specs.md)
- Details: [ERROR_CODES_TYPE_SAFE.md](ERROR_CODES_TYPE_SAFE.md)
- Patterns: [RESULT_PATTERN_GUIDE.md](RESULT_PATTERN_GUIDE.md)

---

## 📅 Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 26.12.2025 | Initial centralized standards documentation |

---

## ✅ Status

**🟢 ALL STANDARDS CENTRALIZED & ACTIVE**

- ✅ CODING_STANDARDS.md created
- ✅ .copilot-specs.md Section 3.3a added (ErrorCodes)
- ✅ ERROR_CODES_TYPE_SAFE.md available
- ✅ RESULT_PATTERN_GUIDE.md available
- ✅ All documents cross-referenced

**Next Steps:**
1. Review all standards with team
2. Enforce in code reviews
3. Add Roslyn analyzers for enforcement
4. Create VS Code snippets for common patterns

---

**Maintained by:** Development Team  
**Last Review:** 26. Dezember 2025  
**Status:** 🟢 ACTIVE & ENFORCED
