# CLI Commands Specification: Customer Integration
# Reference: ADR-038 Customer Integration Stages Framework
# Owner: @Backend
# Status: Specification - Ready for Implementation

## Overview

This specification defines CLI commands for managing customer integration stages per ADR-038.
Commands are organized by stage and support both interactive and scripted usage.

---

## Command Structure

```
b2connect [category] [action] [options]

Categories:
  trial         - Trial/evaluation management
  tenant        - Tenant configuration
  integration   - Integration status and management
  erp           - ERP connector operations (existing, ADR-033)
```

---

## Stage 1: EVALUATE Commands

### `b2connect trial create`

Create a new trial tenant for customer evaluation.

```bash
b2connect trial create \
  --company "Customer Corp" \
  --contact-email "admin@customer.com" \
  --plan [basic|professional|enterprise] \
  --duration [14|30] \
  --demo-catalog [standard|industry-specific]
```

**Options:**
| Option | Required | Default | Description |
|--------|----------|---------|-------------|
| `--company` | Yes | - | Company name |
| `--contact-email` | Yes | - | Primary contact email |
| `--plan` | No | professional | Trial plan tier |
| `--duration` | No | 30 | Trial duration in days |
| `--demo-catalog` | No | standard | Demo catalog to load |
| `--region` | No | eu-central | Deployment region |
| `--output` | No | table | Output format: table, json, yaml |

**Output:**
```json
{
  "tenantId": "trial-customer-corp-a1b2c3",
  "status": "active",
  "urls": {
    "store": "https://trial-customer-corp-a1b2c3.b2connect.cloud",
    "admin": "https://trial-customer-corp-a1b2c3.b2connect.cloud/admin"
  },
  "credentials": {
    "adminEmail": "admin@customer.com",
    "temporaryPassword": "[generated]",
    "apiKey": "sk_trial_xxxxx"
  },
  "expires": "2026-02-05T00:00:00Z"
}
```

### `b2connect trial list`

List all active trial tenants.

```bash
b2connect trial list [--status active|expired|converted] [--output json]
```

### `b2connect trial extend`

Extend a trial period.

```bash
b2connect trial extend --tenant trial-customer-corp-a1b2c3 --days 14
```

### `b2connect trial convert`

Convert trial to production tenant.

```bash
b2connect trial convert \
  --tenant trial-customer-corp-a1b2c3 \
  --production-name "customer-corp" \
  --plan enterprise \
  --contract-id "CONTRACT-2026-001"
```

---

## Stage 2: ONBOARD Commands

### `b2connect tenant create`

Create a production tenant (post-contract).

```bash
b2connect tenant create \
  --name "Customer Corp" \
  --id "customer-corp" \
  --plan [basic|professional|enterprise] \
  --region [eu-central|us-east|ap-southeast] \
  --from-trial trial-customer-corp-a1b2c3  # Optional: migrate from trial
```

### `b2connect tenant configure-domain`

Configure custom domain for tenant.

```bash
b2connect tenant configure-domain \
  --tenant customer-corp \
  --domain store.customer.com \
  --ssl [auto|custom] \
  --certificate-path ./cert.pem  # If ssl=custom
```

**Output:**
```
Domain Configuration for customer-corp
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Domain:     store.customer.com
Status:     pending_verification

Required DNS Records:
┌─────────┬──────────────────────────┬─────────────────────────────────────┐
│ Type    │ Name                     │ Value                               │
├─────────┼──────────────────────────┼─────────────────────────────────────┤
│ CNAME   │ store.customer.com       │ customer-corp.b2connect.cloud       │
│ TXT     │ _b2connect.customer.com  │ b2c-verify=a1b2c3d4e5f6             │
└─────────┴──────────────────────────┴─────────────────────────────────────┘

Run 'b2connect tenant verify-domain --tenant customer-corp' after adding DNS records.
```

### `b2connect tenant verify-domain`

Verify DNS configuration.

```bash
b2connect tenant verify-domain --tenant customer-corp
```

### `b2connect tenant setup-branding`

Configure tenant branding.

```bash
b2connect tenant setup-branding \
  --tenant customer-corp \
  --theme [default|professional|modern|minimal] \
  --primary-color "#1a365d" \
  --secondary-color "#4a90d9" \
  --logo ./assets/logo.svg \
  --favicon ./assets/favicon.ico
```

### `b2connect tenant setup-identity`

Configure identity/authentication.

```bash
b2connect tenant setup-identity \
  --tenant customer-corp \
  --sso-provider [none|azure-ad|okta|google] \
  --sso-client-id "xxx" \
  --sso-client-secret "xxx" \
  --mfa-required true
```

### `b2connect tenant verify-readiness`

Run onboarding readiness checks.

```bash
b2connect tenant verify-readiness --tenant customer-corp [--stage onboard]
```

**Output:**
```
Onboarding Readiness Check: customer-corp
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Stage: ONBOARD
Overall: 85% Complete (17/20 tasks)

Provisioning          ████████████████████ 100%  ✓
Configuration         ████████████████░░░░  80%  
Identity              ████████████████████ 100%  ✓
Domain                ████████████░░░░░░░░  60%  ⚠ SSL pending
Branding              ████████████████████ 100%  ✓
Content               ████████████░░░░░░░░  60%  
Legal                 ████████████████░░░░  80%  

Blockers:
⚠ Domain: SSL certificate pending - waiting for DNS verification
⚠ Content: Email templates not customized

Next Steps:
1. Verify DNS records: b2connect tenant verify-domain --tenant customer-corp
2. Customize email templates in Admin Console
```

