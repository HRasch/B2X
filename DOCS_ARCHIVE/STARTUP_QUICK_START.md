# B2Connect Startup - Quick Start Guide

## Overview

Your B2Connect project uses three automated startup scripts to manage services:

| Script | Purpose | Usage |
|--------|---------|-------|
| `start-all.sh` | Start both backend & frontend | **Use this to start everything** |
| `aspire-start.sh` | Start backend only (Aspire) | For backend development |
| `aspire-stop.sh` | Stop all services | To cleanly shut down |

## Quick Start

### Start Complete Environment (Backend + Frontend)

```bash
./start-all.sh Development Debug
```

**What happens:**
- ✅ Starts .NET Aspire orchestration (AppHost) on port 5200
- ✅ Starts Aspire Dashboard on port 5500
- ✅ Starts Vite frontend dev server on port 5173
- ✅ All services run in background
- ✅ Press `Ctrl+C` to stop all services at once

### Start Backend Only

```bash
./aspire-start.sh Development Debug 5200
```

**Parameters:**
- `Development` - Environment (Development/Production)
- `Debug` - Build configuration (Debug/Release)
- `5200` - AppHost port (optional, defaults to 5200)

### Stop All Services

```bash
./aspire-stop.sh
```

**What happens:**
- ✅ Gracefully stops AppHost with 5-second timeout
- ✅ Force kills if process doesn't respond
- ✅ Cleans up leftover processes
- ✅ Removes PID files

## Service URLs

Once services are running:

| Service | URL | Purpose |
|---------|-----|---------|
| **AppHost Dashboard** | http://localhost:5200 | AppHost API & management |
| **Aspire Dashboard** | http://localhost:5500 | Service orchestration UI |
| **Frontend** | http://localhost:5173 | Vue.js development server |

## Logs

All logs are stored in `/logs/` directory:

```bash
# View AppHost logs
tail -f logs/apphost.log

# View all recent logs
ls -lh logs/
```

## Troubleshooting

### Port Already in Use

```bash
# Find process using port 5200
lsof -i :5200

# Kill the process
kill -9 <PID>

# Or stop all services
./aspire-stop.sh
```

### Services Not Starting

1. **Check prerequisites:**
   ```bash
   dotnet --version  # Should be .NET 10+
   npm --version     # Should be Node 18+
   ```

2. **Check permissions:**
   ```bash
   chmod +x *.sh     # Make scripts executable
   ```

3. **View logs:**
   ```bash
   tail -f logs/apphost.log
   ```

### Frontend Not Connecting to Backend

- Verify AppHost is running: http://localhost:5200/health
- Check frontend console for CORS errors
- Ensure API base URL points to correct backend port

## Environment Variables

Optional configuration:

```bash
# Custom ports
./aspire-start.sh Development Debug 5300

# Production environment
./aspire-start.sh Production Release 5200
```

## What's Running

### Backend Services (via Aspire AppHost)

Automatically orchestrated services:
- **API Gateway** - Central API routing
- **Auth Service** - Authentication/authorization
- **Tenant Service** - Multi-tenant management
- **Layout Service** - UI layout configurations
- **Theme Service** - Theme management
- **Localization Service** - i18n/localization
- **Database** - PostgreSQL (if configured)
- **Redis Cache** - In-memory caching (if configured)

### Frontend Development Server

- **Vite** - Fast development server with hot reload
- **Vue 3** - Frontend framework
- **Vitest** - Unit testing
- **Playwright** - E2E testing

## Full Startup Sequence

When you run `./start-all.sh`:

```
1. Check prerequisites (dotnet, npm, node)
2. Create log and PID directories
3. Build backend if needed
4. Start AppHost (port 5200)
5. Wait for health check (port 5200/health)
6. Start Aspire Dashboard (port 5500)
7. Start frontend dev server (port 5173)
8. Display all service URLs
9. Keep all processes running in background
```

## Stopping Services

### Clean Shutdown

Press `Ctrl+C` in the terminal or run:
```bash
./aspire-stop.sh
```

### What Cleanup Does

- ✅ Gracefully stops all .NET services
- ✅ Kills remaining processes if needed
- ✅ Removes temporary PID files
- ✅ Cleans up log directories

## Development Workflow

### Working on Frontend

```bash
./start-all.sh Development Debug
# Navigate to http://localhost:5173
# Edit files in src/ - auto-reload enabled
```

### Working on Backend Services

```bash
# Start infrastructure
./aspire-start.sh Development Debug

# In VS Code: Open backend solution
# Services will rebuild on save (watch mode)
```

### Running Tests

**Frontend tests:**
```bash
cd frontend
npm run test          # Unit tests (Vitest)
npm run e2e          # End-to-end tests (Playwright)
npm run test:watch   # Watch mode
```

**Backend tests:**
```bash
cd backend
dotnet test
```

## Advanced Usage

### Custom Configuration

Edit scripts to modify:
- Port numbers (APPHOST_PORT, DASHBOARD_PORT)
- Log locations (LOGS_DIR)
- Timeouts and retry attempts
- Health check endpoints

### Monitoring

```bash
# Watch logs in real-time
tail -f logs/apphost.log

# Monitor running processes
ps aux | grep -E "dotnet|npm|vite"

# Check port usage
netstat -an | grep LISTEN | grep -E "5200|5173|5500"
```

### Debugging

```bash
# Run with debug output
bash -x aspire-start.sh Development Debug

# Check script syntax
bash -n aspire-start.sh
bash -n aspire-stop.sh
bash -n start-all.sh
```

## Common Commands

```bash
# Start everything
./start-all.sh Development Debug

# Stop everything
./aspire-stop.sh

# Check AppHost health
curl http://localhost:5200/health

# View all running services
ps aux | grep -E "dotnet|npm|node"

# Clear logs
rm -f logs/*

# Reset everything (careful!)
./aspire-stop.sh
rm -rf logs/ .pids/
./start-all.sh Development Debug
```

## Next Steps

1. **Start the environment:**
   ```bash
   ./start-all.sh Development Debug
   ```

2. **Open in browser:**
   - Frontend: http://localhost:5173
   - Aspire Dashboard: http://localhost:5500

3. **Start developing:**
   - Edit frontend files in `frontend/src/`
   - Edit backend files in `backend/services/`
   - Changes reload automatically

4. **Run tests:**
   ```bash
   cd frontend && npm run test
   cd ../backend && dotnet test
   ```

5. **Stop when done:**
   ```bash
   ./aspire-stop.sh
   ```

## Getting Help

- **Aspire Documentation:** https://learn.microsoft.com/en-us/dotnet/aspire
- **Vue 3 Documentation:** https://vuejs.org/
- **Vite Documentation:** https://vitejs.dev/

---

**Last Updated:** December 25, 2024
**Version:** 1.0
