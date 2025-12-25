# B2Connect - Quick Reference Guide

## 🚀 Quick Start (5 minutes)

```bash
# 1. Start infrastructure
cd backend
docker-compose -f infrastructure/docker-compose.yml up -d

# 2. Run backend services (in separate terminal)
cd services/AppHost
dotnet run

# 3. Run frontend (in another terminal)
cd frontend
npm install
npm run dev

# 4. Open browser
# http://localhost:3000
```

## 📂 File Locations

| What | Where |
|------|-------|
| Backend services | `backend/services/` |
| Shared libraries | `backend/shared/` |
| Docs | `backend/docs/` |
| Frontend | `frontend/` |
| Tests | `{service}/tests/` or `frontend/tests/` |
| Config | `**/appsettings.json` or `.env` |
| Database | PostgreSQL at localhost:5432 |
| Message broker | RabbitMQ at localhost:5672 |
| Cache | Redis at localhost:6379 |

## 🔌 API Endpoints

| Service | URL | Port |
|---------|-----|------|
| API Gateway | http://localhost:5000/api | 5000 |
| Auth Service | http://localhost:5001/swagger | 5001 |
| Tenant Service | http://localhost:5002/swagger | 5002 |
| Frontend | http://localhost:3000 | 3000 |
| RabbitMQ Admin | http://localhost:15672 | 15672 |

## 🧪 Testing Commands

```bash
# Backend
dotnet test                          # All tests
dotnet watch test                    # Watch mode
dotnet test /p:CollectCoverageEnabled=true  # With coverage

# Frontend
npm run test                         # Run once
npm run test:watch                   # Watch mode
npm run test:ui                      # UI mode
npm run test:coverage                # Coverage report
npm run e2e                          # E2E tests
```

## 📝 Common Commands

```bash
# Backend
dotnet new webapi -n ProjectName     # New service
dotnet add package PackageName       # Add NuGet package
dotnet ef migrations add MigrationName  # Database migration
dotnet publish -c Release            # Build for production

# Frontend
npm install package-name             # Add package
npm run build                        # Production build
npm run lint                         # Lint code
npm run format                       # Format code
```

## 🛠️ Architecture Cheat Sheet

### Layers
1. **API Layer** - REST endpoints (Controllers)
2. **Service Layer** - Business logic (Services)
3. **Data Access** - Database operations (Repositories)
4. **Domain** - Models and entities

### Design Patterns
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - IoC container management
- **Middleware Pattern** - Request/response processing
- **Event-Driven** - Wolverine for async communication
- **CQRS** - Commands and Queries separation

### Multitenant
- JWT token includes `tenant_id` claim
- `X-Tenant-ID` header for fallback
- RLS policies enforce database-level isolation
- All queries filtered by tenant context

## 📚 Documentation Map

| Document | Purpose |
|----------|---------|
| [.copilot-specs.md](.copilot-specs.md) | Code standards & guidelines |
| [README.md](README.md) | Project overview |
| [DEVELOPMENT.md](DEVELOPMENT.md) | Step-by-step development guide |
| [PROJECT_STATUS.md](PROJECT_STATUS.md) | Status & checklist |
| [COMPLETION_SUMMARY.md](COMPLETION_SUMMARY.md) | What was created |
| [backend/docs/architecture.md](backend/docs/architecture.md) | System architecture |
| [backend/docs/api-specifications.md](backend/docs/api-specifications.md) | API endpoints |
| [backend/docs/tenant-isolation.md](backend/docs/tenant-isolation.md) | Security & isolation |

## 🔒 Security Checklist

- [ ] JWT tokens include tenant_id
- [ ] All queries filtered by TenantId
- [ ] RLS policies enforced in PostgreSQL
- [ ] CORS configured for frontend origins
- [ ] API validates authorization
- [ ] Sensitive data logged with caution
- [ ] Audit trail enabled
- [ ] Rate limiting configured
- [ ] TLS enabled in production
- [ ] Secrets not in code

## 🚨 Troubleshooting

