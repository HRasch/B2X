![B2Connect Development Environment Setup](https://img.shields.io/badge/B2Connect-Development%20Setup-orange)

# Development Environment Setup Guide

**Last Updated:** 28. Dezember 2025  
**Version:** 1.0  
**Owner:** Tech Lead  
**Estimated Setup Time:** 30-45 minutes

---

## Table of Contents

1. [System Requirements](#system-requirements)
2. [Installation Steps](#installation-steps)
3. [Local Development with Aspire](#local-development-with-aspire)
4. [Verification Checklist](#verification-checklist)
5. [Common Issues & Troubleshooting](#common-issues--troubleshooting)
6. [IDE Configuration](#ide-configuration)
7. [Daily Development Workflow](#daily-development-workflow)

---

## System Requirements

### Hardware

| Component | Minimum | Recommended |
|-----------|---------|-------------|
| **CPU** | 4 cores | 8+ cores (Aspire needs multiple services) |
| **Memory** | 8 GB | 16+ GB (Docker containers + services) |
| **Storage** | 50 GB | 100+ GB (Docker images, node_modules, build artifacts) |
| **OS** | macOS 12.6+, Windows 11, Ubuntu 22.04+ | macOS 14+ (Intel/Apple Silicon) |

### Software (Required)

| Tool | Version | Purpose | Install |
|------|---------|---------|---------|
| **.NET SDK** | 8.0+ | Backend compilation | [Download](https://dotnet.microsoft.com/download) |
| **Node.js** | 18+ | Frontend toolchain | [Download](https://nodejs.org/) |
| **Git** | 2.40+ | Version control | `brew install git` (macOS) |
| **Docker Desktop** | 24.0+ | Container runtime | [Download](https://docker.com/products/docker-desktop) |
| **GitHub CLI** | 2.40+ | GitHub integration | [Download](https://cli.github.com/) |
| **VS Code** | Latest | IDE (recommended) | [Download](https://code.visualstudio.com/) |
| **PowerShell** | 7.0+ | Scripting (Windows/macOS) | Built-in or [Download](https://github.com/PowerShell/PowerShell) |

### Software (Optional)

| Tool | Purpose | Install |
|------|---------|---------|
| **Azure Data Studio** | Database GUI | `brew install --cask azure-data-studio` |
| **DBeaver** | Database GUI | [Download](https://dbeaver.io/) |
| **Postman** | API testing | [Download](https://postman.com/downloads/) |
| **kubectl** | Kubernetes testing | `brew install kubectl` |

---

## Installation Steps

### Step 1: Clone Repository

```bash
# Clone the repository
git clone https://github.com/HRasch/B2Connect.git
cd B2Connect

# Verify structure
ls -la | head -20
```

**Expected Output:**
```
B2Connect.slnx
backend/
frontend/
docs/
scripts/
...
```

### Step 2: Install .NET SDK

**macOS (Intel/Apple Silicon)**
```bash
# Install .NET 8
brew install dotnet

# Verify installation
dotnet --version
# Expected: 8.x.x or higher

# Restore dependencies (do this once)
dotnet restore B2Connect.slnx
```

**Windows (PowerShell)**
```powershell
# Install via Chocolatey (if installed)
choco install dotnet-sdk

# Or download from Microsoft directly
# https://dotnet.microsoft.com/download

# Verify installation
dotnet --version
# Expected: 8.x.x or higher
```

**Ubuntu/Linux**
```bash
# Install via apt (Ubuntu 22.04+)
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Verify
dotnet --version
```

### Step 3: Install Node.js & npm

**macOS**
```bash
# Install via Homebrew
brew install node

# Verify
node --version    # v18+
npm --version     # v9+
```

**Windows (PowerShell)**
```powershell
# Install via Chocolatey
choco install nodejs

# Or download from nodejs.org
# https://nodejs.org/

# Verify
node --version
npm --version
```

**Ubuntu/Linux**
```bash
# Install via NodeSource repository
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# Verify
node --version
npm --version
```

### Step 4: Install Docker Desktop

**macOS**
```bash
# Install Docker Desktop
brew install --cask docker

# Start Docker daemon (one-time setup)
open /Applications/Docker.app
# Wait for "Docker is running" indicator

# Verify
docker --version
docker run hello-world  # Should print welcome message
```

**Windows (PowerShell)**
```powershell
# Install via Chocolatey
choco install docker-desktop

# Or download from Docker directly
# https://www.docker.com/products/docker-desktop

# Verify (after restart)
docker --version
docker run hello-world
```

**Ubuntu/Linux**
```bash
# Install Docker Engine
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Add current user to docker group (optional, for non-sudo)
sudo usermod -aG docker $USER

# Verify
docker --version
docker run hello-world
```

### Step 5: Install GitHub CLI

**macOS**
```bash
brew install gh

# Verify
gh --version

# Authenticate with GitHub
gh auth login
# Follow prompts (choose HTTPS, store token securely)
```

**Windows (PowerShell)**
```powershell
choco install gh

# Verify
gh --version

# Authenticate
gh auth login
```

**Ubuntu/Linux**
```bash
# Install from official repository
sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys C99B11DEB97541F0
sudo apt-add-repository https://cli.github.com/packages
sudo apt update
sudo apt install gh

# Verify & authenticate
gh --version
gh auth login
```

### Step 6: Restore Frontend Dependencies

```bash
# Navigate to frontend directories
cd Frontend/Store
npm install
npm run build  # Verify compilation

# Then Admin frontend
cd ../Admin
npm install
npm run build

# Return to root
cd ../..
```

### Step 7: Verify .NET Project Structure

```bash
# Check solution structure
dotnet sln B2Connect.slnx list | head -20

# Expected output:
# backend/Domain/Identity/src/B2Connect.Identity.csproj
# backend/Domain/Catalog/src/B2Connect.Catalog.csproj
# ...

# Build entire solution (first time = slow)
dotnet build B2Connect.slnx -c Debug

# Expected: "Build succeeded. 0 warnings"
```

---

## Local Development with Aspire

### Understanding Aspire

**Aspire** is .NET's cloud-native orchestration tool that simulates Kubernetes locally:

```
Your Local Machine
├── Aspire Orchestrator (http://localhost:15500)
│   ├── Identity Service (Port 7002)
│   ├── Catalog Service (Port 7005)
│   ├── CMS Service (Port 7006)
│   ├── PostgreSQL (Port 5432)
│   ├── Redis (Port 6379)
│   └── Elasticsearch (Port 9300)
├── Frontend Store (Port 5173) - npm dev
├── Frontend Admin (Port 5174) - npm dev
└── Logs aggregated in Aspire Dashboard
```

### Starting Aspire (One-Command Setup)

**macOS/Linux**
```bash
# Ensure Docker is running
docker info  # Should show system info

# Start Aspire orchestration
cd backend/Orchestration
dotnet run

# Expected output:
# ✅ Identity service started: http://localhost:7002
# ✅ Catalog service started: http://localhost:7005
# ✅ PostgreSQL started: localhost:5432
# ✅ Redis started: localhost:6379
# ✅ Dashboard: http://localhost:15500
```

**Windows (PowerShell)**
```powershell
# Ensure Docker Desktop is running

# Start Aspire
cd backend/Orchestration
dotnet run

# Same output as macOS/Linux
```

### Aspire Dashboard

Once running, open: http://localhost:15500

**Dashboard Shows:**
- ✅ All running services and their status
- ✅ Live logs from each service (color-coded)
- ✅ Resource usage (CPU, memory)
- ✅ HTTP endpoints (clickable links)
- ✅ Database connections
- ✅ Trace/distributed tracing

### Managing Services

**Stop All Services**
```bash
# Graceful shutdown (Ctrl+C in terminal)
Ctrl+C

# Wait for services to stop (10-30 seconds)
# Aspire will close containers cleanly
```

**Kill Stuck Services (if needed)**
```bash
# macOS/Linux
./scripts/kill-all-services.sh

# Windows PowerShell
./scripts/kill-all-services.ps1

# Or manually
docker ps -a | grep b2connect
docker kill <container_id>
```

**Restart Services**
```bash
# Always run cleanup before restarting
./scripts/kill-all-services.sh
sleep 2

# Then restart
cd backend/Orchestration
dotnet run
```

### Database Access (During Development)

**Via Azure Data Studio or DBeaver**
```
Connection:
  Server: localhost
  Port: 5432
  Database: b2connect_[service_name]  (e.g., b2connect_catalog)
  User: postgres
  Password: (check appsettings.Development.json)
```

**Via SQL Command Line**
```bash
# Connect to PostgreSQL
psql -h localhost -U postgres -d b2connect_catalog

# Common queries
SELECT * FROM products LIMIT 5;
SELECT COUNT(*) FROM audit_logs;

# Exit
\q
```

### Cache & Session Management (Redis)

**Check Redis Keys**
```bash
# Connect to Redis
redis-cli -h localhost -p 6379

# View all keys
KEYS *

# View specific user session
GET session:user:123

# Clear all cache (dangerous!)
FLUSHALL

# Exit
EXIT
```

---

## Verification Checklist

After setup, run through this checklist to ensure everything works:

### Backend Services

```bash
# ✅ Verify .NET SDK
dotnet --version  # Should print 8.0+

# ✅ Restore packages
dotnet restore B2Connect.slnx

# ✅ Build solution
dotnet build B2Connect.slnx
# Expected: "Build succeeded. 0 warnings"

# ✅ Run unit tests
dotnet test B2Connect.slnx -v minimal --filter "Category=Unit"
# Expected: "Test Run Successful"

# ✅ Start Aspire
cd backend/Orchestration
dotnet run  # Wait for "Dashboard running at http://localhost:15500"
```

### Frontend Services

```bash
# Open NEW terminal (keep Aspire running)

# ✅ Install Store frontend
cd Frontend/Store
npm install
npm run build

# ✅ Install Admin frontend
cd ../Admin
npm install
npm run build

# ✅ Start Store dev server
cd ../Store
npm run dev
# Expected: "VITE v4.x.x ready in 1234 ms"
# Available at: http://localhost:5173

# ✅ Start Admin dev server (new terminal)
cd Frontend/Admin
npm run dev
# Available at: http://localhost:5174
```

### Database

```bash
# ✅ Connect to PostgreSQL
psql -h localhost -U postgres -d b2connect_catalog

# ✅ List tables
\dt

# ✅ Check for data
SELECT COUNT(*) FROM products;

# ✅ Exit
\q
```

### All Services Running

```
✅ Aspire Dashboard: http://localhost:15500
✅ Store Frontend: http://localhost:5173
✅ Admin Frontend: http://localhost:5174
✅ Identity Service: http://localhost:7002
✅ Catalog Service: http://localhost:7005
✅ CMS Service: http://localhost:7006
✅ PostgreSQL: localhost:5432
✅ Redis: localhost:6379
```

If all services are running, your development environment is ready! 🎉

---

## Common Issues & Troubleshooting

### Issue 1: "Port Already in Use" (Port 5432, 6379, 7002, etc.)

**Symptom:**
```
error: bind: address already in use
```

**Solution:**
```bash
# Kill all lingering services
./scripts/kill-all-services.sh
sleep 2

# Verify ports are free
lsof -i :5432  # Should return empty
lsof -i :6379  # Should return empty

# Start Aspire again
cd backend/Orchestration
dotnet run
```

**If still failing:**
```bash
# Force kill all docker containers
docker rm -f $(docker ps -aq)

# Remove dangling images
docker rmi $(docker images -q --filter "dangling=true")

# Try again
dotnet run --project backend/Orchestration/B2Connect.Orchestration.csproj
```

### Issue 2: Docker Daemon Not Running

**Symptom:**
```
Cannot connect to Docker daemon
Error: Docker is not running
```

**Solution (macOS):**
```bash
# Start Docker Desktop
open /Applications/Docker.app

# Wait for "Docker is running" indicator in menu bar

# Verify
docker ps  # Should show list of containers

# Try Aspire again
cd backend/Orchestration
dotnet run
```

**Solution (Windows):**
```powershell
# Start Docker Desktop (double-click icon or search)
# Wait for "Docker Desktop is running" notification

# PowerShell
docker ps

# Try Aspire again
dotnet run --project backend/Orchestration/B2Connect.Orchestration.csproj
```

### Issue 3: Build Fails with Compiler Errors

**Symptom:**
```
error CS0234: The type or namespace name 'X' does not exist
error CS1704: An assembly named 'Y' has already been imported
```

**Solution:**
```bash
# Clean build cache
rm -rf bin obj  # macOS/Linux
rmdir /s bin obj  # Windows

# Deep clean
dotnet clean B2Connect.slnx

# Restore fresh
dotnet restore B2Connect.slnx

# Build again
dotnet build B2Connect.slnx
```

### Issue 4: Tests Fail After Code Changes

**Symptom:**
```
Test failures after modifying code
Tests pass locally but fail in CI
```

**Solution:**
```bash
# Rebuild test projects
dotnet clean backend/Domain/Catalog/tests/B2Connect.Catalog.Tests.csproj
dotnet build backend/Domain/Catalog/tests/B2Connect.Catalog.Tests.csproj

# Run specific test class
dotnet test backend/Domain/Catalog/tests/B2Connect.Catalog.Tests.csproj \
  --filter "ClassName=ProductServiceTests" -v normal

# Run all tests with verbose output
dotnet test B2Connect.slnx -v normal
```

### Issue 5: Frontend Dependencies Conflict

**Symptom:**
```
npm ERR! peer dep missing
npm ERR! code ERESOLVE
```

**Solution:**
```bash
# Clear npm cache
npm cache clean --force

# Remove node_modules
rm -rf node_modules
rm package-lock.json

# Reinstall
npm install

# If still failing, use legacy peer deps
npm install --legacy-peer-deps
```

### Issue 6: Hot Reload Not Working

**Symptom:**
```
Code changes not reflected in browser
Have to manually refresh
```

**Solution (Backend):**
```bash
# Ensure hot reload is enabled
cd backend/Orchestration
dotnet run  # Should watch for changes

# If not watching, rebuild
dotnet build --no-restore B2Connect.slnx
```

**Solution (Frontend):**
```bash
# Kill current dev server
Ctrl+C

# Restart dev server
cd Frontend/Store
npm run dev

# Check terminal output:
# Should say "ready in XXXms"
# Should show "✓ updates ready"
```

---

## IDE Configuration

### VS Code Setup

**Recommended Extensions**
```json
{
  "C# Dev Kit": "ms-dotnettools.csharp",
  "C#": "ms-dotnettools.csharp",
  "Pylance": "ms-python.vscode-pylance",
  "ES7+ React/Redux/React-Native snippets": "dsznajder.es7-react-js-snippets",
  "Vue - Official": "Vue.ocaiyun.vue",
  "Prettier": "esbenp.prettier-vscode",
  "ESLint": "dbaeumer.vscode-eslint",
  "Docker": "ms-azuretools.vscode-docker",
  "REST Client": "humao.rest-client",
  "GitHub Copilot": "GitHub.copilot"
}
```

**Install Extensions**
```bash
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.vscode-dotnet-runtime
code --install-extension Vue.volar
code --install-extension esbenp.prettier-vscode
code --install-extension dbaeumer.vscode-eslint
code --install-extension ms-azuretools.vscode-docker
code --install-extension GitHub.copilot
```

**VS Code Settings (.vscode/settings.json)**
```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "[csharp]": {
    "editor.defaultFormatter": "ms-dotnettools.csharp",
    "editor.formatOnSave": true
  },
  "[javascript]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "[typescript]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "[vue]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "search.exclude": {
    "**/node_modules": true,
    "**/dist": true,
    "**/bin": true,
    "**/obj": true
  },
  "files.exclude": {
    "**/.git": true
  },
  "[json]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  }
}
```

**Debugging C# in VS Code**
```json
// .vscode/launch.json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Attach to Identity Service",
      "type": "coreclr",
      "request": "attach",
      "processId": "${command:pickProcess}",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/backend/Domain/Identity/src/bin/Debug/net8.0/B2Connect.Identity.dll"
    }
  ]
}
```

### JetBrains Rider (Alternative to VS Code)

**Recommended Setup**
1. Download [Rider](https://www.jetbrains.com/rider/)
2. Install .NET SDK support
3. Open B2Connect.slnx
4. Right-click solution → Tools → Run Inspection

---

## Daily Development Workflow

### Morning Startup

```bash
# 1. Update code
git pull origin main

# 2. Restore dependencies
dotnet restore B2Connect.slnx

# 3. Start Aspire
cd backend/Orchestration
dotnet run
# Wait for "Dashboard running at http://localhost:15500"

# 4. In NEW terminal, start frontends
cd Frontend/Store
npm run dev
# Wait for "ready in XXXms"

# 5. In ANOTHER terminal, start Admin
cd Frontend/Admin
npm run dev

# 6. Open Aspire Dashboard
open http://localhost:15500

# ✅ Now ready to develop
```

### Development Cycle

```bash
# 1. Create feature branch
git checkout -b feature/P0.6-withdrawal

# 2. Make code changes (hot reload automatic)
# 3. Write tests
dotnet test backend/Domain/Catalog/tests -v minimal

# 4. Commit changes
git add .
git commit -m "feat: implement withdrawal right"

# 5. Push to GitHub
git push origin feature/P0.6-withdrawal

# 6. Create pull request
gh pr create --title "feat: implement withdrawal right" \
  --body "Implements VVVG §357 compliance"

# ✅ GitHub Actions run automatically
```

### Shutting Down

```bash
# Stop all services gracefully
Ctrl+C  # In each terminal

# Verify everything stopped
docker ps  # Should show no running containers

# If something stuck, force cleanup
./scripts/kill-all-services.sh
```

### Resuming Work (Next Day)

```bash
# Same as morning startup
git pull origin main
dotnet restore B2Connect.slnx
cd backend/Orchestration
dotnet run

# Check git status
git status  # Should be clean if committed everything
```

---

## CI/CD Integration

### GitHub Actions (Automatic)

When you push code, GitHub Actions automatically:

```
1. Build backend (dotnet build)
2. Build frontend (npm run build)
3. Run unit tests (dotnet test)
4. Run integration tests (TestContainers)
5. Lint code (ESLint, StyleCop)
6. Security scan (SAST)
7. Accessibility check (axe-core)
8. Deploy (if all pass)
```

View results at: https://github.com/HRasch/B2Connect/actions

---

## Next Steps

1. ✅ Complete setup using steps above
2. ✅ Verify all services running (checklist)
3. ✅ Run unit tests: `dotnet test B2Connect.slnx`
4. ✅ Make code change and test hot reload
5. ✅ Read [Backend Developer Quick Start](./docs/by-role/BACKEND_DEVELOPER.md)
6. ✅ Read [Frontend Developer Quick Start](./docs/by-role/FRONTEND_DEVELOPER.md)
7. ✅ Join team Slack #development channel

---

**Last Updated:** 28. Dezember 2025  
**Support:** Ask in #development Slack channel or create GitHub issue  
**Next Review:** 15. Januar 2026
