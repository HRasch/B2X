#!/bin/bash

# Create all P0 (Phase 0 Compliance) issues for B2Connect
# Run: bash scripts/create-p0-issues.sh
# Prerequisites: gh CLI installed and authenticated

set -e

PROJECT_ID="PVT_kwHOAWN9Gs4BLeEd"  # Planner project
REPO="HRasch/B2Connect"

echo "🚀 Creating P0 Compliance Issues for B2Connect..."
echo ""

declare -a ISSUES_CREATED
declare -a ISSUE_NUMBERS

# ============================================================
# P0.1: AUDIT LOGGING
# ============================================================
echo "📝 P0.1: Audit Logging (Immutable, Encrypted)"
echo "---"

# P0.1.1: AuditLogEntry Entity
ID=$(gh issue create --repo "$REPO" \
  --title "P0.1.1: AuditLogEntry Entity & EF Core Mapping" \
  --body "Create immutable audit log entity with encryption support

## Description
Implement AuditLogEntry entity for NIS2 Art. 21 compliance. All CRUD operations must be logged with tenant isolation.

## Acceptance Criteria
- [ ] AuditLogEntry entity created with TenantId, UserId, Action, OldValues, NewValues
- [ ] EF Core mapping configured (immutable on retrieval)
- [ ] Soft delete table for audit logs (IsArchived, ArchivedAt)
- [ ] Unit tests: 3+ test cases

## Compliance
- NIS2 Art. 21 (Audit trail)
- GDPR Art. 32(c) (Accountability)

## Effort: 8 hours" \
  --label "p0,audit-logging,backend" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.1.1: Issue #$ID"

# P0.1.2: EF Core Interceptor
ID=$(gh issue create --repo "$REPO" \
  --title "P0.1.2: EF Core SaveChangesInterceptor (Auto-Logging)" \
  --body "Implement interceptor to automatically log all CRUD operations

## Description
EF Core interceptor that captures all entity changes and creates AuditLogEntry records.

## Acceptance Criteria
- [ ] SaveChangesInterceptor logs CREATE, UPDATE, DELETE
- [ ] JSON serialization of old/new values
- [ ] Performance: < 10ms overhead per operation
- [ ] Tests: 5+ test cases (various entity types)

## Compliance
- NIS2 Art. 21 (Complete audit trail)

## Effort: 12 hours

## Blocked By
- P0.1.1" \
  --label "p0,audit-logging,backend" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.1.2: Issue #$ID"

# P0.1.3: Audit Service & Testing
ID=$(gh issue create --repo "$REPO" \
  --title "P0.1.3: AuditService & Compliance Verification" \
  --body "Implement service layer and verify all operations logged

## Acceptance Criteria
- [ ] AuditService with methods: LogAsync(action, entityId, before, after)
- [ ] Tests verify: product CRUD, user CRUD, order CRUD
- [ ] Audit logs queryable by tenant/user/action/date range
- [ ] Tests: 7+ test cases

## Effort: 12 hours

## Blocked By
- P0.1.2" \
  --label "p0,audit-logging,backend,testing" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.1.3: Issue #$ID"

# ============================================================
# P0.2: ENCRYPTION AT REST
# ============================================================
echo ""
echo "🔐 P0.2: Encryption at Rest (AES-256-GCM)"
echo "---"

# P0.2.1: AES Encryption Service
ID=$(gh issue create --repo "$REPO" \
  --title "P0.2.1: AES-256 Encryption Service" \
  --body "Implement AES encryption for PII fields

## Description
Create IEncryptionService with AES-256-GCM encryption/decryption.

## PII Fields
- Email, Phone, FirstName, LastName, DateOfBirth, Address (all users)
- CostPrice, SupplierName (products)

## Acceptance Criteria
- [ ] Encrypt() returns Base64 (IV included)
- [ ] Decrypt() recovers original plaintext
- [ ] Random IV per encryption (no deterministic ciphertext)
- [ ] Performance: < 5ms per operation
- [ ] Tests: Encryption, decryption, round-trip, different IVs

## Compliance
- GDPR Art. 32(1)(b) (Encryption of personal data)
- NIS2 Art. 21(4)

## Effort: 10 hours" \
  --label "p0,encryption,backend,security" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.2.1: Issue #$ID"

# P0.2.2: EF Core Value Converters
ID=$(gh issue create --repo "$REPO" \
  --title "P0.2.2: EF Core Value Converters (Auto-Encryption)" \
  --body "Configure EF Core value converters for auto-encryption on save/load

## Acceptance Criteria
- [ ] User.Email encrypted automatically
- [ ] User.Phone encrypted automatically
- [ ] Product.CostPrice encrypted automatically
- [ ] All PII fields configured
- [ ] Tests: 5+ test cases (Entity save/load)

## Effort: 8 hours

## Blocked By
- P0.2.1" \
  --label "p0,encryption,backend" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.2.2: Issue #$ID"

# P0.2.3: Key Rotation Policy
ID=$(gh issue create --repo "$REPO" \
  --title "P0.2.3: Annual Key Rotation Policy" \
  --body "Implement automatic encryption key rotation (annual)

## Acceptance Criteria
- [ ] KeyRotationService checks if key > 365 days old
- [ ] Automated rotation triggers monthly check
- [ ] Old ciphertexts decrypted with old key, re-encrypted with new key
- [ ] No service downtime during rotation
- [ ] Tests: 3+ test cases (rotation trigger, re-encryption)

## Compliance
- NIS2 Art. 21(4) (Key rotation requirement)

## Effort: 10 hours

## Blocked By
- P0.2.2" \
  --label "p0,encryption,backend,security" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.2.3: Issue #$ID"

# ============================================================
# P0.3: INCIDENT RESPONSE
# ============================================================
echo ""
echo "🚨 P0.3: Incident Response (< 24h Notification)"
echo "---"

# P0.3.1: Incident Detection Rules
ID=$(gh issue create --repo "$REPO" \
  --title "P0.3.1: Security Incident Detection Rules" \
  --body "Implement detection for brute force, data exfiltration, availability

## Incidents to Detect
- Brute force: > 5 failed logins from same IP in 10 min
- Data exfiltration: Unusual volume downloads (3x baseline)
- Service down: > 5 minutes
- Performance degradation: > 2x response time baseline

## Acceptance Criteria
- [ ] IncidentDetectionService with detection methods
- [ ] SecurityIncident entity with severity levels
- [ ] Tests: 6+ test cases (each incident type)

## Compliance
- NIS2 Art. 21 (Detect incidents)
- NIS2 Art. 23 (Notify within 24h)

## Effort: 12 hours" \
  --label "p0,incident-response,backend,security" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.3.1: Issue #$ID"

# P0.3.2: NIS2 Notification Service
ID=$(gh issue create --repo "$REPO" \
  --title "P0.3.2: NIS2 Authority Notification (< 24h)" \
  --body "Implement automatic notification to competent authorities

## Acceptance Criteria
- [ ] Nis2NotificationService identifies authority by country
- [ ] Notification sent < 24h after detection (automated)
- [ ] Reference number generated for tracking
- [ ] Tests: Authority lookup, notification timing

## Authorities
- Germany (DE): BSI (bsi@bsi.bund.de)
- Austria (AT): NICS (incident@nics.gv.at)
- France (FR): CERT (cert@ssi.gouv.fr)

## Compliance
- NIS2 Art. 23 (Notification requirement)

## Effort: 8 hours

## Blocked By
- P0.3.1" \
  --label "p0,incident-response,backend" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.3.2: Issue #$ID"

# ============================================================
# P0.4: NETWORK SEGMENTATION
# ============================================================
echo ""
echo "🛡️  P0.4: Network Segmentation & DDoS Protection"
echo "---"

# P0.4.1: VPC & Security Groups
ID=$(gh issue create --repo "$REPO" \
  --title "P0.4.1: VPC Segmentation (3 Subnets)" \
  --body "Create VPC with public, services, and database subnets

## Architecture
- Public Subnet: Load Balancer only
- Private Services: Microservices (no internet access)
- Private Databases: PostgreSQL, Redis, Elasticsearch (no outbound)

## Acceptance Criteria
- [ ] VPC created (10.0.0.0/16)
- [ ] 3 subnets configured with proper routing
- [ ] Security Groups: Principle of least privilege
- [ ] No direct internet access to databases

## Compliance
- NIS2 Art. 21(3) (Network segmentation)

## Effort: 12 hours" \
  --label "p0,infrastructure,devops" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.4.1: Issue #$ID"

# P0.4.2: DDoS & WAF Configuration
ID=$(gh issue create --repo "$REPO" \
  --title "P0.4.2: DDoS Protection & WAF Rules" \
  --body "Configure AWS Shield, WAF, and rate limiting

## Protection Rules
- DDoS: AWS Shield (automatic)
- WAF: SQL injection, XSS, large body blocks
- Rate limiting: 2000 req/5min per IP
- Geo-blocking: Non-EU countries blocked

## Acceptance Criteria
- [ ] AWS Shield enabled
- [ ] WAF rules deployed (5+ rules)
- [ ] Rate limiting enforced
- [ ] Tests: Rule validation

## Effort: 10 hours

## Blocked By
- P0.4.1" \
  --label "p0,infrastructure,devops,security" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.4.2: Issue #$ID"

# ============================================================
# P0.5: KEY MANAGEMENT
# ============================================================
echo ""
echo "🔑 P0.5: Key Management (Azure KeyVault)"
echo "---"

# P0.5.1: KeyVault Setup
ID=$(gh issue create --repo "$REPO" \
  --title "P0.5.1: Azure KeyVault Configuration" \
  --body "Setup KeyVault for centralized secret management

## Secrets to Store
- Encryption keys (for P0.2)
- Database connection strings
- API keys (external services)
- JWT secrets
- Certificate passwords

## Acceptance Criteria
- [ ] KeyVault provisioned in Azure
- [ ] Access policies configured (per service)
- [ ] No secrets hardcoded in code/config
- [ ] All secrets migrated to KeyVault
- [ ] Tests: Secret retrieval, access control

## Compliance
- GDPR Art. 32(1)(a) (Key management)
- NIS2 Art. 21(4)

## Effort: 10 hours" \
  --label "p0,infrastructure,devops,security" \
  --milestone "Phase 0" \
  --format json | jq -r '.number')
ISSUE_NUMBERS+=($ID)
echo "✅ P0.5.1: Issue #$ID"

# Add all issues to Planner project
echo ""
echo "📊 Adding issues to Planner project..."
for ISSUE_NUM in "${ISSUE_NUMBERS[@]}"; do
  ISSUE_ID=$(gh issue view "$ISSUE_NUM" --repo "$REPO" --json id --jq '.id')
  gh project item-add --id "$PROJECT_ID" "$ISSUE_ID" 2>/dev/null || echo "⚠️  Issue #$ISSUE_NUM already in project"
  echo "  ✅ Added issue #$ISSUE_NUM to Planner"
done

echo ""
echo "✅ Created ${#ISSUE_NUMBERS[@]} P0 issues!"
echo "📊 View Planner: https://github.com/users/HRasch/projects/5"
