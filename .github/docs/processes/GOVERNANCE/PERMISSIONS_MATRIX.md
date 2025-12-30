# Permissions Matrix

**Owner**: @process-assistant (Exclusive Authority)  
**Last Updated**: 29. Dezember 2025  
**Version**: 1.0  
**Status**: ACTIVE - Reference this when in doubt

---

## 🎯 Quick Permission Lookup

**"Can I [action] [file/object]?"**

Use the matrix below. Find your role (left) and action (top).

---

## 📋 Workflow & Instruction Permissions

```
ACTION: Modify Workflow Files (.github/docs/processes/*)

                    | process-asst | tech-lead | all-agents |
Read                |     YES      |    YES    |    YES     |
Modify (approved)   |     YES      |     NO    |     NO     |
Modify (own req)    |     YES      |     NO    |     NO     |
Delete              |     YES      |     NO    |     NO     |
Version bump        |     YES      |     NO    |     NO     |
├─ Rationale: Only @process-assistant maintains workflows
└─ Exception: Leadership override in emergencies
```

```
ACTION: Modify Agent Instructions (.github/copilot-instructions-*.md)

                    | process-asst | tech-lead | all-agents |
Read                |     YES      |    YES    |    YES     |
Modify (approved)   |     YES      |     NO    |     NO     |
Modify (own req)    |      YES*    |     NO    |     NO     |
Delete              |     YES      |     NO    |     NO     |
Version bump        |     YES      |     NO    |     NO     |
├─ Rationale: Only @process-assistant modifies instructions
├─ *only with documented rationale
└─ Exception: None (strictly enforced)
```

```
ACTION: Modify Agent Files (.github/agents/*)

                    | process-asst | tech-lead | all-agents |
Read                |     YES      |    YES    |    YES     |
Modify              |     YES      |     NO    |     NO     |
Delete              |     YES      |     NO    |     NO     |
Create              |     YES      |     NO    |     NO     |
└─ Rationale: Process-assistant creates agents, updates instructions
```

---

## 📋 Code & Documentation Permissions

```
ACTION: Modify Feature Code (backend/frontend/etc.)

                    | process-asst | tech-lead | all-agents |
Read                |     YES      |    YES    |    YES     |
Modify              |      NO      |    YES    |    YES     |
Code review         |      NO      |    YES    |    SOME    |
Approve merge       |      NO      |    YES    |    SOME    |
└─ Rationale: Feature code is developer responsibility
```

```
ACTION: Modify Process Documentation (.github/docs/processes/)

                    | process-asst | tech-lead | all-agents |
Read                |     YES      |    YES    |    YES     |
Modify              |     YES      |     NO    |     NO     |
Delete              |     YES      |     NO    |     NO     |
Version bump        |     YES      |     NO    |     NO     |
└─ Rationale: Only @process-assistant maintains processes
```

```
ACTION: Modify Feature Documentation (docs/features/, user guides, etc.)

                    | process-asst | tech-lead | all-agents |
Read                |     YES      |    YES    |    YES     |
Modify              |      NO      |    YES    |    YES     |
Add                 |      NO      |    YES    |    YES     |
Delete (with review)|      NO      |    YES    |    SOME    |
└─ Rationale: Feature docs are developer responsibility
```

---

## 🎯 Role-Based Permissions

### **@process-assistant Permissions**

```
✅ CAN DO:
  - Read all files (workflow, instructions, code, docs)
  - Modify .github/docs/processes/*
  - Modify .github/copilot-instructions-*.md
  - Modify .github/agents/*.md
  - Create new workflow definitions
  - Enforce governance rules
  - Revert unauthorized changes
  - Version control workflow/instruction files
  - Maintain PERMISSIONS_MATRIX.md
  - Create process-related documentation

❌ CANNOT DO:
  - Modify feature code
  - Approve/merge PRs for features
  - Make business/product decisions
  - Make architecture decisions
  - Modify individual feature documentation
  - Commit code changes
  - Deploy to production
```

### **@tech-lead Permissions**

```
✅ CAN DO:
  - Read all files (workflow, instructions, code, docs)
  - Modify feature code (review, approve)
  - Modify feature documentation
  - Make architecture decisions
  - Review workflow implementations (not the definition)
  - Approve code reviews
  - Escalate governance issues
  - Request workflow changes (via process-assistant)

❌ CANNOT DO:
  - Modify workflow definitions
  - Modify agent instructions
  - Override governance rules
  - Commit changes to .github/docs/processes/
  - Commit changes to .github/copilot-instructions-*.md
  - Make product decisions
```

