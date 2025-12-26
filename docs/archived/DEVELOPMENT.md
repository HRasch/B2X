# B2Connect Development Guide

This guide provides step-by-step instructions for setting up your development environment and contributing to B2Connect.

## Quick Start (5 Minutes)

The fastest way to get started with B2Connect development:

```bash
# 1. Start backend services
cd backend/services/AppHost
dotnet run

# 2. In another terminal, start frontend
cd frontend
npm install && npm run dev

# 3. Open browser
open http://localhost:5173
```

**That's it!** You now have:
- ✅ Auth Service (port 9002)
- ✅ Tenant Service (port 9003)  
- ✅ Localization Service (port 9004)
- ✅ Frontend App (port 5173)

For detailed information, see [APPHOST_QUICKSTART.md](APPHOST_QUICKSTART.md).

## Getting Started with Development

## Prerequisites

### System Requirements
- macOS (Intel or Apple Silicon), Linux, or Windows with WSL2
- 8GB RAM minimum (16GB recommended)
- 10GB free disk space

### Required Software
```bash
# .NET 10 SDK
brew install dotnet

# Node.js 18+
brew install node

# Docker Desktop
brew install docker

# Git
brew install git

# Optional: Visual Studio Code
brew install visual-studio-code
```

### Verify Installation
```bash
dotnet --version      # Should be 10.0+
node --version        # Should be 18.0+
npm --version         # Should be 9.0+
docker --version      # Should be 20.0+
```

## Initial Setup

### 1. Clone Repository
```bash
git clone https://github.com/yourorg/b2connect.git
cd B2Connect
```

### 2. Setup Environment Variables
```bash
cp .env.example .env
# Edit .env with your configuration
```

### 3. Start Infrastructure
```bash
cd backend
docker-compose -f infrastructure/docker-compose.yml up -d

# Wait for services to be healthy
docker-compose -f infrastructure/docker-compose.yml ps
```

### 4. Setup Backend
```bash
# Restore NuGet packages
dotnet restore

# Run database migrations (when ready)
dotnet ef database update
```

### 5. Setup Frontend
```bash
cd ../frontend
npm install
```

## Development Workflow

### 🎯 Official Backend Orchestration: AppHost

**AppHost is the standard and recommended way to run B2Connect services during development.**

#### Why AppHost?
- ✅ **Single Command**: Start all services at once
- ✅ **Cross-Platform**: Works identically on macOS, Windows, Linux
- ✅ **Zero Dependencies**: No Docker, DCP, or external tools required
- ✅ **Clear Logging**: All service output in one terminal
- ✅ **Graceful Shutdown**: Proper cleanup with Ctrl+C
- ✅ **Easy to Extend**: Add new services in 3 lines of code

See [APPHOST_SPECIFICATIONS.md](APPHOST_SPECIFICATIONS.md) for complete technical details.

#### Quick Start with AppHost (Recommended)

**Terminal 1 - Backend Services:**
```bash
cd backend/services/AppHost
dotnet run

# Expected output:
# [2025-12-26 09:13:35 INF] 🚀 B2Connect Application Host - Starting
# [2025-12-26 09:13:35 INF] ▶ Starting Auth Service on port 9002...
# [2025-12-26 09:13:35 INF]   ✓ Auth Service started (PID: 7976)
# [2025-12-26 09:13:36 INF] ▶ Starting Tenant Service on port 9003...
# [2025-12-26 09:13:36 INF]   ✓ Tenant Service started (PID: 7981)
# [2025-12-26 09:13:37 INF] ▶ Starting Localization Service on port 9004...
# [2025-12-26 09:13:37 INF]   ✓ Localization Service started (PID: 7983)
# [2025-12-26 09:13:37 INF] ✅ B2Connect Application Host initialized
```

**Terminal 2 - Frontend:**
```bash
cd frontend
npm run dev
# Frontend available at http://localhost:5173
```

**Terminal 3 - Admin Frontend (Optional):**
```bash
cd frontend-admin
npm run dev -- --port 5174
# Admin frontend available at http://localhost:5174
```

#### Service Endpoints (via AppHost)

| Service | Port | Health | Purpose |
|---------|------|--------|---------|
| **Auth Service** | 9002 | `http://localhost:9002/health` | Authentication & Authorization |
| **Tenant Service** | 9003 | `http://localhost:9003/health` | Multi-tenant Management |
| **Localization Service** | 9004 | `http://localhost:9004/health` | i18n & Translations |

#### Verify Services are Running

```bash
# Quick health check
curl http://localhost:9002/health
curl http://localhost:9003/health
curl http://localhost:9004/health

# Or use the bundled health check script
bash health-check.sh
```

