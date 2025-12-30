# Process Assistant - Enforcement Rules & Monitoring

**Owner**: @process-assistant  
**Last Updated**: 29. Dezember 2025  
**Version**: 1.0  
**Status**: ACTIVE - Violations will be managed per this document

---

## 🎯 Enforcement Overview

This document defines how @process-assistant will:
1. Monitor for violations
2. Handle unauthorized changes
3. Track compliance
4. Escalate issues

---

## 🔍 Daily Monitoring

@process-assistant monitors:

### **Git Diffs in Protected Files**

```
Protected Directories:
  .github/docs/processes/        (All subdirectories)
  .github/agents/                (All files)
  .github/copilot-instructions*  (All role-specific files)
  .github/GOVERNANCE.md
  .github/PROCESS_STANDARDS.md

Daily Check:
  1. Review git log for last 24 hours
  2. Identify commits in protected directories
  3. Verify commit author = @process-assistant
  4. If not: VIOLATION DETECTED
```

### **GitHub Issues & Pull Requests**

```
Monitor for:
  - PRs attempting to modify protected files
  - Issues requesting unauthorized changes
  - Discussions about workflow modifications
  - Agents modifying own instructions
  
Action:
  - Post comment explaining violation
  - Provide link to GOVERNANCE_RULES.md
  - Close PR (if applicable)
  - Escalate if pattern emerges
```

---

## 🚨 Violation Handling

### **Violation Level 1: Unauthorized File Change**

**Detection**: Non-@process-assistant user commits to protected file

**Response**:
```
IMMEDIATE (< 5 minutes):
  1. Revert commit
  2. Post GitHub comment:
     "@[user] this change was reverted (unauthorized)
     
      Reason: Changes to [file] require @process-assistant approval
      
      Process for requesting changes:
      - See: https://github.com/[repo]/.github/docs/processes/GOVERNANCE/GOVERNANCE_RULES.md
      
      To request this change:
      1. File GitHub issue titled: '@process-assistant instruction-request: [description]'
      2. Include: What/Why/Impact in issue body
      3. Label: 'process-change-request'
      4. Wait for approval
      
      Questions? See: GOVERNANCE_RULES.md"
     
  3. Tag @tech-lead (for awareness)
  4. Log in compliance tracker
```

**Follow-up (within 24h)**:
```
  1. Direct message to violating agent
  2. Explanation of policy
  3. Link to formal process
  4. Offer to help with proper request
```

### **Violation Level 2: Repeated Violations**

**Detection**: Same agent violates 2+ times within 30 days

**Response**:
```
IMMEDIATE:
  1. Escalate to @tech-lead
  2. Provide evidence (git log, reverted commits)
  3. Request @tech-lead discussion with agent
  
Message to @tech-lead:
  "@tech-lead: Pattern of unauthorized changes detected
   
   Agent: @[user]
   Incidents:
     - Commit 1: [date] [description]
     - Commit 2: [date] [description]
   
   Policy: Only @process-assistant can modify protected files
   Reference: GOVERNANCE_RULES.md
   
   Please discuss with @[user]"
```

### **Violation Level 3: Willful Disregard**

**Detection**: Agent continues violations after level 2 escalation

**Response**:
```
ESCALATE TO LEADERSHIP:
  1. @tech-lead escalates to project leadership
  2. Formal notice issued
  3. Potential consequences discussed
  4. Clear expectations set
```

---

## 📊 Compliance Tracking

### **Compliance Log** (Maintained by @process-assistant)

```
Date       | User    | Action      | File              | Status  | Notes
-----------|---------|-------------|-------------------|---------|--------
29.12.2025 | Holger  | Commit OK   | PROCESS_*.md      | OK      | Created files
29.12.2025 | Holger  | Commit OK   | AGENTS_INDEX.md   | OK      | Updated index
[ongoing]  | [name]  | [violation] | [protected file]  | REVERTED| [notes]
```

### **Monthly Compliance Report**

@process-assistant generates monthly:

```
B2Connect Process Governance Compliance Report
Month: December 2025

Metrics:
  - Total commits to protected files: [N]
  - Authorized (by @process-assistant): [N] (100%)
  - Unauthorized (reverted): [N] (0%)
  - Violations reported: [N]
  - Escalations: [N]
  
Compliance Rate: [X]%
Target: 100%

Issues:
  - None detected this month
  - Or: [list of violations]
  
Improvements:
  - [What went well]
  - [Suggestions for next month]
```

---

## 🎯 Flagged Files (Cannot Be Modified by Other Agents)

### **Absolute Protected Files**

These files have ZERO exceptions:

```
.github/docs/processes/README.md
.github/docs/processes/GOVERNANCE/GOVERNANCE_RULES.md
.github/docs/processes/GOVERNANCE/PERMISSIONS_MATRIX.md
.github/docs/processes/PROCESS_STANDARDS.md
.github/docs/processes/CHANGE_CONTROL_PROCESS.md
.github/docs/processes/ENFORCEMENT_RULES_AND_MONITORING.md

.github/agents/*.md (ALL agent files)

.github/copilot-instructions.md
.github/copilot-instructions-quickstart.md
.github/copilot-instructions-index.md
.github/copilot-instructions-backend.md
.github/copilot-instructions-frontend.md
.github/copilot-instructions-devops.md
.github/copilot-instructions-qa.md
.github/copilot-instructions-security.md
[... all other copilot-instructions-*.md files ...]

.github/GOVERNANCE.md
```

