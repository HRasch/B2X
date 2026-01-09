# Project Structure Reorganization Analysis

## Current State Analysis

### Project Distribution (90 active .csproj files)

**Root Level Folders:**
- `backend/` - 25+ projects (BoundedContexts, CLI, Connectors, Domain, Gateway, ServiceDefaults, services, shared, Tests)
- `src/` - 50+ projects (Admin, AI, api, BoundedContexts, Connectors, erp-connector, Hosting, IdsConnectAdapter, Management, seeding, services, shared, Store)
- `Frontend/` - 3 frontend apps (Admin, Management, Store)
- `tests/` - 20+ test projects
- `AppHost/`, `ServiceDefaults/`, `tools/` - Infrastructure projects

### Key Findings

#### 1. **Dual Backend Locations**
- `backend/` contains: BoundedContexts/, CLI/, Domain/, Gateway/, services/, shared/, Tests/
- `src/` contains: Admin/, Management/, Store/, shared/, services/, Connectors/

#### 2. **Bounded Context Organization**
Current bounded contexts appear to be:
- **Store**: Catalog, Customer, Orders, Search (frontend + backend + gateway)
- **Admin**: Legal, Compliance, Monitoring (frontend + backend + gateway)
- **Management**: CMS, Email (frontend + backend)

#### 3. **Shared Components**
Multiple shared locations:
- `backend/shared/` - Infrastructure, Core, Messaging, Search, Monitoring, etc.
- `src/shared/` - Domain models, Infrastructure, ERP, Theming, etc.
- `src/shared/Domain/` - Identity, Localization, Tenancy, ERP, etc.

#### 4. **CLI Components Distribution**
Current CLI structure shows partial distribution with some duplication:
- `backend/CLI/` - Main CLI projects: B2X.CLI.Administration, B2X.CLI.Operations, B2X.CLI, B2X.CLI.Shared
- `src/services/admin/cli/` - Duplicate B2X.CLI.Administration
- `src/services/management/CLI/` - Duplicate B2X.CLI  
- `src/services/store/CLI/` - Duplicate B2X.CLI.Operations

#### 5. **Bounded Context-Specific AI Components**
**Admin AI** (Administrative Management):
- AI Dashboard, Consumption tracking, Provider management
- System prompt management, AI status monitoring
- Admin-specific permissions and gateway APIs

**Management AI** (Content Management):
- AI Assistant component for content operations
- MCP tool integration for CMS tasks
- Management-specific permissions and gateway APIs

**Store AI** (Customer-Facing):
- No AI components currently implemented
- Future: Customer service AI, product recommendations
- Store-specific permissions and gateway APIs

**Admin CLI** (Administrative Tools):
- Tenant management, system configuration
- AI-enhanced administrative operations
- Admin-specific CLI commands and automation

**Management CLI** (Content Management Tools):
- Bulk content operations, AI-assisted content creation
- CMS automation and maintenance tools
- Management-specific CLI commands

**Store CLI** (Store Operations Tools):
- Inventory management, order processing
- Customer service automation tools
- Store-specific CLI commands (future)

**Shared AI Infrastructure**:
- MCP servers (Roslyn, Database, Documentation, Wolverine)
- AI logging and monitoring infrastructure
- Cross-cutting AI services

### Proposed Target Structure

```
src/
├── backend/
│   ├── Admin/              # Admin bounded context
│   │   ├── API/           # Gateway/API layer
│   │   ├── Domain/        # Business logic (Legal, Compliance)
│   │   ├── Infrastructure/# Data access, external services
│   │   ├── AI/            # Admin-specific AI components
│   │   ├── CLI/           # Admin-specific CLI tools
│   │   └── Tests/         # Admin-specific tests
│   ├── Store/             # Store bounded context
│   │   ├── API/           # Gateway/API layer
│   │   ├── Domain/        # Business logic (Catalog, Customer, Orders)
│   │   ├── Infrastructure/# Data access, search
│   │   ├── AI/            # Store-specific AI components (future)
│   │   ├── CLI/           # Store-specific CLI tools (future)
│   │   └── Tests/         # Store-specific tests
│   ├── Management/        # Management bounded context
│   │   ├── API/           # (if needed)
│   │   ├── Domain/        # Business logic (CMS, Email)
│   │   ├── Infrastructure/# Data access
│   │   ├── AI/            # Management-specific AI components
│   │   ├── CLI/           # Management-specific CLI tools
│   │   └── Tests/         # Management-specific tests
│   ├── Infrastructure/    # Cross-cutting infrastructure
│   │   ├── Hosting/       # AppHost, ServiceDefaults
│   │   ├── Monitoring/    # Monitoring services
│   │   ├── Messaging/     # Message bus
│   │   ├── Search/        # Search infrastructure
│   │   ├── AI/            # Shared AI infrastructure (MCP servers)
│   │   └── Tests/         # Infrastructure tests
│   ├── Services/          # Background services
│   │   ├── PunchoutAdapters/
│   │   ├── Search/
│   │   ├── Scheduler/      # Job scheduling and orchestration
│   │   ├── BackgroundJobs/ # Message handling and maintenance jobs
│   │   └── Tests/
│   └── Shared/            # Shared kernel
│       ├── Domain/        # Shared domain models
│       ├── Infrastructure/# Shared infrastructure
│       └── Tests/
├── frontend/              # All frontend applications
│   ├── Admin/
│   │   ├── AI/            # Admin AI views (Dashboard, Consumption, etc.)
│   ├── Store/
│   │   ├── AI/            # Store AI components (future)
│   ├── Management/
│   │   ├── AI/            # Management AI components (AiAssistant)
│   └── shared/            # Shared frontend components
├── tools/                 # Development tools
│   ├── AI/                # AI development tools
│   ├── MCP/               # MCP server tools
│   └── seeders/
└── tests/                 # All tests (organized by bounded context)
    ├── Admin/
    ├── Store/
    ├── Management/
    ├── Infrastructure/
    ├── Services/
    └── Shared/
```

