# GitHub Issues for B2Connect → B2X Rename

**Created**: 2026-01-07  
**Reference**: [ADR-051] Rename B2Connect to B2X

Copy each issue below to create in GitHub Issues.

---

## Issue 1: [Rename] Phase 1: Preparation & Backup

**Labels**: `rename`, `phase-1`, `infrastructure`  
**Assignee**: @SARAH

### Description

Phase 1 of the B2Connect → B2X rename initiative.

**Owner**: @SARAH  
**Duration**: 1 day  
**Dependencies**: None

### Tasks

- [ ] Create feature branch `feature/rename-b2connect-to-b2x`
- [ ] Generate complete file inventory (all files containing B2Connect)
- [ ] Categorize files by type and priority
- [ ] Create verification checklist
- [ ] Prepare PowerShell rename script with rollback capability
- [ ] Prepare Bash rename script for CI/CD
- [ ] Notify team of upcoming changes
- [ ] Document current state for rollback

### Acceptance Criteria

- [ ] Complete inventory of 800+ files documented
- [ ] Rename scripts tested on sample files
- [ ] Team notified and development freeze communicated
- [ ] Backup/rollback procedure documented

### References

- [ADR-051] Rename B2Connect to B2X
- [BS-RENAME-001] Brainstorm Document

---

## Issue 2: [Rename] Phase 2: Backend Core (Solution, Projects, Namespaces)

**Labels**: `rename`, `phase-2`, `backend`  
**Assignee**: @Backend

### Description

Phase 2: Rename all backend core files including solution, projects, and namespaces.

**Owner**: @Backend, @Architect  
**Duration**: 2 days  
**Dependencies**: Phase 1

### Tasks

- [ ] Rename `B2Connect.slnx` → `B2X.slnx`
- [ ] Update all ~82 `.csproj` file names
- [ ] Update `<RootNamespace>` in all projects
- [ ] Update `<AssemblyName>` in all projects
- [ ] Update `<ProjectReference>` paths
- [ ] Refactor all `namespace B2Connect.*` → `namespace B2X.*`
- [ ] Update all `using B2Connect.*` statements
- [ ] Verify build succeeds after each domain

### Affected Projects (Sample)

- B2Connect.Admin.csproj
- B2Connect.Store.csproj
- B2Connect.Catalog.API.csproj
- B2Connect.CMS.csproj
- B2Connect.Identity.API.csproj
- ... (~82 total)

### Acceptance Criteria

- [ ] `dotnet build B2X.slnx` succeeds
- [ ] All namespace references updated
- [ ] No B2Connect references in .cs files

### References

- [ADR-051] Rename B2Connect to B2XGate
- Depends on: Issue #1 (Phase 1)

---

## Issue 3: [Rename] Phase 3: Shared & Infrastructure Libraries

**Labels**: `rename`, `phase-3`, `backend`, `infrastructure`  
**Assignee**: @Backend, @DevOps

### Description

Phase 3: Rename shared libraries and infrastructure components.

**Owner**: @Backend, @DevOps  
**Duration**: 2 days  
**Dependencies**: Phase 2

### Tasks

- [ ] Rename ServiceDefaults project and namespace
- [ ] Update OTEL service name configuration
- [ ] Rename all `B2Connect.Shared.*` projects
- [ ] Rename CLI projects (`B2Connect.CLI.*` → `B2XGate.CLI.*`)
- [ ] Update ERP Connector solution and projects
- [ ] Update all shared library namespaces

### Affected Projects

- B2Connect.ServiceDefaults
- B2Connect.Shared.Core
- B2Connect.Shared.Infrastructure
- B2Connect.Shared.Messaging
- B2Connect.Shared.Search
- B2Connect.Shared.Kernel
- B2Connect.CLI.*
- B2Connect.ErpConnector

### Acceptance Criteria

- [ ] All shared libraries build
- [ ] CLI tools functional
- [ ] ERP Connector builds and tests pass

### References

- [ADR-051] Rename B2Connect to B2X
- Depends on: Issue #2 (Phase 2)

---

## Issue 4: [Rename] Phase 4: MCP Servers

