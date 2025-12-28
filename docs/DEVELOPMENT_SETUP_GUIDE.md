# Development Setup & Getting Started Guide

**Version:** 1.0 | **Last Updated:** 28. Dezember 2025 | **Target:** macOS/Linux/Windows WSL

---

## ⚡ Quick Setup (5 minutes)

### Prerequisites
- ✅ **Git** (v2.40+)
- ✅ **.NET 10 SDK** ([download](https://dotnet.microsoft.com/download/dotnet/10.0))
- ✅ **Node.js 20+** ([download](https://nodejs.org))
- ✅ **VS Code** with extensions installed
- ✅ **GitHub CLI** (`brew install gh` or [download](https://cli.github.com))

### One-Time Setup

```bash
# 1. Clone repository
git clone https://github.com/HRasch/B2Connect.git
cd B2Connect

# 2. Install .NET dependencies
dotnet restore

# 3. Install Frontend dependencies
cd Frontend/Store && npm install && cd ../..
cd Frontend/Admin && npm install && cd ../..

# 4. Configure local secrets (development only)
dotnet user-secrets init -p backend/Domain/Identity/src/B2Connect.Identity.csproj
dotnet user-secrets set -p backend/Domain/Identity/src/B2Connect.Identity.csproj "Jwt:Secret" "your-dev-secret-min-32-chars-12345"

# 5. Build everything
dotnet build B2Connect.slnx

# 6. Run tests
dotnet test B2Connect.slnx -v minimal

# ✅ Ready to develop!
```

---

## 🚀 Running the Application

### Option 1: Full Stack (Recommended for Development)

```bash
# Start Aspire orchestration (all services + dashboard)
dotnet run --project backend/Orchestration/B2Connect.Orchestration.csproj

# Opens: http://localhost:15500 (Aspire Dashboard)
# Services auto-start:
#   - Identity Service (7002)
#   - Catalog Service (7005)
#   - CMS Service (7006)
#   - Frontend Store (5173)
#   - Frontend Admin (5174)
```

### Option 2: Services Only (No Frontend)

```bash
# Start backend services only
cd backend/Orchestration
dotnet run
```

### Option 3: Frontend Only (API mocked)

```bash
# Terminal 1: Start Store frontend
cd Frontend/Store
npm run dev  # http://localhost:5173

# Terminal 2: Start Admin frontend
cd Frontend/Admin
npm run dev  # http://localhost:5174
```

### Option 4: Individual Service (Debug)

```bash
# Identity Service (standalone)
dotnet run --project backend/Domain/Identity/src/B2Connect.Identity.csproj
# http://localhost:7002

# Catalog Service (standalone)
dotnet run --project backend/Domain/Catalog/src/B2Connect.Catalog.csproj
# http://localhost:7005

# etc.
```

---

## 🛠️ Common Development Tasks

### Build & Test

```bash
# Build all
dotnet build B2Connect.slnx

# Build specific service
dotnet build backend/Domain/Identity/src/B2Connect.Identity.csproj

# Run all tests
dotnet test B2Connect.slnx -v minimal

# Run specific test suite
dotnet test backend/Domain/Identity/tests/B2Connect.Identity.Tests.csproj -v minimal

# Run with coverage
dotnet test B2Connect.slnx /p:CollectCoverage=true /p:CoverageFormat=opencover

# Run specific test
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"
```

### Database

```bash
# Show current migrations
dotnet ef migrations list -p backend/Domain/Identity/src/B2Connect.Identity.csproj

# Create migration
dotnet ef migrations add "AddNewTable" -p backend/Domain/Identity/src/B2Connect.Identity.csproj

# Update database
dotnet ef database update -p backend/Domain/Identity/src/B2Connect.Identity.csproj

# Remove last migration
dotnet ef migrations remove -p backend/Domain/Identity/src/B2Connect.Identity.csproj

# Drop database (careful!)
dotnet ef database drop -p backend/Domain/Identity/src/B2Connect.Identity.csproj --force
```

### Code Quality

```bash
# Format code
dotnet format B2Connect.slnx

# Run linter (if configured)
npm run lint -w Frontend/Store
npm run lint -w Frontend/Admin

# Check for unused imports (Roslyn analyzer)
dotnet build B2Connect.slnx /p:EnforceCodeStyleInBuild=true
```

### Git Workflow

```bash
# Create feature branch (issue #123)
git checkout -b feature/us-123-description

# Make changes
# Test changes
# Commit
git add .
git commit -m "feat(identity): implement JWT token refresh (#123)"

# Push
git push origin feature/us-123-description

# Create PR
gh pr create --title "feat: JWT token refresh" \
  --body "Closes #123" \
  --assignee @me

# Check PR status
gh pr view

# Get code review feedback...
# Make changes...
# Commit and push (PR auto-updates)

# After approval, merge
gh pr merge --squash
```

---

## 📝 Folder Structure Quick Reference

```
B2Connect/
├── backend/                         # All backend code
│   ├── Domain/                      # Individual microservices
│   │   ├── Identity/                # Authentication service
│   │   ├── Catalog/                 # Product catalog
│   │   ├── CMS/                     # Content management
│   │   └── [other services]         # More services...
│   ├── BoundedContexts/             # DDD bounded contexts
│   ├── Orchestration/               # Aspire orchestration
│   ├── ServiceDefaults/             # Shared configuration
│   └── shared/                      # Shared libraries
│
├── Frontend/                        # Frontend applications
│   ├── Store/                       # Public storefront (Vue.js)
│   ├── Admin/                       # Admin panel (Vue.js)
│   └── Management/                  # Management app
│
├── docs/                            # Documentation
│   ├── architecture/                # Architecture guides
│   ├── compliance/                  # Compliance specs
│   ├── guides/                      # Developer guides
│   └── by-role/                     # Role-specific docs
│
├── scripts/                         # Automation scripts
│   ├── create-p0-issues.sh          # Create P0 issues
│   ├── kill-all-services.sh         # Kill all services
│   └── [other scripts]
│
└── .github/                         # GitHub configuration
    ├── workflows/                   # GitHub Actions
    └── copilot-instructions.md      # AI instructions
```

---

## 🔧 VS Code Setup

### Recommended Extensions

```json
{
  "recommendations": [
    "ms-dotnettools.csharp",              // C# intellisense
    "ms-dotnettools.vscode-dotnet-runtime",
    "eamodio.gitlens",                    // Git history
    "ms-vscode.makefile-tools",           // Build tasks
    "Vue.volar",                          // Vue 3 support
    "esbenp.prettier-vscode",             // Code formatter
    "dbaeumer.vscode-eslint",             // Linting
    "GitHub.copilot",                     // AI assistance
    "GitHub.github-vscode-theme"          // GitHub theme
  ]
}
```

### Recommended Settings (`.vscode/settings.json`)

```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "[csharp]": {
    "editor.defaultFormatter": "ms-dotnettools.csharp"
  },
  "[json]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "files.exclude": {
    "**/bin": true,
    "**/obj": true,
    "**/.DS_Store": true
  },
  "csharp.inlayHints.enableInlayHintsForParameters": true,
  "csharp.inlayHints.enableInlayHintsForLiteralParameters": false,
  "dotnet.liveShareExtensionUriScheme": "vsls"
}
```

### Useful Keybindings

```json
[
  {
    "key": "cmd+shift+t",
    "command": "workbench.action.tasks.runTask",
    "args": "test-backend"
  },
  {
    "key": "cmd+shift+b",
    "command": "workbench.action.tasks.runTask",
    "args": "build-backend"
  },
  {
    "key": "cmd+shift+d",
    "command": "workbench.action.tasks.runTask",
    "args": "backend-start"
  }
]
```

---

## 🐛 Troubleshooting

### Issue: Build Fails - "Could not find dotnet"

```bash
# Solution: Check .NET installation
dotnet --version
# If not found, install from https://dotnet.microsoft.com/download/dotnet/10.0

# Verify PATH
which dotnet
```

### Issue: Tests Fail - "Database connection failed"

```bash
# Solution: Use InMemory database for tests
export Database__Provider=inmemory
dotnet test
```

### Issue: Frontend Port Already in Use

```bash
# Solution: Kill existing process
# macOS/Linux:
lsof -i :5173        # Check what's using port
kill -9 <PID>        # Kill the process

# Or use different port:
npm run dev -- --port 5175
```

### Issue: Aspire Dashboard Not Loading

```bash
# Solution: Check if Aspire is running
ps aux | grep dotnet  # Look for Orchestration process

# Restart Aspire
./scripts/kill-all-services.sh
dotnet run --project backend/Orchestration/B2Connect.Orchestration.csproj
```

### Issue: Git - "Authentication failed"

```bash
# Solution: Configure GitHub CLI
gh auth login
# Follow prompts to authenticate

# Verify
gh auth status
```

---

## 📚 Learning Resources

### Architecture
- **DDD & Bounded Contexts**: [docs/architecture/DDD_BOUNDED_CONTEXTS.md](../docs/architecture/DDD_BOUNDED_CONTEXTS.md)
- **Onion Architecture**: [docs/ONION_ARCHITECTURE.md](../docs/ONION_ARCHITECTURE.md)
- **Wolverine Patterns**: [.github/copilot-instructions.md](../../.github/copilot-instructions.md#wolverine-http-handlers)

### Coding Standards
- **C# Best Practices**: [.github/copilot-instructions.md](../../.github/copilot-instructions.md#net-10--c-14-best-practices)
- **Frontend Guidelines**: [.github/copilot-instructions.md](../../.github/copilot-instructions.md#frontend-development-best-practices)

### Compliance
- **Compliance Checklist**: [docs/COMPLIANCE_IMPLEMENTATION_CHECKLIST.md](../docs/COMPLIANCE_IMPLEMENTATION_CHECKLIST.md)
- **Architecture Quick Start**: [docs/ARCHITECTURE_QUICK_START.md](../docs/ARCHITECTURE_QUICK_START.md)

### Project Management
- **GitHub Projects Guide**: [GITHUB_PROJECT_MANAGEMENT_GUIDE.md](../GITHUB_PROJECT_MANAGEMENT_GUIDE.md)
- **Creating Issues**: [GITHUB_ISSUE_CREATION_GUIDE.md](../GITHUB_ISSUE_CREATION_GUIDE.md)

---

## 🎯 Your First Feature (10-Minute Setup)

### Step 1: Pick an Issue
```bash
# View issues ready to work on
gh issue list --state open --label "ready"

# Assign to yourself
gh issue edit 123 --assignee @me
```

### Step 2: Create Branch
```bash
# Create feature branch
git checkout -b feature/us-123-short-description

# Verify you're on correct branch
git branch  # Should show * feature/us-123-...
```

### Step 3: Code
```bash
# Example: Add a new endpoint to Identity service
# 1. Create handler in Domain/Identity/src/Handlers/
# 2. Add tests in Domain/Identity/tests/
# 3. Build and test

dotnet build backend/Domain/Identity/src/B2Connect.Identity.csproj
dotnet test backend/Domain/Identity/tests/B2Connect.Identity.Tests.csproj
```

### Step 4: Commit & Push
```bash
# Commit with clear message
git add .
git commit -m "feat(identity): add new endpoint (#123)"

# Push
git push origin feature/us-123-short-description
```

### Step 5: Create PR
```bash
# Create pull request
gh pr create \
  --title "feat(identity): add new endpoint" \
  --body "Closes #123

## Changes
- New endpoint for XYZ
- Tests added

## Testing
- [ ] Manual test on localhost
- [x] Unit tests passing
- [x] Integration tests passing" \
  --assignee @me \
  --label "backend,ready-for-review"

# View your PR
gh pr view
```

### Step 6: Code Review & Merge
```bash
# Wait for review feedback...
# Make changes if needed
git add .
git commit -m "fix: address review feedback"
git push

# After approval, merge
gh pr merge --squash  # Squash commits

# Delete local branch
git branch -d feature/us-123-short-description
```

---

## ✅ Definition of Proper Setup

You're ready to develop when:

- [ ] ✅ `dotnet build` succeeds without warnings
- [ ] ✅ `dotnet test` passes all tests
- [ ] ✅ `dotnet run` starts all services
- [ ] ✅ http://localhost:15500 (Aspire) loads
- [ ] ✅ `gh auth status` shows you're authenticated
- [ ] ✅ `npm run dev` starts frontend without errors
- [ ] ✅ VS Code extensions installed and working

---

**Need help?** 
- Ask in team Slack: `#b2connect-dev`
- Check [docs](../docs/) folder for specific guides
- Review [architecture documentation](../docs/architecture/)

**Happy coding! 🚀**