---

### Alternative: Individual Service Run

If you need to run services individually (not recommended for normal development):

**Terminal 1: Auth Service**
```bash
cd backend/services/auth-service
dotnet run
```

**Terminal 2: Tenant Service**
```bash
cd backend/services/tenant-service
dotnet run
```

**Terminal 3: Localization Service**
```bash
cd backend/services/LocalizationService
dotnet run
```

⚠️ **Note**: This approach requires manual service start/stop and doesn't provide centralized logging.

### Running Services Locally
- **Tenant Service**: http://localhost:5002/swagger
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

## Code Organization

### Backend Directory Structure
```
backend/
├── services/
│   ├── {service}/
│   │   ├── Program.cs           # Service entry point
│   │   ├── appsettings.json     # Configuration
│   │   └── src/
│   │       ├── Controllers/      # REST endpoints
│   │       ├── Handlers/         # Wolverine handlers
│   │       ├── Services/         # Business logic
│   │       ├── Repositories/     # Data access
│   │       └── Models/           # Domain models
├── shared/
│   ├── types/
│   │   ├── Entities.cs          # Domain entities
│   │   └── DTOs.cs              # Data transfer objects
│   ├── utils/
│   │   └── Extensions.cs         # Extension methods
│   └── middleware/
│       └── MiddlewareExtensions.cs
└── docs/
```

### Frontend Directory Structure
```
frontend/
├── src/
│   ├── components/              # Vue components
│   │   ├── common/              # Shared components
│   │   ├── auth/                # Auth components
│   │   └── tenant/              # Tenant components
│   ├── views/                   # Page components
│   ├── stores/                  # Pinia stores
│   ├── services/                # API clients
│   ├── router/                  # Vue Router config
│   └── types/                   # TypeScript types
├── tests/
│   ├── unit/                    # Unit tests
│   ├── components/              # Component tests
│   └── e2e/                     # E2E tests
└── public/                      # Static assets
```

## Creating a New Feature

### Backend Feature (Microservice)

#### 1. Define Domain Model
```csharp
// backend/shared/types/Entities.cs
public class Feature : Entity
{
    public Guid TenantId { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
```

#### 2. Create DTO
```csharp
// backend/shared/types/DTOs.cs
public record FeatureDto(
    Guid Id,
    string Name,
    string Description,
    bool IsActive
);
```

#### 3. Create Repository
```csharp
// backend/services/{service}/src/Repositories/FeatureRepository.cs
public interface IFeatureRepository
{
    Task<FeatureDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<FeatureDto>> GetAllAsync();
    Task<FeatureDto> CreateAsync(CreateFeatureRequest request);
    Task<FeatureDto> UpdateAsync(Guid id, UpdateFeatureRequest request);
    Task DeleteAsync(Guid id);
}

public class FeatureRepository : IFeatureRepository
{
    private readonly DbContext _context;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    // Implement interface methods with tenant isolation
    public async Task<FeatureDto?> GetByIdAsync(Guid id)
    {
        var tenantId = _tenantContextAccessor.GetTenantId();
        return await _context.Features
            .Where(f => f.TenantId == tenantId && f.Id == id)
            .Select(f => new FeatureDto(f.Id, f.Name, f.Description, f.IsActive))
            .FirstOrDefaultAsync();
    }
}
```

#### 4. Create Service
```csharp
// backend/services/{service}/src/Services/FeatureService.cs
public interface IFeatureService
{
    Task<FeatureDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<FeatureDto>> GetAllAsync();
    Task<FeatureDto> CreateAsync(CreateFeatureRequest request);
}

public class FeatureService : IFeatureService
{
    private readonly IFeatureRepository _repository;
    private readonly ILogger<FeatureService> _logger;

    public async Task<FeatureDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
}
```

#### 5. Create Controller
```csharp
// backend/services/{service}/src/Controllers/FeaturesController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeaturesController : ControllerBase
{
    private readonly IFeatureService _service;

    [HttpGet("{id}")]
    public async Task<ActionResult<FeatureDto>> GetFeature(Guid id)
    {
        var feature = await _service.GetByIdAsync(id);
        if (feature == null)
            return NotFound();
        return Ok(feature);
    }

    [HttpPost]
    public async Task<ActionResult<FeatureDto>> CreateFeature([FromBody] CreateFeatureRequest request)
    {
        var feature = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetFeature), new { id = feature.Id }, feature);
    }
}
```

#### 6. Register Dependencies
```csharp
// In Program.cs
builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
```

