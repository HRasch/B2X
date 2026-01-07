# 🔄 Brainstorm: Rename B2X → B2X

**DocID**: `BS-RENAME-001`  
**Created**: 7. Januar 2026  
**Owner**: @SARAH  
**Status**: Brainstorm / Planning

---

## 📊 Executive Summary

Renaming **B2X** to **B2X** is a comprehensive refactoring task affecting:

| Category | Estimated Count |
|----------|-----------------|
| C# Project Files (`.csproj`) | ~82 files |
| C# Namespaces | ~500+ files |
| Solution File (`.slnx`) | 1 file |
| TypeScript/Vue/JS Files | ~50+ files |
| Docker/K8s Configs | ~20+ files |
| Documentation (`.md`) | ~100+ files |
| Scripts (`.sh`, `.ps1`) | ~30+ files |
| Configuration Files | ~20+ files |
| MCP Servers | 12+ packages |
| Git Repository | Rename folder |

**Estimated Total Changes**: 800+ file modifications

---

## 🎯 Scope Analysis

### 1. Backend (.NET/C#)

#### 1.1 Solution & Project Files
- `B2X.slnx` → `B2X.slnx`
- All 82 `.csproj` files containing `B2X`
- `Directory.Build.props` and `Directory.Packages.props`

#### 1.2 Namespaces (CRITICAL)
All namespaces starting with `B2X.` must change to `B2X.`:
```csharp
// Before
namespace B2X.Catalog.Domain.Models;

// After
namespace B2X.Catalog.Domain.Models;
```

**Affected Namespace Patterns**:
- `B2X.Admin.*`
- `B2X.Store.*`
- `B2X.Catalog.*`
- `B2X.CMS.*`
- `B2X.Customer.*`
- `B2X.Domain.*`
- `B2X.Email.*`
- `B2X.ERP.*`
- `B2X.Gateway.*`
- `B2X.Identity.*`
- `B2X.Legal.*`
- `B2X.Localization.*`
- `B2X.Orders.*`
- `B2X.PatternAnalysis.*`
- `B2X.Returns.*`
- `B2X.SearchService.*`
- `B2X.ServiceDefaults.*`
- `B2X.Shared.*`
- `B2X.SmartDataIntegration.*`
- `B2X.Tenancy.*`
- `B2X.Theming.*`
- `B2X.Tools.*`
- `B2X.Utils.*`
- `B2X.CLI.*`
- `B2X.ErpConnector.*`

#### 1.3 Assembly Names
All `<AssemblyName>` and `<RootNamespace>` tags in `.csproj` files.

#### 1.4 Using Statements
All `using B2X.*;` statements across the codebase.

### 2. Frontend (TypeScript/Vue/JavaScript)

#### 2.1 Package Names
- `package.json` names in:
  - `frontend/Store/`
  - `frontend/Admin/`
  - `frontend/Management/`
  
#### 2.2 References
- API client configurations
- i18n keys (if branded)
- Comments and documentation strings

### 3. MCP Servers (tools/)

| Server | Changes Required |
|--------|-----------------|
| B2XMCP | Complete rename → B2XGateMCP |
| RoslynMCP | Namespace references |
| WolverineMCP | Namespace references, solution path |
| VueMCP | Package description |
| TypeScriptMCP | Package description |
| SecurityMCP | Package description |
| DockerMCP | Author field |
| GitMCP | Author field |
| DatabaseMCP | Package description |
| PerformanceMCP | Package description |
| HtmlCssMCP | Package description |
| MonitoringMCP | Package description |
| DocumentationMCP | Package description |
| PlaywrightMCP | Package description |

### 4. Docker & Kubernetes

#### 4.1 Docker
- `docker-compose.yml` - container names, image names
- `Dockerfile` files - labels, comments
- Container names: `B2X-*` → `b2xgate-*`
- Image names: `B2X/*` → `b2xgate/*`
- Volume names: `B2X-*` → `b2xgate-*`
- Network names: `B2X-*` → `b2xgate-*`

#### 4.2 Kubernetes
- Namespace: `B2X` → `b2xgate`
- Service labels
- ConfigMaps
- Deployments

### 5. Scripts

All shell scripts containing `B2X` or `B2X`:
- `scripts/aspire-start.sh`
- `scripts/aspire-stop.sh`
- `scripts/check-ports.sh`
- `scripts/deployment-status.sh`
- `scripts/B2X-heartbeat.service` → `b2xgate-heartbeat.service`
- `scripts/B2X-heartbeat.timer` → `b2xgate-heartbeat.timer`
- `scripts/cleanup-cdp.sh`
- And more...

