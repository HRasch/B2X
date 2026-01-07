---
docid: KB-MCP-TEAM-TRAINING
title: MCP Token Optimization - Team Training Guide
owner: "@TechLead"
status: Active
date: 2026-01-07
---

# MCP Token Optimization - Team Training Guide

## Überblick

B2X hat ein umfassendes **MCP Token Optimization System** implementiert, das die Token-Nutzung um 60-80% reduziert. Dieses Trainings-Guide ermöglicht jedem Team-Mitglied, das System effektiv zu nutzen.

---

## 🎯 Quick Start (5 Minuten)

### Für Daily Development

```bash
# 1. Vor dem Commit: Pre-Commit Hook läuft automatisch
git commit -m "feat: add new component"
# Hook validiert Limits und Cache automatisch ✓

# 2. Morgens: Check Dashboard
node scripts/mcp-metrics-dashboard.js print

# 3. Bei Fragen: Consult Status
cat .ai/status/mcp-optimization.md
```

### Kritische Dateien

| Datei | Zweck |
|-------|-------|
| `.vscode/mcp.json` | MCP Server Konfiguration |
| `scripts/mcp-*.js` | Optimization Tools |
| `.ai/status/mcp-optimization.md` | Status & Progress |
| `.ai/logs/mcp-usage/` | Logs & Reports |

---

## 📚 Detailliertes Training

### Modul 1: Verstehen der MCP-Architektur (15 Min)

**Was sind MCP-Server?**
- TypeScript MCP: Frontend Code-Analyse (Vue, TS)
- Security MCP: Sicherheits-Scanning (XSS, SQL-Injection)
- Performance MCP: Performance-Profiling
- Vue MCP: Vue-spezifische Validierung (i18n, Responsive)
- Und 8 weitere spezialisierte Server

**Warum Token Optimization wichtig ist:**
- Jeder MCP-Aufruf kostet Tokens (KI-API Kosten)
- Unbegrenzte Nutzung führt zu exponentiellen Kosten
- Optimization reduziert Kosten um 60-80%

**4-Säulen-Architektur:**
1. **Selective Activation**: Nur aktive Server nutzen
2. **Caching**: Gleiche Dateien nicht 2x scannen
3. **Rate Limiting**: Pro-Server Limits setzen
4. **Monitoring**: Realtime Visibility

---

### Modul 2: Tägliche Workflow-Integration (10 Min)

#### Scenario 1: Bug Fix Branch

```bash
# 1. Create branch
git checkout -b bugfix/user-auth

# 2. Make changes
vim src/api/auth.cs
vim frontend/Store/modules/auth.ts

# 3. Commit (pre-commit hook runs automatically)
git commit -m "fix: resolve JWT token expiration bug"

# Output:
# ✓ Rate limits OK (Security MCP: 150/300)
# ✓ Cache hit on similar analysis (+15 tokens saved!)
# ✓ Activity logged for audit

# 4. Monitor during development (optional)
node scripts/mcp-metrics-dashboard.js print

# Shows:
# - Security MCP: 2 cache hits, 1 miss
# - Cache efficiency: 66% hit rate
# - Rate limit: 50% used today
```

#### Scenario 2: Feature Branch with Security-Critical Code

```bash
# Large refactoring with auth changes
git checkout -b feature/oauth2-integration

# Edit multiple auth files
# ... code changes ...

# Before commit: Check if security MCP budget available
node scripts/mcp-rate-limiter.js check security-mcp 100

# Response:
# {
#   "allowed": true,
#   "remaining": 120,
#   "limit": 300
# }

git commit -m "feat: add OAuth2 integration"

# Pre-commit hook:
# ✓ Security files detected - validating...
# ✓ 2 cache hits (similar OAuth patterns)
# ✓ 3 new security checks run (45 tokens)
# ✓ Audit logged for compliance
```

---

### Modul 3: Monitoring & Reporting (10 Min)

#### Daily Report (Morning Standup)

