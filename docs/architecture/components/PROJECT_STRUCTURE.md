---
docid: ARCH-001
title: Project Structure - Verified
owner: @Architect
status: Active
created: 2026-01-10
---

# Project Structure Documentation

**DocID**: `ARCH-001`  
**Status**: ✅ VERIFIED - Current as of January 10, 2026  
**Architecture**: Onion Architecture with DDD Bounded Contexts

## Overview

The B2X project follows a unified structure under `src/` with clear separation of bounded contexts, shared infrastructure, and frontend applications. This structure supports independent development, deployment, and scaling of each bounded context while maintaining shared infrastructure components.

## Directory Structure

```
B2X/
├── src/
│   ├── backend/                          # Backend bounded contexts and infrastructure
│   │   ├── Admin/                       # Administrative bounded context
│   │   │   ├── AI/                      # Admin-specific AI services
│   │   │   ├── API/                     # Admin gateway/API layer
│   │   │   ├── CLI/                     # Admin CLI tools
│   │   │   ├── Domain/                  # Admin business logic
│   │   │   ├── Gateway/                 # Admin gateway implementation
│   │   │   ├── Infrastructure/          # Admin data access
│   │   │   └── Tests/                   # Admin tests
│   │   ├── Infrastructure/              # Cross-cutting infrastructure
│   │   │   ├── AI/                      # Shared AI infrastructure (MCP servers)
│   │   │   ├── Connectors/              # ERP and external connectors
│   │   │   ├── ERP/                     # ERP integration infrastructure
│   │   │   ├── Hosting/                 # AppHost, ServiceDefaults
│   │   │   ├── Messaging/               # Message bus infrastructure
│   │   │   ├── Monitoring/              # Monitoring and logging
│   │   │   └── Search/                  # Search infrastructure
│   │   ├── Management/                  # Content management bounded context
│   │   │   ├── AI/                      # Management-specific AI services
│   │   │   ├── API/                     # Management gateway/API layer
│   │   │   ├── CLI/                     # Management CLI tools
│   │   │   ├── Domain/                  # CMS, Email business logic
│   │   │   ├── Infrastructure/          # Management data access
│   │   │   └── Tests/                   # Management tests
│   │   ├── Services/                    # Background services
│   │   │   ├── BackgroundJobs/          # Message handling and maintenance
│   │   │   ├── PunchoutAdapters/        # IDS Connect and other adapters
│   │   │   ├── Scheduler/               # Job scheduling service
│   │   │   └── Search/                  # Search background services
│   │   ├── Shared/                      # Shared kernel
│   │   │   ├── Domain/                  # Shared domain models
│   │   │   │   ├── B2X.Shared.Core/     # Core shared types
│   │   │   │   ├── B2X.Shared.Kernel/   # Kernel abstractions
│   │   │   │   ├── B2X.Types/           # Common type definitions
│   │   │   │   ├── B2X.Utils/           # Utility functions
│   │   │   │   ├── ERP/                 # ERP domain models
│   │   │   │   ├── Identity/            # Authentication/authorization
│   │   │   │   ├── Localization/        # i18n infrastructure
│   │   │   │   ├── PatternAnalysis/     # Pattern analysis utilities
│   │   │   │   ├── Shared/              # General shared models
│   │   │   │   ├── SmartDataIntegration/# Smart data integration
│   │   │   │   ├── Tenancy/             # Multi-tenancy support
│   │   │   │   └── Theming/             # Theme configuration
│   │   │   └── Infrastructure/          # Shared infrastructure
│   │   │       ├── B2X.Shared.Infrastructure/ # Common infrastructure
│   │   │       ├── Messaging/           # Message bus implementations
│   │   │       ├── middleware/          # HTTP middleware
│   │   │       ├── Monitoring/          # Monitoring infrastructure
│   │   │       ├── Search/              # Search implementations
│   │   │       └── tools/               # Development tools
│   │   └── Store/                       # E-commerce bounded context
│   │       ├── AI/                      # Store-specific AI services (future)
│   │       ├── API/                     # Store gateway/API layer
│   │       ├── CLI/                     # Store CLI tools (future)
│   │       ├── Domain/                  # Catalog, Orders, Search logic
│   │       ├── Infrastructure/          # Store data access
│   │       └── Tests/                   # Store tests
│   ├── AI/                              # AI-specific projects
│   │   ├── HtmlCssMCP/                  # HTML/CSS MCP server
│   │   ├── KnowledgeBaseMCP/            # Knowledge base MCP server
│   │   ├── RoslynMCP/                   # Roslyn MCP server
│   │   ├── SecurityMCP/                 # Security MCP server
│   │   ├── TypeScriptMCP/               # TypeScript MCP server
│   │   ├── VueMCP/                      # Vue.js MCP server
│   │   └── WolverineMCP/                # Wolverine MCP server
│   ├── api/                             # Legacy API projects (migration pending)
│   ├── BoundedContexts/                 # Legacy bounded contexts (migration pending)
│   ├── Conntectors/                     # Legacy connectors (typo - migration pending)
│   ├── docs/                            # Project documentation
│   ├── erp-connector/                   # Legacy ERP connector (migration pending)
│   ├── Hosting/                         # Legacy hosting (migration pending)
│   ├── IdsConnectAdapter/               # Legacy IDS adapter (migration pending)
│   ├── kubernetes/                      # Kubernetes manifests
│   ├── Management/                      # Legacy management (migration pending)
│   ├── services/                        # Legacy services (migration pending)
│   ├── shared/                          # Legacy shared (migration pending)
│   ├── Store/                           # Legacy store (migration pending)
│   ├── tests/                           # Test projects
│   └── tools/                           # Development tools
├── frontend/                            # Frontend applications
│   ├── Admin/                           # Admin frontend application
│   ├── Management/                      # Management frontend application
│   └── Store/                           # Store frontend application
├── docs/                                # Documentation
├── scripts/                             # Build and deployment scripts
├── tests/                               # Test projects (organized by bounded context)
├── tools/                               # Development tools
└── [root-level config files]            # Package.json, solution files, etc.
```

