# 🤖 KI-Assistenten Richtlinien & Prompt-Bibliothek

**Zielgruppe:** Development Team  
**Status:** AI Guidelines & Prompt Collection  
**Version:** 1.0  
**Erstellt:** 27. Dezember 2025

---

## 📋 Inhaltsverzeichnis

1. [Überblick](#überblick)
2. [KI-Integration Prinzipien](#ki-integration-prinzipien)
3. [Sicherheits-Checklisten](#sicherheits-checklisten)
4. [Architektur-Anforderungen](#architektur-anforderungen)
5. [Prompt-Bibliothek](#prompt-bibliothek)
6. [Code-Review Checklisten](#code-review-checklisten)
7. [Common Mistakes](#common-mistakes)
8. [Best Practices](#best-practices)

---

## 🎯 Überblick

Dieses Dokument wurde aus den umfassenden Reviews erstellt:
- 6-Perspective Review (Lead Dev, Architect, QA, Security, GDPR, Code Quality)
- Pentester Security Review (5 CRITICAL, 8 HIGH vulnerabilities)
- Software/Technical Documentation
- Requirements & Specifications

**Ziel:** KI-Assistenten (wie GitHub Copilot, ChatGPT, Claude) in zukunftigen Entwicklungsaufgaben die richtigen Anforderungen, Patterns und Best Practices vorgeben, damit sie bessere Code-Vorschläge machen können.

---

## 🛡️ KI-Integration Prinzipien

### 1. KI als Code-Vorschlag-System (Nicht Entscheidungsträger)
- ✅ KI generiert Code-Basis
- ✅ Developer überprüft & validiert
- ✅ Code wird vollständig getestet
- ✅ Security wird manuell überprüft
- ❌ KI macht keine Sicherheits-Entscheidungen
- ❌ KI definiert nicht Architektur
- ❌ KI entscheidet nicht über Compliance

### 2. Alles muss überprüft werden
```
Code von KI
  ↓ 
Automatische Tests
  ↓
Sicherheits-Review
  ↓
Code Review (2+ Approvals)
  ↓
Merge zu Main
```

### 3. Context ist kritisch
KI braucht:
- ✅ Projekt-Kontext (Tech Stack, Patterns)
- ✅ Security Requirements
- ✅ Architektur-Regeln
- ✅ Testing Standards
- ✅ Fehler-Handling Patterns
- ✅ Beispiel-Code

### 4. Spezifizität zahlt sich aus
```
❌ "Generiere einen Login-Endpoint"

✅ "Generiere einen Login-Endpoint der:
   - JWT Tokens mit RS256 (Schlüssel aus Environment)
   - Tenant ID aus Datenbank liest
   - Serilog Logging nutzt
   - Result<LoginDto> Pattern implementiert
   - FluentValidation für Input
   - Xunit Tests (happy + error path)
   - Rate Limiting (3 Versuche/5 min)
   - Salted Password Hashing (BCrypt)
   macht"
```

---

## 🔐 Sicherheits-Checklisten

### Kritische Sicherheits-Anforderungen

Immer in KI-Prompts mitteilen:

```markdown
### SECURITY REQUIREMENTS (NON-NEGOTIABLE)

1. **Secrets Management**
   - Keine hardcodierten Secrets
   - Alle Secrets aus Umgebungsvariablen
   - Validierung: if (string.IsNullOrEmpty(secret)) throw new Exception()

2. **Input Validation**
   - Alle Inputs validieren
   - FluentValidation für Business Logic
   - Datenbank-Constraints als Failsafe

3. **Data Encryption**
   - PII Felder verschlüsseln (Email, Phone, FirstName, LastName, Address)
   - AES-256 mit EF Core Value Converters
   - Encryption Keys aus Environment

4. **Multi-Tenant Isolation**
   - Tenant ID nur aus JWT Claims lesen
   - Niemals aus User Input
   - Validation: `if (user.TenantId != requestTenantId) return 403`

5. **SQL Injection Prevention**
   - Immer parameterized queries
   - Niemals string concatenation
   - EF Core LINQ queries verwenden

6. **CORS Security**
   - CORS Origins aus Environment konfigurieren
   - Keine hardcodierten localhost URLs
   - Origin Validation: `if (!allowedOrigins.Contains(origin)) return 403`

7. **Authentication & Authorization**
   - JWT Token Validation
   - Role-Based Access Control (RBAC)
   - Claim-Based Authorization

8. **Audit Logging**
   - Alle CRUD Operations loggen
   - User & Timestamp erfassen
   - Before/After State speichern
   - Serilog structured logging verwenden

9. **Rate Limiting**
   - Auf allen public APIs
   - IP-basiert für Login (3 Versuche/5 min)
   - API Key-basiert für Admin APIs

10. **Error Handling**
    - Keine sensitiven Daten in Error Messages
    - Stack Traces nur in Development
    - Konsistente Error Response Format
```

### Security Code Template

```csharp
// ✅ TEMPLATE: Sichere Endpoint Implementation

[HttpPost("login")]
public async Task<Result<LoginDto>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
{
    // 1. VALIDIERUNG
    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
        return Result<LoginDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));

    // 2. AUTHENTIFIZIERUNG
    var user = await _userRepository.FindByEmailAsync(request.Email);
    if (user == null)
        return Result<LoginDto>.Failure("Invalid credentials"); // Vorsicht: Nicht spezifizieren, ob Email existiert

    // 3. PASSWORD VALIDATION
    if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    {
        _auditLogger.LogFailedLogin(user.Id, user.TenantId, HttpContext.Connection.RemoteIpAddress);
        return Result<LoginDto>.Failure("Invalid credentials");
    }

    // 4. JWT GENERATION
    var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
    if (string.IsNullOrEmpty(jwtSecret))
        throw new InvalidOperationException("JWT_SECRET not configured");

    var token = _tokenService.GenerateToken(user.Id, user.TenantId, user.Roles);

    // 5. AUDIT LOGGING
    _auditLogger.LogSuccessfulLogin(user.Id, user.TenantId, HttpContext.Connection.RemoteIpAddress);

    // 6. RATE LIMITING
    await _rateLimiter.CheckLoginAttemptsAsync(request.Email);

    // 7. RESPONSE
    return Result<LoginDto>.Success(new LoginDto 
    { 
        Token = token,
        ExpiresIn = 3600 
    });
}
```

---

## 🏗️ Architektur-Anforderungen

### Immer in Prompts mitteilen:

```markdown
### ARCHITECTURE REQUIREMENTS

**Pattern:** Onion Architecture (Domain → Application → Presentation)

**Technology Stack:**
- Backend: .NET 10, ASP.NET Core Aspire
- Database: PostgreSQL (Primary), EF Core 8
- Messaging: Wolverine (Event-driven)
- Validation: FluentValidation + AOP
- Logging: Serilog
- Testing: xUnit
- Frontend: Vue 3, Vite, TypeScript, Pinia

**Core Patterns:**
1. Repository Pattern für Data Access
2. Dependency Injection für alle Dependencies
3. Result<T> für Error Handling (nicht Exceptions)
4. CQRS für Read/Write Separation
5. Events für Cross-Service Communication
6. FluentValidation für Input Validation
7. Soft Deletes für Data Retention

**Entity Base Template:**
```csharp
public abstract class AuditedEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; } // Multi-Tenant
    
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    
    public DateTime? ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    
    public DateTime? DeletedAt { get; set; } // Soft Delete
    public Guid? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }
}
```

**Bounded Contexts:**
- Identity Service (Auth, Users, Roles)
- Catalog Service (Products, Categories)
- Order Service (Orders, Cart)
- CMS Service (Pages, Content)
- Localization Service (Languages, Translations)
- Search Service (Elasticsearch, Full-text Search)
```

### Architecture Validation Checklist

```
Vor KI-Codegen:
☐ Service Architecture verstanden (Bounded Contexts)
☐ Entity-Relationships definiert
☐ Validation Rules dokumentiert
☐ Error Cases definiert
☐ Authorization Rules klar

Nach KI-Codegen:
☐ Onion Architecture eingehalten
☐ Keine zirkulären Dependencies
☐ Repository Pattern implementiert
☐ Dependency Injection verwendet
☐ Proper Error Handling
☐ Audit Logging vorhanden
☐ Unit Tests vorhanden
```

---

## 📚 Prompt-Bibliothek

### Prompt Template 1: API Endpoint

```markdown
# Generate API Endpoint

Generate a [HTTP_METHOD] endpoint in [ServiceName]Service for [FEATURE]

**Requirements:**
- Response Format: Standard B2X Response (success, data, errors, timestamp, traceId)
- Authentication: JWT Token (user must have [ROLE] role)
- Authorization: Tenant isolation (user.TenantId == request.TenantId)
- Input Validation: FluentValidation validator in separate class
- Error Handling: Return Result<T> with descriptive error messages
- Logging: Use Serilog ILogger for info/error logging
- Status Codes: 200, 201, 400, 401, 403, 404, 409, 500
- Rate Limiting: Apply to login endpoints (3 attempts / 5 minutes)
- Tests: Include xUnit unit tests with happy path + error cases

**Data Model:**
[PROVIDE DTO/ENTITY DETAILS]

**Business Logic:**
[PROVIDE BUSINESS RULES]

**Error Cases:**
- [ERROR 1]: Return [STATUS_CODE]
- [ERROR 2]: Return [STATUS_CODE]

**Example:**
Generate endpoint: POST /api/v1/products
Input: CreateProductRequest { Name, Description, Price, CategoryId }
Output: ProductDto { Id, Name, Description, Price, CreatedAt }
Errors: Duplicate Name (409), Invalid Price (400), Unauthorized (403)
```

### Prompt Template 2: Database Migration

```markdown
# Generate EF Core Migration

Generate EF Core migration for [ENTITY] entity with:

**Entity Details:**
- Base Class: AuditedEntity (includes TenantId, CreatedAt, CreatedBy, etc.)
- Properties: [LIST PROPERTIES WITH TYPES]
- Relationships: [DEFINE FOREIGN KEYS]
- Indexes: [DEFINE INDEXES FOR QUERY PERFORMANCE]
- Constraints: [UNIQUE, CHECK, DEFAULT VALUES]

**Encryption:**
- Encrypted Fields: [LIST PII FIELDS LIKE EMAIL, PHONE]
- Encryption: AES-256 using EF Core Value Converters
- Key Management: From IConfiguration

**Data Retention:**
- Soft Delete: Include IsDeleted, DeletedAt, DeletedBy columns
- Cascade Delete: No automatic cascade (validate manually)

**Migration:**
1. Generate migration file: `dotnet ef migrations add Add[Entity]`
2. Add column comments for clarity
3. Include rollback strategy
4. Update DbContext

**Also Generate:**
- EF Core entity configuration (HasKey, HasIndex, etc.)
- Seed data if applicable
- Documentation comments
```

### Prompt Template 3: Validation Handler

```markdown
# Generate Validation Handler

Generate FluentValidation handler for [COMMAND/QUERY]:

**Using:**
- Framework: FluentValidation for .NET
- Pattern: AOP with [ASPECT NAME]
- Input Type: [REQUEST DTO CLASS]

**Validation Rules:**
[LIST RULES WITH EXAMPLES]

**Custom Validators:**
- Rule 1: [DESCRIPTION]
- Rule 2: [DESCRIPTION]

**Error Messages:**
- Use localization keys for multilingual support
- Format: validation.errors.[field].[rule]
- Example: validation.errors.email.required

**Also Generate:**
- Unit tests with valid/invalid inputs
- Integration tests if using AOP
- Error response examples
```

### Prompt Template 4: Unit Test Suite

```markdown
# Generate Unit Tests

Generate xUnit tests for [CLASS/METHOD]:

**Class Under Test:** [CLASS_NAME]
**Methods to Test:** [METHOD_1], [METHOD_2]

**Pattern:**
- Arrange → Act → Assert
- Mock all external dependencies (IRepository, IService, etc.)
- Test happy path + all error cases
- Use Moq for mocking

**Test Cases:**
1. [HAPPY_PATH_SCENARIO]
2. [ERROR_CASE_1]
3. [ERROR_CASE_2]
4. [EDGE_CASE]

**Assertions:**
- Verify return value
- Verify exception thrown if expected
- Verify mock calls (method called X times with Y parameters)
- Verify state changes if applicable

**Also Generate:**
- Test fixtures / shared data
- Helper methods for test setup
- Clear test names (should_DoSomething_When_Condition)
```

---

## ✅ Code-Review Checklisten

### Pre-Merge Security Checklist

```markdown
## 🔐 SECURITY REVIEW

**Secrets & Configuration:**
- [ ] Keine hardcodierten Secrets in Code
- [ ] Secrets aus Environment.GetEnvironmentVariable() gelesen
- [ ] Secret-Validierung: `if (string.IsNullOrEmpty(secret)) throw ...`
- [ ] Configuration über appsettings.json + IConfiguration
- [ ] Keine Secrets in Logs/Error Messages

**Input Validation:**
- [ ] Alle HTTP Inputs validiert
- [ ] Length/Format/Type Validierung
- [ ] SQL Injection Prevention (parameterized queries)
- [ ] File Upload Validation (size, extension, content-type)
- [ ] XSS Prevention (output encoding)

**Authentication & Authorization:**
- [ ] JWT Token Validation
- [ ] Tenant ID aus Claims, nicht Input
- [ ] Role-Based Access Control
- [ ] Authorization Attribute auf Endpoints
- [ ] Proper HTTP Status Codes (401, 403)

**Data Protection:**
- [ ] PII Fields verschlüsselt (Email, Phone, FirstName, LastName, Address)
- [ ] Encryption Keys aus Environment
- [ ] Proper Key Rotation Strategy
- [ ] GDPR Compliance (right to delete, data export)

**Audit & Logging:**
- [ ] Audit Log für alle CRUD Operations
- [ ] User & Timestamp erfasst
- [ ] Before/After State für Updates
- [ ] Failed Login Attempts geloggt
- [ ] API Access geloggt (optional)

**Error Handling:**
- [ ] Keine sensitiven Daten in Error Response
- [ ] Stack Traces nur in Development
- [ ] Konsistente Error Response Format
- [ ] Proper Exception Types
- [ ] Logging von Exceptions

**Rate Limiting:**
- [ ] Login Endpoints Rate Limited (3/5 min)
- [ ] API Endpoints Rate Limited
- [ ] DDoS Protection
- [ ] JWT Token Expiration

**CORS & Headers:**
- [ ] CORS Origins aus Environment
- [ ] Sicherheits-Header (HSTS, CSP, X-Content-Type-Options)
- [ ] No Wildcard CORS
```

### Pre-Merge Architecture Checklist

```markdown
## 🏗️ ARCHITECTURE REVIEW

**Design Patterns:**
- [ ] Onion Architecture (Domain → Application → Presentation)
- [ ] Repository Pattern für Data Access
- [ ] Dependency Injection für Dependencies
- [ ] Result<T> für Error Handling
- [ ] CQRS für Read/Write Separation (wo applicable)
- [ ] Event-Driven für Service Communication

**Code Quality:**
- [ ] Keine Zirkulären Dependencies
- [ ] Single Responsibility Principle
- [ ] Method Size < 30 Zeilen
- [ ] Class Size < 300 Zeilen
- [ ] Nesting Depth < 3 Levels
- [ ] Keine Magic Numbers/Strings

**Naming & Documentation:**
- [ ] PascalCase für Classes/Methods/Properties
- [ ] camelCase für Variables/Parameters
- [ ] Semantic Names (nicht x, temp, data)
- [ ] XML Documentation auf Public Members
- [ ] Comments für Komplexe Logik

**Testing:**
- [ ] Unit Tests vorhanden
- [ ] Happy Path + Error Cases getestet
- [ ] Mocks für External Dependencies
- [ ] Test Coverage > 80%
- [ ] Integration Tests für API Endpoints

**Performance:**
- [ ] No N+1 Queries
- [ ] Indexes auf Query Columns
- [ ] Pagination für List Endpoints
- [ ] Async/Await Patterns
- [ ] Caching Strategy (wenn needed)
```

---

## ⚠️ Common Mistakes (Zu vermeiden)

### KI macht häufig diese Fehler:

| Fehler | Problem | Lösung |
|--------|---------|--------|
| **Hardcoded Secrets** | `var secret = "key-123"` | Aus Environment: `Environment.GetEnvironmentVariable("KEY")` |
| **String SQL** | `$"SELECT * FROM Users WHERE Id = {id}"` | Parameterized: EF Core LINQ oder `@id` Parameter |
| **Silent Exception Swallow** | `try { ... } catch { }` | Logg Exception: `_logger.LogError(ex, ...)` |
| **Sync over Async** | `Task.Result` | Awaiten: `await _service.GetAsync()` |
| **No Validation** | Direkt Datenbank speichern | Validieren vor Save |
| **Hardcoded URLs** | `"http://localhost:5000"` | Aus Environment Config |
| **Wrong Auth Check** | Nur JWT validieren, nicht Tenant | Prüfen: `user.TenantId == request.TenantId` |
| **Missing Audit Log** | Keine User-Tracking | CreatedBy/ModifiedBy speichern |
| **Bad Error Messages** | `"Error: something went wrong"` | Spezifisch: `"Email must contain @ symbol"` |
| **No Tests** | Nur manuelles Testen | Xunit Unit + Integration Tests |

---

## 🎯 Best Practices

### 1. Prompt Engineering

```markdown
**GUT:**
"Generate a CreateOrderCommand handler that:
- Validates order items (quantity > 0, product exists)
- Checks inventory (quantity <= available stock)
- Encrypts sensitive data (credit card)
- Publishes OrderCreated event
- Logs audit trail
- Returns Result<OrderDto>
- Includes xUnit tests"

**SCHLECHT:**
"Generate order creation code"
```

### 2. Code Review Discipline

```
Immer reviewen, nicht blind akzeptieren:
✅ Read every line of AI-generated code
✅ Verify security practices
✅ Check test coverage
✅ Run tests locally
✅ Review SQL generated by EF Core
✅ Check error handling
```

### 3. Iterative Refinement

```
Wenn KI-Code nicht perfekt:
1. Identifiziere genaue Probleme
2. Schreibe spezifischen Refinement-Prompt
3. Ersetze nur fehlerhaften Code-Teil
4. Verifiziere erneut
5. Merge nur wenn 100% sicher
```

### 4. Know When NOT to Use AI

```
❌ Sicherheits-kritische Logik (KI kann Fehler machen)
❌ Complex Business Rules (KI kann Anforderungen missverstehen)
❌ Architectural Decisions (KI hat keine Kontex)
❌ GDPR/Compliance Implementation (zu riskant)

✅ Boilerplate Code
✅ Tests generieren
✅ Documentation schreiben
✅ Refactorings durchführen
```

### 5. Security-First Mindset

```
Immer diese Fragen stellen:
1. Kann ein Attacker diese Funktion missbrauchen?
2. Sind Secrets hardcoded?
3. Sind alle Inputs validiert?
4. Ist Tenant Isolation gewährleistet?
5. Wird alles gelogged?
6. Können normale Users sensible Daten sehen?
7. Sind Error Messages zu spezifisch?
8. Gibt es SQL Injection Möglichkeiten?
```

---

## 📋 Zusammenfassung der Key-Insights

**Aus dem Review extrahiert:**

### P0 - CRITICAL FINDINGS
1. **Hardcoded JWT Secrets** (CVSS 9.8)
   - KI-Anforderung: Immer Environment.GetEnvironmentVariable("JWT_SECRET") verwenden
   
2. **No PII Encryption** (CVSS 8.6)
   - KI-Anforderung: Email, Phone, FirstName, LastName, Address mit AES-256 verschlüsseln
   
3. **Tenant Isolation Bypass** (CVSS 8.9)
   - KI-Anforderung: TenantId IMMER aus JWT Claims lesen, niemals aus User Input
   
4. **No Audit Logging** (CVSS 7.2)
   - KI-Anforderung: Immer CreatedBy/ModifiedBy/DeletedBy speichern

5. **Hardcoded CORS** (CVSS 7.5)
   - KI-Anforderung: CORS Origins aus Environment Configuration lesen

### P1 - HIGH PRIORITY FINDINGS
- Rate Limiting fehlend → KI muss RateLimiter implementieren
- Keine Inputvalidierung → KI muss FluentValidation verwenden
- Fehlendes Error Logging → KI muss Serilog nutzen
- Keine HTTPS Enforcement → KI muss HSTS Header setzen

---

## 🚀 Next Steps

### Für Development Team:
1. ✅ Diese Datei in Projekt-Wiki integrieren
2. ✅ Prompt-Bibliothek in IDE Snippets speichern
3. ✅ Code-Review Checklisten für PR-Prozess nutzen
4. ✅ KI-Maßnahmen in APPLICATION_SPECIFICATIONS.md überprüfen
5. ✅ Bei jedem KI-Prompt diese Richtlinien beachten

### Für Future AI Integrations:
1. Pass diese Richtlinien als "System Prompt" an KI-Tools
2. Verwende Prompt-Templates für konsistente Ergebnisse
3. Aktualisiere Richtlinien bei neuen Findings
4. Dokumentiere neue Best Practices

---

**Version:** 1.0  
**Basis:** Comprehensive 6-Perspective Review  
**Gültigkeit:** Bis Dezember 2026  
**Review-Zyklus:** Vierteljährlich

**Verwandte Dokumente:**
- [APPLICATION_SPECIFICATIONS.md](docs/guides/index.md) - Kapitel "AI Development Guidelines"
- [COMPREHENSIVE_REVIEW.md](../../guides/QUICK_REFERENCE.md) - Original Review
- [PENTESTER_REVIEW.md](docs/guides/index.md) - Security Findings
- [SECURITY_HARDENING_GUIDE.md](../../guides/SECURITY_HARDENING_GUIDE.md) - Implementation Guide
