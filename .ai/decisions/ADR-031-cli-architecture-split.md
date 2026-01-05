# ADR-031: CLI Architecture Split - Operations vs. Administration

**Status:** Proposed  
**Date:** January 5, 2026  
**Context:** B2Connect multi-tenant SaaS platform  
**Decision Authors:** @Architect, @Backend, @Security, @DevOps

---

## Problem

The current B2Connect CLI (`b2connect`) combines two distinct operational contexts:

1. **Platform Operations** - System management, monitoring, infrastructure operations
2. **Tenant Administration** - User management, catalog operations, tenant configuration

This unified approach creates several concerns:

### Security Concerns
- Platform operators need **infrastructure-level access** (health checks, service restarts)
- Tenant administrators need **tenant-scoped access** (user CRUD, catalog imports)
- Single CLI means tenant admins could potentially access system commands
- Unclear security boundary between operations and administration

### Distribution Concerns
- Platform operations tool should remain **internal only**
- Tenant administration tool could be **distributed to customers**
- Current unified CLI cannot be safely provided to external users

### Permission Model
- Platform commands require **cluster/infrastructure credentials**
- Tenant commands require **tenant-scoped API tokens**
- Mixed authentication models in single tool create complexity

### User Experience
- Platform operators (DevOps/SRE) have different workflows than tenant admins
- Command discovery cluttered with irrelevant commands for each persona
- Help documentation mixes infrastructure and business operations

---

## Current Architecture

```
backend/CLI/B2Connect.CLI/
├── Commands/
│   ├── AuthCommands/           # Tenant: User management
│   ├── MonitoringCommands/     # Platform: System monitoring
│   ├── SystemCommands/         # Platform: Health checks, restarts
│   └── TenantCommands/         # Tenant: Tenant CRUD
├── Program.cs
└── Services/

Single entry point: b2connect
Single package: B2Connect.CLI
Single authentication model
```

**Current Usage:**
```bash
# Mixed commands - all in one CLI
b2connect system health-check      # Platform operation
b2connect tenant create             # Tenant operation
b2connect auth create-user          # Tenant operation
b2connect monitoring dashboard      # Platform operation
```

---

## Decision

**Split the CLI into two separate tools with distinct purposes:**

### 1. B2Connect.CLI.Operations (Internal Platform Management)
- **Target Users:** DevOps, SRE, Platform Engineers
- **Distribution:** Internal only, not customer-facing
- **Authentication:** Infrastructure/cluster credentials
- **Purpose:** System health, monitoring, service management

### 2. B2Connect.CLI.Administration (Tenant Management)
- **Target Users:** Tenant Administrators, Support Engineers
- **Distribution:** Can be provided to customers
- **Authentication:** Tenant-scoped API tokens
- **Purpose:** User management, catalog operations, tenant config

### 3. B2Connect.CLI.Shared (Common Foundation)
- **Shared library** for both CLIs
- Configuration management
- HTTP clients
- Common utilities

---

## Proposed Architecture

```
backend/CLI/
├── B2Connect.CLI.Shared/              # Shared library
│   ├── Configuration/
│   │   ├── CliConfiguration.cs
│   │   └── ServiceEndpoints.cs
│   ├── HttpClients/
│   │   ├── AuthenticatedHttpClient.cs
│   │   └── RetryPolicyFactory.cs
│   ├── Services/
│   │   └── OutputFormatter.cs
│   └── B2Connect.CLI.Shared.csproj
│
├── B2Connect.CLI.Operations/          # Platform operators
│   ├── Commands/
│   │   ├── HealthCommands/
│   │   │   ├── CheckHealthCommand.cs
│   │   │   └── SystemStatusCommand.cs
│   │   ├── MonitoringCommands/
│   │   │   ├── DashboardCommand.cs
│   │   │   └── MetricsCommand.cs
│   │   ├── ServiceCommands/
│   │   │   ├── RestartCommand.cs
│   │   │   └── ScaleCommand.cs
│   │   └── DeploymentCommands/
│   │       ├── MigrateCommand.cs
│   │       └── RollbackCommand.cs
│   ├── Program.cs
│   └── B2Connect.CLI.Operations.csproj
│
└── B2Connect.CLI.Administration/      # Tenant administrators
    ├── Commands/
    │   ├── TenantCommands/
    │   │   ├── CreateTenantCommand.cs
    │   │   ├── UpdateTenantCommand.cs
    │   │   └── ListTenantsCommand.cs
    │   ├── UserCommands/
    │   │   ├── CreateUserCommand.cs
    │   │   ├── AssignRoleCommand.cs
    │   │   └── ListUsersCommand.cs
    │   └── CatalogCommands/
    │       ├── ImportCatalogCommand.cs
    │       └── ExportCatalogCommand.cs
    ├── Program.cs
    └── B2Connect.CLI.Administration.csproj
```

### Command Structure

#### Operations CLI (Internal)
```bash
# Installation
dotnet tool install -g B2Connect.CLI.Operations

# Usage
b2connect-ops health check
b2connect-ops monitoring dashboard
b2connect-ops service restart catalog
b2connect-ops deployment migrate --env production
b2connect-ops metrics --service all

# Authentication
export B2CONNECT_OPS_TOKEN="cluster-admin-token"
b2connect-ops config set-endpoint --env production
```

