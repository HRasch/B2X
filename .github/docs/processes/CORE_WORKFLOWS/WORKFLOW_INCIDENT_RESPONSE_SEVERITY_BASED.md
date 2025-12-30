# WORKFLOW_INCIDENT_RESPONSE - Severity-Based (Not Time-Based)

**Owner**: @process-assistant  
**Last Updated**: 29. Dezember 2025  
**Version**: 2.0 - AGENT-DRIVEN SEVERITY MODEL  
**Status**: ACTIVE - Replaces time-based model

---

## 🎯 Core Principle

**Incident severity determines priority, not time. Highest severity = work stops and responds. Severity drives response, not clock.**

---

## 📋 Severity Levels

### **🔴 CRITICAL - Production Down**

**Criteria**:
- System unavailable or seriously degraded (>50% affected)
- Data loss or corruption
- Security breach
- Affects majority of tenants/users

**Immediate Action**:
- Page @devops-engineer + @tech-lead + @security-engineer immediately
- Stop all other work
- War room in Slack #incident-critical-[id]
- Triage within 5 minutes
- Mitigate within 15 minutes
- Resolve within 30 minutes (if possible)

**Example**: Production database down, encryption keys compromised, API returning 500s for all users

---

### **🟠 HIGH - Significant Issue**

**Criteria**:
- Feature broken for subset of users
- Performance degraded significantly (response time 3x+ normal)
- Security issue affecting subset of data
- Recurring failures

**Immediate Action**:
- Notify @tech-lead + @devops-engineer immediately
- Context switch required
- Triage within 30 minutes
- Resolve within 4 hours (target)

**Example**: Search not working for new products, login rate-limiting too aggressive, 1 customer's data exposed

---

### **🟡 MEDIUM - Workaround Available**

**Criteria**:
- Feature broken but workaround exists
- Performance issue with manual fix available
- Minor data inconsistency
- Affects small subset

**Immediate Action**:
- Create GitHub issue
- Notify @tech-lead in Slack
- Triage within 4 hours
- Resolve within 24 hours

**Example**: Sort function on dashboard broken (but manual filtering works), one customer invoice format incorrect

---

### **🟢 LOW - Cosmetic/Non-Urgent**

**Criteria**:
- UI issue (button color, spacing)
- Minor wording issue
- No user impact
- No data affected

**Immediate Action**:
- Create GitHub issue
- Add to backlog
- No immediate notification required
- Resolve within 1 week or add to next iteration

**Example**: Button text typo, dashboard button slightly misaligned, unused error code in logs

---

## 🔄 Incident Response Flow

### **Phase 1: Detection & Alert** (Immediate)

**Trigger**:
- Automated monitoring alert
- User report
- Team member discovery
- Error log spike

**Action**:
1. Confirm the issue (not false positive)
2. Classify severity level
3. Create incident channel: Slack #incident-[severity]-[brief-id]
4. Document what's broken in channel

**Example**:
```
@devops: "CRITICAL: Production database connection failing. API returning 500s."
@tech-lead: "Confirmed. Connection pool exhausted. Starting investigation."
```

---

### **Phase 2: Triage & Escalation**

**Owner**: First responder  
**Actions**:
1. Determine scope (how many users affected?)
2. Assess data impact (is data lost?)
3. Identify business impact (customer revenue loss, reputation, etc.)
4. Page on-call team based on severity
5. Document investigation findings

**Decision Tree**:
- Is production down? → CRITICAL → Page everyone
- Is customer data lost? → CRITICAL → Page everyone
- Is security compromised? → CRITICAL → Page security engineer
- Are users blocked (no workaround)? → HIGH → Page tech-lead
- Workaround exists? → MEDIUM → Notify tech-lead

---

### **Phase 3: Immediate Response** (Within Severity Window)

**Owner**: @devops-engineer (infrastructure) + @tech-lead (code)

**CRITICAL** (next 5-15 minutes):
- Fast temporary fix (even if not perfect)
- Restart services
- Switch to backup
- Roll back recent changes
- Scale up capacity

**HIGH** (within 30 min):
- Investigate root cause
- Apply targeted fix
- Test in staging
- Deploy to production
- Verify fix working

