# 🚀 QUICK START - Build & Debug

## ⚡ Fastest Way to Run (F5 Debug)

1. Open VS Code
2. Press **F5**
3. Select **"🚀 Aspire AppHost (Orchestration)"**
4. Wait for services to start
5. Open http://localhost:5173

## 📋 What Was Fixed

| Issue | Fix |
|-------|-----|
| `dotnet restore` failing | Added `B2Connect.sln` to task args |
| `dotnet build` failing | Added `B2Connect.sln` to task args |
| Quartz version conflict | Updated 3.10.5 → 3.11.0 |
| Launch config broken | Tasks now available and working |

## ✅ Build Status

```
dotnet restore B2Connect.sln     ✅ SUCCESS
dotnet build B2Connect.sln       ✅ SUCCESS (0 errors, 0 warnings)
AppHost binary                   ✅ EXISTS
All services                      ✅ READY TO START
```

## 🎯 Available Debug Configurations

1. **🚀 Aspire AppHost (Orchestration)** ← START HERE
   - Launches entire system
   - All services start automatically
   - Port: 15500

2. **AppHost (Debug) - Legacy**
   - Alternative config
   - Port: 9000

3. **Catalog Service (Debug)**
   - Debug single service only
   - Port: 9001

## 📁 Files Modified

- ✏️ `.vscode/tasks.json` - Added solution file spec
- ✏️ `backend/Directory.Packages.props` - Updated Quartz version
- ✏️ `BUILD_FIX_SUMMARY.md` - Complete change log
- ✏️ `LAUNCH_CONFIG_UPDATE.md` - Detailed documentation

## 🔗 Service Ports

| Service | Port | Internal |
|---------|------|----------|
| Frontend | 5173 | localhost |
| Admin Frontend | 5174 | localhost |
| Auth Service | 9002 | orchestrated |
| Tenant Service | 9003 | orchestrated |
| Catalog Service | 9001 | orchestrated |
| Localization | 9004 | orchestrated |

## 🧪 Run Tests

```bash
# All backend tests
cd backend
dotnet test

# Specific test project
cd backend/Tests/CatalogService.Tests
dotnet test

# With filter
dotnet test --filter "CatalogValidators"

# Frontend tests
cd frontend
npm run test
```

## 💡 Pro Tips

- Use **Ctrl+Shift+P** → "Tasks: Run Task" to run any configured task
- All preLaunchTasks run automatically when you press F5
- Add breakpoints and step through code with F5
- Stop debugging with Ctrl+C in the integrated terminal

## 🐛 Troubleshooting

**Build fails?**
```bash
cd backend
dotnet clean B2Connect.sln
dotnet restore B2Connect.sln
dotnet build B2Connect.sln
```

**Port already in use?**
```bash
lsof -i :15500    # Find what's using the port
kill -9 <PID>     # Kill the process
```

**AppHost won't start?**
- Check launchSettings.json for correct environment
- Verify all dependent services have ports available
- Check logs in integrated terminal

---

✅ **Everything is ready. Press F5 to start!** 🎉