```bash
# Runs automatically at 09:00 via cron
# Or manually:
bash scripts/daily-mcp-review.sh

# Output includes:
# - Current rate limit status
# - Cache hit efficiency
# - Any alerts or warnings
# - Recommendations for action
```

#### Weekly Review

```bash
# Check audit trail for the week
node scripts/mcp-audit-trail.js report weekly

# Shows:
# - Total token usage (target: <1200/week)
# - Server-by-server breakdown
# - Error patterns & issues
# - Compliance status
```

#### Dashboard Access

```bash
# Real-time metrics
node scripts/mcp-metrics-dashboard.js print

# HTML dashboard (for sharing)
open .ai/logs/mcp-usage/dashboard.html
```

---

### Modul 4: Troubleshooting & Recovery (15 Min)

#### Scenario: Rate Limit Exceeded

**Problem:** Pre-commit hook blocked commit due to exceeded limit

```bash
# Check which server is over limit
node scripts/mcp-rate-limiter.js summary

# Output:
# ⚠️ security-mcp: 295/300 (98.3%)
# ✓ typescript-mcp: 120/500 (24%)
```

**Solutions:**

Option 1: Wait for daily reset (24:00 UTC)
```bash
# Check when reset happens
node scripts/mcp-rate-limiter.js summary
# Shows: Reset in X hours

# Or force reset if emergency
node scripts/mcp-rate-limiter.js reset
```

Option 2: Contact @Security for exception
```bash
# Document reason for override
echo "Emergency production hotfix - SQL injection vulnerability" | tee override-reason.txt

# Request permission from @Security
# They can temporarily increase limit for critical issues
```

#### Scenario: Cache Not Working

**Problem:** Same file scanned twice, tokens wasted

```bash
# Check cache status
node scripts/mcp-cache-manager.js stats

# Output shows:
# typescript-mcp: 0 hits, 5 misses (0% hit rate)

# Likely causes:
# 1. File changed between scans
# 2. Cache directory corrupted
# 3. Different MCP parameters

# Solutions:
# 1. Verify file hasn't changed
git diff src/components/Button.vue

# 2. Clear and rebuild cache
node scripts/mcp-cache-manager.js clear-expired
# or
node scripts/mcp-cache-manager.js clear-all

# 3. Check logs for errors
cat .ai/logs/mcp-usage/rate-limit-alerts.log
```

---

### Modul 5: Best Practices & Dos/Don'ts (10 Min)

#### ✅ DO

- **DO**: Commit frequently (cache benefits from stable patterns)
- **DO**: Check dashboard before heavy code review sessions
- **DO**: Use pre-commit hook (it prevents problems automatically)
- **DO**: Report anomalies to @Security or @TechLead
- **DO**: Batch similar changes (better cache utilization)
- **DO**: Review daily reports during standups

#### ❌ DON'T

- **DON'T**: Disable pre-commit hook
- **DON'T**: Force reset rate limits without @Security approval
- **DON'T**: Ignore rate limit warnings (they escalate)
- **DON'T**: Run MCP servers outside of defined workflows
- **DON'T**: Commit to `.ai/logs/mcp-usage/` (auto-generated)
- **DON'T**: Try to "game" the system with manual limit adjustments

---

## 🔧 Configuration Management

### For Developers

**Standard Configuration** (No changes needed):
- `.vscode/mcp.json`: Keep defaults
- Rate Limits: No overrides
- Caching: Auto-enabled

**When to Request Changes:**
- Need higher rate limits? → Contact @Security
- Want to enable disabled server? → Contact @CopilotExpert
- Need custom caching strategy? → Contact @TechLead

### For Admin/Lead Roles

**Adjust Rate Limits:**
```bash
# Edit rate limits in code
vim scripts/mcp-rate-limiter.js
# Change DEFAULT_LIMITS object

# Reload
npm run mcp-rates-reset
```

**Enable Optional Servers:**
```bash
# In .vscode/mcp.json
"roslyn-code-navigator": {
  "disabled": false  // Enable for refactoring
}

# Reload VS Code
```

---

## 📞 Support & Escalation

### Quick Questions

