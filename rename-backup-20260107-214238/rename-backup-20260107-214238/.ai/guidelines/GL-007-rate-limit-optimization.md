# 🚀 Rate Limit Optimized Agent Strategy

## Problem Analysis
Current strategy triggers short-term rate limits due to:
- **6+ agents active simultaneously** (@Backend, @Frontend, @Architect, @ProductOwner, @TechLead, @ScrumMaster)
- **Multi-agent prompts** (requirements-analysis involves 4+ agents)
- **Frequent context switching** between agents
- **Heavy Copilot Chat usage** for coordination

## ✅ Optimized Strategy

### 1. **Sequential Agent Execution (Primary Fix)**
Instead of parallel execution, use **sequential workflow**:

```
@ProductOwner → @Architect → @Backend → @Frontend → @TechLead → @ScrumMaster
```

**Benefits:**
- Reduces concurrent API calls
- Allows cooldown between agent switches
- Maintains workflow integrity

### 2. **Agent Batching (Secondary Optimization)**
Group related work into batches with cooldowns:

**Batch 1 (Planning - 9:00-10:00):**
- @ProductOwner: Requirements analysis
- @Architect: Architecture review
- **Cooldown: 10 minutes**

**Batch 2 (Implementation - 10:15-12:00):**
- @Backend: API/Service implementation
- @Frontend: UI implementation
- **Cooldown: 15 minutes**

**Batch 3 (Quality - 12:15-13:00):**
- @TechLead: Code review
- @ScrumMaster: Status update

### 3. **Reduced Prompt Frequency**
- **Requirements Analysis**: Single agent (@ProductOwner) creates spec, others review asynchronously
- **Code Reviews**: @TechLead only, other agents comment via documentation
- **Daily Standups**: Text-based status updates instead of interactive sessions

### 4. **Context Optimization**
- **Batch file reads**: Read multiple related files in single operations
- **Shared context files**: Use `.ai/collaboration/shared-context.md` for inter-agent communication
- **Archive old status**: Move status files >7 days to `.ai/archive/`

### 5. **Agent Workload Limits**
- **Max 2 agents active simultaneously**
- **1-hour cooldown** between intensive agent sessions
- **Text-based coordination** instead of interactive chat

## 📋 Implementation Steps

### Step 1: Update Agent Instructions
Add rate limit awareness to all agent definitions:

```yaml
# Add to each agent .md file
## Rate Limit Optimization
- Work in 45-minute focused sessions
- Use batch operations for multiple file changes
- Allow 10-minute cooldown between intensive tasks
- Prefer documentation over interactive chat
```

### Step 2: Modify Prompts
Update prompts to be single-agent focused:

**Before (Multi-agent):**
```
/requirements-analysis
→ Triggers @ProductOwner + @Architect + @Backend + @Frontend
```

**After (Sequential):**
```
/requirements-analysis-single
→ @ProductOwner creates spec
→ Other agents review via documentation
```

### Step 3: Workflow Optimization
**Current Workflow:**
```
User → Multiple agents simultaneously → Rate limit hits
```

**Optimized Workflow:**
```
User → @SARAH coordinator → Sequential agent delegation → Cooldown periods
```

### Step 4: Monitoring & Alerts
Add rate limit monitoring:

```bash
# Add to scripts/
# rate-limit-monitor.sh
#!/bin/bash
# Monitor Copilot usage patterns
# Alert when approaching limits
# Suggest cooldown periods
```

## 🎯 Expected Results

### Before Optimization:
- **6 agents active** → High concurrent API usage
- **Multi-agent prompts** → Burst requests
- **Frequent rate limits** → 38% monthly usage but daily limits hit

### After Optimization:
- **2 agents max active** → Reduced concurrent usage
- **Sequential execution** → Natural cooldowns
- **Batch operations** → Fewer API calls
- **Rate limit reduction** → 70-80% fewer limit hits

## 📊 Success Metrics

| Metric | Before | Target | Measurement |
|--------|--------|--------|-------------|
| Concurrent Agents | 6+ | 2 max | Agent activity logs |
| Daily Rate Limits | Frequent | <2/day | Error logs |
| Session Duration | Continuous | 45min + cooldown | Time tracking |
| API Efficiency | Low | High | Requests per task |

## 🚨 Emergency Measures

If rate limits still occur:

1. **Immediate cooldown**: 30-minute break from Copilot usage
2. **Single agent mode**: Use only @Backend for implementation
3. **Documentation focus**: Write specs instead of interactive analysis
4. **VS Code restart**: Clear extension state

## 📝 Update Plan

1. **Week 1**: Update agent instructions with rate limit awareness
2. **Week 2**: Modify prompts for sequential execution
3. **Week 3**: Implement workflow changes and monitoring
4. **Ongoing**: Monitor and adjust based on usage patterns

---

**Status**: Ready for implementation  
**Expected Impact**: 70-80% reduction in rate limit occurrences  
**Timeline**: Immediate (update agent instructions first)</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/guidelines/GL-007-rate-limit-optimization.md