---
docid: KB-QR-001
title: MCP Token Optimization - Team Quick Reference
owner: "@TechLead"
status: Active
updated: 7. Januar 2026
---

# 🚀 MCP Token Optimization - Team Quick Reference

**For complete training**, see `.ai/knowledgebase/mcp-team-training-guide.md`

---

## ⚡ Your Daily Workflow

### 1. **Before Your Day Starts**
```bash
# Check MCP health
cat .ai/logs/mcp-usage/dashboard-text.txt

# Review overnight issues (if any)
tail -20 .ai/logs/mcp-usage/rate-limit-alerts.log
```

### 2. **During Development**
- ✅ Write code normally - MCP servers work transparently
- ✅ Run your typical analysis commands
- ⚠️ If you hit rate limits, pre-commit hook will tell you
- ⚠️ Fix issues before committing

### 3. **Before Git Commit**
```bash
git add .
git commit -m "feat: description"
# Pre-commit hook automatically validates:
# ✓ Rate limits not exceeded
# ✓ Cache is healthy
# ✓ No security files included
```

---

## 📊 Monitor MCP Health (3 Commands)

### See Current Status
```bash
node scripts/mcp-metrics-dashboard.js status
```
**Shows**: Cache hits, rate limit usage, active servers

### View Rate Limits Per Server
```bash
node scripts/mcp-rate-limiter.js summary
```
**Shows**: 13 servers with remaining tokens for today

### Check Yesterday's Report
```bash
tail -50 .ai/logs/mcp-usage/daily-reviews/latest-report.md
```
**Shows**: Full metrics, cache stats, alerts

---

## 🛑 If Rate Limit Is Exceeded

### Option 1: Wait for Reset (Recommended)
- Rate limits reset daily at **00:00 UTC**
- Most European developers can continue at 01:00 CET
- Server will auto-reject requests until reset

### Option 2: Check Which Server Is Maxed
```bash
node scripts/mcp-rate-limiter.js summary
```
**Look for**: ⚠️ Orange status (>80%) or 🔴 Red (100%)

### Option 3: Understand Your Usage
```bash
node scripts/mcp-audit-trail.js report --server typescript
```
**Shows**: Your recent calls to that server

---

## 💾 Cache Behavior (Automatic)

### How Cache Works
- ✅ File not changed since yesterday → **cache hit** (instant, no tokens)
- ✅ File changed → **cache miss** (fresh analysis, uses tokens)
- ✅ Results expire after 7 days → re-analyze automatically

### No Configuration Needed
- Cache is transparent
- Same commands work, just faster
- Typical cache hit rate: **40-60%** (varies by project)

---

## 🚨 Troubleshooting (5 Most Common Issues)

### "Rate limit exceeded for TypeScript"
```bash
# Check when it resets
node scripts/mcp-rate-limiter.js summary | grep typescript

# Or wait ~1 hour for reset at UTC midnight
# Then commit
```
**Prevention**: Run `node scripts/mcp-metrics-dashboard.js status` before intensive work

### "Cache hit rate is 0%"
```bash
# This is normal when changing many files
# Cache will warm up after a few commits
# Check disk space in .ai/cache/mcp/
ls -lh .ai/cache/mcp/ | head -20
```

### "Pre-commit hook rejected my commit"
```bash
# Read the error message - it says what's wrong
# Run the same check manually:
node scripts/mcp-rate-limiter.js check

# If rate limit, wait for reset (UTC midnight)
# If cache issue, clear and rebuild:
rm -rf .ai/cache/mcp/*
node scripts/mcp-cache-manager.js cleanup
```

### "Dashboard shows 'N/A' for some metrics"
```bash
# This is normal on first run
# Metrics build up over 24 hours
# Check again tomorrow
# Or run: node scripts/mcp-metrics-dashboard.js generate
```

### "I think MCP server crashed"
```bash
# Check audit trail for errors
node scripts/mcp-audit-trail.js report --event ERROR --limit 20

# See console logs
tail -100 .ai/logs/mcp-usage/mcp-console.log | grep ERROR

# If critical, escalate to @TechLead
```

---

## 📞 When to Escalate

### @TechLead
- Rate limits keep getting exceeded (possible misconfiguration)
- Multiple servers failing (possible MCP server issue)
- Need to adjust daily limits for your team
- Questions about optimization strategy

### @DevOps
- Cron job not running (automated daily reviews missing)
- Git hook installation issues
- Need to scale limits for production deployment

### @SARAH
- Permission changes needed
- Policy changes requested
- Conflict with development workflow

---

## ✅ Checklist: You're Set Up When...

- [ ] Pre-commit hook installed (should block on your next commit if rate limit exceeded)
- [ ] Can run `git commit` successfully
- [ ] Can read `.ai/logs/mcp-usage/daily-reviews/latest-report.md`
- [ ] Understand the 3 monitor commands (status, summary, report)
- [ ] Know the rate limit reset time (UTC midnight)

---

## 📚 Need More Info?

| Topic | Reference |
|-------|-----------|
| **Complete Training** | `.ai/knowledgebase/mcp-team-training-guide.md` |
| **Best Practices** | `.ai/knowledgebase/mcp-token-optimization-lessons.md` |
| **Status/Roadmap** | `.ai/status/mcp-optimization.md` |
| **Rate Limits** | `scripts/mcp-rate-limiter.js summary` |
| **Cache Status** | `node scripts/mcp-cache-manager.js stats` |

---

## 🎯 Token Budget (Daily Limits Per Server)

| Server | Daily Limit | Type |
|--------|------------|------|
| TypeScript | 500 | Type Analysis |
| Vue | 400 | Component Analysis |
| Security | 300 | Vulnerability Scan |
| Performance | 150 | Performance Analysis |
| HTML/CSS | 150 | UI Analysis |
| Database | 150 | Schema Validation |
| Git | 100 | Commit Analysis |
| Docker | 100 | Container Analysis |
| Documentation | 100 | Doc Generation |
| B2Connect | 100 | Domain Analysis |
| Roslyn | 0 | **DISABLED** |
| Wolverine | 0 | **DISABLED** |
| Chrome DevTools | 0 | **DISABLED** |

**Total Daily Budget**: ~2,000 tokens (saves ~40% vs. unrestricted)

---

**Last Updated**: 7. Januar 2026  
**Questions?** → See section "When to Escalate"