### 6. Documentation

- All `.md` files in `.ai/`
- All `.md` files in `docs/`
- README files throughout
- ADR documents
- Knowledgebase articles
- Agent definitions
- Instruction files

### 7. Configuration Files

- `.vscode/tasks.json`
- `.vscode/mcp.json`
- `appsettings.*.json`
- `stylecop.json` (company name if applicable)

### 8. Git Repository

- Folder rename: `B2X/` → `B2XGate/`
- Update all local clones
- Update CI/CD pipelines
- Update GitHub repository name (if applicable)

---

## 📋 Phased Execution Plan

### Phase 1: Preparation (Day 1)
**Owner**: @SARAH, @Architect

1. **Create comprehensive backup**
   - Git branch: `feature/rename-B2X-to-b2xgate`
   - Export current state

2. **Generate complete file inventory**
   - List all files containing `B2X` (case-insensitive)
   - Categorize by type and priority
   - Create verification checklist

3. **Prepare rename scripts**
   - PowerShell script for Windows
   - Bash script for CI/CD
   - Include rollback capability

4. **Notify team**
   - All agents aware of upcoming changes
   - Freeze other development temporarily

### Phase 2: Backend Core (Day 2-3)
**Owner**: @Backend, @Architect

**Order of execution**:

1. **Solution file** (`.slnx`)
   ```xml
   B2X.slnx → B2XGate.slnx
   ```

2. **Project files** (`.csproj`)
   - Update file names
   - Update `<RootNamespace>`
   - Update `<AssemblyName>`
   - Update `<ProjectReference>` paths

3. **Directory.*.props**
   - Update any hardcoded references

4. **Namespace refactoring**
   - Use IDE refactoring tools (Rider/VS)
   - Batch processing with Roslyn
   - Verify builds after each domain

5. **Using statements**
   - Automatic update via namespace refactoring

### Phase 3: Shared & Infrastructure (Day 3-4)
**Owner**: @Backend, @DevOps

1. **ServiceDefaults**
   - Namespace changes
   - OTEL service name configuration

2. **Shared libraries**
   - All `B2X.Shared.*` projects

3. **CLI projects**
   - `B2X.CLI.*` → `B2XGate.CLI.*`

4. **ERP Connector**
   - Standalone solution update
   - Namespace changes

### Phase 4: MCP Servers (Day 4)
**Owner**: @CopilotExpert, @Backend

1. **B2XMCP → B2XGateMCP**
   - Full rename (folder, package, code)

2. **All other MCP servers**
   - Update package.json descriptions
   - Update README references
   - Update namespace lookups in Roslyn/Wolverine MCP

### Phase 5: Frontend (Day 5)
**Owner**: @Frontend

1. **Package.json updates**
   - All three frontend projects

2. **Configuration files**
   - API references if branded

3. **Code comments**
   - Search and replace in `.ts`, `.vue`, `.js`

### Phase 6: Docker & Kubernetes (Day 5-6)
**Owner**: @DevOps

1. **Docker Compose**
   - Container names
   - Image names
   - Volume names
   - Network names

2. **Dockerfiles**
   - Labels
   - Comments

3. **Kubernetes manifests**
   - Namespace
   - Labels
   - ConfigMaps
   - Deployments

### Phase 7: Scripts & Config (Day 6)
**Owner**: @DevOps

1. **Shell scripts**
   - Content updates
   - File renames where applicable

2. **VS Code tasks**
   - `.vscode/tasks.json`
   - `.vscode/mcp.json`

3. **AppSettings**
   - Any branded references

### Phase 8: Documentation (Day 7)
**Owner**: @DocMaintainer

1. **AI documentation**
   - `.ai/` folder
   - ADRs, guidelines, KB articles

2. **Project documentation**
   - `docs/` folder
   - READMEs throughout

3. **GitHub files**
   - `.github/` folder
   - Agent definitions
   - Instruction files

### Phase 9: Verification & Testing (Day 8)
**Owner**: @QA, @TechLead

1. **Build verification**
   ```bash
   dotnet build B2XGate.slnx
   ```

2. **Test execution**
   ```bash
   dotnet test B2XGate.slnx
   ```

3. **Frontend builds**
   ```bash
   npm run build  # in each frontend folder
   ```

4. **Docker builds**
   ```bash
   docker-compose build
   ```

5. **Integration tests**
   - Full E2E validation

6. **Search for remaining references**
   ```bash
   grep -ri "B2X" --include="*.cs" --include="*.ts" --include="*.json"
   ```

### Phase 10: Repository Migration (Day 9)
**Owner**: @SARAH, @DevOps

