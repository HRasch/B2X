# B2Connect Application Specifications

## 📋 Overview

This document defines the official technical specifications and requirements for the B2Connect application across all environments.

---

## 1. Architecture & Orchestration

### 1.1 Service Orchestration: AppHost (Official)

**Status:** ✅ **OFFICIAL & REQUIRED**

AppHost is the **mandatory approach** for running B2Connect services in any environment.

**Rationale:**
- ✅ Consistent across macOS, Windows, Linux
- ✅ No external dependencies (only .NET SDK)
- ✅ Simple to understand and maintain
- ✅ Explicit error handling with Result-Pattern
- ✅ Graceful service lifecycle management

**Implementation:**
- Location: `backend/services/AppHost/`
- Technology: System.Diagnostics.Process + Serilog
- See: [APPHOST_SPECIFICATIONS.md](APPHOST_SPECIFICATIONS.md)

### 1.2 Core Services

| Service | Port | Status | Framework |
|---------|------|--------|-----------|
| Auth Service | 9002 | ✅ Active | ASP.NET Core |
| Tenant Service | 9003 | ✅ Active | ASP.NET Core |
| Localization Service | 9004 | ✅ Active | ASP.NET Core |

### 1.3 Frontend Applications

| Application | Port | Status | Framework |
|-------------|------|--------|-----------|
| Customer App | 5173 | ✅ Active | Vue.js 3 + Vite |
| Admin App | 5174 | ✅ Active | Vue.js 3 + Vite |

---

## 2. Error Handling Policy

### 2.1 Result-Pattern (Official)

**Status:** ✅ **REQUIRED FOR ALL CODE**

**Policy:** Avoid Exceptions for Flow Control - Use Result-Pattern Instead

**Why:**
- Explicit error handling (no hidden exceptions)
- Better composability and function chaining
- Type-safe error handling
- Easier to test and reason about

**Implementation:**
```csharp
// ✅ CORRECT: Result-Pattern
public Result<User> GetUser(int id)
{
    return id > 0
        ? new Result<User>.Success(user)
        : new Result<User>.Failure("Invalid ID");
}

// ❌ WRONG: Exception-based
public User GetUser(int id)
{
    if (id <= 0) throw new ArgumentException("Invalid ID");
    return user;
}
```

