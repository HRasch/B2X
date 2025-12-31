# Getting Started with B2Connect

Welcome! This guide gets you up and running in **5 minutes**.

## Prerequisites
- **Windows, macOS, or Linux** with WSL2
- **.NET 10 SDK** ([download](https://dotnet.microsoft.com/download/dotnet/10.0))
- **Node.js 20+** ([download](https://nodejs.org/))
- **Git** ([download](https://git-scm.com/))
- **VS Code** with recommended extensions (auto-install on first open)

## ⚡ Quick Start (5 minutes)

### 1. Clone & Install Dependencies
```bash
git clone <repo-url> B2Connect
cd B2Connect
dotnet restore
npm install --prefix frontend
npm install --prefix frontend-admin
```

### 2. Start Backend (Aspire Orchestration)
```bash
cd AppHost
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

Aspire dashboard opens at http://localhost:15500

### 3. Start Frontend (In New Terminal)
```bash
cd Frontend/Store
npm install
npm run dev
```

Services available at:
- 📊 **Aspire Dashboard**: http://localhost:15500
- 🛒 **Store Gateway**: http://localhost:6000
- 🔧 **Admin Gateway**: http://localhost:6100
- 🎨 **Frontend Store**: http://localhost:5173
- 🔐 **Frontend Admin**: http://localhost:5174

## 📚 Next Steps

**New to the project?**
→ Read [DEVELOPMENT.md](DEVELOPMENT.md) for architecture overview

**Need to debug something?**
→ See [docs/guides/DEBUG_QUICK_REFERENCE.md](docs/guides/DEBUG_QUICK_REFERENCE.md)

**Want to understand architecture?**
→ See [DDD Bounded Contexts](../../.ai/knowledgebase/architecture/DDD_BOUNDED_CONTEXTS_REFERENCE.md)

**Need Wolverine patterns?**
→ See [Wolverine Pattern Reference](../../.ai/knowledgebase/architecture/WOLVERINE_PATTERN_REFERENCE.md)

**Running tests?**
→ See [docs/guides/TESTING_GUIDE.md](docs/guides/TESTING_GUIDE.md)

## 🛠️ Development Tasks

### Build Backend
```bash
dotnet build backend/B2Connect.slnx
```

### Test Backend
```bash
dotnet test backend/B2Connect.slnx
# Or from VS Code: Press F5 → "Run Backend Tests"
```

### Start Frontend Dev Server
```bash
npm run dev --prefix frontend
```

### Build for Production
```bash
dotnet publish -c Release
npm run build --prefix frontend
```

## 📖 Key Documentation

| Document | Purpose |
|----------|---------|
| [README.md](README.md) | Project overview & architecture |
| [DEVELOPMENT.md](DEVELOPMENT.md) | Development workflow & guidelines |
| [.copilot-specs.md](.copilot-specs.md) | Coding standards & patterns (24 sections) |
| [docs/architecture/ASPIRE_GUIDE.md](docs/architecture/ASPIRE_GUIDE.md) | Aspire setup & microservices |
| [docs/guides/TESTING_GUIDE.md](docs/guides/TESTING_GUIDE.md) | Testing approach (unit/integration/E2E) |
| [docs/features/](docs/features/) | Feature implementations (one file per feature) |

## 🐛 Troubleshooting

**Port Already in Use?**
```bash
# Kill process on port 9000 (example)
lsof -i :9000 | grep LISTEN | awk '{print $2}' | xargs kill -9
```

**Dependencies Not Installing?**
```bash
dotnet clean backend/B2Connect.slnx
dotnet restore backend/B2Connect.slnx
```

**Frontend Not Loading?**
```bash
rm -rf frontend/node_modules
npm install --prefix frontend
npm run dev --prefix frontend
```

For more help, see [BUSINESS_REQUIREMENTS.md](BUSINESS_REQUIREMENTS.md) or check the feature docs in [docs/features/](docs/features/).

---

**Questions?** Check [docs/guides/DEBUG_QUICK_REFERENCE.md](docs/guides/DEBUG_QUICK_REFERENCE.md) or the feature docs.
