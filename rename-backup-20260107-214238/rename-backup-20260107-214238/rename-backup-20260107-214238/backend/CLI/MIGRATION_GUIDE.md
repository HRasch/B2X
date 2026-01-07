# B2X CLI Migration Guide

**From:** `B2X.CLI` (Unified CLI)  
**To:** Specialized CLI Tools (`B2X.CLI.Operations` + `B2X.CLI.Administration`)  
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

The unified `B2X.CLI` has been deprecated due to:

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
dotnet tool uninstall -g B2X.CLI

# Install Operations CLI
dotnet tool install -g B2X.CLI.Operations

# Verify installation
B2X-ops --help
```

### For Tenant Administrators

```bash
# Uninstall old CLI
dotnet tool uninstall -g B2X.CLI

# Install Administration CLI
dotnet tool install -g B2X.CLI.Administration

# Verify installation
B2X-admin --help
```

---

## Command Mapping

### Authentication & User Management
→ **Migrate to:** `B2X.CLI.Administration`

| Old Command | New Command |
|-------------|-------------|
| `B2X auth create-user` | `B2X-admin auth create-user` |
| `B2X auth login` | `B2X-admin auth login` |

### Tenant Management
→ **Migrate to:** `B2X.CLI.Administration`

| Old Command | New Command |
|-------------|-------------|
| `B2X tenant create` | `B2X-admin tenant create` |

### Catalog Operations
→ **Migrate to:** `B2X.CLI.Administration`

| Old Command | New Command |
|-------------|-------------|
| `B2X catalog import` | `B2X-admin catalog import` |
| `B2X catalog export` | `B2X-admin catalog export` |

### System Operations
→ **Migrate to:** `B2X.CLI.Operations`

| Old Command | New Command |
|-------------|-------------|
| `B2X system status` | `B2X-ops health check` |
| `B2X system health-check` | `B2X-ops health check` |

### Monitoring
→ **Migrate to:** `B2X.CLI.Operations`

| Old Command | New Command |
|-------------|-------------|
| `B2X monitoring register-service` | `B2X-ops monitoring register` |
| `B2X monitoring test-connectivity` | `B2X-ops health connectivity` |
| `B2X monitoring service-status` | `B2X-ops monitoring status` |
| `B2X monitoring alerts` | `B2X-ops monitoring alerts` |

---

## Step-by-Step Migration

### Step 1: Identify Your Use Case

**Are you a Platform Operator or Tenant Administrator?**

- **Platform Operator (DevOps/SRE):** Install `B2X.CLI.Operations`
- **Tenant Administrator:** Install `B2X.CLI.Administration`
- **Both roles:** Install both CLIs

### Step 2: Install New CLI(s)

```bash
# For platform operators
dotnet tool install -g B2X.CLI.Operations

# For tenant administrators
dotnet tool install -g B2X.CLI.Administration
```

### Step 3: Update Scripts

**Example:** User creation script

```bash
# Before
#!/bin/bash
B2X auth create-user \
  --email john@company.com \
  --password SecureP@ss123 \
  --role admin

# After
#!/bin/bash
B2X-admin auth create-user \
  --email john@company.com \
  --password SecureP@ss123 \
  --role admin
```

### Step 4: Update Environment Variables

**Operations CLI:**
```bash
# Before
export B2X_TOKEN="your-token"

# After
export B2X_OPS_TOKEN="cluster-admin-token"
```

**Administration CLI:**
```bash
# Before
export B2X_TOKEN="your-token"

# After
export B2X_TENANT_TOKEN="tenant-scoped-api-key"
```

### Step 5: Test

```bash
# Test Operations CLI
B2X-ops health check

# Test Administration CLI
B2X-admin auth login --email admin@example.com
```

### Step 6: Uninstall Old CLI

```bash
dotnet tool uninstall -g B2X.CLI
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
curl -o migrate-cli.sh https://raw.githubusercontent.com/B2X/cli/main/scripts/migrate-cli.sh

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
  run: dotnet tool install -g B2X.CLI

- name: Create Tenant
  run: B2X tenant create --name "Test Tenant"
```

**After:**
```yaml
- name: Install Administration CLI
  run: dotnet tool install -g B2X.CLI.Administration

- name: Create Tenant
  run: B2X-admin tenant create --name "Test Tenant"
  env:
    B2X_TENANT_TOKEN: ${{ secrets.TENANT_API_KEY }}
```

### Azure DevOps

**Before:**
```yaml
- script: dotnet tool install -g B2X.CLI
  displayName: 'Install CLI'

- script: B2X system status
  displayName: 'Check System Status'
```

**After:**
```yaml
- script: dotnet tool install -g B2X.CLI.Operations
  displayName: 'Install Operations CLI'

- script: B2X-ops health check
  displayName: 'Check System Health'
  env:
    B2X_OPS_TOKEN: $(OpsToken)
```

### GitLab CI

**Before:**
```yaml
deploy:
  script:
    - dotnet tool install -g B2X.CLI
    - B2X system status
```

**After:**
```yaml
deploy:
  script:
    - dotnet tool install -g B2X.CLI.Operations
    - B2X-ops health check
  variables:
    B2X_OPS_TOKEN: $OPS_TOKEN
```

---

## Troubleshooting

### Issue: "Command not found" after migration

**Solution:**
```bash
# Verify installation
dotnet tool list -g

# Reinstall if missing
dotnet tool install -g B2X.CLI.Administration
```

### Issue: Authentication errors with new CLI

**Problem:** Old token format or wrong environment variable

**Solution:**
```bash
# Operations CLI uses different token
export B2X_OPS_TOKEN="your-cluster-admin-token"

# Administration CLI uses tenant-scoped token
export B2X_TENANT_TOKEN="your-tenant-api-key"
```

### Issue: Scripts fail after migration

**Problem:** Command syntax changed

**Solution:** Use the [Command Mapping](#command-mapping) table above to update commands.

### Issue: Need both CLIs but getting conflicts

**Solution:** Both CLIs can coexist. They use different executable names:
- Operations: `B2X-ops`
- Administration: `B2X-admin`

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

**A:** The new CLIs use the same configuration format from `B2X.CLI.Shared`. Your existing `~/.B2X/config.json` will work with both new CLIs.

### Q: How do I report migration issues?

**A:** Open an issue on GitHub: https://github.com/B2X/cli/issues

### Q: Can I run both old and new CLIs simultaneously?

**A:** Yes, during the migration period. The old CLI executable is `B2X`, while the new ones are `B2X-ops` and `B2X-admin`.

---

## Support

### Documentation
- **Operations CLI:** https://docs.B2X.com/cli/operations
- **Administration CLI:** https://docs.B2X.com/cli/administration
- **Migration Help:** https://docs.B2X.com/cli/migration

### Getting Help
- **GitHub Issues:** https://github.com/B2X/cli/issues
- **Slack:** #cli-support (internal)
- **Email:** support@B2X.com

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