## Bounded Contexts

### Admin Context (`src/backend/Admin/`)
**Purpose**: Administrative management and system configuration
**Key Features**:
- Legal compliance management
- System monitoring and alerting
- User administration
- AI consumption tracking and provider management

**Layers**:
- **Domain**: Legal, Compliance business logic
- **API/Gateway**: REST APIs for admin operations
- **Infrastructure**: Admin-specific data access
- **AI**: Admin AI dashboard, consumption tracking
- **CLI**: Administrative command-line tools

### Store Context (`src/backend/Store/`)
**Purpose**: E-commerce storefront and customer-facing operations
**Key Features**:
- Product catalog management
- Order processing
- Customer management
- Search functionality

**Layers**:
- **Domain**: Catalog, Orders, Search business logic
- **API/Gateway**: Storefront APIs
- **Infrastructure**: Product and order data access
- **AI**: Future customer service AI, recommendations
- **CLI**: Future inventory and order management tools

### Management Context (`src/backend/Management/`)
**Purpose**: Content management and tenant administration
**Key Features**:
- CMS (Content Management System)
- Email template management
- Tenant customization
- AI-assisted content creation

**Layers**:
- **Domain**: CMS, Email business logic
- **API/Gateway**: Management APIs
- **Infrastructure**: Content and email data access
- **AI**: AI content assistant, template generation
- **CLI**: Content management command-line tools

## Shared Infrastructure

### Hosting (`src/backend/Infrastructure/Hosting/`)
- **AppHost**: .NET Aspire orchestration
- **ServiceDefaults**: Common service configuration