### **@backend-developer, @frontend-developer, etc. Permissions**

```
✅ CAN DO:
  - Read all files
  - Modify feature code
  - Create tests
  - Write feature documentation
  - Create feature-specific guides
  - Request instruction changes (via process-assistant)
  - Escalate to @tech-lead if blocked

❌ CANNOT DO:
  - Modify workflow definitions
  - Modify agent instructions (even own!)
  - Modify .github/docs/processes/*
  - Modify .github/copilot-instructions-*.md
  - Commit to protected governance files
  - Make deployment decisions
```

### **@product-owner Permissions**

```
✅ CAN DO:
  - Read all files
  - Prioritize work
  - Request process changes (via process-assistant)
  - Make business decisions
  - Update roadmap
  - Review process compliance

❌ CANNOT DO:
  - Modify workflow definitions directly
  - Modify code
  - Make technical decisions
  - Override @tech-lead architecture decisions
  - Commit code changes
```

---

## 🚨 Protected Files (Process-Assistant Only)

These files have strict access control:

```
PROTECTED (No one except @process-assistant can modify):
├── .github/docs/processes/*
├── .github/docs/processes/*/*
├── .github/copilot-instructions.md
├── .github/copilot-instructions-*.md
├── .github/agents/*.md
└── .github/GOVERNANCE.md

Exception Process:
  1. File GitHub issue with @process-assistant tag
  2. Provide rationale
  3. @process-assistant reviews
  4. If approved → @process-assistant makes change
  5. Change documented in commit with rationale
```

---

## 📊 Permission Summary Table

| File/Action | process-asst | tech-lead | developers | product-owner |
|-------------|--------------|-----------|-----------|---------------|
| Feature code | R | R/W | R/W | R |
| Feature docs | R | R/W | R/W | R |
| Workflow defs | R/W | R | R | R |
| Instructions | R/W | R | R | R |
| Governance | R/W | R | R | R |
| Tests | R | R/W | R/W | R |
| Approvals | N/A | YES | LIMITED | N/A |

**Legend**:
- R = Read
- W = Write
- R/W = Read & Write
- LIMITED = Can approve some; not others

---

## 🔄 Escalation & Exception Process

### **When You Need Something You Can't Do**

```
Step 1: Check this matrix
  └─ Confirm you don't have permission

Step 2: File request with appropriate authority
  ├─ For workflow changes → @process-assistant
  ├─ For code changes → @tech-lead
  ├─ For product decisions → @product-owner
  └─ For governance → Leadership (via @tech-lead)

Step 3: Document your need
  ├─ What: [specific action needed]
  ├─ Why: [business rationale]
  ├─ Impact: [what's blocked without this]
  └─ Timeline: [how urgent]

Step 4: Wait for approval
  └─ Approved authority makes the change
```

### **Emergency Override**

```
In true emergencies (production down, security breach):

1. @tech-lead can authorize temporary process override
2. Action taken immediately
3. Must be documented within 24 hours
4. Formal process change request submitted within 48 hours
5. Leadership reviews decision

⚠️ Abuse of emergency override = Escalation
```

---

## ✅ Compliance Checklist

Before taking any action:

- [ ] Check PERMISSIONS_MATRIX.md
- [ ] Confirm I have permission
- [ ] If not, file request via proper process
- [ ] Wait for approval before acting
- [ ] Document decision made

---

## 📞 Questions?

| Question | Answer | Who to Ask |
|----------|--------|-----------|
| "Can I modify [file]?" | Check this matrix | @process-assistant |
| "Who should I ask for [action]?" | Check PERMISSIONS_MATRIX.md | @process-assistant |
| "What if I disagree with these permissions?" | File escalation per GOVERNANCE_RULES.md | @process-assistant → @tech-lead → Leadership |

---

## 📋 Matrix Updates

When new roles/permissions added:

1. @process-assistant updates this matrix
2. Version number bumped
3. All agents notified
4. 7-day grace period for learning
5. Full enforcement after 7 days

---

**Last Updated**: 29. Dezember 2025  
**Version**: 1.0  
**Status**: ACTIVE - Reference before acting

**Remember**: When in doubt, ask @process-assistant rather than assuming permission.