#### 7. Create Tests
```csharp
// backend/services/{service}/tests/Services/FeatureServiceTests.cs
[TestClass]
public class FeatureServiceTests
{
    private Mock<IFeatureRepository> _repositoryMock;
    private FeatureService _service;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IFeatureRepository>();
        _service = new FeatureService(_repositoryMock.Object, new Mock<ILogger<FeatureService>>().Object);
    }

    [TestMethod]
    public async Task GetById_WithValidId_ReturnsFeature()
    {
        // Arrange
        var featureId = Guid.NewGuid();
        var expectedFeature = new FeatureDto(featureId, "Test", "Test Feature", true);
        _repositoryMock.Setup(r => r.GetByIdAsync(featureId))
            .ReturnsAsync(expectedFeature);

        // Act
        var result = await _service.GetByIdAsync(featureId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedFeature.Name, result.Name);
    }
}
```

### Frontend Feature (Vue Component)

#### 1. Create API Service
```typescript
// frontend/src/services/featureService.ts
import { api } from './api'
import type { FeatureDto } from '@/types'

export const featureService = {
  getAll: async () => {
    const response = await api.get<FeatureDto[]>('/features')
    return response.data
  },

  getById: async (id: string) => {
    const response = await api.get<FeatureDto>(`/features/${id}`)
    return response.data
  },

  create: async (feature: Omit<FeatureDto, 'id'>) => {
    const response = await api.post<FeatureDto>('/features', feature)
    return response.data
  },

  update: async (id: string, feature: Partial<FeatureDto>) => {
    const response = await api.put<FeatureDto>(`/features/${id}`, feature)
    return response.data
  },

  delete: async (id: string) => {
    await api.delete(`/features/${id}`)
  },
}
```

#### 2. Create Pinia Store
```typescript
// frontend/src/stores/features.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { featureService } from '@/services/featureService'
import type { FeatureDto } from '@/types'

export const useFeaturesStore = defineStore('features', () => {
  const features = ref<FeatureDto[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const all = computed(() => features.value)
  const isEmpty = computed(() => features.value.length === 0)

  const fetchAll = async () => {
    loading.value = true
    try {
      features.value = await featureService.getAll()
    } catch (err) {
      error.value = 'Failed to fetch features'
    } finally {
      loading.value = false
    }
  }

  const add = async (feature: Omit<FeatureDto, 'id'>) => {
    try {
      const newFeature = await featureService.create(feature)
      features.value.push(newFeature)
      return newFeature
    } catch (err) {
      error.value = 'Failed to create feature'
      throw err
    }
  }

  return { features: all, loading, error, isEmpty, fetchAll, add }
})
```

#### 3. Create Vue Component
```vue
<!-- frontend/src/components/features/FeatureList.vue -->
<template>
  <div class="features">
    <h2>Features</h2>

    <button @click="showCreate = true" class="btn btn-primary">
      + New Feature
    </button>

    <div v-if="featuresStore.loading" class="loading">Loading...</div>

    <div v-else-if="featuresStore.isEmpty" class="empty-state">
      No features yet
    </div>

    <ul v-else class="feature-list">
      <li v-for="feature in featuresStore.features" :key="feature.id">
        <h3>{{ feature.name }}</h3>
        <p>{{ feature.description }}</p>
        <span v-if="feature.isActive" class="badge badge-active">Active</span>
        <span v-else class="badge badge-inactive">Inactive</span>
      </li>
    </ul>

    <FeatureCreate v-if="showCreate" @created="onFeatureCreated" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useFeaturesStore } from '@/stores/features'
import FeatureCreate from './FeatureCreate.vue'

const featuresStore = useFeaturesStore()
const showCreate = ref(false)

const onFeatureCreated = async () => {
  await featuresStore.fetchAll()
  showCreate.value = false
}

onMounted(async () => {
  await featuresStore.fetchAll()
})
</script>

<style scoped>
.features {
  padding: 1rem;
}

.feature-list {
  list-style: none;
  padding: 0;
  margin: 1rem 0;
}

.feature-list li {
  padding: 1rem;
  border: 1px solid #e0e0e0;
  margin-bottom: 0.5rem;
  border-radius: 4px;
}

.badge {
  display: inline-block;
  padding: 0.3rem 0.8rem;
  border-radius: 12px;
  font-size: 0.85rem;
}

.badge-active {
  background-color: #d4edda;
  color: #155724;
}

.badge-inactive {
  background-color: #f8d7da;
  color: #721c24;
}
</style>
```