---

## Stage 3: INTEGRATE Commands

### `b2connect integration init`

Initialize integration tracking for a tenant.

```bash
b2connect integration init \
  --tenant customer-corp \
  --tier [basic|standard|enterprise] \
  --erp-type [enventa|sap|dynamics|oracle|other] \
  --target-go-live 2026-03-01
```

### `b2connect integration status`

Check integration status and progress.

```bash
b2connect integration status --tenant customer-corp [--detailed]
```

**Output (summary):**
```
Integration Status: customer-corp
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Current Stage: INTEGRATE (3.5 Customer Sync)
Progress:      65%
Target:        2026-03-01
Days Remaining: 55

Stage Progress:
┌────────────┬──────────┬────────────────────────────────────────┐
│ Stage      │ Status   │ Progress                               │
├────────────┼──────────┼────────────────────────────────────────┤
│ Evaluate   │ ✓ Done   │ ████████████████████ 100%              │
│ Onboard    │ ✓ Done   │ ████████████████████ 100%              │
│ Integrate  │ ▶ Active │ █████████████░░░░░░░  65%              │
│ Optimize   │ Pending  │ ░░░░░░░░░░░░░░░░░░░░   0%              │
└────────────┴──────────┴────────────────────────────────────────┘

Active Tasks:
• Customer mapping definition
• Initial customer import

Blockers: None
```

### `b2connect integration report`

Generate detailed integration report.

```bash
b2connect integration report \
  --tenant customer-corp \
  --format [text|markdown|html|pdf] \
  --output ./reports/customer-corp-integration.md
```

### `b2connect integration check-readiness`

Verify readiness for specific stage or go-live.

```bash
b2connect integration check-readiness \
  --tenant customer-corp \
  --stage [integrate|go-live]
```

### `b2connect integration update-stage`

Manually update integration stage (admin only).

```bash
b2connect integration update-stage \
  --tenant customer-corp \
  --stage integrate \
  --substage "3.6 ORDER FLOW" \
  --notes "Customer sync completed, starting order flow testing"
```

---

## Stage 4: OPTIMIZE Commands

### `b2connect integration enable-feature`

Enable advanced features for optimized tenants.

```bash
b2connect integration enable-feature \
  --tenant customer-corp \
  --feature [analytics|recommendations|visual-search|marketplace]
```

### `b2connect integration metrics`

View integration and operational metrics.

```bash
b2connect integration metrics \
  --tenant customer-corp \
  --period [7d|30d|90d] \
  --metrics [sync-success|orders|uptime|errors]
```

---

## Cross-Stage Commands

### `b2connect integration export`

Export integration tracker data.

```bash
b2connect integration export \
  --tenant customer-corp \
  --format [yaml|json] \
  --output ./trackers/customer-corp.yml
```

### `b2connect integration import`

Import/update from tracker file.

```bash
b2connect integration import \
  --tenant customer-corp \
  --file ./trackers/customer-corp.yml
```

### `b2connect integration notify`

Send stage notifications.

```bash
b2connect integration notify \
  --tenant customer-corp \
  --type [stage-complete|blocker|reminder] \
  --recipients [customer|internal|both]
```

---

## Global Options

All commands support these global options:

| Option | Description |
|--------|-------------|
| `--output`, `-o` | Output format: table, json, yaml |
| `--quiet`, `-q` | Suppress non-essential output |
| `--verbose`, `-v` | Verbose output for debugging |
| `--config` | Path to config file |
| `--profile` | Named profile to use |
| `--help`, `-h` | Show help for command |

---

## Exit Codes

| Code | Meaning |
|------|---------|
| 0 | Success |
| 1 | General error |
| 2 | Invalid arguments |
| 3 | Authentication failed |
| 4 | Resource not found |
| 5 | Permission denied |
| 10 | Readiness check failed |
| 11 | Validation failed |

---

## Environment Variables

| Variable | Description |
|----------|-------------|
| `B2CONNECT_API_URL` | API endpoint URL |
| `B2CONNECT_API_KEY` | API authentication key |
| `B2CONNECT_TENANT` | Default tenant ID |
| `B2CONNECT_OUTPUT` | Default output format |

---

## Implementation Priority

### Phase 1 (Week 1-2)
1. `b2connect trial create`
2. `b2connect trial list`
3. `b2connect tenant verify-readiness`
4. `b2connect integration status`

### Phase 2 (Week 3-4)
1. `b2connect tenant configure-domain`
2. `b2connect tenant verify-domain`
3. `b2connect integration init`
4. `b2connect integration report`

### Phase 3 (Week 5-6)
1. `b2connect tenant setup-branding`
2. `b2connect tenant setup-identity`
3. `b2connect integration check-readiness`
4. `b2connect integration export/import`

### Phase 4 (Week 7-8)
1. `b2connect trial convert`
2. `b2connect integration enable-feature`
3. `b2connect integration metrics`
4. `b2connect integration notify`

---

## Related Documents

- [ADR-038] Customer Integration Stages Framework
- [ADR-033] Tenant-Admin Download for ERP-Connector
- [ADR-031] CLI Architecture Split
- [TPL-001] Customer Integration Tracker Template
- [TPL-002] Customer Integration Checklist

---

**Specification Version:** 1.0  
**Created:** 2026-01-05  
**Owner:** @Backend  
**Reviewer:** @SARAH, @Architect
