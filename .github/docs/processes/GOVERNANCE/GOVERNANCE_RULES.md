# Governance Rules

**Owner**: @process-assistant (Exclusive Authority)  
**Last Updated**: 29. Dezember 2025  
**Version**: 1.0  
**Status**: ACTIVE - All agents must comply

---

## 🎯 Core Principle

**EXCLUSIVE AUTHORITY**: Only @process-assistant can modify:
- Workflow definitions
- Agent instructions
- Process documentation
- Governance rules

All other agents must REQUEST changes via formal process.

---

## 📋 Rule 1: Instruction Modification Authority

### **Who Can Modify Agent Instructions?**

| Action | Allowed For | Process |
|--------|-------------|---------|
| **Read instructions** | All agents | Direct access to `.github/agents/` and `.github/copilot-instructions-*.md` |
| **Modify own instructions** | ❌ NO AGENT (not even @process-assistant modifies others' without review) | Submit request to @process-assistant with rationale |
| **Modify other agent's instructions** | ❌ NEVER | Discuss with @process-assistant first |
| **Modify process-assistant instructions** | Only @process-assistant | Self-governance (but document rationale) |
| **Modify workflow documents** | Only @process-assistant | Via [CHANGE_CONTROL_PROCESS.md](./CHANGE_CONTROL_PROCESS.md) |

### **How to Request an Instruction Change**

```
Step 1: Identify issue
  ├─ Current instruction: [what it says now]
  ├─ Problem: [what's wrong with it]
  └─ Impact: [how this affects your work]

Step 2: File GitHub Issue
  ├─ Title: "@process-assistant instruction-request: [brief description]"
  ├─ Body:
  │  ├─ File affected: [exact path]
  │  ├─ Current text: [relevant section]
  │  ├─ Proposed text: [what you want it to say]
  │  ├─ Rationale: [why this change is needed]
  │  └─ Impact: [who else is affected]
  └─ Label: "process-change-request"

Step 3: Wait for @process-assistant review
  ├─ ✅ APPROVED → @process-assistant makes change
  ├─ 🟡 MODIFY → @process-assistant suggests revisions
  └─ ❌ REJECTED → @process-assistant explains why

Step 4: @process-assistant makes change (if approved)
  ├─ Updates file
  ├─ Updates version number
  ├─ Documents in commit message
  └─ Notifies all affected agents
```

---

## 📋 Rule 2: Workflow Definition Authority

### **What Counts as "Workflow Definition"?**

✅ **WORKFLOW** (only @process-assistant controls):
- `.github/docs/processes/*.md`
- `.github/docs/processes/*/*.md`
- Process step definitions
- Decision trees
- Escalation paths
- Governance documentation

❌ **NOT WORKFLOW** (agents can create/modify):
- Feature implementation code
- Test code
- Feature documentation
- Code comments
- Commit messages

### **Detecting Unauthorized Changes**

@process-assistant actively monitors for:

```
Violation Type 1: Agent modifying .github/docs/processes/
  Detection: Git diff shows changes by non-process-assistant user
  Response: Revert change + notification
  
Violation Type 2: Instruction file modified without approval
  Detection: Agent commits change to .github/copilot-instructions-*.md
  Response: Revert change + require formal request
  
Violation Type 3: Workflow contradiction
  Detection: Workflow says X but instructions say Y
  Response: Fix + notify affected agents
```

---

## 📋 Rule 3: Conflict Resolution Authority

### **If Workflow and Instructions Contradict**

```
Responsibility: @process-assistant

Process:
1. Identify contradiction
2. Analyze both documents
3. Determine which is more recent/accurate
4. Update BOTH to match
5. Document the resolution
6. Notify affected agents

Example:
  Workflow says: "Code review required before merge"
  Instructions say: "Merge after single approval"
  
  Solution: @process-assistant reviews both, updates to same standard
```

---

## 📋 Rule 4: Change Control Process

All changes to workflows/instructions follow formal process:

```
Step 1: Request Submitted
  └─ Via GitHub issue with "process-change-request" label

Step 2: @process-assistant Reviews
  ├─ Is request valid? (technical, logical, consistent)
  ├─ Does it contradict other rules?
  ├─ What else needs updating?
  └─ Decision: APPROVE / MODIFY / REJECT

Step 3: If APPROVED
  ├─ @process-assistant makes change
  ├─ Updates all related documentation
  ├─ Bumps version number (X.Y → X.Y+1)
  ├─ Creates change log entry
  └─ Commits with full rationale

Step 4: If REJECTED
  ├─ Documents reasons clearly
  ├─ Suggests alternatives (if any)
  └─ Leaves issue open for discussion

Step 5: Notify All Agents
  └─ Comment on issue with final decision
```

---

## 📋 Rule 5: Version Control & Traceability

Every workflow/instruction change must:

### **Include in Commit Message**
```
docs(process): update [workflow name]

What changed:
- [Specific change 1]
- [Specific change 2]

Why:
- [Business rationale]
- [Problem being solved]

Impact:
- Affects: @agent-1, @agent-2
- Version: X.Y → X.Y+1

GitHub Issue: #XXX
Reviewed by: @process-assistant
```

### **Version Format**
```
SEMANTIC VERSIONING: MAJOR.MINOR.PATCH

MAJOR (X._._.): Breaking changes to workflow steps
MINOR (_.Y._): Non-breaking additions (new steps, decision branches)
PATCH (_._.Z): Documentation improvements, clarifications

Example:
  v1.0.0 → Initial release of sprint workflow
  v1.1.0 → Add new step for accessibility review
  v1.1.1 → Clarify step 3 language
```

---

## 📋 Rule 6: Enforcement & Compliance

### **For Violations of This Policy**

```
Violation Level 1: Unauthorized instruction change
  Action: Revert change immediately
  Notification: GitHub comment explaining policy
  Documentation: Added to compliance log

Violation Level 2: Repeated violations
  Action: Escalate to @tech-lead
  Notification: Direct message to violating agent
  Documentation: Updated compliance log

Violation Level 3: Willful disregard
  Action: Escalate to leadership
  Notification: Formal notice
  Documentation: Logged as governance violation
```

### **Monitoring & Auditing**

@process-assistant conducts:

```
Daily:
  - Check git diffs in .github/ for unauthorized changes
  - Monitor GitHub issues for process-related activity
  
Weekly:
  - Review compliance log
  - Check for workflow-instruction conflicts
  - Verify documentation currency
  
Monthly:
  - Comprehensive audit of all processes
  - Report to leadership on compliance
  - Plan improvements
```

---

## 📋 Rule 7: Exception Handling

### **When Can Rules Be Bypassed?**

```
Approved Exceptions:
1. Emergency: Production outage requires immediate process change
   Approval: Requires @tech-lead + @process-assistant approval
   Action: Implement immediately, document after
   Deadline: Document within 24 hours
   
2. Blocking Issue: Process prevents critical work
   Approval: Requires @process-assistant approval only
   Action: Temporary override approved
   Deadline: Formal change request submitted within 48 hours
   
3. Governance Issue: Rule itself is wrong/harmful
   Approval: Requires @process-assistant + leadership approval
   Action: Rule can be modified/removed
   Deadline: New rule documented within 1 week
```

---

## 📋 Rule 8: Documentation Standards

All process documentation must have:

### **Mandatory Sections**
- [ ] Owner (who maintains this?)
- [ ] Last Updated (date)
- [ ] Version (semantic versioning)
- [ ] Scope (when does this apply?)
- [ ] Participants (which agents involved?)
- [ ] Steps (sequential, clear process)
- [ ] Decision Points (if-then logic)
- [ ] Success Criteria (how do we know it's done?)
- [ ] Exceptions (edge cases)
- [ ] Related Processes (cross-references)

### **Quality Checks**
- [ ] No ambiguous language
- [ ] All acronyms defined
- [ ] Steps are unambiguous
- [ ] Owner/authority is explicit
- [ ] Exception handling documented
- [ ] Version number matches actual state

---

## 📋 Rule 9: Authority Chain

When decisions need to be made:

```
Level 1 (Process-Assistant Authority):
  ├─ Routine process changes
  ├─ Instruction clarifications
  ├─ Documentation updates
  └─ Conflict resolution between workflows

Level 2 (Tech-Lead Authority):
  ├─ Architecture implications of process changes
  ├─ Technical workflow decisions
  ├─ Tool/infrastructure changes affecting workflow
  └─ Appeals of Level 1 decisions

Level 3 (Product Owner Authority):
  ├─ Business process changes
  ├─ Priority/timeline for process changes
  ├─ Resource allocation to process improvements
  └─ Strategic workflow decisions

Level 4 (Leadership Authority):
  ├─ Policy changes
  ├─ Governance rule modifications
  ├─ Conflicts between Level 1-3
  └─ Emergency overrides
```

---

## ✅ Compliance Checklist

Every agent must:

- [ ] Read [GOVERNANCE_RULES.md](./GOVERNANCE_RULES.md) (this file)
- [ ] Check [PERMISSIONS_MATRIX.md](./PERMISSIONS_MATRIX.md) before acting
- [ ] Request changes via formal process (not direct edits)
- [ ] Follow documented workflows (exact steps, no shortcuts)
- [ ] Document decisions made per [GOVERNANCE_RULES.md](./GOVERNANCE_RULES.md)
- [ ] Accept @process-assistant decisions on workflow changes
- [ ] Escalate conflicts per [AGENT_ESCALATION_PATH.md](../AGENT_COORDINATION/AGENT_ESCALATION_PATH.md)

---

## 📊 Governance Metrics

@process-assistant tracks:

```
Compliance Metrics:
  - % of process changes via formal request process
  - % of workflows with up-to-date documentation
  - Number of workflow-instruction conflicts found/month
  - % of agents compliant with governance rules
  - Average time to resolve process change requests
  
Target Goals:
  - 100% of changes via formal process
  - 100% of workflows current
  - 0 active conflicts
  - 95%+ agent compliance
  - <1 week average change request resolution
```

---

## 📞 Questions About Governance?

| Question | Answer | Who to Ask |
|----------|--------|-----------|
| "Can I modify my own instructions?" | NO - request via process | @process-assistant |
| "Is this action allowed?" | Check PERMISSIONS_MATRIX.md | @process-assistant |
| "What's the escalation path?" | See AGENT_ESCALATION_PATH.md | @process-assistant |
| "What do I do if I disagree with a rule?" | Escalate per AGENT_ESCALATION_PATH.md | @process-assistant then @tech-lead |

---

## 🚀 Enforcement Timeline

```
Day 1 (29. Dez): Rules published, all agents notified
Day 2-7: Monitoring period (violations reported, not reverted)
Day 8+: Full enforcement (violations reverted immediately)
```

---

**CRITICAL**: These rules are binding. All agents must comply.  
**Questions?** Contact @process-assistant  
**Violations?** Will be escalated per Rule 6

---

**Last Updated**: 29. Dezember 2025  
**Version**: 1.0  
**Status**: ACTIVE