#### Administration CLI (Can be distributed)
```bash
# Installation
dotnet tool install -g B2Connect.CLI.Administration

# Usage
b2connect tenant create --name "Acme Corp"
b2connect user add john@acme.com --tenant acme-corp
b2connect catalog import bmecat --file catalog.xml
b2connect user assign-role --user john@acme.com --role admin

# Authentication
export B2CONNECT_TENANT_TOKEN="tenant-scoped-api-key"
b2connect config login --tenant acme-corp
```

---

## Implementation Plan

### Phase 1: Shared Library Extraction (Week 1) ✅ COMPLETE
- ✅ Create `B2Connect.CLI.Shared` project
- ✅ Extract common configuration handling
- ✅ Extract HTTP client factories  
- ✅ Extract output formatting utilities
- ✅ Unit tests for shared components
- ✅ Update original CLI to reference shared library
- ✅ Fix all compilation errors and namespace issues
- ✅ Verify CLI builds and runs successfully

### Phase 2: Operations CLI (Week 2) ⏳ IMPLEMENTED
- ⏳ Create `B2Connect.CLI.Operations` project
- ⏳ Move system/monitoring/deployment commands
- ⏳ Implement infrastructure authentication
- ⏳ Create NuGet package
- ⏳ Internal documentation

### Phase 3: Administration CLI (Week 2-3) ✅ IMPLEMENTED
- ✅ Create `B2Connect.CLI.Administration` project
- ✅ Move tenant/user/catalog commands from original CLI
- ✅ Implement tenant-scoped authentication
- ✅ Create NuGet package structure
- ✅ Customer-facing documentation
- ✅ Build verification and command testing
- ✅ Proper namespace organization (AuthCommands, TenantCommands, CatalogCommands)
- ⏳ **Not yet released** - CLI implemented but not published
- Move tenant/user/catalog commands
- Implement tenant-scoped authentication
- Create NuGet package
- Customer-facing documentation

### Phase 4: Deprecation of Original CLI (Week 4)
- Mark `B2Connect.CLI` as deprecated
- Migration guide for existing users
- Automated migration script
- Support for legacy CLI for 2 releases

---

## Authentication Models

### Operations CLI
```json
{
  "authentication": {
    "type": "infrastructure",
    "tokenSource": "B2CONNECT_OPS_TOKEN",
    "scopes": ["cluster:read", "cluster:write", "monitoring:read"],
    "audience": "https://ops.b2connect.internal"
  }
}
```

### Administration CLI
```json
{
  "authentication": {
    "type": "tenant-scoped",
    "tokenSource": "B2CONNECT_TENANT_TOKEN",
    "scopes": ["tenant:admin", "users:manage", "catalog:write"],
    "audience": "https://api.b2connect.com",
    "tenantId": "required"
  }
}
```

---

## Security Considerations

### Operations CLI Security
- ✅ Infrastructure credentials never leave internal network
- ✅ Not distributed to customers
- ✅ Audit logging for all platform commands
- ✅ MFA required for production operations
- ✅ IP allowlist enforcement

### Administration CLI Security
- ✅ Tenant isolation enforced at API level
- ✅ Can be safely distributed to customers
- ✅ Limited to tenant-scoped operations only
- ✅ No access to infrastructure commands
- ✅ Rate limiting per tenant

### Shared Library Security
- ✅ No hardcoded credentials
- ✅ Secure credential storage (OS keychain)
- ✅ Certificate pinning for production endpoints
- ✅ Automatic token refresh
- ✅ Audit trail for sensitive operations

---

## Consequences

### Positive ✅

1. **Security Isolation**
   - Clear separation between platform and tenant operations
   - Reduced attack surface (tenant CLI cannot access system commands)
   - Principle of least privilege enforced by tool selection
   - Safe distribution to customers

2. **Improved User Experience**
   - Context-specific commands only
   - Clearer help documentation
   - Faster command discovery
   - Persona-aligned workflows

3. **Distribution Flexibility**
   - Administration CLI can be published to public NuGet
   - Operations CLI remains internal
   - Different versioning strategies per CLI

4. **Independent Evolution**
   - Operations CLI can evolve with infrastructure needs
   - Administration CLI can evolve with product features
   - No coupling between release cycles

5. **Compliance & Auditing**
   - Separate audit logs per CLI type
   - Clear compliance boundaries
   - Easier to certify customer-facing CLI

### Challenges ⚠️

1. **Code Duplication Risk**
   - Must maintain shared library discipline
   - Risk of duplicating logic if not careful
   - **Mitigation:** Shared library with clear interfaces

2. **Increased Maintenance**
   - Two NuGet packages to version and release
   - Two sets of documentation
   - Two CLI tools to support
   - **Mitigation:** Automated release pipeline, shared docs framework

3. **Migration Effort**
   - Existing users need to migrate
   - Scripts using old CLI need updates
   - CI/CD pipelines need changes
   - **Mitigation:** Migration guide, deprecation period, automated migration script

