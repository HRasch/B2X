# VS Code Aspire Configuration Update

**Date**: 2025-12-26  
**Status**: ✅ COMPLETE

## Updates Applied

### 1. **aspire-start.sh** - Enhanced Startup Script

**New Features:**
- ✅ Catalog Service Port Configuration (9001)
- ✅ Elasticsearch Integration Flag
- ✅ Event Validation System Flag
- ✅ Localization/i18n Support Flag
- ✅ Enhanced Dashboard Output
- ✅ Service Port Mapping

**Environment Variables Added:**
```bash
export CATALOG_SERVICE_PORT="9001"
export ELASTICSEARCH_ENABLED="true"
export EVENTVALIDATION_ENABLED="true"
export LOCALIZATION_ENABLED="true"
```

**Output Improvements:**
- Microservices list with ports
- Feature enablement status
- Better visual organization
- Color-coded messages

### 2. **launch.json** - Debug Configurations

**New Configurations Added:**

#### Catalog Service (Debug)
```json
{
  "name": "Catalog Service (Debug)",
  "type": "coreclr",
  "program": ".../CatalogService/bin/Debug/net10.0/B2Connect.CatalogService.dll",
  "env": {
    "ASPNETCORE_URLS": "http://localhost:9001",
    "ELASTICSEARCH_ENABLED": "true",
    "EVENTVALIDATION_ENABLED": "true",
    "LOCALIZATION_ENABLED": "true"
  }
}
```

#### Frontend Admin (Debug)
```json
{
  "name": "Frontend Admin (Debug)",
  "type": "node",
  "runtimeExecutable": "npm",
  "cwd": "${workspaceFolder}/frontend-admin",
  "env": {
    "VITE_API_URL": "http://localhost:9000",
    "VITE_CATALOG_API": "http://localhost:9001"
  }
}
```

**New Debug Compounds:**
- ✅ `Full Stack (AppHost + Admin Frontend)`
- ✅ `Catalog Service Standalone`

### 3. **tasks.json** - Build & Test Tasks

**New Tasks Added:**

#### Catalog Service Tests
```json
{
  "label": "🗂️  Catalog Service Tests",
  "command": "dotnet",
  "args": ["test"],
  "cwd": "${workspaceFolder}/backend/Tests/CatalogService.Tests"
}
```

#### Catalog Validators Test
```json
{
  "label": "🔍 Catalog Validators Test",
  "command": "dotnet",
  "args": ["test", "--filter", "CatalogValidators"]
}
```

#### Event Validation Tests
```json
{
  "label": "📨 Event Validation Tests",
  "command": "dotnet",
  "args": ["test", "--filter", "EventValidator"]
}
```

#### Admin Frontend Dev
```json
{
  "label": "👨‍💼 Admin Frontend Dev (port 5174)",
  "command": "npm",
  "args": ["run", "dev", "--", "--port", "5174"],
  "cwd": "${workspaceFolder}/frontend-admin"
}
```

**New Compound Tasks:**
- ✅ `Full Startup (Backend + Admin Frontend)`
- ✅ `Full Startup (All Services)`

---

## Quick Start Guide

### Option 1: Full Stack with Default Frontend
```bash
# Start from VS Code Command Palette (Ctrl+Shift+P):
> Run Task: ✅ Full Startup (Backend + Frontend)
```

### Option 2: Full Stack with Admin Frontend
```bash
# Start from VS Code Command Palette:
> Run Task: ✅ Full Startup (Backend + Admin Frontend)
```

### Option 3: All Services (Both Frontends)
```bash
# Start from VS Code Command Palette:
> Run Task: ✅ Full Startup (All Services)
```

### Option 4: Individual Service Debug
```bash
# Press F5 or from Command Palette:
> Debug: Start Debugging > Catalog Service Standalone
```

---

## Service URLs

### Microservices
| Service | URL | Port |
|---------|-----|------|
| AppHost Dashboard | http://localhost:9000 | 9000 |
| Catalog Service | http://localhost:9001 | 9001 |
| Auth Service | http://localhost:9002 | 9002 |
| Search Service | http://localhost:9003 | 9003 |
| Order Service | http://localhost:9004 | 9004 |

### Frontend Applications
| App | URL | Port |
|-----|-----|------|
| Customer Frontend | http://localhost:5173 | 5173 |
| Admin Frontend | http://localhost:5174 | 5174 |
| Aspire Dashboard | http://localhost:5500 | 5500 |

### Health Checks
| Endpoint | URL |
|----------|-----|
| Overall Health | http://localhost:9000/health |
| Service Status | http://localhost:9000/api/health |

---

## Available Debug Configurations

### Single Service Debug
- **AppHost (Debug)** - Main orchestrator
- **Catalog Service (Debug)** - Catalog microservice standalone
- **Frontend (Debug)** - Customer-facing frontend
- **Frontend Admin (Debug)** - Admin management frontend
- **Frontend Tests (Vitest)** - Unit test debugger
- **E2E Tests (Playwright)** - End-to-end test debugger

### Compound Debuggers
- **Full Stack (AppHost + Frontend)** - Backend + customer app
- **Full Stack (AppHost + Admin Frontend)** - Backend + admin app
- **Catalog Service Standalone** - Isolated catalog service
- **Full Stack with All Services** - Complete orchestration + all frontends
- **Testing Suite** - All test runners