### New Projects to Create

#### B2X.Scheduler Service
**Location**: `src/backend/Services/Scheduler/`
**Purpose**: Centralized job scheduling and orchestration
**Key Features**:
- Cron-like scheduled task execution
- Job queue management with priorities
- Periodic maintenance tasks (data cleanup, report generation)
- Integration with monitoring and alerting
- Configurable retry policies and error handling

**Implementation Notes**:
- Use Quartz.NET for job scheduling
- Implement job persistence with PostgreSQL and SQL Server
- Include health checks for job execution monitoring
- Support cron expressions and recurring jobs

#### B2X.BackgroundJobs Service  
**Location**: `src/backend/Services/BackgroundJobs/`
**Purpose**: Message handling and maintenance operations
**Key Features**:
- Wolverine message processing and routing
- Event-driven background task execution
- Long-running data processing jobs
- Database maintenance operations
- External system integration jobs
- Dead letter queue handling

**Implementation Notes**:
- Integrate with Wolverine message bus
- Implement circuit breaker patterns for external integrations
- Add dead letter queue processing
- Include job retry mechanisms with exponential backoff

Both services will follow the standard bounded context structure with API, Domain, Infrastructure, and Tests layers.

### Migration Mapping

#### Phase 1: Consolidate Backend Projects

**From `backend/` → `src/backend/`:**
- `backend/BoundedContexts/Admin/` → `src/backend/Admin/Domain/`
- `backend/CLI/B2X.CLI.Administration/` → `src/backend/Admin/CLI/`
- `backend/CLI/B2X.CLI/` → `src/backend/Management/CLI/`
- `backend/CLI/B2X.CLI.Operations/` → `src/backend/Store/CLI/`
- `backend/CLI/B2X.CLI.Shared/` → `src/backend/Shared/Infrastructure/`
- `backend/Connectors/` → `src/backend/Infrastructure/Connectors/`
- `backend/Domain/` → Distribute to appropriate bounded contexts:
  - Catalog, Customer, Orders, Search → `src/backend/Store/Domain/`
  - CMS, Email → `src/backend/Management/Domain/`
  - Legal → `src/backend/Admin/Domain/`
  - Identity, Localization, Tenancy, ERP, Theming → `src/backend/Shared/Domain/`

**From `src/` → `src/backend/`:**
- `src/Admin/` → `src/backend/Admin/`
- `src/Management/` → `src/backend/Management/`
- `src/Store/` → `src/backend/Store/`
- `src/shared/` → `src/backend/Shared/`
- `src/services/` → `src/backend/Services/` (except CLI components, see backend mapping)
- `src/Connectors/` → `src/backend/Infrastructure/Connectors/`
- `src/erp-connector/` → `src/backend/Infrastructure/ERP/`
- `src/IdsConnectAdapter/` → `src/backend/Services/PunchoutAdapters/`

#### Phase 2: Reorganize Frontend with Bounded Context AI
- `Frontend/` → `src/frontend/`
- **Admin AI**: `Frontend/Admin/src/views/ai/` → `src/frontend/Admin/AI/`
- **Management AI**: `Frontend/Management/src/components/AiAssistant.vue` + `src/services/aiService.ts` → `src/frontend/Management/AI/`
- **Store AI**: No current components (prepare directory structure)
- **Frontend AI Logs**: `Frontend/.ai/logs/` → `src/frontend/shared/AI/logs/`

