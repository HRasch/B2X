# VS Code Debug & Tasks Quick Reference

**Updated**: 2025-12-26

## 🚀 Quick Start (F5 or Ctrl+Shift+P > Run Task)

### Start Full Stack
```
Ctrl+Shift+P > Run Task: ✅ Full Startup (Backend + Admin Frontend)
```

### Debug Single Service
```
F5 > Select Debug Configuration
  → Catalog Service (Debug)
  → Frontend Admin (Debug)
  → AppHost (Debug)
```

### Run Tests
```
Ctrl+Shift+P > Run Task
  → 🗂️ Catalog Service Tests
  → 🔍 Catalog Validators Test
  → 📨 Event Validation Tests
```

---

## Debug Configurations (F5)

| Config | Service | Port | Purpose |
|--------|---------|------|---------|
| **AppHost (Debug)** | Main orchestrator | 9000 | Debug all services |
| **Catalog Service (Debug)** | Catalog API | 9001 | Isolated debugging |
| **Frontend (Debug)** | Customer app | 5173 | Frontend debugging |
| **Frontend Admin (Debug)** | Admin panel | 5174 | Admin UI debugging |
| **Frontend Tests (Vitest)** | Unit tests | - | Debug tests |
| **E2E Tests (Playwright)** | Integration tests | - | Debug E2E |

## Compound Debuggers (F5)

| Config | Includes | Use When |
|--------|----------|----------|
| **Full Stack (AppHost + Frontend)** | Backend + customer app | Debugging customer features |
| **Full Stack (AppHost + Admin Frontend)** | Backend + admin app | Debugging admin features |
| **Catalog Service Standalone** | Catalog Service only | Isolated catalog work |
| **Full Stack with All Services** | All services + both apps | Full system testing |
| **Testing Suite** | All test runners | Running all tests |

---

## Tasks (Ctrl+Shift+P > Run Task)

### 🚀 Startup Tasks
```
✅ Full Startup (Backend + Frontend)
✅ Full Startup (Backend + Admin Frontend)
✅ Full Startup (All Services)
🚀 Backend Aspire (aspire-start.sh)
🎨 Frontend Dev (port 5173)
👨‍💼 Admin Frontend Dev (port 5174)
```

### 🧪 Test Tasks
```
🗂️ Catalog Service Tests
🔍 Catalog Validators Test
📨 Event Validation Tests
frontend-test
🎯 E2E Tests (Language Selection)
```

### 🛠️ Build Tasks
```
backend-build
backend-restore
frontend-build
frontend-install
```

### 🛑 Stop
```
🛑 Stop Services
```

---

## Service URLs

```
AppHost (Main)           → http://localhost:9000
Catalog Service          → http://localhost:9001
Auth Service             → http://localhost:9002
Search Service           → http://localhost:9003
Order Service            → http://localhost:9004

Frontend (Customer)      → http://localhost:5173
Frontend (Admin)         → http://localhost:5174
Aspire Dashboard         → http://localhost:5500
```

---

## Common Workflows

### 💻 Develop Catalog Feature
```
1. F5 → "Catalog Service (Debug)"
2. Set breakpoints in CatalogService code
3. Make request to http://localhost:9001/api/products
4. Breakpoint hits in VS Code
5. Step through code, inspect variables
```

### 📱 Build Admin Dashboard
```
1. Ctrl+Shift+P → Run Task: "👨‍💼 Admin Frontend Dev"
2. F5 → "Frontend Admin (Debug)"
3. Edit src/views/catalog/*.vue
4. Auto-reload in browser (http://localhost:5174)
5. Debug console in VS Code shows logs
```

### ✅ Test Validators
```
1. Ctrl+Shift+P → Run Task: "🔍 Catalog Validators Test"
2. Tests run automatically
3. View results in Terminal
4. Modify validator, tests re-run
5. Red/Green feedback loop
```

### 🔧 Debug Full Stack
```
1. Ctrl+Shift+P → Run Task: "✅ Full Startup (All Services)"
2. Wait for "Press Ctrl+C" message
3. F5 → "Full Stack with All Services"
4. All services running with debugger attached
5. Set breakpoints anywhere
```

---

## Keyboard Shortcuts

| Action | Shortcut |
|--------|----------|
| Run Task | Ctrl+Shift+` |
| Start Debug | F5 |
| Continue (debug) | F5 |
| Step Over | F10 |
| Step Into | F11 |
| Step Out | Shift+F11 |
| Stop Debug | Ctrl+Shift+F5 |
| Toggle Breakpoint | Ctrl+B |
| Debug Console | Ctrl+Shift+Y |
| Terminal | Ctrl+` |
| Command Palette | Ctrl+Shift+P |

---

## Environment Status

**Backend Services:**
- ✅ AppHost Orchestrator
- ✅ Catalog Service
- ✅ Auth Service
- ✅ Search Service
- ✅ Order Service
- ✅ Elasticsearch
- ✅ Event Validation
- ✅ Localization (i18n)

**Frontend Apps:**
- ✅ Customer App (5173)
- ✅ Admin Dashboard (5174)
- ✅ Vitest Unit Tests
- ✅ Playwright E2E Tests

**Features:**
- ✅ AOP Filters
- ✅ FluentValidation
- ✅ Multi-Language Support
- ✅ Event System
- ✅ Full-Text Search

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Port in use | `lsof -ti:PORT \| xargs kill -9` |
| Debug not attaching | Stop, wait 5s, start again |
| No breakpoints hit | Ensure task running first |
| Cannot reach API | Check firewall, verify port open |
| Tests failing | Check error in Terminal, fix code |

---

## Pro Tips

💡 **Parallel Development**
- Start backend with task
- Debug frontend in separate debug session
- Edit both simultaneously
- Auto-reload picks up changes

💡 **Fast Testing**
- Run specific test filter: `🔍 Catalog Validators Test`
- Watch mode auto-reruns on file change
- See immediate feedback

💡 **Port Conflicts**
- Change ports in tasks.json
- Or stop previous session
- Check: `netstat -an | grep LISTEN`

💡 **Clean Restart**
- Run: `🛑 Stop Services`
- Wait 2 seconds
- Then: `✅ Full Startup`

---

**Last Updated**: 2025-12-26  
**Version**: 2.0

Quick reference for all debug configurations, tasks, and workflows.