- **"How much budget do I have left?"** → `node scripts/mcp-rate-limiter.js summary`
- **"Is caching working?"** → `node scripts/mcp-cache-manager.js stats`
- **"What's wrong?"** → `bash scripts/daily-mcp-review.sh`

### Escalation Path

| Issue | Contact | Response Time |
|-------|---------|----------------|
| Questions | @TechLead | During work hours |
| Rate limit exception | @Security | 1 hour |
| MCP configuration | @CopilotExpert | 2 hours |
| System emergency | @SARAH | Immediate |

### Support Channels

```bash
# Create support ticket with detailed info
echo "Issue: [description]" > /tmp/mcp-support.txt
echo "Error: [error message]" >> /tmp/mcp-support.txt
node scripts/mcp-audit-trail.js report daily >> /tmp/mcp-support.txt

# Share with team on Slack/Discord
```

---

## 📊 Success Metrics

**You know the system is working when:**

| Metric | Target | Your Status |
|--------|--------|-------------|
| Pre-commit hook blocking commits | <1 per week | ___ |
| Cache hit rate | >40% | ___ |
| Rate limit warnings | <2 per week | ___ |
| Daily reports completed | 100% | ___ |
| Zero audit trail gaps | 100% | ___ |

---

## 🚀 Advanced Topics (Optional)

### A/B Testing New Strategies

```bash
# Create test configuration
node scripts/mcp-ab-testing.js create caching_only

# Run for 24 hours...

# Compare results
node scripts/mcp-ab-testing.js compare
node scripts/mcp-ab-testing.js results
```

### Custom Audit Queries

```bash
# Find all errors from specific server
node scripts/mcp-audit-trail.js query security-mcp

# Generate compliance report
node scripts/mcp-audit-trail.js report weekly > compliance-report.txt
```

### Performance Profiling

```bash
# Analyze token usage patterns
node scripts/mcp-metrics-dashboard.js json | jq '.rateLimits'

# Find slowest servers
cat .ai/logs/mcp-usage/metrics.json | jq '.cache.servers | sort_by(.totalTokens)'
```

---

## ✅ Certification Checklist

Complete this checklist to certify you understand the system:

- [ ] Can access daily report without help
- [ ] Understand what pre-commit hook does
- [ ] Know how to check rate limit status
- [ ] Can interpret cache statistics
- [ ] Know how to escalate issues
- [ ] Have read complete lessons document
- [ ] Can explain 4-pillar architecture
- [ ] Know when to contact @Security vs @TechLead

**Certification Complete?** Mark date: ___________

---

## 📋 Quick Reference Card

```
┌─ DAILY COMMANDS ────────────────────┐
│ Morning:  daily-mcp-review.sh       │
│ Check:    mcp-metrics-dashboard.js  │
│ Limits:   mcp-rate-limiter.js       │
│ Cache:    mcp-cache-manager.js      │
│ Audit:    mcp-audit-trail.js        │
└─────────────────────────────────────┘

┌─ HELP CONTACTS ─────────────────────┐
│ General Questions: @TechLead        │
│ Rate Limit Issues: @Security        │
│ Configuration: @CopilotExpert       │
│ Emergencies: @SARAH                 │
└─────────────────────────────────────┘

┌─ KEY FILES ─────────────────────────┐
│ Config:    .vscode/mcp.json         │
│ Status:    .ai/status/               │
│ Logs:      .ai/logs/mcp-usage/      │
│ Scripts:   scripts/mcp-*.js         │
└─────────────────────────────────────┘
```

---

## 📚 Related Documentation

- [MCP Operations Guide](mcp-operations.instructions.md) - Comprehensive MCP reference
- [Token Optimization Lessons](mcp-token-optimization-lessons.md) - Technical deep-dive
- [Copilot Instructions](copilot-instructions.md) - Global AI behavior rules
- [Security Instructions](security.instructions.md) - Security best practices

---

**Training Version**: 1.0  
**Last Updated**: 7. Januar 2026  
**Certification Expiry**: 7. Januar 2027  
**Contact**: @TechLead for updates or clarifications