### AI Infrastructure (`src/backend/Infrastructure/AI/`)
- MCP (Model Context Protocol) servers
- Shared AI utilities and abstractions

### Messaging (`src/backend/Infrastructure/Messaging/`)
- Wolverine message bus configuration
- Event publishing and handling

### Monitoring (`src/backend/Infrastructure/Monitoring/`)
- Application metrics collection
- Health checks and alerting

### Search (`src/backend/Infrastructure/Search/`)
- Elasticsearch configuration
- Search indexing and querying

### Connectors (`src/backend/Infrastructure/Connectors/`)
- ERP system integrations
- External service adapters

## Services Layer

### BackgroundJobs (`src/backend/Services/BackgroundJobs/`)
- Message processing and routing
- Long-running data operations
- Maintenance tasks

### Scheduler (`src/backend/Services/Scheduler/`)
- Cron-like job scheduling
- Periodic maintenance tasks
- Configurable retry policies

### PunchoutAdapters (`src/backend/Services/PunchoutAdapters/`)
- IDS Connect adapter
- cXML punchout integration
- External catalog integration

### Search (`src/backend/Services/Search/`)
- Background search indexing
- Search optimization tasks

## Frontend Applications

### Admin Frontend (`frontend/Admin/`)
- Administrative dashboard
- AI consumption monitoring
- System configuration UI

### Management Frontend (`frontend/Management/`)
- CMS interface
- Email template editor
- AI content assistant

### Store Frontend (`frontend/Store/`)
- E-commerce storefront
- Product catalog browsing
- Customer account management

## Development Workflow

### Building Individual Contexts
```bash
# Build specific bounded context
dotnet build src/backend/Admin/
dotnet build src/backend/Store/
dotnet build src/backend/Management/

# Build infrastructure
dotnet build src/backend/Infrastructure/

# Build services
dotnet build src/backend/Services/
```

### Running Applications
```bash
# Run full application stack
dotnet run --project src/backend/Infrastructure/Hosting/AppHost/

# Run individual services
dotnet run --project src/backend/Store/API/
dotnet run --project src/backend/Admin/Gateway/
```

### Testing
```bash
# Run all tests
dotnet test B2X.slnx

# Run context-specific tests
dotnet test src/backend/Store/Tests/
dotnet test src/backend/Admin/Tests/
```

## Migration Status

✅ **COMPLETED**:
- Bounded contexts reorganized under `src/backend/`
- Frontend applications moved to `frontend/`
- Infrastructure consolidated
- Solution file updated
- Project references corrected

🔄 **IN PROGRESS**:
- Legacy directories cleanup (`src/api/`, `src/BoundedContexts/`, etc.)
- Namespace standardization
- Test reorganization
- Documentation updates

## Architecture Principles

1. **Bounded Context Isolation**: Each context has independent deployment and scaling
2. **Shared Kernel**: Common domain concepts in `Shared/` with careful dependency management
3. **Onion Architecture**: Domain at center, infrastructure at edges
4. **CQRS Pattern**: Command and query separation where appropriate
5. **Event-Driven**: Asynchronous communication between contexts

## Key Benefits

- **Independent Deployment**: Each bounded context can be deployed separately
- **Team Autonomy**: Teams can work on contexts independently
- **Scalability**: Contexts can be scaled based on load requirements
- **Maintainability**: Clear separation of concerns and responsibilities
- **AI Integration**: Context-specific AI capabilities with appropriate permissions

## References

- [DDD Bounded Contexts](DDD_BOUNDED_CONTEXTS.md)
- [Onion Architecture](ARCHITECTURAL_DOCUMENTATION_STANDARDS.md)
- [CQRS with Wolverine](WOLVERINE_ARCHITECTURE_ANALYSIS.md)
- [Migration Plan](../../PROJECT_RESTRUCTURE_MIGRATION_PLAN.md)</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\docs\architecture\components\PROJECT_STRUCTURE.md