**References:**
- [RESULT_PATTERN_GUIDE.md](RESULT_PATTERN_GUIDE.md) - Implementation details
- [.copilot-specs.md](.copilot-specs.md#33-exception-handling---result-pattern-approach) - GitHub Specs

---

## 3. Development Environment Setup

### 3.1 Prerequisites

**Required:**
- .NET 10 SDK
- Node.js 18+
- Git

**Optional:**
- Docker Desktop (for advanced scenarios)
- Visual Studio Code
- Postman or similar API testing tool

### 3.2 Initial Setup

```bash
# 1. Clone repository
git clone https://github.com/yourorg/b2connect.git
cd B2Connect

# 2. Install dependencies
cd backend && dotnet restore
cd ../frontend && npm install

# 3. Start AppHost
cd ../backend/services/AppHost
dotnet run

# 4. In new terminal, start frontend
cd ../../../frontend
npm run dev
```

### 3.3 Verification

```bash
# Check services are running
curl http://localhost:9002/health  # Auth
curl http://localhost:9003/health  # Tenant
curl http://localhost:9004/health  # Localization

# Frontend should be at
open http://localhost:5173
```

**Time to Running:** ~3-5 minutes

---

## 4. Code Organization & Standards

### 4.1 Language & Framework

- **Primary Language:** C# (.NET 10+)
- **Backend Framework:** ASP.NET Core
- **Message Bus:** WolverineFx 5.9.2
- **Logging:** Serilog
- **Frontend Framework:** Vue.js 3
- **Frontend Build:** Vite

### 4.2 Namespace Structure

```
B2Connect
├── [ServiceName]              # Auth, Tenant, Localization, etc.
│   ├── API                    # HTTP endpoints
│   ├── Application            # Business logic (CQRS)
│   ├── Domain                 # Domain models
│   ├── Infrastructure         # External integrations
│   └── Persistence            # Data access (EF Core)
├── Shared
│   ├── Types                  # Common DTOs & models
│   ├── Utils                  # Utilities & helpers
│   └── Middleware             # Cross-cutting concerns
└── Tests
    └── [ServiceName].Tests
```

### 4.3 Naming Conventions

- **Classes:** PascalCase
- **Methods:** PascalCase
- **Properties:** PascalCase
- **Local Variables:** camelCase
- **Private Fields:** _camelCase
- **Constants:** UPPER_SNAKE_CASE
- **Interfaces:** IPascalCase
- **Namespaces:** B2Connect.[Domain].[Feature]

---

## 5. API Contracts

### 5.1 HTTP Status Codes

| Code | Usage | Example |
|------|-------|---------|
| 200 | Success | GET request succeeded |
| 201 | Created | POST created new resource |
| 204 | No Content | DELETE succeeded |
| 400 | Bad Request | Invalid input |
| 401 | Unauthorized | Missing/invalid auth |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Duplicate or invalid state |
| 500 | Server Error | Unexpected failure |

### 5.2 Error Response Format

```json
{
  "error": {
    "code": "INVALID_EMAIL",
    "message": "Email format is invalid",
    "details": "Email must contain @ symbol",
    "timestamp": "2025-12-26T10:30:00Z",
    "traceId": "550e8400-e29b-41d4-a716-446655440000"
  }
}
```

### 5.3 Pagination

```json
{
  "data": [...],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 42,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

---

## 6. Data & Persistence

### 6.1 Database

- **Primary:** PostgreSQL 15+
- **Migrations:** Entity Framework Core
- **Async Patterns:** Required for all DB operations

### 6.2 Caching

- **In-Memory Cache:** IMemoryCache for local caching
- **Distributed Cache:** Redis for cross-service caching
- **Cache Invalidation:** Event-driven via Wolverine

### 6.3 Message Bus

- **Framework:** WolverineFx 5.9.2
- **Broker:** RabbitMQ
- **Pattern:** CQRS with events
- **Reliability:** Transactional outbox pattern

---

## 7. Testing Standards

### 7.1 Test Pyramid

```
        △  E2E Tests (Frontend)
       ║ ║ Integration Tests
      ║ ║ ║ Unit Tests
     ║ ║ ║ ║
```

### 7.2 Coverage Requirements

- **Unit Tests:** ≥ 80% for business logic
- **Integration Tests:** All API endpoints
- **E2E Tests:** Critical user workflows

### 7.3 Test Organization

```
backend/Tests/
├── [ServiceName].Tests/
│   ├── Unit/
│   │   ├── API/
│   │   ├── Application/
│   │   └── Domain/
│   └── Integration/
│       ├── API/
│       └── Infrastructure/
└── E2E/
```

---

## 8. Deployment & Environments

### 8.1 Environment Tiers

| Environment | Purpose | AppHost | Docker |
|-------------|---------|---------|--------|
| **Development** | Local development | ✅ Required | Optional |
| **Testing** | Automated testing | ✅ Required | Optional |
| **Staging** | Pre-production testing | Optional | ✅ Required |
| **Production** | Live system | ❌ Not used | ✅ Required |

### 8.2 Configuration Management

- **Development:** `.env` file (local overrides)
- **All Environments:** Environment variables
- **Sensitive Data:** Azure Key Vault / AWS Secrets Manager
- **No Secrets in Code:** Enforced via git hooks

---

## 9. Security Requirements

### 9.1 Authentication & Authorization

- **Auth Method:** JWT tokens
- **Issuer:** Auth Service (port 9002)
- **Validation:** All endpoints require valid token
- **Tenant Isolation:** Always validated on each request

### 9.2 Data Protection

- **Encryption in Transit:** TLS 1.3+
- **Encryption at Rest:** Database encryption enabled
- **Tenant Isolation:** Complete data isolation per tenant
- **No Data Leakage:** Error messages never expose tenant data

### 9.3 API Security

- **Rate Limiting:** Per-user and per-IP limits
- **CORS:** Whitelist only known origins
- **CSRF Protection:** Token-based for state-changing requests
- **Input Validation:** All inputs validated server-side

---

## 10. Monitoring & Observability

### 10.1 Logging

- **Framework:** Serilog
- **Format:** Structured JSON
- **Minimum Level:** Information (Development), Warning (Production)
- **Correlation ID:** Included in all log entries

### 10.2 Metrics

- **Framework:** OpenTelemetry
- **Granularity:** 
  - Request duration
  - Error rates
  - Service health
  - Business metrics (orders, users, etc.)

### 10.3 Health Checks

Every service must expose:
```
GET /health
{
  "status": "healthy",
  "timestamp": "2025-12-26T10:30:00Z",
  "details": {
    "database": "healthy",
    "cache": "healthy",
    "messageBroker": "healthy"
  }
}
```

---

## 11. Documentation Standards

### 11.1 Code Documentation

- **XML Comments:** All public methods and classes
- **Format:** `<summary>`, `<param>`, `<returns>`, `<exception>`
- **Examples:** Include for complex operations
- **Keep Current:** Update with code changes

### 11.2 Project Documentation

- **README:** Each service has a README
- **API Docs:** OpenAPI/Swagger for all endpoints
- **Architecture:** Document decision patterns
- **Setup:** Clear environment setup instructions

---

## 12. Continuous Integration & Delivery

### 12.1 Build Pipeline

1. **Compile:** `dotnet build`
2. **Unit Tests:** `dotnet test --filter UnitTest`
3. **Code Analysis:** StyleCop + SonarQube
4. **Integration Tests:** `dotnet test --filter IntegrationTest`
5. **Build Docker Images:** (if applicable)
6. **Push to Registry:** (if applicable)

### 12.2 Deployment Pipeline

1. **Staging Deployment:** Automatic on PR merge
2. **Staging Tests:** Automated E2E test suite
3. **Production Deployment:** Manual approval required
4. **Health Checks:** Immediate post-deployment verification
5. **Rollback Plan:** Keep previous version ready

---

## 13. Version Requirements

### 13.1 Required Versions

| Component | Version | Rationale |
|-----------|---------|-----------|
| .NET | 10.0+ | Latest LTS |
| Node.js | 18+ | LTS release |
| PostgreSQL | 15+ | Latest stable |
| RabbitMQ | 3.12+ | Latest with AMQP 0.9.1 |
| Redis | 7.0+ | Latest stable |

### 13.2 NuGet Packages

Core packages specified in `backend/Directory.Packages.props`:
- WolverineFx 5.9.2 (CQRS & Messaging)
- Entity Framework Core 10.0+
- Serilog 4.3.0+
- MediatR for command/query handling

---

## 14. Common Tasks & Workflows

### 14.1 Running AppHost

```bash
cd backend/services/AppHost
dotnet run
```

### 14.2 Adding a New Service

1. Create directory under `backend/services/[service-name]`
2. Create new project with `dotnet new` template
3. Add to AppHost's service list in `Program.cs`
4. Test with `dotnet run` from AppHost
5. Document in this file

### 14.3 Running Tests

```bash
# All tests
cd backend && dotnet test

# Specific test project
dotnet test Tests/[ServiceName].Tests

# Specific test
dotnet test --filter TestName
```

### 14.4 Database Migrations

```bash
cd backend
dotnet ef migrations add MigrationName -p Services/[ServiceName] -s Services/[ServiceName]
dotnet ef database update -p Services/[ServiceName] -s Services/[ServiceName]
```

---

## 15. Troubleshooting

### 15.1 AppHost Won't Start

```bash
# 1. Verify .NET is installed
dotnet --version

# 2. Check if ports are free
lsof -i :9002
lsof -i :9003
lsof -i :9004

# 3. Clean and rebuild
cd backend/services/AppHost
dotnet clean
dotnet build
dotnet run
```

### 15.2 Frontend Won't Connect to Backend

```bash
# 1. Verify services are running
curl http://localhost:9002/health

# 2. Check CORS configuration in service
# Should allow http://localhost:5173

# 3. Check browser console for errors
# F12 → Network tab → look for 404/403/500 errors
```

---

## 16. References & Links

- [APPHOST_SPECIFICATIONS.md](APPHOST_SPECIFICATIONS.md) - Complete AppHost documentation
- [APPHOST_QUICKSTART.md](APPHOST_QUICKSTART.md) - Command reference
- [RESULT_PATTERN_GUIDE.md](RESULT_PATTERN_GUIDE.md) - Error handling patterns
- [.copilot-specs.md](.copilot-specs.md) - GitHub Copilot guidelines
- [DEVELOPMENT.md](DEVELOPMENT.md) - Development workflow
- [GETTING_STARTED.md](GETTING_STARTED.md) - Initial setup guide

---

**Status:** ✅ APPROVED & EFFECTIVE  
**Last Updated:** 26. Dezember 2025  
**Scope:** All B2Connect Development (Local + CI/CD)