1. **Rename repository folder**
2. **Update remote URLs if needed**
3. **Update CI/CD pipelines**
4. **Update any external integrations**
5. **Merge to main branch**

---

## 🛠️ Tools & Scripts

### Automated Rename Script (PowerShell)

```powershell
# B2X-to-B2XGate-rename.ps1
# Phase 1: File content replacement
# Phase 2: File/folder renaming
# Phase 3: Verification

$oldName = "B2X"
$newName = "B2XGate"
$oldNameLower = "B2X"
$newNameLower = "b2xgate"

# File content patterns to replace
$patterns = @(
    @{old="B2X"; new="B2XGate"},
    @{old="B2X"; new="b2xgate"},
    @{old="B2X"; new="B2XGATE"}
)

# File extensions to process
$extensions = @(
    "*.cs", "*.csproj", "*.slnx", "*.props",
    "*.ts", "*.vue", "*.js", "*.json",
    "*.md", "*.yml", "*.yaml",
    "*.sh", "*.ps1", "*.service", "*.timer"
)
```

### Roslyn-Based Namespace Refactoring

```csharp
// Use dotnet-format or custom Roslyn tool
// for bulk namespace updates
```

### Verification Script

```bash
#!/bin/bash
# verify-rename.sh
echo "Checking for remaining B2X references..."
find . -type f \( -name "*.cs" -o -name "*.csproj" -o -name "*.json" \) \
    -exec grep -l -i "B2X" {} \;
```

---

## ⚠️ Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Build failures | High | Incremental builds after each phase |
| Test failures | High | Run tests after each domain rename |
| NuGet package issues | Medium | Update all package references together |
| Docker image references | Medium | Update all compose files before builds |
| External integrations | High | Inventory all external dependencies first |
| Git history | Low | Use single PR with good commit structure |
| Developer confusion | Medium | Clear communication and documentation |

---

## ✅ Success Criteria

- [ ] `dotnet build B2XGate.slnx` succeeds
- [ ] `dotnet test B2XGate.slnx` all tests pass
- [ ] All frontend projects build
- [ ] Docker compose up works
- [ ] No remaining `B2X` references in code
- [ ] Documentation updated
- [ ] CI/CD pipelines functional

---

## 📅 Timeline Estimate

| Phase | Duration | Dependencies |
|-------|----------|--------------|
| Preparation | 1 day | None |
| Backend Core | 2 days | Preparation |
| Shared/Infra | 2 days | Backend Core |
| MCP Servers | 1 day | Backend Core |
| Frontend | 1 day | None (parallel) |
| Docker/K8s | 1 day | Backend Core |
| Scripts/Config | 0.5 days | Backend Core |
| Documentation | 1 day | All code changes |
| Verification | 1 day | All phases |
| Migration | 0.5 days | Verification |

**Total Estimated Time**: 8-10 working days

---

## 👥 Agent Assignments

| Agent | Responsibilities |
|-------|------------------|
| @SARAH | Coordination, planning, quality gate |
| @Architect | Technical decisions, namespace strategy |
| @Backend | C# code refactoring |
| @Frontend | TypeScript/Vue refactoring |
| @DevOps | Docker, K8s, scripts, CI/CD |
| @CopilotExpert | MCP servers, agent/prompt files |
| @DocMaintainer | Documentation updates |
| @QA | Testing, verification |
| @TechLead | Code review, quality validation |

---

## 📝 Decision Required

### Options:

**Option A: Big Bang**
- Complete rename in single sprint
- Freeze other development
- Pros: Clean break, no partial states
- Cons: High risk, requires full team focus

**Option B: Incremental (Recommended)**
- Phase by phase over 2 weeks
- Allow parallel development with caution
- Pros: Lower risk, easier rollback
- Cons: Temporary inconsistency

**Option C: Parallel Codebase**
- Create B2XGate as new repo
- Migrate features incrementally
- Pros: Zero downtime, safe
- Cons: Double maintenance during transition

### Recommendation

**Option B (Incremental)** with feature freeze on affected areas during each phase.

---

## 🔗 Related Documents

- [ADR-TBD] Rename Decision Record (to be created)
- [GL-004] Branch Naming Strategy
- [WF-003] Dependency Upgrade Workflow (similar process)

---

**Next Steps**:
1. ✅ Review this brainstorm document
2. ⏳ Create ADR for rename decision
3. ⏳ Get @Architect and @TechLead approval
4. ⏳ Create detailed task breakdown in GitHub Issues
5. ⏳ Schedule execution window

---

**Maintained by**: @SARAH  
**Last Updated**: 7. Januar 2026
