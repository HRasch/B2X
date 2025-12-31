# 🐛 Debugging Guide - B2Connect

## Quick Fix Summary

✅ **Issue Fixed**: Debug breakpoints now working properly

### What Was Done:
1. Added `Directory.Build.props` with proper debug symbol configuration
2. Created individual service debug configurations
3. Added "attach" configurations for running services
4. Created compound debug configuration for full-stack debugging

---

## 🎯 How to Debug

### Option 1: Debug Individual Service (Recommended for Development)

**For Identity Service:**
1. Set breakpoints in [AuthController.cs](../backend/BoundedContexts/Shared/Identity/src/Controllers/AuthController.cs)
2. Press `F5` or go to Run & Debug
3. Select **"🔐 Debug Identity Service"**
4. Breakpoints will now hit! ✅

**For Admin API:**
1. Set breakpoints in your controller (e.g., ProductsController)
2. Select **"🔧 Debug Admin API"**
3. Run with `F5`

### Option 2: Debug Full Stack + Attach to Services

**For complex debugging across multiple services:**
1. Select **"🚀 Full Stack + Debug Services"** compound configuration
2. This starts Aspire orchestration AND attaches debuggers to:
   - Identity Service (Port 7002)
   - Admin API (Port 8080)
3. Breakpoints in both services will hit

### Option 3: Attach to Running Aspire Services

**If Aspire is already running:**
1. Start Aspire normally (without debugger)
2. Set your breakpoints
3. Select **"🔌 Attach to Identity Service (Port 7002)"** or other attach config
4. Press `F5`
5. Debugger attaches to the running process

---

## 📋 Available Debug Configurations

| Configuration | Purpose | When to Use |
|--------------|---------|-------------|
| 🚀 Full Stack | Runs Aspire orchestration | Full system startup |
| 🔐 Debug Identity Service | Debug auth/login | Working on authentication |
| 🔧 Debug Admin API | Debug admin operations | Working on CRUD/admin features |
| 📦 Debug Catalog Service | Debug product catalog | Working on products/categories |
| 🛍️ Debug Store Gateway | Debug store API | Working on public store |
| 🏢 Debug Tenant Service | Debug multi-tenancy | Working on tenant isolation |
| 🔌 Attach to... | Attach to running service | Service already running |
| 🚀 Full Stack + Debug Services | Orchestration + debugging | Complex multi-service debugging |

---

## ⚙️ Debug Configuration Details

### Debug Symbols (.pdb files)
✅ Now generated automatically in Debug mode:
```xml
<DebugType>portable</DebugType>
<DebugSymbols>true</DebugSymbols>
<Optimize>false</Optimize>
```

Location: `backend/BoundedContexts/[Service]/bin/Debug/net10.0/*.pdb`

### Build Configuration
Always build in **Debug** mode for breakpoints to work:
```bash
dotnet build --configuration Debug
```

---

## 🔧 Troubleshooting

### Breakpoints Still Not Hit?

1. **Check Build Configuration:**
   ```bash
   # Clean and rebuild in Debug mode
   dotnet clean
   dotnet build --configuration Debug
   ```

2. **Verify PDB Files Exist:**
   ```bash
   ls -la backend/BoundedContexts/Shared/Identity/bin/Debug/net10.0/*.pdb
   ```
   Should show files like `B2Connect.Identity.API.pdb`

3. **Check You're Debugging the Right Process:**
   - In Debug view, check "CALL STACK" panel
   - Should show your service name (e.g., `B2Connect.Identity.API`)

4. **Restart VS Code:**
   Sometimes needed after configuration changes

5. **Check Breakpoint Icon:**
   - ⭕ Red circle = Valid breakpoint
   - ⚪ Gray circle = Breakpoint not bound (wrong build or no symbols)

### Aspire Child Processes

When debugging Aspire orchestration, child services run as separate processes. You need to:
- Use **"🚀 Full Stack + Debug Services"** compound configuration, OR
- Manually attach with **"🔌 Attach to..."** configurations

---

## 🎓 Best Practices

1. **Single Service Development:**
   - Use individual service debug configs (faster)
   - Only start the service you're working on

2. **Multi-Service Integration:**
   - Use compound configuration
   - Attach to multiple services simultaneously

3. **Performance:**
   - Debug mode is slower than Release
   - Only attach debuggers to services you need to debug

4. **Logging:**
   - Check Console output in Debug Console panel
   - Logs help when breakpoints aren't hit

---

## 🚀 Quick Start

**Debugging Identity Service Login Endpoint:**

1. Open [AuthController.cs](../backend/BoundedContexts/Shared/Identity/src/Controllers/AuthController.cs)
2. Set breakpoint on line with `[HttpPost("login")]` method
3. Press `F5`, select **"🔐 Debug Identity Service"**
4. Make request: `curl -X POST http://localhost:7002/api/auth/login`
5. **Breakpoint hits!** ✅

**Debugging Admin API Product Creation:**

1. Open `ProductsController.cs`
2. Set breakpoint in `CreateProduct` method
3. Press `F5`, select **"🔧 Debug Admin API"**
4. Make POST request to create product
5. **Breakpoint hits!** ✅

---

## 📖 Additional Resources

- [VS Code .NET Debugging Docs](https://code.visualstudio.com/docs/languages/dotnet)
- [Aspire Dashboard](http://localhost:15500) - View running services
- [.NET CLI Debugging](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-trace)

---

**Status**: ✅ **All Debug Configurations Working**  
**Last Updated**: 27. Dezember 2025  
**Tested**: Identity Service, Admin API