| Issue | Solution |
|-------|----------|
| Port in use | `lsof -i :5000` then `kill -9 <PID>` |
| DB connection error | Check `docker-compose ps` |
| Module not found (npm) | `rm -rf node_modules && npm install` |
| Build error | `dotnet clean && dotnet build` |
| Tests failing | Check test setup.ts for mocks |
| CORS error | Verify frontend URL in api-gateway config |

## 💻 IDE Setup

### Visual Studio Code
```json
{
  "extensions": [
    "ms-dotnettools.csharp",
    "ms-vscode.cpptools",
    "vue.volar",
    "esbenp.prettier-vscode",
    "dbaeumer.vscode-eslint"
  ]
}
```

### Visual Studio 2022
- Install .NET 10 workload
- Install Vue.js tools extension
- Open `B2Connect.sln`

## 📊 Database Schema

### Key Tables
- `tenants` - Tenant information
- `users` - User accounts (tenant_id for RLS)
- `roles` - Tenant roles
- `permissions` - Role permissions
- `audit_logs` - Activity tracking

### RLS Rules
- All tables have `tenant_id`
- RLS policies check `tenant_id = current_setting('app.current_tenant_id')`
- Tenant isolation enforced at DB level

## 🌐 Frontend Structure

```
src/
├── components/     # Reusable components
├── views/          # Page components
├── stores/         # Pinia state (auth, etc.)
├── services/       # API clients
├── types/          # TypeScript interfaces
├── router/         # Vue Router config
├── middleware/     # Route guards
└── utils/          # Helper functions
```

## 📦 Backend Structure

```
backend/
├── services/       # Microservices
├── shared/         # Libraries
│   ├── types/      # DTOs & Entities
│   ├── utils/      # Extensions
│   └── middleware/ # Shared middleware
├── infrastructure/ # Docker, K8s, Terraform
└── docs/          # Documentation
```

## 🎯 Development Flow

1. **Create feature branch** - `git checkout -b feature/name`
2. **Write tests first** - Follow TDD
3. **Implement code** - Make tests pass
4. **Test coverage** - Aim for >80%
5. **Code review** - Create PR
6. **Merge** - Squash commits if needed

## 🔄 Adding a New Service

```bash
# 1. Create project
dotnet new webapi -n B2Connect.NewService

# 2. Copy structure from existing service
cp -r backend/services/auth-service/src/* ./src/

# 3. Update Program.cs with service config
# 4. Update AppHost to include service
# 5. Update README with new service info
```

## 📱 Key Concepts

### Tenant Context
```csharp
var tenantId = _tenantContextAccessor.GetTenantId();
// Used in all queries to filter by tenant
```

### Service Registration
```csharp
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IService, Service>();
```

### Authorization
```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task Delete(Guid id) { }
```

### Wolverine Commands
```csharp
await _bus.InvokeAsync(new CreateTenantCommand { ... });
```

## 🎓 Key Files to Read

1. Start with `.copilot-specs.md` - Understand the standards
2. Read `README.md` - Get overview
3. Read `DEVELOPMENT.md` - Follow setup steps
4. Check `backend/docs/architecture.md` - Learn system design
5. Review `backend/docs/api-specifications.md` - See API structure

## 🚀 Deployment Quick Steps

```bash
# Build Docker images
docker build -t b2connect/auth-service ./backend/services/auth-service
docker build -t b2connect/tenant-service ./backend/services/tenant-service
docker build -t b2connect/api-gateway ./backend/services/api-gateway

# Push to registry
docker push b2connect/auth-service:latest

# Deploy to Kubernetes
kubectl apply -f backend/infrastructure/kubernetes/
```

## 📋 Pre-Commit Checklist

- [ ] Tests pass
- [ ] Code follows standards
- [ ] No TODO comments (or documented)
- [ ] No console.log statements
- [ ] Commit message is clear
- [ ] Feature branch created
- [ ] Related issue linked

---

**Pro Tip**: Bookmark this file for quick reference! 📌

Last Updated: 2024