#### Phase 3: Consolidate Infrastructure
- `AppHost/` → `src/backend/Infrastructure/Hosting/AppHost/`
- `ServiceDefaults/` → `src/backend/Infrastructure/Hosting/ServiceDefaults/`
- `tools/` → `src/tools/`
- **MCP Servers**: `tools/B2XMCP/`, `tools/DatabaseMCP/`, etc. → `src/backend/Infrastructure/AI/MCP/`

#### Phase 4: Update Tests
- `tests/` → `src/tests/` (maintain bounded context organization)

### Dependencies Analysis

**Critical Dependencies to Update:**
1. **Project References**: ~90 .csproj files with relative paths
2. **Solution File**: B2X.slnx with folder structure
3. **Build Scripts**: Any scripts referencing old paths
4. **CI/CD**: Pipeline configurations
5. **Documentation**: README, contribution guides

### Risk Assessment

**High Risk:**
- Breaking project references during moves
- Build failures from incorrect paths
- Test execution failures

**Medium Risk:**
- Frontend build configurations
- Docker configurations
- Documentation becoming outdated

**Low Risk:**
- Moving infrastructure projects
- Consolidating shared components

### Implementation Plan

#### Phase 1: Preparation (1-2 days)
1. Create backup branch
2. Document all current paths and references
3. Create migration scripts
4. Test migration scripts on subset

#### Phase 2: Infrastructure Move (1 day)
1. Move AppHost, ServiceDefaults, tools
2. Update solution file
3. Verify builds

#### Phase 3: Shared Components (1-2 days)
1. Consolidate shared projects
2. Update references
3. Verify builds

#### Phase 4: Bounded Contexts (2-3 days)
1. Move one bounded context at a time
2. Update references after each move
3. Run full test suite after each move

#### Phase 5: Frontend & Tests (1 day)
1. Move frontend projects
2. Reorganize tests
3. Final verification

#### Phase 6: Cleanup (1 day)
1. Remove old directories
2. Update documentation
3. Final testing

### Success Criteria

1. **Build Success**: All projects build without errors
2. **Test Pass**: All tests execute and pass
3. **References Valid**: No broken project references
4. **CI/CD Working**: Build pipelines functional
5. **Documentation Updated**: README and guides reflect new structure

### Tools Needed

1. **Migration Scripts**: PowerShell/bash scripts to move files and update references
2. **Reference Checker**: Script to validate all project references
3. **Build Validator**: Automated build testing
4. **Documentation Generator**: Update docs with new paths

### Estimated Timeline: 1-2 weeks

**Week 1:**
- Analysis and planning
- Infrastructure and shared components migration
- Partial bounded context migration

**Week 2:**
- Complete bounded context migration
- Frontend and test reorganization
- Cleanup and validation

### Benefits of New Structure

1. **Clear Boundaries**: Each bounded context is self-contained with its own AI capabilities and gateway APIs
2. **Context-Specific AI**: Admin, Management, and Store AI components have appropriate permissions and logic
3. **API Isolation**: Each bounded context exposes its own AI APIs through dedicated gateways
4. **Scalability**: Easy to add new bounded contexts with their own AI features
5. **Team Autonomy**: Teams can develop AI features independently within their bounded context
6. **Security**: AI permissions are scoped to bounded context responsibilities
7. **Dependency Clarity**: Clear separation between shared AI infrastructure (MCP servers) and bounded context AI features
8. **Build Optimization**: Can build individual bounded contexts with their AI components
9. **Maintenance**: Easier to understand and maintain context-specific AI implementations

### Next Steps

1. Review and approve this analysis
2. Create detailed migration scripts
3. Set up testing environment
4. Begin Phase 1 implementation

## ✅ COMPLETED: Namespace Renaming Plan Added

**Added to PROJECT_RESTRUCTURE_MIGRATION_PLAN.md:**
- Phase 3.5: Namespace Renaming (2 days)
- Comprehensive namespace mapping strategy
- Automated PowerShell scripts for bulk renaming
- Validation scripts for migration verification
- Risk mitigation for namespace changes
- Additional missing items identified and addressed

**Created Files:**
- `namespace-mapping.json` - Namespace transformation mappings
- `scripts/namespace-renamer.ps1` - Automated namespace updating
- `scripts/update-project-namespaces.ps1` - Project file namespace updates
- `scripts/validate-namespace-migration.ps1` - Migration validation

**Scripts Tested:**
- ✅ Namespace renamer script working correctly
- ✅ Project file updater ready for use
- ✅ Validation script created and ready

**Next Steps:**
1. Review namespace mapping for completeness
2. Test scripts on larger scope
3. Update migration timeline to include Phase 3.5
4. Begin implementation when ready</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\PROJECT_RESTRUCTURE_ANALYSIS.md