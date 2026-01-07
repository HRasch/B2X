# B2Connect CLI Migration Guide

**From:** `B2Connect.CLI` (Unified CLI)  
**To:** Specialized CLI Tools (`B2Connect.CLI.Operations` + `B2Connect.CLI.Administration`)  
**Migration Deadline:** v3.0.0 (Q2 2026)

---

## Table of Contents

- [Why Migrate?](#why-migrate)
- [Quick Start](#quick-start)
- [Command Mapping](#command-mapping)
- [Step-by-Step Migration](#step-by-step-migration)
- [Automated Migration](#automated-migration)
- [CI/CD Pipeline Updates](#cicd-pipeline-updates)
- [Troubleshooting](#troubleshooting)
- [FAQ](#faq)

---

## Why Migrate?

The unified `B2Connect.CLI` has been deprecated due to:

### Security Concerns
- Mixed platform operations and tenant administration in single tool
- Potential for tenant admins to access system commands
- Unclear security boundaries

### Solution: Specialized CLIs
- **Operations CLI:** Internal platform management (DevOps/SRE)
- **Administration CLI:** Customer-facing tenant management
- Clear separation with distinct authentication models

---

## Quick Start

### For Platform Operators

```bash
# Uninstall old CLI
dotnet tool uninstall -g B2Connect.CLI

# Install Operations CLI
dotnet tool install -g B2Connect.CLI.Operations

# Verify installation
b2connect-ops --help
```

### For Tenant Administrators

```bash
# Uninstall old CLI
dotnet tool uninstall -g B2Connect.CLI

# Install Administration CLI
dotnet tool install -g B2Connect.CLI.Administration

# Verify installation
b2connect-admin --help
```

---

## Command Mapping

### Authentication & User Management
→ **Migrate to:** `B2Connect.CLI.Administration`

| Old Command | New Command |
|-------------|-------------|
| `b2connect auth create-user` | `b2connect-admin auth create-user` |
| `b2connect auth login` | `b2connect-admin auth login` |

### Tenant Management
→ **Migrate to:** `B2Connect.CLI.Administration`

| Old Command | New Command |
|-------------|-------------|
| `b2connect tenant create` | `b2connect-admin tenant create` |

### Catalog Operations
→ **Migrate to:** `B2Connect.CLI.Administration`

| Old Command | New Command |
|-------------|-------------|
| `b2connect catalog import` | `b2connect-admin catalog import` |
| `b2connect catalog export` | `b2connect-admin catalog export` |

### System Operations
→ **Migrate to:** `B2Connect.CLI.Operations`

| Old Command | New Command |
|-------------|-------------|
| `b2connect system status` | `b2connect-ops health check` |
| `b2connect system health-check` | `b2connect-ops health check` |

### Monitoring
→ **Migrate to:** `B2Connect.CLI.Operations`

| Old Command | New Command |
|-------------|-------------|
| `b2connect monitoring register-service` | `b2connect-ops monitoring register` |
| `b2connect monitoring test-connectivity` | `b2connect-ops health connectivity` |
| `b2connect monitoring service-status` | `b2connect-ops monitoring status` |
| `b2connect monitoring alerts` | `b2connect-ops monitoring alerts` |

---

## Step-by-Step Migration

### Step 1: Identify Your Use Case

**Are you a Platform Operator or Tenant Administrator?**

- **Platform Operator (DevOps/SRE):** Install `B2Connect.CLI.Operations`
- **Tenant Administrator:** Install `B2Connect.CLI.Administration`
- **Both roles:** Install both CLIs

### Step 2: Install New CLI(s)

```bash
# For platform operators
dotnet tool install -g B2Connect.CLI.Operations

# For tenant administrators
dotnet tool install -g B2Connect.CLI.Administration
```

### Step 3: Update Scripts

**Example:** User creation script

```bash
# Before
#!/bin/bash
b2connect auth create-user \
  --email john@company.com \
  --password SecureP@ss123 \
  --role admin

# After
#!/bin/bash
b2connect-admin auth create-user \
  --email john@company.com \
  --password SecureP@ss123 \
  --role admin
```

### Step 4: Update Environment Variables

**Operations CLI:**
```bash
# Before
export B2CONNECT_TOKEN="your-token"

# After
export B2CONNECT_OPS_TOKEN="cluster-admin-token"
```

**Administration CLI:**
```bash
# Before
export B2CONNECT_TOKEN="your-token"

# After
export B2CONNECT_TENANT_TOKEN="tenant-scoped-api-key"
```

### Step 5: Test

```bash
# Test Operations CLI
b2connect-ops health check

# Test Administration CLI
b2connect-admin auth login --email admin@example.com
```

### Step 6: Uninstall Old CLI

```bash
dotnet tool uninstall -g B2Connect.CLI
```

---

## Automated Migration

### Migration Script

We provide an automated migration script that:
- Detects current usage patterns
- Installs appropriate new CLI(s)
- Updates scripts and configuration files
- Validates migration

**Download and run:**

```bash
# Download migration script
curl -o migrate-cli.sh https://raw.githubusercontent.com/b2connect/cli/main/scripts/migrate-cli.sh

# Make executable
chmod +x migrate-cli.sh

# Run migration
./migrate-cli.sh

# Follow interactive prompts
```

### Script Features

- ✅ Automatic CLI detection
- ✅ Script file updates (bash, PowerShell)
- ✅ Environment variable migration
- ✅ Configuration file updates
- ✅ Validation and testing
- ✅ Rollback capability

---

## CI/CD Pipeline Updates

### GitHub Actions

**Before:**
```yaml
- name: Install CLI
  run: dotnet tool install -g B2Connect.CLI

- name: Create Tenant
  run: b2connect tenant create --name "Test Tenant"
```

**After:**
```yaml
- name: Install Administration CLI
  run: dotnet tool install -g B2Connect.CLI.Administration

- name: Create Tenant
  run: b2connect-admin tenant create --name "Test Tenant"
  env:
    B2CONNECT_TENANT_TOKEN: ${{ secrets.TENANT_API_KEY }}
```

### Azure DevOps

**Before:**
```yaml
- script: dotnet tool install -g B2Connect.CLI
  displayName: 'Install CLI'

- script: b2connect system status
  displayName: 'Check System Status'
```

**After:**
```yaml
- script: dotnet tool install -g B2Connect.CLI.Operations
  displayName: 'Install Operations CLI'

- script: b2connect-ops health check
  displayName: 'Check System Health'
  env:
    B2CONNECT_OPS_TOKEN: $(OpsToken)
```

### GitLab CI

**Before:**
```yaml
deploy:
  script:
    - dotnet tool install -g B2Connect.CLI
    - b2connect system status
```

**After:**
```yaml
deploy:
  script:
    - dotnet tool install -g B2Connect.CLI.Operations
    - b2connect-ops health check
  variables:
    B2CONNECT_OPS_TOKEN: $OPS_TOKEN
```

---

## Troubleshooting

### Issue: "Command not found" after migration

**Solution:**
```bash
# Verify installation
dotnet tool list -g

# Reinstall if missing
dotnet tool install -g B2Connect.CLI.Administration
```

### Issue: Authentication errors with new CLI

**Problem:** Old token format or wrong environment variable

**Solution:**
```bash
# Operations CLI uses different token
export B2CONNECT_OPS_TOKEN="your-cluster-admin-token"

# Administration CLI uses tenant-scoped token
export B2CONNECT_TENANT_TOKEN="your-tenant-api-key"
```

### Issue: Scripts fail after migration

**Problem:** Command syntax changed

**Solution:** Use the [Command Mapping](#command-mapping) table above to update commands.

### Issue: Need both CLIs but getting conflicts

**Solution:** Both CLIs can coexist. They use different executable names:
- Operations: `b2connect-ops`
- Administration: `b2connect-admin`

---

## FAQ

### Q: Can I keep using the old CLI?

**A:** Yes, but only until v3.0.0 (Q2 2026). After that, it will be removed. We recommend migrating soon to avoid disruption.

### Q: Will my scripts break immediately?

**A:** No. The old CLI will continue to work during the deprecation period, but it will show deprecation warnings. Plan your migration within the support window.

### Q: Do I need both CLIs?

**A:** It depends on your role:
- **DevOps/SRE:** Only Operations CLI
- **Tenant Admin:** Only Administration CLI
- **Both roles:** Install both CLIs

### Q: What if I'm using the CLI in CI/CD?

**A:** Update your pipeline configuration to use the new CLI. See [CI/CD Pipeline Updates](#cicd-pipeline-updates) section.

### Q: Is there a migration script?

**A:** Yes! See [Automated Migration](#automated-migration) section for details.

### Q: What about my configuration files?

**A:** The new CLIs use the same configuration format from `B2Connect.CLI.Shared`. Your existing `~/.b2connect/config.json` will work with both new CLIs.

### Q: How do I report migration issues?

**A:** Open an issue on GitHub: https://github.com/b2connect/cli/issues

### Q: Can I run both old and new CLIs simultaneously?

**A:** Yes, during the migration period. The old CLI executable is `b2connect`, while the new ones are `b2connect-ops` and `b2connect-admin`.

---

## Support

### Documentation
- **Operations CLI:** https://docs.b2connect.com/cli/operations
- **Administration CLI:** https://docs.b2connect.com/cli/administration
- **Migration Help:** https://docs.b2connect.com/cli/migration

### Getting Help
- **GitHub Issues:** https://github.com/b2connect/cli/issues
- **Slack:** #cli-support (internal)
- **Email:** support@b2connect.com

---

## Timeline

| Date | Milestone |
|------|-----------|
| January 5, 2026 | Deprecation announced |
| Q1 2026 | Migration period (support for both CLIs) |
| Q2 2026 | v3.0.0 release - Old CLI removed |

---

**Last Updated:** January 5, 2026  
**Migration Status:** Active - Please migrate before v3.0.0