**MEDIUM** (within 4 hours):
- Scheduled fix (doesn't interrupt current work)
- Proper testing required
- Deploy during normal deployment window

**LOW** (add to backlog):
- Not urgent
- Schedule in normal flow
- Can wait for next iteration

---

### **Phase 4: Root Cause Analysis** (During/After Resolution)

**Owner**: @tech-lead or incident commander  
**Timeline**: While resolving (CRITICAL) or within 1 hour (HIGH/MEDIUM)

**Questions**:
1. What was the root cause?
2. Why didn't monitoring catch this?
3. Why did failover not work?
4. How could we prevent this?

**Output**: Document in incident issue comments

---

### **Phase 5: Resolution & Verification** (Severity-Dependent)

**Owner**: @devops-engineer + @tech-lead  
**Process**:
1. Apply fix to main branch
2. Tests passing (100%)
3. Deploy to production
4. Smoke tests: Can users login? Can they complete basic workflows?
5. Monitor metrics: Error rate normal? Response times normal? DB healthy?

**Exit Criteria**: System back to normal operation, metrics green

---

### **Phase 6: Post-Incident Action** (Within 24 Hours)

**Owner**: @tech-lead  
**Process**:
1. **War Room** (within 1 hour of resolution)
   - Incident commander reviews timeline
   - Team discusses what happened
   - Document findings

2. **Action Items** (within 24 hours)
   - Prevent recurrence (code fix? monitoring? automation?)
   - Update documentation
   - Update runbooks
   - Assign owners and deadlines

3. **Retrospective** (next iteration)
   - Add incident learning to retrospective
   - Track closure of action items

**Example Action Items**:
- [ ] Add monitoring alert for connection pool exhaustion (@devops-engineer, due 3 days)
- [ ] Implement connection pool timeout handling (@backend-developer, due 5 days)
- [ ] Update scaling automation runbook (@devops-engineer, due 2 days)
- [ ] Add integration test for connection failure scenarios (@qa-engineer, due 1 week)

---

## 🎯 No Time-Based Expectations

### **Severity Drives Response, Not Time**

**OLD MODEL** (time-based):
- CRITICAL: 5 min response, 15 min mitigate, 30 min resolve
- HIGH: 30 min response, 4 hours resolve
- MEDIUM: 4 hours response, 24 hours resolve
- LOW: 24 hours response, 2 week resolve

**NEW MODEL** (severity-driven):
- CRITICAL: Stop everything, respond immediately, best effort
- HIGH: Context switch, triage in 30 min, resolve next 4 hours
- MEDIUM: Investigate, schedule fix, resolve within 24 hours
- LOW: Add to backlog, handle in normal flow

**Key Difference**: We respond based on severity, not time targets. A CRITICAL issue at 3 PM on Friday = immediate response. A LOW issue at 3 PM on Friday = add to Monday backlog.

---

## 📋 Incident Checklist

### **Before Page On-Call**:
- [ ] Confirmed this is real incident (not false positive)
- [ ] Classified severity level
- [ ] Assessed scope (how many users?)
- [ ] Documented what's broken

### **During Response**:
- [ ] Immediate mitigation in place
- [ ] Root cause being investigated
- [ ] Team updated in Slack channel
- [ ] Customers notified (if applicable)

### **After Resolution**:
- [ ] System verified working
- [ ] Metrics showing normal operation
- [ ] War room completed
- [ ] Action items assigned
- [ ] Incident documented

---

## 🚀 Escalation

### **When to Page**

**CRITICAL**: Always page (no judgment)
- @devops-engineer
- @tech-lead
- @security-engineer (if breach)
- @product-owner (for customer notification)

**HIGH**: Page @tech-lead + @devops-engineer
**MEDIUM**: Notify @tech-lead in Slack (non-blocking)
**LOW**: Create issue, no notification needed

### **If First Responder Can't Handle**

- CRITICAL: Escalate to @software-architect if root cause unclear
- HIGH: Escalate to @tech-lead if needs architecture decision
- MEDIUM: Schedule proper investigation in next iteration
- LOW: Add to backlog

---

## 📊 Incident Metrics

| Metric | Purpose |
|--------|---------|
| **Detection Time** | How fast alert triggered |
| **Response Time** | How fast team responded |
| **Resolution Time** | Total time to fix |
| **Customer Impact** | # of users/data affected |
| **Root Cause** | Why it happened |
| **Action Items** | Prevention for next time |

---

## 🔗 Related Documents

- [WORKFLOW_SPRINT_EXECUTION_VELOCITY_BASED.md](./WORKFLOW_SPRINT_EXECUTION_VELOCITY_BASED.md) - How incidents affect velocity
- [AGENT_ESCALATION_PATH.md](../AGENT_COORDINATION/AGENT_ESCALATION_PATH.md) - Escalation authority

---

**Owner**: @process-assistant  
**Version**: 2.0 (Severity-Based, No Time SLAs)  
**Status**: ACTIVE  
**Key Change**: Severity determines priority, not clock. Immediate response for CRITICAL, flexible for others.

