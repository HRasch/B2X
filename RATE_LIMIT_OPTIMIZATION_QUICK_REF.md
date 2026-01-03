# 🚀 Rate Limit Optimized Workflow - Quick Reference

**Status**: ✅ ACTIVE - Sequential agent execution to prevent short-term rate limits

## ⚡ Current Strategy

**Problem**: 6+ concurrent agents causing burst API calls → Short-term rate limit hits at 38% monthly usage

**Solution**: Sequential workflow with cooldowns + batching

## 📋 Workflow Rules

### Session Structure
```
45 minutes: Active work (single agent)
10 minutes: Cooldown (no Copilot usage)
Maximum: 3 sessions per requirement
```

### Sequential Agent Order
```
@ProductOwner → Requirements analysis
    ↓ (10 min cooldown)
@Architect → Technical design review
    ↓ (10 min cooldown)
@Backend → Implementation
    ↓ (10 min cooldown)
@Frontend → UI integration
    ↓ (10 min cooldown)
@QA → Testing & validation
```

## 🛠️ Available Tools

### Monitoring Script
```bash
# Check current status
./scripts/rate-limit-monitor.sh status

# Continuous monitoring (5 min intervals)
./scripts/rate-limit-monitor.sh monitor

# Show usage statistics
./scripts/rate-limit-monitor.sh stats
```

### Optimized Prompts
- `/start-feature` → Single agent initiation
- `/requirements-analysis-single` → Rate limit optimized analysis
- `/code-review` → Sequential review process

## 📊 Success Metrics

| Metric | Target | Current Status |
|--------|--------|----------------|
| Concurrent agents | ≤ 2 | ✅ Implemented |
| Session length | ≤ 45 min | ✅ Implemented |
| Cooldown periods | ≥ 10 min | ✅ Implemented |
| Monthly usage | < 80% | 🔄 Monitoring |

## 🚨 Emergency Protocols

### If Rate Limited:
1. **Stop all Copilot usage immediately**
2. **Wait 15-30 minutes**
3. **Resume with single-agent workflow**
4. **Monitor with script**: `./scripts/rate-limit-monitor.sh monitor`

### Prevention:
- Never trigger multiple agents simultaneously
- Use text-based coordination via `.ai/` files
- Archive old files regularly
- Batch operations instead of individual edits

## 📁 Key Files

| File | Purpose | Status |
|------|---------|--------|
| `.ai/guidelines/GL-007-rate-limit-optimization.md` | Complete strategy | ✅ Created |
| `.ai/prompts/requirements-analysis-single.prompt.md` | Single-agent analysis | ✅ Created |
| `scripts/rate-limit-monitor.sh` | Usage monitoring | ✅ Created |
| `.github/agents/*.agent.md` | Updated configurations | ✅ Updated |

## 🔄 Next Steps

1. **Monitor effectiveness** over next week
2. **Adjust cooldown periods** based on results
3. **Fine-tune session lengths** for optimal productivity
4. **Create automated alerts** for high activity

## 📞 Support

- **Rate limit hit**: Run monitoring script, wait 15+ min, resume sequentially
- **High activity warning**: Complete current work, take cooldown
- **Strategy questions**: Reference GL-007 or ask @SARAH

---

**Last Updated**: Dec 30, 2025
**Strategy**: Sequential execution with cooldowns
**Goal**: Sustainable multi-agent development without rate limits