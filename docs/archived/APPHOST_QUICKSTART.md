# AppHost Quick Reference

## 🚀 Start Services

```bash
cd backend/services/AppHost
dotnet run
```

**Expected Output:**
```
[2025-12-26 09:13:35 INF] 🚀 B2Connect Application Host - Starting
[2025-12-26 09:13:35 INF] ▶ Starting Auth Service on port 9002...
[2025-12-26 09:13:35 INF]   ✓ Auth Service started (PID: 7976)
[2025-12-26 09:13:36 INF] ▶ Starting Tenant Service on port 9003...
[2025-12-26 09:13:36 INF]   ✓ Tenant Service started (PID: 7981)
[2025-12-26 09:13:37 INF] ▶ Starting Localization Service on port 9004...
[2025-12-26 09:13:37 INF]   ✓ Localization Service started (PID: 7983)
[2025-12-26 09:13:37 INF] ✅ B2Connect Application Host initialized
```

---

## 🎯 Service Endpoints

| Service | Port | Health | Purpose |
|---------|------|--------|---------|
| **Auth** | 9002 | `/health` | Authentication & Authorization |
| **Tenant** | 9003 | `/health` | Multi-tenant Management |
| **Localization** | 9004 | `/health` | i18n & Translations |

---

## ✅ Verify Services Running

```bash
# All at once
curl http://localhost:9002/health && \
curl http://localhost:9003/health && \
curl http://localhost:9004/health

# Individual checks
curl http://localhost:9002/health  # Auth
curl http://localhost:9003/health  # Tenant
curl http://localhost:9004/health  # Localization
```

---

## 🛑 Stop Services

```bash
# Option 1: Press Ctrl+C in AppHost terminal
# (recommended - graceful shutdown)

# Option 2: Kill from another terminal
pkill -f "B2Connect.AppHost"

# Option 3: Kill all .NET processes (nuclear option)
killall dotnet
```

---

## 🎨 Start Frontends (In Separate Terminal)

```bash
# Customer App (Port 5173)
cd frontend
npm install
npm run dev

# Or Admin App (Port 5174)
cd frontend-admin
npm install
npm run dev -- --port 5174
```

---

## 📊 Check Running Processes

```bash
# See all running services
ps aux | grep -E "9002|9003|9004|dotnet" | grep -v grep

# Count active processes
ps aux | grep -E "Auth|Tenant|Localization" | grep -v grep | wc -l
# Should return: 3 (or more if counting child processes)
```

---

## 🔍 Troubleshooting

### Services won't start

```bash
# Check if ports are free
lsof -i :9002
lsof -i :9003
lsof -i :9004

# Kill process on port if needed
kill -9 <PID>

# Verify dotnet is in PATH
which dotnet

# Rebuild AppHost
cd backend/services/AppHost && dotnet clean && dotnet build
```

### Port already in use

```bash
# Find process using port 9002
lsof -i :9002

# Kill it
kill -9 <PID>

# Try starting AppHost again
dotnet run
```

### Path not found error

```bash
# Verify you're in the right directory
pwd
# Should end with: /B2Connect/backend/services/AppHost

# Check services directory structure
ls -la ../
# Should see: auth-service, tenant-service, LocalizationService
```

---

## 📝 Configuration

**To add a new service to AppHost:**

Edit: `backend/services/AppHost/Program.cs`

```csharp
var services = new List<(string name, string path, int port)>
{
    ("Auth Service", Path.Combine(servicesDir, "auth-service"), 9002),
    ("Tenant Service", Path.Combine(servicesDir, "tenant-service"), 9003),
    ("Localization Service", Path.Combine(servicesDir, "LocalizationService"), 9004),
    // Add new service:
    ("MyService", Path.Combine(servicesDir, "my-service"), 9005),
};
```

---

## 🌍 Cross-Platform Support

✅ **Tested and Working:**
- macOS (Apple Silicon) - Full support
- macOS (Intel) - Expected to work
- Windows - Expected to work
- Linux - Expected to work

**Why AppHost works everywhere:**
- Uses native .NET Process API
- Uses `Path.Combine()` for cross-platform path handling
- No external dependencies (no DCP, no Docker required)
- Minimal logging framework (Serilog only)

---

## 📚 Full Documentation

For detailed information about AppHost architecture, read:
**[APPHOST_SPECIFICATIONS.md](APPHOST_SPECIFICATIONS.md)**

Key topics:
- Why AppHost vs Aspire.Hosting
- Path resolution cross-platform
- Service lifecycle management
- Error handling & debugging
- Future extensions

---

## 🎯 Next Steps

1. ✅ Start AppHost: `dotnet run`
2. ✅ Verify services: `curl http://localhost:9002/health`
3. ✅ Start frontend: `npm run dev`
4. 🎉 Develop!

**Happy coding!** 🚀
