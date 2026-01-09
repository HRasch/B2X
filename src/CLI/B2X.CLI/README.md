# B2X CLI Tools

**Multi-Tenant SaaS Platform Management Suite**

> **⚠️ DEPRECATED - DO NOT USE FOR NEW PROJECTS**
> 
> This unified CLI tool (`B2X.CLI`) is **deprecated** and will be removed in **v3.0.0**.
> 
> **Migration Required:** Please migrate to the new specialized CLI tools.
> See [Migration Guide](#migration-guide) below.

---

## 🚨 Deprecation Notice

**Effective Date:** January 5, 2026  
**End of Life:** v3.0.0 (Q2 2026)  
**Support Period:** 2 releases (until v2.10.0)

### Why Deprecated?

This unified CLI mixed two distinct operational contexts:
1. **Platform Operations** (system management, monitoring)
2. **Tenant Administration** (user management, catalog operations)

This created security concerns as tenant administrators could potentially access system commands. The new architecture provides:
- ✅ **Security Isolation:** Clear separation between platform and tenant operations
- ✅ **Safe Distribution:** Administration CLI can be safely provided to customers
- ✅ **Better UX:** Context-specific commands for each user role

### Migration Path

**Replace `B2X.CLI` with specialized tools:**

| Old Command | New Command |
|-------------|-------------|
| `B2X auth create-user` | `B2X-admin auth create-user` |
| `B2X tenant create` | `B2X-admin tenant create` |
| `B2X system status` | `B2X-ops health check` |
| `B2X monitoring dashboard` | `B2X-ops monitoring dashboard` |

---

## Migration Guide

### For Platform Operators (DevOps/SRE)

**1. Install Operations CLI:**
```bash
dotnet tool uninstall -g B2X.CLI
dotnet tool install -g B2X.CLI.Operations
```

**2. Update Scripts:**
```bash
# Before
B2X system status

# After
B2X-ops health check
```

### For Tenant Administrators

**1. Install Administration CLI:**
```bash
dotnet tool uninstall -g B2X.CLI
dotnet tool install -g B2X.CLI.Administration
```

**2. Update Scripts:**
```bash
# Before
B2X auth create-user --email user@example.com

# After  
B2X-admin auth create-user --email user@example.com
```

### Automated Migration Script

```bash
# Download migration script
curl -o migrate-cli.sh https://raw.githubusercontent.com/B2X/cli/main/scripts/migrate-cli.sh

# Run migration
chmod +x migrate-cli.sh
./migrate-cli.sh
```

---

## Architecture Overview

B2X provides two specialized CLI tools for different operational contexts:

### 🏗️ Operations CLI (`B2X-ops`)
**Platform Infrastructure Management**
- **Target Users:** DevOps, SRE, Platform Engineers
- **Distribution:** Internal only
- **Purpose:** System health, monitoring, service management, deployments
- **Package:** `B2X.CLI.Operations`

### 👥 Administration CLI (`B2X`)
**Tenant & User Management**
- **Target Users:** Tenant Administrators, Support Engineers
- **Distribution:** Customer-facing (public NuGet)
- **Purpose:** User management, catalog operations, tenant configuration
- **Package:** `B2X.CLI.Administration`

### 🔧 Shared Library
**Common Foundation**
- **Purpose:** Shared configuration, HTTP clients, utilities
- **Package:** `B2X.CLI.Shared`

---

## Installation

### Operations CLI (Internal Teams)

```bash
# Install from internal NuGet feed
dotnet tool install --global B2X.CLI.Operations

# Verify installation
B2X-ops --help
```

### Administration CLI (Tenant Administrators)

```bash
# Install from public NuGet
dotnet tool install --global B2X.CLI.Administration

# Verify installation
B2X --help
```

### Build from Source

```bash
# Operations CLI
cd backend/CLI/B2X.CLI.Operations
dotnet build
dotnet pack -c Release
dotnet tool install --global --add-source ./bin/Release B2X.CLI.Operations

# Administration CLI
cd backend/CLI/B2X.CLI.Administration
dotnet build
dotnet pack -c Release
dotnet tool install --global --add-source ./bin/Release B2X.CLI.Administration
```

---

## When to Use Each CLI

### Use Operations CLI (`B2X-ops`) for:
- ✅ System health checks and monitoring
- ✅ Service restarts and scaling
- ✅ Database migrations and rollbacks
- ✅ Infrastructure metrics and dashboards
- ✅ Deployment operations
- ✅ Cluster-level troubleshooting

### Use Administration CLI (`B2X`) for:
- ✅ Creating and managing tenants
- ✅ User account management
- ✅ Catalog import/export operations
- ✅ Tenant configuration
- ✅ Customer support tasks

---

## Configuration

### Operations CLI Configuration
```json
{
  "services": {
    "identity": { "url": "http://localhost:7002" },
    "catalog": { "url": "http://localhost:7005" }
  },
  "environment": "development",
  "authentication": {
    "type": "infrastructure",
    "tokenSource": "B2X_OPS_TOKEN"
  }
}
```

### Administration CLI Configuration
```json
{
  "services": {
    "identity": { "url": "https://api.B2X.com" }
  },
  "environment": "production",
  "authentication": {
    "type": "tenant-scoped",
    "tokenSource": "B2X_TENANT_TOKEN",
    "tenantId": "your-tenant-id"
  }
}
```

---

## Authentication

### Operations CLI
```bash
# Set infrastructure token
export B2X_OPS_TOKEN="your-cluster-admin-token"

# Login with infrastructure credentials
B2X-ops auth login --cluster-admin
```

### Administration CLI
```bash
# Set tenant-scoped token
export B2X_TENANT_TOKEN="your-tenant-api-key"

# Login with tenant credentials
B2X auth login --tenant your-tenant-id
```

---

## Command Examples

### Operations CLI
```bash
# Check all services health
B2X-ops health check

# View monitoring dashboard
B2X-ops monitoring dashboard

# Restart a service
B2X-ops service restart catalog

# Run database migration
B2X-ops deployment migrate --version 1.2.3
```

### Administration CLI
```bash
# Create new tenant
B2X tenant create "Acme Corp" \
  --admin-email admin@acme.com

# Add user to tenant
B2X user add john@acme.com \
  --tenant acme-corp \
  --role administrator

# Import product catalog
B2X catalog import bmecat \
  --file products.xml \
  --tenant acme-corp
```

---

## Migration from Unified CLI

### ⚠️ Important Changes

| Old Command | New Command | CLI Tool |
|-------------|-------------|----------|
| `B2X system status` | `B2X-ops health check` | Operations |
| `B2X monitoring dashboard` | `B2X-ops monitoring dashboard` | Operations |
| `B2X tenant create` | `B2X tenant create` | Administration |
| `B2X auth create-user` | `B2X user add` | Administration |

### Automated Migration

```bash
# Run migration script (provided with new CLIs)
./scripts/migrate-cli-config.sh --from unified --to split

# This will:
# 1. Install both new CLI tools
# 2. Migrate configuration files
# 3. Update authentication tokens
# 4. Provide command mapping guide
```

### Manual Migration Steps

1. **Install new CLIs:**
   ```bash
   dotnet tool install --global B2X.CLI.Operations
   dotnet tool install --global B2X.CLI.Administration
   ```

2. **Update authentication:**
   ```bash
   # For operations work
   export B2X_OPS_TOKEN="your-ops-token"

   # For tenant administration
   export B2X_TENANT_TOKEN="your-tenant-token"
   ```

3. **Update scripts and CI/CD:**
   ```bash
   # Replace in scripts
   sed 's/B2X system/B2X-ops health/g' your-scripts.sh
   sed 's/B2X tenant/B2X tenant/g' your-scripts.sh
   ```

### Breaking Changes
- **Command syntax changes** for some operations commands
- **Separate authentication** required for each CLI
- **Configuration file location** changes (`~/.B2X/` → `~/.B2X-ops/` and `~/.B2X/`)
- **No unified help** - each CLI has focused command set

---

## Architecture Overview

```
backend/CLI/
├── B2X.CLI.Shared/              # Shared library
│   ├── Configuration/                 # Common config handling
│   ├── HttpClients/                   # HTTP client factories
│   ├── Services/                      # Output formatting, utilities
│   └── B2X.CLI.Shared.csproj
│
├── B2X.CLI.Operations/          # Platform operations
│   ├── Commands/
│   │   ├── HealthCommands/           # System health checks
│   │   ├── MonitoringCommands/       # Metrics & dashboards
│   │   ├── ServiceCommands/          # Service management
│   │   └── DeploymentCommands/       # Migrations & rollbacks
│   ├── Program.cs
│   └── B2X.CLI.Operations.csproj
│
└── B2X.CLI.Administration/      # Tenant administration
    ├── Commands/
    │   ├── TenantCommands/           # Tenant CRUD
    │   ├── UserCommands/             # User management
    │   └── CatalogCommands/          # Catalog operations
    ├── Program.cs
    └── B2X.CLI.Administration.csproj
```

---

## Development

### Project Structure (New Architecture)

Each CLI project follows this structure:
```
B2X.CLI.{ToolName}/
├── Commands/
│   └── {Feature}Commands/
│       └── {CommandName}Command.cs
├── Services/
├── Program.cs
└── B2X.CLI.{ToolName}.csproj
```

### Adding Commands

1. **For Operations CLI:**
   ```csharp
   // In B2X.CLI.Operations/Commands/{Feature}Commands/
   public static class NewCommand
   {
       public static Command Create() => new Command("new-command", "Description")
           .WithHandler(ExecuteAsync);
   }
   ```

2. **For Administration CLI:**
   ```csharp
   // In B2X.CLI.Administration/Commands/{Feature}Commands/
   public static class NewCommand
   {
       public static Command Create() => new Command("new-command", "Description")
           .WithHandler(ExecuteAsync);
   }
   ```

3. **Register in respective Program.cs**

---

## Security Model

### Operations CLI
- 🔒 **Internal distribution only**
- 🔒 **Infrastructure-level credentials**
- 🔒 **Audit logging** for all commands
- 🔒 **MFA required** for production operations

### Administration CLI
- 🔒 **Tenant-scoped authentication**
- 🔒 **No infrastructure access**
- 🔒 **Rate limiting** per tenant
- 🔒 **Safe for customer distribution**

---

## Troubleshooting

### Command Not Found
```bash
# Check which CLI has the command
B2X-ops --help | grep "your-command"
B2X --help | grep "your-command"

# Install missing CLI
dotnet tool install --global B2X.CLI.Operations  # or Administration
```

### Authentication Issues
```bash
# For Operations CLI
echo $B2X_OPS_TOKEN
B2X-ops auth login --cluster-admin

# For Administration CLI
echo $B2X_TENANT_TOKEN
B2X auth login --tenant your-tenant-id
```

### Service Connection Failed
```bash
# Check service endpoints in config
B2X-ops config show
B2X config show

# Test connectivity
curl -H "Authorization: Bearer $TOKEN" https://api.B2X.com/health
```

---

## CI/CD Integration

### Operations Pipeline
```yaml
- name: Install Operations CLI
  run: dotnet tool install --global B2X.CLI.Operations

- name: Health Check
  run: B2X-ops health check

- name: Deploy
  run: B2X-ops deployment migrate --env production
  env:
    B2X_OPS_TOKEN: ${{ secrets.OPS_TOKEN }}
```

### Administration Pipeline
```yaml
- name: Install Admin CLI
  run: dotnet tool install --global B2X.CLI.Administration

- name: Setup Tenant
  run: B2X tenant create "CI-Test"
  env:
    B2X_TENANT_TOKEN: ${{ secrets.TENANT_TOKEN }}
```

---

## Roadmap

### Phase 1 (Current) - Architecture Split ✅
- [x] Shared library extraction
- [x] Operations CLI implementation
- [x] Administration CLI implementation
- [x] Migration guide and scripts

### Phase 2 - Enhanced Features
- [ ] Interactive shell mode for both CLIs
- [ ] Configuration wizards
- [ ] Advanced bulk operations
- [ ] Webhook management
- [ ] Analytics commands

### Phase 3 - Ecosystem Integration
- [ ] Kubernetes operator integration
- [ ] Terraform provider
- [ ] Ansible modules
- [ ] CI/CD marketplace integrations

---

## Support

### Operations CLI
- **Internal Wiki:** Platform Operations Guide
- **Support Channel:** #platform-ops Slack
- **Documentation:** `docs/archive/reference-docs/`

### Administration CLI
- **Public Docs:** https://docs.B2X.com/cli
- **Support Channel:** support@B2X.com
- **Documentation:** `docs/user-guides/`

---

## License

**Operations CLI:** Internal Use Only  
**Administration CLI:** MIT License  
**Shared Library:** MIT License

---

**Migration Deadline:** March 2026  
**Legacy CLI Support:** Until Q2 2026  
**Documentation:** See ADR-031 for full architecture details

## Configuration

Configuration is stored in separate locations for each CLI:

### Operations CLI: `~/.B2X-ops/config.json`
```json
{
  "services": {
    "identity": {
      "url": "http://localhost:7002",
      "description": "Identity Service"
    },
    "catalog": {
      "url": "http://localhost:7005",
      "description": "Catalog Service"
    }
  },
  "environment": "development",
  "authentication": {
    "type": "infrastructure",
    "tokenSource": "B2X_OPS_TOKEN"
  }
}
```

### Administration CLI: `~/.B2X/config.json`
```json
{
  "services": {
    "identity": {
      "url": "https://api.B2X.com",
      "description": "Identity Service"
    }
  },
  "environment": "production",
  "authentication": {
    "type": "tenant-scoped",
    "tokenSource": "B2X_TENANT_TOKEN",
    "tenantId": "required"
  }
}
```

## Environment Variables

### Operations CLI
```bash
# Infrastructure token
export B2X_OPS_TOKEN="eyJhbGc..."

# Default environment
export B2X_OPS_ENV="production"
```

### Administration CLI
```bash
# Tenant-scoped token
export B2X_TENANT_TOKEN="eyJhbGc..."

# Default tenant ID
export B2X_TENANT="00000000-0000-0000-0000-000000000000"
```

## Usage

### Help

```bash
B2X --help
B2X auth --help
B2X auth create-user --help
```

### Authentication Commands

#### Create User

```bash
# Interactive password prompt
B2X auth create-user john@example.com \
  --first-name John \
  --last-name Doe \
  --tenant-id 00000000-0000-0000-0000-000000000000

# With password
B2X auth create-user john@example.com \
  --password "SecurePassword123!" \
  --first-name John \
  --last-name Doe
```

#### Login

```bash
# Interactive password prompt
B2X auth login john@example.com

# With password
B2X auth login john@example.com -p "SecurePassword123!"

# Save token for future use
B2X auth login john@example.com --save
```

### Tenant Commands

#### Create Tenant

```bash
B2X tenant create "Acme Corp" \
  --admin-email admin@acme.com \
  --admin-password "AdminPassword123!"
```

### System Commands

#### Check Service Status

```bash
# All services
B2X system status

# Specific service
B2X system status --service identity

# All services verbose
B2X system status --service all
```

#### Show Configuration

```bash
B2X info
```

## Command Structure

```
B2X/
├── auth                    # Authentication & User Management
│   ├── create-user        # Create new user
│   └── login              # Get JWT token
│
├── tenant                  # Tenant Management
│   ├── create             # Create new tenant
│   ├── list               # List all tenants
│   └── show               # Show tenant details
│
├── product                # Product Management (Catalog)
│   ├── create             # Create product
│   ├── import             # Bulk import
│   └── list               # List products
│
├── content                # Content Management (CMS)
│   ├── create-page        # Create page
│   └── publish            # Publish content
│
├── system                 # System Operations
│   ├── status             # Service health check
│   ├── migrate            # Database migrations
│   └── seed               # Seed data
│
└── info                   # Show configuration
```

## Examples

### Full Workflow

```bash
# 1. Create tenant
TENANT_ID=$(B2X tenant create "My Shop" \
  --admin-email admin@myshop.com \
  --admin-password "AdminPassword123!" | grep "Tenant ID" | awk '{print $NF}')

export B2X_TENANT=$TENANT_ID

# 2. Create users
B2X auth create-user john@myshop.com \
  --first-name John \
  --last-name Doe

# 3. Login
B2X auth login admin@myshop.com --save

# 4. Check services
B2X system status

# 5. Create product
B2X product create "SKU-001" "Product Name" \
  --price 99.99 \
  --category "Electronics"
```

### Scripting

```bash
#!/bin/bash

# Setup new tenant
TENANT_NAME="$1"
ADMIN_EMAIL="$2"

echo "Creating tenant: $TENANT_NAME"
B2X tenant create "$TENANT_NAME" \
  --admin-email "$ADMIN_EMAIL" \
  --admin-password "$(openssl rand -base64 12)"

# Wait for service
sleep 2

# Health check
B2X system status || exit 1

echo "Tenant setup complete!"
```

## Advanced Features

### Custom Service Endpoints

Edit `~/.B2X/config.json`:

```json
{
  "services": {
    "custom-service": {
      "url": "http://custom-service:9000",
      "description": "Custom Service"
    }
  }
}
```

### Authentication Token Management

```bash
# Save token after login
B2X auth login user@example.com --save

# Use saved token in requests
B2X auth create-user newuser@example.com
# Token from B2X_TOKEN env var is automatically used

# Clear token
unset B2X_TOKEN
```

### Batch Operations

```bash
# Create multiple users from file
cat users.txt | while read email; do
  B2X auth create-user "$email" \
    --password "TempPassword123!" \
    --tenant-id "$B2X_TENANT"
done
```

## Development

### Project Structure

```
backend/CLI/B2X.CLI/
├── Commands/
│   ├── AuthCommands/           # User & auth commands
│   ├── TenantCommands/         # Tenant management
│   ├── ProductCommands/        # Catalog operations
│   ├── ContentCommands/        # CMS operations
│   └── SystemCommands/         # System operations
│
├── Services/
│   ├── CliHttpClient.cs        # HTTP communication
│   ├── ConfigurationService.cs # Config management
│   └── ConsoleOutputService.cs # Formatted output
│
├── Program.cs                  # Entry point
└── B2X.CLI.csproj
```

### Adding New Commands

1. Create command file in appropriate folder: `Commands/{Group}/{Name}Command.cs`

```csharp
using System.CommandLine;
using B2X.CLI.Services;

namespace B2X.CLI.Commands.{Group};

public static class {Name}Command
{
    public static Command Create()
    {
        var command = new Command("{name}", "Description");
        
        // Add arguments and options
        var argument = new Argument<string>("name", "Description");
        command.AddArgument(argument);
        
        command.SetHandler(ExecuteAsync, argument);
        return command;
    }
    
    private static async Task ExecuteAsync(string arg)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();
        
        // Implementation
        console.Success("Done!");
    }
}
```

2. Register in `Program.cs`:

```csharp
var customCommand = new Command("custom", "Custom operations");
customCommand.AddCommand({Name}Command.Create());
rootCommand.AddCommand(customCommand);
```

## Troubleshooting

### Service Connection Failed

```bash
# Check if service is running
B2X system status

# Verify config
B2X info

# Test URL directly
curl http://localhost:7002/health
```

### Authentication Errors

```bash
# Clear saved token
unset B2X_TOKEN

# Login again
B2X auth login user@example.com --save
```

### Configuration Issues

```bash
# Reset to defaults
rm -rf ~/.B2X

# Reconfigure services in config.json
```

## CI/CD Integration

### GitHub Actions

```yaml
- name: Create Test Tenant
  run: |
    dotnet tool install --global --add-source ./nupkg B2X.CLI
    B2X tenant create "CI-Test-${{ github.run_number }}" \
      --admin-email ci@test.com \
      --admin-password "${{ secrets.ADMIN_PASSWORD }}"
```

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0

# Install CLI
COPY . /src
WORKDIR /src/backend/CLI/B2X.CLI
RUN dotnet build && dotnet pack -c Release && \
    dotnet tool install --global --add-source ./bin/Release B2X.CLI

ENTRYPOINT ["B2X"]
```

## Roadmap

### Phase 1 (Current)
- [x] Auth commands (create-user, login)
- [x] Tenant commands (create)
- [x] System commands (status)
- [x] HTTP client & configuration

### Phase 2
- [ ] Product commands (create, import, list)
- [ ] Content commands (create-page, publish)
- [ ] Database migration commands
- [ ] Data seeding commands
- [ ] Bulk operations

### Phase 3
- [ ] Interactive shell mode
- [ ] Configuration UI wizard
- [ ] Export/import configurations
- [ ] Webhook management
- [ ] Advanced analytics commands

## License

MIT

---

**Documentation**: [CLI_IMPLEMENTATION_GUIDE.md](../../CLI_IMPLEMENTATION_GUIDE.md)