---

## Available Test Tasks

### Backend Tests
| Task | Purpose |
|------|---------|
| 🗂️ Catalog Service Tests | All catalog tests |
| 🔍 Catalog Validators Test | FluentValidation tests |
| 📨 Event Validation Tests | Event validator tests |
| backend-test | All backend tests |

### Frontend Tests
| Task | Purpose |
|------|---------|
| frontend-test | Vitest unit tests |
| 🎯 E2E Tests (Language Selection) | Specific E2E test |

---

## Environment Variables Enabled

### Backend (.NET Aspire)
```
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:9000
DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true
CATALOG_SERVICE_PORT=9001
ELASTICSEARCH_ENABLED=true
EVENTVALIDATION_ENABLED=true
LOCALIZATION_ENABLED=true
```

### Frontend (Customer App)
```
VITE_API_URL=http://localhost:9000
```

### Admin Frontend
```
VITE_API_URL=http://localhost:9000
VITE_CATALOG_API=http://localhost:9001
```

---

## Features Active

When starting Aspire, the following are automatically enabled:

✅ **Catalog Service**
- Product management
- Category management
- Brand management
- Multi-language support

✅ **Event Validation**
- Automatic event validation
- FluentValidation rules
- Error handling

✅ **Elasticsearch**
- Search service integration
- Full-text search
- Document indexing

✅ **Localization (i18n)**
- Multi-language support
- Language-specific content
- Localized error messages

✅ **AOP & Filters**
- Request validation
- Exception handling
- Request logging

---

## Troubleshooting

### Port Already in Use
If you get "port already in use" error:
```bash
# Kill existing process
lsof -ti:5173 | xargs kill -9  # Frontend
lsof -ti:9000 | xargs kill -9  # AppHost
lsof -ti:5174 | xargs kill -9  # Admin Frontend
```

Or use the Stop task:
```bash
# From VS Code: Run Task > 🛑 Stop Services
```

### Services Not Starting
1. Ensure Docker daemon is running (for Elasticsearch)
2. Check ports are available: `netstat -an | grep LISTEN`
3. Verify .NET SDK: `dotnet --version`
4. Check logs: `tail -f logs/apphost.log`

### Debug Not Attaching
1. Ensure task is running first (check Terminal)
2. Wait for "listening" message before debugging
3. Check firewall settings
4. Restart debug session

---

## Keyboard Shortcuts

| Action | Shortcut |
|--------|----------|
| Run Task | Ctrl+Shift+` |
| Start Debugging | F5 |
| Stop Debugging | Ctrl+Shift+F5 |
| Toggle Debug Console | Ctrl+Shift+Y |
| View Integrated Terminal | Ctrl+` |

---

## Workflow Examples

### Develop Catalog Feature
```
1. F5 → Select "Catalog Service (Debug)"
2. Open frontend-admin source
3. Make changes, save (auto-reload)
4. Refresh admin UI in browser
5. Breakpoints in VS Code stop execution
```

### Debug Admin Frontend
```
1. Run Task → "👨‍💼 Admin Frontend Dev (port 5174)"
2. F5 → Select "Frontend Admin (Debug)"
3. DevTools Console visible in VS Code
4. Make changes, auto-reload in browser
```

### Run Full Stack with Tests
```
1. Run Task → "✅ Full Startup (All Services)"
2. Run Task → "🗂️ Catalog Service Tests"
3. Run Task → "frontend-test"
4. View test results in Terminal
```

### Test-Driven Development
```
1. Run Task → "🔍 Catalog Validators Test"
2. Modify validator code in editor
3. Test auto-reruns on save
4. See failures/passes in Terminal
```

---

## Configuration Files Modified

| File | Changes |
|------|---------|
| `aspire-start.sh` | Added service ports, feature flags, enhanced output |
| `.vscode/launch.json` | Added Catalog & Admin debug configs, new compounds |
| `.vscode/tasks.json` | Added test tasks, admin frontend task, new compounds |

---

## Next Steps

### Immediate
1. ✅ Test `Full Startup (All Services)` task
2. ✅ Verify all service URLs are accessible
3. ✅ Run catalog validator tests
4. ✅ Check admin frontend loads

### This Week
1. Implement Admin Frontend forms
2. Connect to Catalog Service API
3. Add more test coverage
4. Document API contracts

### This Month
1. Production deployment prep
2. Performance optimization
3. Security hardening
4. Load testing

---

## Support

### Issues?
- Check Terminal output for errors
- Review `.vscode/launch.json` syntax
- Verify all ports are available
- Check prerequisites (dotnet, node, npm)

### Questions?
- See CATALOG_FRONTEND_STATUS.md for frontend details
- See CATALOG_INTEGRATION.md for integration guide
- Review aspire-start.sh for service configuration

---

**Status**: ✅ CONFIGURATION UPDATE COMPLETE

All tasks, debug configurations, and startup scripts have been updated to support the new Catalog Service, Event Validation, and Admin Frontend integration.

**Last Updated**: 2025-12-26  
**Version**: 2.0

