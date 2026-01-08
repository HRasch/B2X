---
docid: UNKNOWN-181
title: Deploy.Prompt
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# 🚀 DEPLOY - Deployment & Release Management

**Trigger**: Feature/release ready for deployment
**Coordinator**: @DevOps
**Output**: Deployment confirmation, release notes, rollback procedure

---

## Quick Start
```
@DevOps: /deploy
Environment: [staging | production]
Version: [x.y.z semantic version]
Components: [List what's being deployed]
Type: [feature | hotfix | maintenance]
```

---

## Pre-Deployment Checklist

### Code Readiness (@TechLead)
- [ ] All tests passing (unit, integration, e2e)
- [ ] Code review completed and approved
- [ ] No outstanding review comments
- [ ] Test coverage ≥ 80%
- [ ] Performance benchmarks met
- [ ] Security review passed

### Release Readiness (@ProductOwner)
- [ ] Feature spec complete
- [ ] Acceptance criteria verified
- [ ] Documentation updated
- [ ] User guide prepared
- [ ] Changelog entry created
- [ ] Stakeholder sign-off obtained

### Security & Compliance (@Security)
- [ ] Security audit passed
- [ ] No critical/high-risk vulnerabilities
- [ ] Secrets properly managed
- [ ] Compliance requirements verified
- [ ] Data protection verified
- [ ] Backup/recovery tested

### Infrastructure (@DevOps)
- [ ] Target environment verified
- [ ] Database migrations prepared
- [ ] Configuration reviewed
- [ ] Load capacity verified
- [ ] Monitoring configured
- [ ] Alerting rules tested

### Operations (@ScrumMaster)
- [ ] Deployment window scheduled
- [ ] Stakeholders notified
- [ ] Support team briefed
- [ ] Rollback procedure documented
- [ ] Communication plan ready
- [ ] Incident response plan activated

---

## Deployment Process

### Phase 1️⃣: Pre-Deployment
```bash
# Verify environment
./scripts/health-check.sh staging

# Database backup
./scripts/backup-database.sh staging

# Load test (optional)
./scripts/load-test.sh staging --users 100
```

### Phase 2️⃣: Staging Deployment
```bash
# Deploy to staging
./scripts/deploy.sh staging v1.2.3

# Run smoke tests
npm run e2e:smoke

# Verify functionality
./scripts/verify-deployment.sh staging

# Performance baseline
./scripts/benchmark.sh staging
```

### Phase 3️⃣: Production Deployment

#### Blue-Green Deployment
```bash
# Deploy to green environment
./scripts/deploy.sh production-green v1.2.3

# Run verification suite
./scripts/verify-deployment.sh production-green

# Switch traffic
./scripts/switch-traffic.sh green

# Monitor metrics
./scripts/monitor.sh --duration 1h
```

#### Canary Deployment
```bash
# Deploy to 10% of traffic
./scripts/deploy-canary.sh production v1.2.3 --traffic 10%

# Monitor error rates and latency
./scripts/monitor-canary.sh

# If good, gradually increase
./scripts/deploy-canary.sh production v1.2.3 --traffic 50%
./scripts/deploy-canary.sh production v1.2.3 --traffic 100%
```

### Phase 4️⃣: Post-Deployment Verification

```bash
# Health checks
curl -f https://api.B2X.de/health

# Database integrity
./scripts/verify-database.sh production

# Log monitoring
./scripts/check-logs.sh production --errors

# User reports
# Wait 30-60 minutes for user feedback
```

### Phase 5️⃣: Finalization

```bash
# Mark deployment complete
./scripts/mark-deployed.sh v1.2.3

# Update documentation
./scripts/update-deployment-doc.sh

# Archive logs
./scripts/archive-deployment-logs.sh v1.2.3
```

---

## Rollback Procedure

### Immediate Rollback (Critical Issues)

```bash
# 1. Assess severity
if [critical_issue]; then
  # 2. Notify stakeholders
  ./scripts/notify-incident.sh CRITICAL

  # 3. Prepare rollback
  ./scripts/prepare-rollback.sh staging

  # 4. Execute rollback
  ./scripts/deploy.sh production v1.2.2  # Previous version

  # 5. Verify
  ./scripts/verify-deployment.sh production

  # 6. Incident review
  ./scripts/incident-review.sh rollback
fi
```

### Rollback Criteria
- [ ] Error rate > 5% (from 0.5% baseline)
- [ ] Response time > 2x normal
- [ ] Failed critical functionality
- [ ] Data corruption detected
- [ ] Security incident discovered
- [ ] Compliance violation detected

### Rollback Communication
1. Immediate notification to stakeholders
2. Public status update if external system
3. Post-incident review within 24 hours
4. Root cause analysis
5. Prevention measures documentation

---

## Deployment Documentation

### Release Notes Template
```markdown
# Release v1.2.3 - 2025-01-15

## New Features
- [Feature 1]: [Description]
- [Feature 2]: [Description]

## Bug Fixes
- [Bug 1]: [Description]
- [Bug 2]: [Description]

## Breaking Changes
- [Change 1]: [How to migrate]

## Deprecations
- [Deprecated feature]: [Alternative]

## Performance Improvements
- [Improvement 1]: [Metric]
- [Improvement 2]: [Metric]

## Dependencies
- [Updated library]: v1.2.3 → v1.3.0

## Migration Guide
[Step-by-step for users]

## Known Issues
- [Issue 1]: [Workaround]

## Support
[Support contact information]
```

### Deployment Log Template
```markdown
# Deployment Log - v1.2.3

## Timeline
- 10:00 - Deployment started
- 10:05 - Database migrations completed
- 10:08 - Blue-green switch executed
- 10:15 - Health checks passed
- 10:30 - Post-deployment verification complete
- 10:45 - Deployment completed

## Metrics
- Deployment duration: 45 minutes
- Health check success: 100%
- Error rate post-deployment: 0.2%
- Response time (p95): 245ms
- User reports: 0 issues

## Changes Deployed
- Backend service: v1.2.3
- Frontend assets: v1.2.3
- Database schema: v12

## Sign-Off
- DevOps: [Name] ✅
- ScrumMaster: [Name] ✅
- ProductOwner: [Name] ✅
```

---

## Post-Deployment Monitoring (24 hours)

| Metric | Normal | Alert | Critical |
|---|---|---|---|
| Error Rate | < 1% | 2-5% | > 5% |
| Response Time (p95) | < 200ms | 200-500ms | > 500ms |
| CPU Usage | < 60% | 70-85% | > 85% |
| Memory Usage | < 70% | 75-90% | > 90% |
| DB Connections | < 50 | 70-80 | > 80 |

---

## Approval Workflow

1. **@DevOps** - Deployment execution
2. **@TechLead** - Pre-deployment approval
3. **@Security** - Security clearance
4. **@ScrumMaster** - Go/no-go decision
5. **@ProductOwner** - Business sign-off

**Deployment Condition**: All checks pass, stakeholders approved, monitoring active