4. **Discovery Complexity**
   - Users need to know which CLI to use
   - Installation requires choosing correct package
   - **Mitigation:** Clear naming, documentation, installation wizard

---

## Alternatives Considered

### Alternative 1: Single CLI with Command Namespacing
```bash
b2connect ops:health check
b2connect admin:tenant create
```

**Pros:**
- Single tool to install
- Shared codebase

**Cons:**
- ❌ Still single authentication model
- ❌ Cannot distribute safely to customers
- ❌ All commands accessible regardless of role
- ❌ Doesn't solve security isolation

**Rejected:** Does not address core security concerns

---

### Alternative 2: Feature Flags in Single CLI
```bash
# Different builds with different features
b2connect --mode=operations health check
b2connect --mode=administration tenant create
```

**Pros:**
- Single codebase
- Flexible feature enabling

**Cons:**
- ❌ Complex build configuration
- ❌ Still requires two distributions
- ❌ User confusion about modes
- ❌ Doesn't simplify command structure

**Rejected:** Added complexity without clear benefits

---

### Alternative 3: Web-based Admin Portal Only
**Eliminate CLI for tenant administration, provide web UI only**

**Pros:**
- No CLI maintenance for admin tasks
- Better UX for most operations

**Cons:**
- ❌ No automation support for customers
- ❌ CLI essential for bulk operations
- ❌ Power users prefer CLI
- ❌ Integration scripts require API access

**Rejected:** CLI is required for automation and power users

---

## Migration Path

### For Platform Operators (Internal Teams)

```bash
# Before
dotnet tool install -g B2Connect.CLI
b2connect system health-check

# After
dotnet tool install -g B2Connect.CLI.Operations
b2connect-ops health check

# Migration script provided
./scripts/migrate-cli-config.sh --from b2connect --to b2connect-ops
```

### For Tenant Administrators

```bash
# Before
dotnet tool install -g B2Connect.CLI
b2connect tenant create

# After
dotnet tool install -g B2Connect.CLI.Administration
b2connect tenant create  # Same command syntax

# Migration script provided
./scripts/migrate-cli-config.sh --from b2connect --to b2connect
```

### For CI/CD Pipelines

```yaml
# Before
- run: b2connect auth create-user

# After - Operations pipeline
- run: b2connect-ops deployment migrate

# After - Administration pipeline
- run: b2connect user add
```

---

## Success Metrics

### Technical Metrics
- ✅ Zero security incidents from CLI misuse
- ✅ 100% command coverage in both CLIs
- ✅ <500ms p95 command response time
- ✅ <5% code duplication between CLIs

### User Experience Metrics
- ✅ 90% user satisfaction (survey)
- ✅ <10% increase in support tickets during migration
- ✅ 50% reduction in "command not found" errors
- ✅ 100% of users migrated within 2 releases

### Operational Metrics
- ✅ Both CLIs deployed within 4 weeks
- ✅ Migration guide published
- ✅ Automated tests cover all commands
- ✅ Documentation complete for both CLIs

---

## Documentation Requirements

### Operations CLI Documentation
- Installation guide (internal wiki)
- Command reference with examples
- Authentication setup
- CI/CD integration guide
- Troubleshooting runbook

### Administration CLI Documentation
- Public installation guide (docs.b2connect.com)
- Customer-facing command reference
- Authentication flow
- Use case examples
- API key management

### Migration Documentation
- Migration guide from legacy CLI
- Breaking changes documentation
- Side-by-side command comparison
- Automated migration scripts
- FAQ for common issues

---

## Related Decisions

- **ADR-001:** Event-Driven Architecture (CLI publishes events)
- **ADR-020:** PR Quality Gate (CLI testing requirements)
- **ADR-025:** Gateway-Service Communication (CLI authentication flow)

---

## Stakeholder Approval

### Required Approvals
- [ ] @Architect - Architecture design
- [ ] @Security - Security model
- [ ] @Backend - Implementation feasibility
- [ ] @DevOps - Deployment strategy
- [ ] @TechLead - Code quality standards
- [ ] @ProductOwner - User experience impact

### Reviewed By
- [ ] @Legal - License compliance for NuGet distribution
- [ ] @Support - Support implications
- [ ] @Documentation - Documentation effort

---

## References

### External
- [.NET Global Tools](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools)
- [System.CommandLine Documentation](https://github.com/dotnet/command-line-api)
- [AWS CLI Architecture](https://docs.aws.amazon.com/cli/) - Split CLI precedent
- [Kubernetes CLI (kubectl) + kubeadm](https://kubernetes.io/docs/reference/) - Operations vs. Admin separation

### Internal
- [KB-014] Git Commit Strategy
- [GL-008] Governance Policies
- Current CLI README: `backend/CLI/B2Connect.CLI/README.md`
- Authentication Design: `.ai/decisions/ADR-025-gateway-service-communication-strategy.md`

---

**Status:** 📋 Proposed - Phase 3 Implemented (Administration CLI)  
**Next Review:** January 12, 2026  
**Implementation Target:** Q1 2026
