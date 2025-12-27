# Application Specifications

**Version:** 1.0  
**Last Updated:** 27. Dezember 2025  
**Status:** Active

---

## Table of Contents

1. [System Overview](#system-overview)
2. [Core Requirements](#core-requirements)
3. [Security Requirements](#security-requirements)
4. [Data Requirements](#data-requirements)
5. [API Specifications](#api-specifications)
6. [Database Schema](#database-schema)
7. [Audit & Compliance Requirements](#audit--compliance-requirements)
8. [Performance Requirements](#performance-requirements)
9. [Deployment Requirements](#deployment-requirements)

---

## System Overview

### Architecture
```
B2Connect - Multi-tenant SaaS E-Commerce Platform
├── Microservices Architecture
├── Bounded Contexts (DDD)
├── API Gateway Pattern (YARP)
└── Multi-cloud Deployment (AWS, Azure, GCP)
```

### Core Services
- **Admin API:** Management and operational tasks
- **Store API:** Customer-facing e-commerce platform
- **Identity Service:** User authentication and authorization
- **Catalog Service:** Product catalog management
- **CMS Service:** Content management
- **Localization Service:** Multi-language support
- **Search Service:** Product search (Elasticsearch)

### Technology Stack
- **Backend:** .NET 10, ASP.NET Core
- **Frontend:** Vue.js 3, Vite, TypeScript
- **Database:** PostgreSQL (Primary), SQL Server (Optional)
- **Messaging:** Wolverine (Event-driven)
- **Orchestration:** .NET Aspire
- **Caching:** Redis
- **Search:** Elasticsearch

---

## Core Requirements

### Functional Requirements

#### User Management
- [ ] User registration with email verification
- [ ] User authentication with JWT tokens
- [ ] Role-based access control (RBAC)
- [ ] Multi-tenant isolation (by tenant ID)
- [ ] User profile management
- [ ] Password reset functionality
- [ ] OAuth2 integration (Google, Microsoft, GitHub)

#### Tenant Management
- [ ] Tenant creation and setup
- [ ] Tenant configuration (branding, localization)
- [ ] Tenant member invitation
- [ ] Tenant data isolation
- [ ] Tenant subscription management

#### Product Management
- [ ] Create/Read/Update/Delete products
- [ ] Product categorization
- [ ] Product search and filtering
- [ ] Product variants (size, color, etc.)
- [ ] Product availability tracking
- [ ] Product images and media
- [ ] Product reviews and ratings

#### Order Management
- [ ] Shopping cart functionality
- [ ] Order creation and tracking
- [ ] Order status workflow
- [ ] Order history
- [ ] Return and refund management
- [ ] Invoice generation

#### Content Management
- [ ] Page creation and management
- [ ] Content publishing workflow
- [ ] Content versioning
- [ ] Content scheduling

---

## Security Requirements

### Authentication & Authorization
- [ ] **P0.1 - JWT Secrets Management**
  - Secrets must not be hardcoded
  - Minimum 32-character length
  - Environment variable configuration
  - Rotation capability

- [ ] **JWT Implementation**
  - HS256 or RS256 algorithms
  - Token expiration (default: 1 hour access, 7 days refresh)
  - Signature validation
  - Role/claim verification

- [ ] **Authorization**
  - Role-based access control (Admin, Manager, User)
  - Resource-level authorization
  - Tenant-scoped authorization

### Network Security
- [ ] **P0.2 - CORS Configuration**
  - Environment-specific origin configuration
  - No hardcoded localhost domains in production
  - Credentials included only when required
  - Preflight request handling

- [ ] **HTTPS Enforcement**
  - SSL/TLS for all communications
  - HSTS header (min-age: 31536000)
  - Certificate pinning for critical endpoints

- [ ] **API Rate Limiting**
  - Global rate limit: 1000 requests/minute per IP
  - Per-user rate limit: 100 requests/minute
  - Burst handling
  - Rate limit headers in response

### Data Security
- [ ] **P0.3 - Encryption at Rest**
  - AES-256 encryption for sensitive data
  - Field-level encryption for PII (Email, Phone, Address, SSN)
  - Key management (Key Vault or equivalent)
  - IV generation and randomization

- [ ] **Encryption in Transit**
  - TLS 1.2 minimum
  - Perfect Forward Secrecy
  - No cleartext password transmission

- [ ] **Sensitive Data Handling**
  - No PII in logs
  - No secrets in configuration files
  - No credit card storage (use tokenization)
  - No password hashing with plain MD5/SHA1 (use bcrypt/Argon2)

### Input Validation & Prevention
- [ ] **Input Validation**
  - All inputs validated server-side
  - Whitelist approach
  - Length limits enforced
  - Data type validation

- [ ] **Injection Prevention**
  - SQL injection: Parameterized queries
  - Command injection: No shell execution of user input
  - Script injection: Content Security Policy headers
  - LDAP injection: Escaped LDAP strings

- [ ] **Output Encoding**
  - HTML encoding for web output
  - JSON encoding for API responses
  - URL encoding for redirects

---

## Data Requirements

### PII Protection
- Email address (encrypted)
- Phone number (encrypted)
- First name (encrypted)
- Last name (encrypted)
- Address (encrypted)
- SSN/Tax ID (encrypted)
- Date of birth (encrypted)

### Audit Logging
- [ ] **P0.4 - Audit Trail**
  - All data modifications logged
  - Timestamp, user, action recorded
  - Before/after values captured
  - Soft deletes (logical deletion)
  - Immutable audit logs

### Data Retention
- Default: Keep all data
- Configurable retention policies per tenant
- GDPR right-to-be-forgotten capability
- Data export capability (GDPR Article 15)

---

## API Specifications

### Common Response Format

```json
{
  "status": "success|error|validation_error",
  "data": {},
  "errors": [
    {
      "code": "ERROR_CODE",
      "message": "Human-readable message",
      "field": "field_name"
    }
  ],
  "meta": {
    "timestamp": "2025-12-27T10:00:00Z",
    "version": "1.0",
    "request_id": "uuid"
  }
}
```

### Common Status Codes

| Code | Meaning | When Used |
|------|---------|-----------|
| 200  | OK | Successful request |
| 201  | Created | Resource created |
| 204  | No Content | Successful request with no response body |
| 400  | Bad Request | Invalid input |
| 401  | Unauthorized | Missing/invalid authentication |
| 403  | Forbidden | Valid auth but insufficient permissions |
| 404  | Not Found | Resource doesn't exist |
| 409  | Conflict | Resource already exists |
| 422  | Unprocessable Entity | Validation error |
| 429  | Too Many Requests | Rate limit exceeded |
| 500  | Server Error | Internal error |
| 503  | Service Unavailable | Service down |

### Authentication Headers

```
Authorization: Bearer <jwt_token>
X-Tenant-ID: <tenant_uuid>
X-Request-ID: <request_uuid>
```

### Pagination

```json
{
  "data": [],
  "pagination": {
    "page": 1,
    "page_size": 20,
    "total": 100,
    "has_next": true,
    "has_previous": false
  }
}
```

### Error Response Format

```json
{
  "status": "error",
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Email is required",
      "field": "email"
    }
  ],
  "meta": {
    "timestamp": "2025-12-27T10:00:00Z"
  }
}
```

---

## Database Schema

### New Tables for P0.4 (Audit Logging)

```sql
-- Audit Log Table
CREATE TABLE AuditLogs (
    Id UUID PRIMARY KEY,
    TenantId UUID NOT NULL,
    UserId UUID,
    EntityType VARCHAR(255) NOT NULL,
    EntityId UUID NOT NULL,
    Action VARCHAR(50) NOT NULL, -- Create, Update, Delete
    OldValues JSONB,
    NewValues JSONB,
    CreatedAt TIMESTAMPTZ NOT NULL,
    IPAddress VARCHAR(45),
    UserAgent TEXT,
    
    INDEX idx_tenant_id (TenantId),
    INDEX idx_entity_type_id (EntityType, EntityId),
    INDEX idx_created_at (CreatedAt),
    CONSTRAINT fk_user FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Audit Log Archive (for data retention)
CREATE TABLE AuditLogsArchive (
    Id UUID PRIMARY KEY,
    TenantId UUID NOT NULL,
    UserId UUID,
    EntityType VARCHAR(255) NOT NULL,
    EntityId UUID NOT NULL,
    Action VARCHAR(50) NOT NULL,
    OldValues JSONB,
    NewValues JSONB,
    CreatedAt TIMESTAMPTZ NOT NULL,
    ArchivedAt TIMESTAMPTZ NOT NULL,
    
    INDEX idx_tenant_id (TenantId),
    INDEX idx_archived_at (ArchivedAt)
);
```

### Updated Base Entity Schema

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    
    // Audit Trail
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    
    // Soft Delete
    public bool IsDeleted { get; set; } = false;
    
    // Optimistic Concurrency
    public byte[] RowVersion { get; set; }
}
```

### Encryption Column Configuration

```csharp
modelBuilder.Entity<User>()
    .Property(u => u.Email)
    .HasMaxLength(256)
    .HasConversion(
        v => encryptionService.Encrypt(v),
        v => encryptionService.Decrypt(v)
    );

modelBuilder.Entity<User>()
    .Property(u => u.PhoneNumber)
    .HasMaxLength(20)
    .HasConversion(
        v => encryptionService.Encrypt(v ?? string.Empty),
        v => string.IsNullOrEmpty(v) ? null : encryptionService.Decrypt(v)
    );
```

---

## Audit & Compliance Requirements

### GDPR Compliance (Article 32 - Security of processing)
- [ ] Data protection by design
- [ ] Encryption of personal data
- [ ] Audit logging of data processing
- [ ] Access control and authentication
- [ ] Regular security assessments

### GDPR Rights Implementation
- [ ] **Article 15 - Right of Access:** Data export API
- [ ] **Article 17 - Right to be Forgotten:** Deletion API
- [ ] **Article 20 - Data Portability:** Export in standard format
- [ ] **Article 21 - Right to Object:** Unsubscribe from marketing

### Audit Requirements
- [ ] All data access logged
- [ ] All modifications logged (user, timestamp, old/new values)
- [ ] Failed authentication attempts logged
- [ ] Configuration changes logged
- [ ] Logs retained for minimum 90 days

### Compliance Standards
- [ ] SOC2 Type II (Security, Availability, Integrity)
- [ ] ISO 27001 (Information Security Management)
- [ ] GDPR (EU data protection)
- [ ] CCPA (California Privacy Rights)

---

## Performance Requirements

### API Response Times
- GET endpoints: < 100ms (95th percentile)
- POST/PUT endpoints: < 500ms (95th percentile)
- Batch operations: < 2s (95th percentile)
- Search operations: < 500ms (95th percentile)

### Database Performance
- Database queries: < 100ms per request
- N+1 query detection: 0 allowed
- Connection pool size: 20-50 based on load
- Query optimization: Always use indexes for WHERE/JOIN conditions

### Caching
- Cache-Control headers set appropriately
- Redis for session and temporary data
- Browser cache for static assets (max-age: 1 year for versioned assets)
- CDN for image and media distribution

### Load Capacity
- Concurrent users: Support 10,000+ concurrent connections
- Requests per second: 1,000+ RPS per service
- Data volume: Support 1TB+ datasets
- File upload: Support up to 100MB files

### Monitoring & Metrics
- API latency (p50, p95, p99)
- Error rate (4xx, 5xx percentages)
- Database connection pool utilization
- Cache hit/miss rates
- Service availability (99.9% target)

---

## Deployment Requirements

### Environment Configuration
- **Development:** Localhost, InMemory database, debug logging
- **Staging:** Cloud environment, PostgreSQL, info logging
- **Production:** Multi-region, PostgreSQL, error logging only

### Infrastructure
- Containerized deployment (Docker)
- Orchestration (Kubernetes or AWS ECS)
- Auto-scaling (based on CPU/Memory metrics)
- Load balancing (HTTPS, health checks)
- Database backup and recovery

### CI/CD Pipeline
- Build on every commit
- Run tests on every PR
- Security scanning (SAST/DAST)
- Container image scanning
- Automated deployment to staging
- Manual approval for production

### Secrets Management
- No secrets in configuration files
- Use environment variables or Key Vault
- Rotate secrets every 90 days
- Audit access to secrets
- Separate secrets per environment

### Monitoring & Logging
- Centralized logging (ELK/Splunk/CloudWatch)
- Distributed tracing (APM)
- Real-time alerts for errors
- Performance monitoring
- Security event logging

---

## Development Guidelines

### Code Quality
- Code coverage minimum: 80%
- No critical/high security issues
- No duplicate code > 5 lines
- Maintainability index > 80
- Cyclomatic complexity < 10

### Testing Requirements
- Unit tests: All business logic
- Integration tests: Service boundaries
- E2E tests: Critical user flows
- Load tests: Performance validation

### Commit Standards
```
<type>(<scope>): <subject>

<body>

<footer>

Types: fix, feat, docs, style, refactor, perf, test, chore
Scope: service/component affected (e.g., auth, catalog)
Subject: Imperative mood, lowercase, no period, max 50 chars
```

### Code Review Requirements
- Minimum 2 approvals for main
- Address all comments
- Squash commits before merge
- Clean commit history

---

## AI Development Guidelines

**Purpose:** Enable AI-assisted development while maintaining quality, security, and architecture consistency.

### AI Code Generation Requirements

#### Security Guidelines (Non-negotiable)
When generating code, AI MUST:
- [ ] Never use hardcoded secrets (JWT keys, API keys, passwords)
- [ ] Always read secrets from environment variables: `Environment.GetEnvironmentVariable("KEY_NAME")`
- [ ] Implement proper secret validation: `if (string.IsNullOrEmpty(secret)) throw new InvalidOperationException("Secret not configured")`
- [ ] Use configuration builders: `builder.Configuration.AddEnvironmentVariables()`
- [ ] Encrypt PII fields (Email, Phone, FirstName, LastName, Address) at database level
- [ ] Use EF Core value converters for encryption: `entity.Property(e => e.Email).HasConversion(new ValueConverter<string, string>(...));`
- [ ] Include audit logging for all CRUD operations
- [ ] Validate tenant ID from JWT claims, never from user input
- [ ] Set CORS origins from environment: `if (!corsOrigins.Contains(origin)) return 403`
- [ ] Include rate limiting on all public APIs

#### Architecture & Pattern Requirements
When generating code, AI MUST:
- [ ] Follow Onion Architecture: Domain → Application → Presentation
- [ ] Implement Repository pattern for data access
- [ ] Use Dependency Injection for all external dependencies
- [ ] Create bounded contexts for logical domain separation
- [ ] Implement FluentValidation for input validation with AOP
- [ ] Use Result<T> pattern for error handling (not exceptions)
- [ ] Implement CQRS for read/write separation where applicable
- [ ] Use events for cross-service communication via Wolverine
- [ ] Add soft deletes to entities (IsDeleted, DeletedAt, DeletedBy)
- [ ] Track creation/modification metadata (CreatedAt, CreatedBy, ModifiedAt, ModifiedBy)

#### Testing Requirements
When generating code, AI MUST:
- [ ] Include unit tests alongside code (minimum 80% coverage)
- [ ] Mock external dependencies (database, HTTP clients)
- [ ] Use xUnit for test framework
- [ ] Follow AAA pattern: Arrange → Act → Assert
- [ ] Include negative test cases (error scenarios)
- [ ] Test authorization checks (tenant isolation, roles)
- [ ] Test validation rules
- [ ] Create integration tests for API endpoints

#### API Design Requirements
When generating API code, AI MUST:
- [ ] Use consistent response format:
  ```json
  {
    "success": true/false,
    "data": {},
    "errors": [],
    "timestamp": "ISO8601",
    "traceId": "guid"
  }
  ```
- [ ] Include proper HTTP status codes (200, 201, 400, 401, 403, 404, 409, 500)
- [ ] Validate all inputs (length, type, format)
- [ ] Return error details for debugging without exposing internals
- [ ] Include request/response logging
- [ ] Add API documentation comments (XML docs)
- [ ] Version APIs (v1, v2) for backward compatibility
- [ ] Implement pagination for list endpoints (page, pageSize, total)
- [ ] Add filters/search capabilities
- [ ] Include sorting options

#### Database Requirements
When generating database code, AI MUST:
- [ ] Use EF Core for data access
- [ ] Create proper relationships (FK, cascade delete consideration)
- [ ] Add appropriate indexes on frequently queried columns
- [ ] Include timestamps (CreatedAt, ModifiedAt, DeletedAt)
- [ ] Implement soft deletes (IsDeleted flag)
- [ ] Encrypt sensitive data at column level
- [ ] Add database constraints (unique, check, default values)
- [ ] Create migrations for schema changes: `dotnet ef migrations add [Name]`
- [ ] Use stored procedures only for complex reporting
- [ ] Include query optimization (includes, selects, no N+1)

#### Frontend Requirements
When generating frontend code, AI MUST:
- [ ] Use Vue 3 Composition API (not Options API)
- [ ] Use TypeScript for type safety (no `any` types)
- [ ] Use Pinia for state management
- [ ] Implement proper error handling and user feedback
- [ ] Include form validation before submission
- [ ] Display loading states
- [ ] Handle API errors gracefully
- [ ] Include CSRF token in form submissions
- [ ] Sanitize user input before display
- [ ] Implement proper authentication token storage (httpOnly cookie)

### Code Quality Standards

#### Naming Conventions
- [ ] PascalCase for classes, interfaces, methods
- [ ] camelCase for variables, parameters
- [ ] UPPER_SNAKE_CASE for constants
- [ ] Prefix interfaces with "I": `IUserRepository`
- [ ] Use semantic names (not `x`, `temp`, `data`)
- [ ] Use full names, not abbreviations (not `usr`, use `user`)

#### Code Style
- [ ] No magic strings/numbers (use constants/enums)
- [ ] Limit method size (max 30 lines)
- [ ] Limit class size (max 300 lines)
- [ ] One responsibility per class
- [ ] Maximum nesting depth: 3 levels
- [ ] Remove dead code and unused variables
- [ ] Add meaningful comments only (not obvious ones)
- [ ] Use early returns to reduce nesting

#### Error Handling
- [ ] Use strongly-typed exceptions (not generic Exception)
- [ ] Log all errors with context
- [ ] Never expose sensitive info in error messages
- [ ] Include stack traces only in development
- [ ] Use Result<T> pattern for expected errors
- [ ] Implement retry logic for transient failures
- [ ] Add timeout handling for external API calls

### Review Requirements for AI-Generated Code

Before merging AI-generated code, verify:

#### Security Checklist
- [ ] No hardcoded secrets in code
- [ ] All inputs are validated
- [ ] All outputs are sanitized
- [ ] No SQL injection vulnerabilities
- [ ] Proper authentication checks
- [ ] Proper authorization checks (tenant isolation)
- [ ] No exposed error details
- [ ] PII is encrypted
- [ ] Audit logging included
- [ ] CORS configured correctly

#### Architecture Checklist
- [ ] Follows Onion Architecture
- [ ] Uses dependency injection
- [ ] No circular dependencies
- [ ] Proper separation of concerns
- [ ] Uses repositories for data access
- [ ] Implements validation layer
- [ ] Proper error handling

#### Testing Checklist
- [ ] Unit tests written
- [ ] Tests are meaningful (not just mocking)
- [ ] Tests cover happy path and error cases
- [ ] Mock objects are realistic
- [ ] No hardcoded test data
- [ ] Tests are isolated and repeatable
- [ ] Test coverage > 80%

#### Documentation Checklist
- [ ] XML documentation added
- [ ] Complex logic explained
- [ ] New patterns documented
- [ ] API changes documented
- [ ] Database changes documented

### Common Pitfalls to Avoid

AI often generates code that:
- ❌ Uses string concatenation for SQL (use parameterized queries)
- ❌ Swallows exceptions silently
- ❌ Uses sync code in async context
- ❌ Creates circular dependencies
- ❌ Doesn't validate foreign keys
- ❌ Doesn't implement proper logging
- ❌ Misses edge cases
- ❌ Doesn't consider performance
- ❌ Forgets CSRF protection
- ❌ Uses default error messages

### Prompt Engineering Tips for Better Results

When asking AI to generate code:

1. **Be Specific About Architecture:**
   ```
   Generate a CreateProductCommand handler using CQRS pattern.
   It should:
   - Validate input using FluentValidation
   - Return Result<ProductDto>
   - Include soft delete support
   - Add audit logging
   - Use dependency injection
   ```

2. **Include Context:**
   ```
   We're using:
   - .NET 10
   - Entity Framework Core
   - Wolverine for messaging
   - Serilog for logging
   - xUnit for testing
   ```

3. **Specify Security Requirements:**
   ```
   Security requirements:
   - Validate tenant from JWT claims
   - Encrypt email and phone fields
   - Add rate limiting
   - Use environment variables for secrets
   ```

4. **Define Error Handling:**
   ```
   Handle these errors:
   - Product not found (404)
   - Invalid input (400)
   - Unauthorized access (403)
   - Duplicate product (409)
   Return Result<T> pattern
   ```

5. **Request Tests:**
   ```
   Also generate:
   - Unit tests with xUnit
   - Mock dependencies
   - Happy path + error cases
   - Minimum 80% coverage
   ```

---

## Success Criteria

### P0 (Critical) - Must Complete by Friday
- ✅ P0.1: Hardcoded secrets removed
- ✅ P0.2: CORS environment-specific
- ✅ P0.3: PII encrypted at rest
- ✅ P0.4: Audit logging implemented

### P1 (High) - Target: Week 2-3
- Rate limiting implemented
- API response format standardized
- Test coverage > 40%

### P2 (Medium) - Target: Week 4-6
- GDPR features implemented
- Performance optimized
- Architecture refactoring (Event Sourcing/CQRS)

---

## References & Tools

- [CRITICAL_ISSUES_ROADMAP.md](../CRITICAL_ISSUES_ROADMAP.md)
- [SECURITY_HARDENING_GUIDE.md](../SECURITY_HARDENING_GUIDE.md)
- [COMPREHENSIVE_REVIEW.md](../COMPREHENSIVE_REVIEW.md)
- [DAILY_STANDUP_TEMPLATE.md](../DAILY_STANDUP_TEMPLATE.md)
- [QUICK_START_P0.md](../QUICK_START_P0.md)

---

**Document Owner:** Architecture Team  
**Last Reviewed:** 27. Dezember 2025  
**Next Review:** 10. Januar 2026