#### 4. Create Tests
```typescript
// frontend/tests/unit/features.spec.ts
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useFeaturesStore } from '@/stores/features'
import * as featureService from '@/services/featureService'

describe('Features Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('should fetch all features', async () => {
    const mockFeatures = [
      { id: '1', name: 'Feature 1', description: 'Desc 1', isActive: true },
      { id: '2', name: 'Feature 2', description: 'Desc 2', isActive: false },
    ]

    vi.spyOn(featureService, 'getAll').mockResolvedValue(mockFeatures)

    const store = useFeaturesStore()
    await store.fetchAll()

    expect(store.features).toHaveLength(2)
    expect(store.features[0].name).toBe('Feature 1')
  })
})
```

## Testing

### Running Tests

#### Backend
```bash
# All tests
dotnet test

# Specific project
dotnet test backend/services/auth-service/tests/

# With code coverage
dotnet test /p:CollectCoverageEnabled=true

# Watch mode
dotnet watch test
```

#### Frontend
```bash
# Unit and component tests
npm run test

# Watch mode
npm run test:watch

# With UI
npm run test:ui

# Coverage report
npm run test:coverage

# E2E tests
npm run e2e

# E2E watch mode
npm run e2e:ui
```

### Test File Organization
```
tests/
├── unit/
│   ├── Services/
│   ├── Repositories/
│   └── Utilities/
├── integration/
│   ├── Controllers/
│   └── Services/
└── fixtures/
    ├── SampleData.cs
    └── MockProviders.cs
```

## Debugging

### Backend Debugging

#### Visual Studio Code
1. Open `backend/services/{service}` as workspace
2. Install C# extension
3. Create `.vscode/launch.json`:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Launch",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net8.0/{ServiceName}.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "stopAtEntry": false,
      "console": "internalConsole"
    }
  ]
}
```

#### Console Logging
```csharp
_logger.LogInformation("Debug message: {@Data}", someObject);
_logger.LogError(ex, "Error occurred");
```

### Frontend Debugging

#### Visual Studio Code
1. Install Chrome DevTools extension
2. Add breakpoints in VS Code
3. Use `debugger` statement in code

#### Browser DevTools
- Open Chrome DevTools (F12)
- Vue DevTools extension for component inspection
- Network tab for API calls monitoring

## Git Workflow

### Branch Naming Convention
```
feature/description        # New features
bugfix/description         # Bug fixes
refactor/description       # Code refactoring
docs/description          # Documentation updates
test/description          # Test improvements
chore/description         # Maintenance tasks
```

### Commit Message Format
```
type(scope): description

[optional body]

[optional footer]
```

### Examples
```
feat(auth): implement jwt token refresh
fix(api-gateway): handle timeout in request routing
docs(architecture): update service integration diagram
test(tenant-service): add repository tests
```

### Pull Request Process
1. Create feature branch from `main`
2. Commit changes with clear messages
3. Create PR with description
4. Request code review
5. Address feedback
6. Merge when approved

## Common Tasks

### Add New NuGet Package
```bash
cd backend/services/{service}
dotnet add package PackageName
# Or update Directory.Packages.props for centralized version
```

### Add New NPM Package
```bash
cd frontend
npm install package-name
```

### Database Migration
```bash
cd backend/services/{service}
dotnet ef migrations add {MigrationName}
dotnet ef database update
```

### Generate API Documentation
```bash
# Swagger is auto-generated from XML comments
# Access at {ServiceUrl}/swagger
```

### Build for Production

#### Backend
```bash
dotnet publish -c Release -o ./publish
```

#### Frontend
```bash
npm run build
# Output in dist/
```

## Troubleshooting

### Port Already in Use
```bash
# Find process using port
lsof -i :5000

# Kill process (macOS)
kill -9 <PID>

# Or change port in appsettings.json or environment
```

### Database Connection Issues
```bash
# Verify PostgreSQL is running
docker-compose -f backend/infrastructure/docker-compose.yml ps

# Check logs
docker-compose -f backend/infrastructure/docker-compose.yml logs postgres
```

### Node Modules Issues
```bash
# Clear cache and reinstall
rm -rf node_modules package-lock.json
npm install
```

### Vite Dev Server Issues
```bash
# Clear cache
rm -rf frontend/.vite

# Try with different port
npm run dev -- --port 3001
```

## Performance Profiling

### Backend Profiling
```csharp
using var activity = _tracer.StartActivity("OperationName");
// ... operation code
// Activity automatically tracks duration and tags
```

### Frontend Performance
```typescript
const start = performance.now()
// ... operation
console.log(`Operation took ${performance.now() - start}ms`)
```

## Documentation

All code should include:
- XML comments (C#) / JSDoc (TypeScript)
- README in complex directories
- Examples in comments for non-obvious logic

## Getting Help

1. Check existing documentation in `backend/docs/`
2. Review `.copilot-specs.md` for guidelines
3. Search existing issues
4. Ask in team chat
5. Create documentation issue for unclear parts

---

Happy coding! 🚀