**Labels**: `rename`, `phase-4`, `mcp`, `tooling`  
**Assignee**: @CopilotExpert

### Description

Phase 4: Rename all MCP server packages and references.

**Owner**: @CopilotExpert, @Backend  
**Duration**: 1 day  
**Dependencies**: Phase 2

### Tasks

- [ ] Rename B2ConnectMCP → B2XGateMCP (folder, package, code)
- [ ] Update RoslynMCP namespace references
- [ ] Update WolverineMCP namespace references and solution path
- [ ] Update all MCP package.json descriptions
- [ ] Update all MCP README files
- [ ] Update `.vscode/mcp.json` configuration

### Affected MCP Servers

- B2ConnectMCP → B2XGateMCP
- RoslynMCP (namespace refs)
- WolverineMCP (namespace refs, solution path)
- VueMCP (description)
- TypeScriptMCP (description)
- SecurityMCP (description)
- All other MCP servers

### Acceptance Criteria

- [ ] All MCP servers start successfully
- [ ] MCP tools functional in VS Code
- [ ] No B2Connect references in MCP code

### References

- [ADR-051] Rename B2Connect to B2X
- Depends on: Issue #2 (Phase 2)

---

## Issue 5: [Rename] Phase 5: Frontend Packages

**Labels**: `rename`, `phase-5`, `frontend`  
**Assignee**: @Frontend

### Description

Phase 5: Rename frontend packages and references.

**Owner**: @Frontend  
**Duration**: 1 day  
**Dependencies**: None (can run parallel)

### Tasks

- [ ] Update `frontend/Store/package.json` name
- [ ] Update `frontend/Admin/package.json` name
- [ ] Update `frontend/Management/package.json` name
- [ ] Update any branded API references
- [ ] Update code comments containing B2Connect
- [ ] Run `npm install` in all projects

### Acceptance Criteria

- [ ] All frontend projects build (`npm run build`)
- [ ] No B2Connect references in frontend code
- [ ] Linting passes

### References

- [ADR-051] Rename B2Connect to B2X

---

## Issue 6: [Rename] Phase 6: Docker & Kubernetes

**Labels**: `rename`, `phase-6`, `devops`, `infrastructure`  
**Assignee**: @DevOps

### Description

Phase 6: Rename Docker and Kubernetes configurations.

**Owner**: @DevOps  
**Duration**: 1 day  
**Dependencies**: Phase 2

### Tasks

- [ ] Update `docker-compose.yml` (container names, images, volumes, networks)
- [ ] Update all Dockerfiles (labels, comments)
- [ ] Rename container names: `b2connect-*` → `b2xgate-*`
- [ ] Rename image names: `b2connect/*` → `b2xgate/*`
- [ ] Rename volume names: `b2connect-*` → `b2xgate-*`
- [ ] Rename network names: `b2connect-*` → `b2xgate-*`
- [ ] Update Kubernetes namespace: `b2connect` → `b2xgate`
- [ ] Update K8s service labels, ConfigMaps, Deployments

### Acceptance Criteria

- [ ] `docker-compose up` works
- [ ] All containers start with new names
- [ ] K8s manifests valid

### References

- [ADR-051] Rename B2Connect to B2X
- Depends on: Issue #2 (Phase 2)

---

## Issue 7: [Rename] Phase 7: Scripts & Configuration

**Labels**: `rename`, `phase-7`, `devops`, `configuration`  
**Assignee**: @DevOps

### Description

Phase 7: Update all scripts and configuration files.

**Owner**: @DevOps  
**Duration**: 0.5 days  
**Dependencies**: Phase 2

### Tasks

- [ ] Update all shell scripts in `scripts/`
- [ ] Rename `b2connect-heartbeat.service` → `b2xgate-heartbeat.service`
- [ ] Rename `b2connect-heartbeat.timer` → `b2xgate-heartbeat.timer`
- [ ] Update `.vscode/tasks.json`
- [ ] Update `appsettings.*.json` branded references
- [ ] Update `stylecop.json` if company name referenced

### Acceptance Criteria