### **Workflow Files (Protected)**

```
.github/docs/processes/CORE_WORKFLOWS/*.md (ALL)
.github/docs/processes/AGENT_COORDINATION/*.md (ALL)
.github/docs/processes/GOVERNANCE/*.md (ALL)
```

---

## 🔔 Notification Protocol

### **Who Gets Notified**

**Unauthorized Change Detected**:
```
IMMEDIATE notifications:
  - @[violating agent] (GitHub comment on commit)
  - @tech-lead (GitHub mention for awareness)
  - @process-assistant logs in compliance tracker
```

**Pattern of Violations**:
```
Level 2+ notifications:
  - @[violating agent] (direct message)
  - @tech-lead (detailed escalation)
  - Leadership (if Level 3+)
```

**Monthly Compliance Report**:
```
Recipients:
  - @tech-lead
  - @scrum-master
  - Leadership (if issues detected)
```

---

## 🚨 Emergency Override Procedures

### **When Can Rules Be Bypassed?**

**Scenario 1: Production Outage**

```
IF: Production system down due to process/instruction issue
AND: Immediate change required

THEN:
  1. @tech-lead authorizes override
  2. @process-assistant makes emergency change
  3. Temporary fix implemented immediately
  4. Must be documented within 24 hours
  5. Formal change request filed within 48 hours
  
Override Rule: Changes marked [EMERGENCY] in commit
```

**Scenario 2: Security Breach**

```
IF: Security issue requires immediate instruction update
AND: Waiting for normal process approval risks security

THEN:
  1. @security-engineer requests override
  2. @tech-lead + @process-assistant approve
  3. Change implemented immediately
  4. Documented within 24 hours
  5. Formal post-mortem analysis
```

**Scenario 3: Blocking Issue**

```
IF: Current process prevents critical work
AND: Cannot proceed via normal channels

THEN:
  1. Document blocking issue
  2. Request @process-assistant review
  3. If approved: Temporary exception granted
  4. Formal change request filed within 48 hours
```

### **Emergency Override Logging**

```
All overrides tracked:
  - Date/time
  - User who requested
  - User who approved
  - What changed
  - Why (emergency reason)
  - Date documented
  - Date formal process filed
```

---

## 📋 Audit Trail

@process-assistant maintains audit trail of:

```
1. All changes to protected files (git log)
2. All unauthorized change attempts (logs)
3. All escalations (issue links)
4. All emergency overrides (documented)
5. Compliance metrics (monthly report)
6. Governance violations (tracked)
```

**Audit Trail Access**:
```
@tech-lead: Full access
@security-engineer: Full access (security-related only)
@process-assistant: Full access (maintainer)
Other agents: Read access (own violations only)
```

---

## 🔄 Escalation Path

```
Violation Detected
    ↓
Level 1: Revert + notify violating agent
    ↓
Repeated (2+ violations)? → Level 2: Escalate to @tech-lead
    ↓
Still violated? → Level 3: Escalate to leadership
    ↓
Resolution: Clear expectations, update policies, or terminate
```

---

## ✅ What Good Compliance Looks Like

```
✅ All changes to protected files made by @process-assistant
✅ All agent instruction changes requested via GitHub issues
✅ All requests include rationale and impact analysis
✅ Formal process followed without exception
✅ Changes documented with version numbers
✅ Compliance rate: 100%
✅ Zero violations
✅ Quick resolution of any questions
```

---

## 📊 Success Metrics

@process-assistant tracks:

```
Metric                                  | Target | Actual
----------------------------------------|--------|--------
% of changes via formal process         | 100%   | [TBD]
Unauthorized change attempts/month      | 0      | [TBD]
Average time to resolve violations      | <24h   | [TBD]
Compliance rate (agents following rules)| 100%   | [TBD]
Workflow-instruction conflicts detected | 0      | [TBD]
Emergency overrides/month               | <1     | [TBD]

Monthly Report: [.github/docs/processes/MONITORING/]
```

---

## 📞 Questions About Enforcement?

**"What happens if I modify my instructions?"**
→ Change will be reverted + you'll be notified

**"Can I commit to protected files in emergency?"**
→ Request override from @tech-lead first

**"What if I disagree with this policy?"**
→ Escalate per AGENT_ESCALATION_PATH.md

**"How do I request a process change?"**
→ File GitHub issue with "process-change-request" label

---

## 🎯 Effective Date

This enforcement policy is **EFFECTIVE IMMEDIATELY**: 29. Dezember 2025

**Grace Period**: 7 days (violations reported, not yet reverted)  
**Full Enforcement**: Day 8+ (violations reverted immediately)

---

**Last Updated**: 29. Dezember 2025  
**Version**: 1.0  
**Status**: ACTIVE - @process-assistant is now monitoring