- [ ] All scripts execute successfully
- [ ] VS Code tasks work
- [ ] No B2Connect references in scripts

### References

- [ADR-051] Rename B2Connect to B2X
- Depends on: Issue #2 (Phase 2)

---

## Issue 8: [Rename] Phase 8: Documentation

**Labels**: `rename`, `phase-8`, `documentation`  
**Assignee**: @DocMaintainer

### Description

Phase 8: Update all documentation files.

**Owner**: @DocMaintainer  
**Duration**: 1 day  
**Dependencies**: All code phases complete

### Tasks

- [ ] Update all `.ai/` documentation
- [ ] Update all `docs/` documentation
- [ ] Update all README files throughout repo
- [ ] Update ADR documents
- [ ] Update KB articles
- [ ] Update `.github/` files (agents, prompts, instructions)
- [ ] Update DOCUMENT_REGISTRY.md

### Acceptance Criteria

- [ ] No B2Connect references in documentation
- [ ] All links valid
- [ ] Consistent B2X branding

### References

- [ADR-051] Rename B2Connect to B2X
- Depends on: Issues #2-7

---

## Issue 9: [Rename] Phase 9: Verification & Testing

**Labels**: `rename`, `phase-9`, `testing`, `qa`  
**Assignee**: @QA

### Description

Phase 9: Comprehensive verification and testing.

**Owner**: @QA, @TechLead  
**Duration**: 1 day  
**Dependencies**: All phases complete

### Tasks

- [ ] Run `dotnet build B2XGate.slnx`
- [ ] Run `dotnet test B2XGate.slnx`
- [ ] Run `npm run build` in all frontend projects
- [ ] Run `docker-compose build`
- [ ] Run integration tests
- [ ] Execute grep search for remaining B2Connect references
- [ ] Validate all MCP servers
- [ ] Test CI/CD pipeline

### Verification Script

```bash
# Find remaining references
grep -ri "b2connect" --include="*.cs" --include="*.ts" --include="*.json" --include="*.md"
```

### Acceptance Criteria

- [ ] All builds succeed
- [ ] All tests pass
- [ ] Zero B2Connect references in code
- [ ] CI/CD pipeline functional

### References

- [ADR-051] Rename B2Connect to B2X
- Depends on: Issues #1-8

---

## Issue 10: [Rename] Phase 10: Repository Migration

**Labels**: `rename`, `phase-10`, `infrastructure`  
**Assignee**: @SARAH, @DevOps

### Description

Phase 10: Final repository migration and cleanup.

**Owner**: @SARAH, @DevOps  
**Duration**: 0.5 days  
**Dependencies**: Phase 9 verification complete

### Tasks

- [ ] Rename repository folder (if applicable)
- [ ] Update remote URLs if needed
- [ ] Update CI/CD pipeline configurations
- [ ] Update any external integrations
- [ ] Merge feature branch to main
- [ ] Tag release version
- [ ] Communicate completion to team

### Acceptance Criteria

- [ ] Repository renamed/migrated
- [ ] CI/CD fully functional
- [ ] Team notified of completion
- [ ] Documentation reflects final state

### References

- [ADR-051] Rename B2Connect to B2X
- Depends on: Issue #9 (Phase 9)

---

## Summary

| Issue | Phase | Owner | Duration | Dependencies |
|-------|-------|-------|----------|--------------|
| #1 | Preparation | @SARAH | 1 day | None |
| #2 | Backend Core | @Backend | 2 days | #1 |
| #3 | Shared/Infra | @Backend, @DevOps | 2 days | #2 |
| #4 | MCP Servers | @CopilotExpert | 1 day | #2 |
| #5 | Frontend | @Frontend | 1 day | None |
| #6 | Docker/K8s | @DevOps | 1 day | #2 |
| #7 | Scripts/Config | @DevOps | 0.5 days | #2 |
| #8 | Documentation | @DocMaintainer | 1 day | #2-7 |
| #9 | Verification | @QA | 1 day | #1-8 |
| #10 | Migration | @SARAH, @DevOps | 0.5 days | #9 |

**Total Estimated Time**: 8-10 working days
