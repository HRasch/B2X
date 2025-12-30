# 📬 Agent Collaboration Mailbox System - Implementation Complete

**Date**: 2025-12-30  
**Status**: ✅ ACTIVE & ENFORCED  
**Authority**: @process-assistant  
**Governance**: COLLABORATION_MAILBOX_SYSTEM.md

---

## 🎯 What Was Implemented

A **centralized mailbox system** for all agent-to-agent collaboration in B2Connect, replacing scattered GitHub comments with organized, git-tracked message folders.

### Key Features

✅ **Centralized**: All agent messages in `B2Connect/collaborate/{issue-id}/`  
✅ **Tracked**: Full git history of all inter-agent communication  
✅ **Scalable**: Supports unlimited agents and issues  
✅ **Organized**: INBOX (receives), OUTBOX (sends), archived folders  
✅ **Auditable**: Every request/response documented with timestamps  
✅ **Maintained**: @team-assistant checks daily and updates status  

---

## 📁 Folder Structure

### Standard Layout
```
B2Connect/collaborate/
├── issue-56/                           (specific issue collaboration)
│   ├── COORDINATION_SUMMARY.md         (status dashboard - updated daily)
│   ├── @ui-expert/
│   │   ├── INBOX/                      (2 research requests)
│   │   │   └── 2025-12-30-from-product-owner-template-analysis-request.md
│   │   └── OUTBOX/                     (awaiting response)
│   ├── @ux-expert/
│   │   ├── INBOX/                      (1 research request)
│   │   │   └── 2025-12-30-from-product-owner-ux-research-request.md
│   │   └── OUTBOX/                     (awaiting response)
│   └── @frontend-developer/
│       ├── INBOX/                      (awaiting design specs)
│       └── OUTBOX/
│
├── COLLABORATION_MAILBOX_SYSTEM.md     (master governance rules - 1400+ lines)
├── README.md                            (index of all active issues)
└── archive/                             (completed issues)
    └── 2025-12/
        └── issue-XX/                    (historical issues for reference)
```

### Agent Message Naming Convention

**INBOX Messages** (receive requests):
```
{YYYY-MM-DD}-from-{sender-agent}-{request-type}.md

Examples:
2025-12-30-from-product-owner-ux-research-request.md
2025-12-30-from-tech-lead-architectural-review-request.md
2025-12-30-from-qa-engineer-test-strategy-request.md
```

**OUTBOX Messages** (send responses):
```
{YYYY-MM-DD}-to-{recipient-agent}-{response-type}.md

Examples:
2025-12-31-to-product-owner-ux-research-findings.md
2025-12-31-to-tech-lead-architecture-review-complete.md
2025-12-31-to-qa-engineer-test-strategy-ready.md
```

---

## 📋 Request/Response Templates

### Request Message (Posted to Recipient's INBOX)

```markdown
# [Request Type] Request from @{sender}

**Issue**: #{number}
**From**: @{sender}
**To**: @{recipient}
**Due**: YYYY-MM-DD EOD
**Status**: 🔄 Pending Response

## What I Need

[Clear description]

## Acceptance Criteria

- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

## Deliverable Format

[Expected format - markdown, diagrams, code, etc.]

## Timeline

- **Due**: YYYY-MM-DD EOD
- **Usage**: [What happens next]
- **Next Step**: [Following phase]

## Questions?

Comment on GitHub Issue #{number}

---
**Mailbox**: B2Connect/collaborate/{issue-id}/@{recipient}/INBOX/
**Governance**: B2Connect/collaborate/COLLABORATION_MAILBOX_SYSTEM.md
```

### Response Message (Posted to Recipient in OUTBOX)

```markdown
# [Response Type] Response to @{requester}

**Issue**: #{number}
**From**: @{my-agent}
**To**: @{requester}
**Fulfills**: [Original request date]
**Status**: ✅ Complete

## Summary

[Brief overview]

## Main Findings

### Finding 1
[Details]

### Finding 2
[Details]

## Files Included

- File 1: [What it is]
- File 2: [What it is]

## Next Steps

[Recommendations for how requester should use this response]

---
**Original INBOX File**: Deleted (processed)
**Governance**: B2Connect/collaborate/COLLABORATION_MAILBOX_SYSTEM.md
```

---

## 🔄 Workflow Steps

### Step 1: Create Request
**Who**: Requesting agent (e.g., @product-owner)

```bash
# Create recipient's mailbox if needed
mkdir -p B2Connect/collaborate/{issue-id}/@{recipient}/{INBOX,OUTBOX}

# Post request in recipient's INBOX
# File: B2Connect/collaborate/{issue-id}/@{recipient}/INBOX/{YYYY-MM-DD}-from-{you}-{type}.md
# Content: Use request template above
```

### Step 2: Respond
**Who**: Recipient agent (e.g., @ui-expert)

```bash
# Review request in your INBOX
# Create response in your OUTBOX
# File: B2Connect/collaborate/{issue-id}/@{you}/OUTBOX/{YYYY-MM-DD}-to-{requester}-{type}.md
# Content: Use response template above
```

### Step 3: Mark as Processed
**Who**: Recipient agent

```bash
# Delete your INBOX file (marks as processed)
rm B2Connect/collaborate/{issue-id}/@{you}/INBOX/{original-request-file}.md

# Git will track the deletion - shows message was processed
```

### Step 4: Requester Reviews Response
**Who**: Requesting agent

```bash
# Check recipient's OUTBOX folder for response
# Review the findings
# Plan next phase
```

### Step 5: Archive (if completed)
**Who**: @team-assistant

```bash
# After issue complete, archive to collaborate/archive/
mv B2Connect/collaborate/{issue-id}/ B2Connect/collaborate/archive/2025-12/{issue-id}/

# Keep COORDINATION_SUMMARY.md at new location
```

---

## 👥 Role Responsibilities

### @team-assistant (Daily Coordinator)

**Daily Check** (5 minutes):
- [ ] Review all agent INBOX folders
- [ ] Check for new OUTBOX responses
- [ ] Update COORDINATION_SUMMARY.md
- [ ] Flag overdue messages (>24h without response)

**COORDINATION_SUMMARY.md** (Maintained by @team-assistant):
```markdown
| Agent | INBOX | OUTBOX | Status | Due |
|-------|-------|--------|--------|-----|
| @ui-expert | 2 | 0 | 🔄 In Progress | Dec 31 |
| @ux-expert | 1 | 0 | 🔄 In Progress | Dec 31 |

Last Check: 2025-12-30 14:00
Next Check: 2025-12-31 09:00
```

**Weekly Archive** (Friday):
- [ ] Move completed issues to `/archive/`
- [ ] Update main `/collaborate/README.md` index
- [ ] Prepare handoff for next sprint

**Escalation** (if agent doesn't respond within 48h):
- [ ] Post GitHub issue comment: "@tech-lead - @agent-x has overdue INBOX"
- [ ] Flag in COORDINATION_SUMMARY.md
- [ ] Notify @tech-lead directly

### Requesting Agent (@product-owner, @tech-lead, etc.)

**When Posting Request**:
1. [ ] Create recipient's mailbox folder
2. [ ] Write clear request using template
3. [ ] Set realistic due date (typically 24-48h)
4. [ ] Post file in recipient's INBOX
5. [ ] Comment on GitHub issue linking to mailbox

**When Reviewing Response**:
1. [ ] Check recipient's OUTBOX folder
2. [ ] Review findings against acceptance criteria
3. [ ] Mark complete or request clarifications
4. [ ] Plan next phase based on findings

### Responding Agent (@ui-expert, @ux-expert, @frontend-developer, etc.)

**When You Receive Request** (in your INBOX):
1. [ ] Review request and acceptance criteria
2. [ ] Ask clarifying questions if needed (GitHub comment)
3. [ ] Plan response
4. [ ] Set work schedule to meet deadline

**When You Respond**:
1. [ ] Create response file in your OUTBOX using template
2. [ ] Address all acceptance criteria
3. [ ] Provide clear deliverables
4. [ ] Delete original INBOX file (marks as processed)
5. [ ] Git will track: file deletion = response sent

**If You Can't Meet Deadline**:
1. [ ] Post comment on GitHub issue: "@team-assistant - I need extension until X date"
2. [ ] Provide status update
3. [ ] Explain what you're waiting for (if blocked)

---

## 🎯 Real-World Example: Issue #56

### Context
Store Frontend needs UI/UX modernization. @product-owner needs research input from @ui-expert (design templates) and @ux-expert (UX best practices).

### Execution Timeline

**Dec 30, 14:00** - @product-owner Posts Requests
```
Mailbox Structure Created:
/collaborate/issue-56/
├── @ui-expert/
│   ├── INBOX/
│   │   └── 2025-12-30-from-product-owner-template-analysis-request.md ← NEW
│   └── OUTBOX/
├── @ux-expert/
│   ├── INBOX/
│   │   └── 2025-12-30-from-product-owner-ux-research-request.md ← NEW
│   └── OUTBOX/
└── COORDINATION_SUMMARY.md ← NEW

GitHub Issue #56 Comment Posted:
"Agent collaboration moved to mailbox system at B2Connect/collaborate/issue-56/"
```

**Dec 30-31** - Agents Work on Responses
- @ui-expert reviews design templates (Vercel, Stripe, Shopify, etc.)
- @ux-expert researches UX best practices (checkout, discovery, trust, etc.)
- Both work toward deadline: Dec 31 EOD

**Dec 31, 16:00** - @ui-expert Posts Response
```
Response File Created:
/collaborate/issue-56/@ui-expert/OUTBOX/
└── 2025-12-31-to-product-owner-template-analysis.md ← NEW

INBOX File Deleted:
/collaborate/issue-56/@ui-expert/INBOX/
  2025-12-30-from-product-owner-template-analysis-request.md ← DELETED
  (Git tracks: file was processed)

Status Updated:
COORDINATION_SUMMARY.md shows:
  @ui-expert | 0 | 1 | ✅ Complete | Dec 31
```

**Dec 31, 17:00** - @ux-expert Posts Response
```
Response File Created:
/collaborate/issue-56/@ux-expert/OUTBOX/
└── 2025-12-31-to-product-owner-ux-research-findings.md ← NEW

INBOX File Deleted:
/collaborate/issue-56/@ux-expert/INBOX/
  2025-12-30-from-product-owner-ux-research-request.md ← DELETED
  (Git tracks: file was processed)

Status Updated:
COORDINATION_SUMMARY.md shows:
  @ux-expert | 0 | 1 | ✅ Complete | Dec 31
```

**Jan 1, 09:00** - @product-owner Reviews & Consolidates
```
Requests Review:
✅ @ui-expert response: template-analysis.md
✅ @ux-expert response: ux-research-findings.md

Consolidation:
/collaborate/issue-56/@product-owner/OUTBOX/
└── 2025-01-01-to-frontend-developer-design-specifications.md

Next Handoff:
GitHub Issue #56 Comment:
"Design specifications ready in mailbox for @frontend-developer"
```

**Jan 2+** - Implementation Phase
```
@frontend-developer:
- Reviews consolidated specs in OUTBOX
- Creates response with Phase 1 plan
- Updates COORDINATION_SUMMARY.md
- Begins implementation
```

---

## 🛡️ Governance & Enforcement

### Mandatory Rules

1. **Only Use This System**: Do NOT post requests/responses directly to GitHub issue comments
   - ✅ Use mailbox: `B2Connect/collaborate/{issue-id}/{agent}/INBOX/`
   - ❌ Don't use: GitHub issue comments for agent coordination

2. **Required Format**: All requests/responses must use templates
   - ✅ Use template: Acceptance criteria, deliverable format, timeline
   - ❌ Don't use: Ad-hoc requests without structure

3. **Timestamp Everything**: Every file must have date in name
   - ✅ Use format: `{YYYY-MM-DD}-from-{sender}-{type}.md`
   - ❌ Don't use: `request.md` or `response.md`

4. **Delete When Done**: After responding, delete your INBOX file
   - ✅ Mark processed: `rm INBOX/{filename}`
   - ❌ Don't accumulate: Leave old requests in INBOX

5. **Maintain Summaries**: @team-assistant updates status daily
   - ✅ Status clear: COORDINATION_SUMMARY.md updated EOD
   - ❌ Status unclear: Outdated or missing summaries

### Violations & Enforcement

| Violation | First Time | Repeated |
|-----------|-----------|----------|
| Post request to GitHub comments instead of mailbox | Warning + redirect | Escalate to @tech-lead |
| Missing acceptance criteria in request | Request revision | Escalate to @tech-lead |
| Overdue response (>48h) without update | @team-assistant notification | Escalate to @tech-lead |
| Deleted OUTBOX file accidentally | Recover from git | Escalate if pattern |
| Duplicate mailboxes for same issue | Merge & clean up | Document in process |

---

## 📊 Advantages Over Previous Approach

### Before (GitHub Comments)

❌ **Scattered**: Comments lost in issue thread  
❌ **Hard to track**: Which agent responded? Who's blocked?  
❌ **Cluttered**: 100+ comments = impossible to follow  
❌ **No cleanup**: Comments stay forever  
❌ **Not auditable**: Can't easily see full conversation history  

### After (Mailbox System)

✅ **Centralized**: All in `B2Connect/collaborate/{issue-id}/`  
✅ **Easy to track**: INBOX/OUTBOX shows status clearly  
✅ **Organized**: Each agent has folder, easy to navigate  
✅ **Cleanup**: Delete processed files = clean inbox  
✅ **Auditable**: Full git history of every message  

---

## 🚀 Getting Started

### For Requesting Work from Another Agent

```bash
# 1. Create mailbox folder structure
mkdir -p B2Connect/collaborate/{issue-id}/@{recipient-agent}/{INBOX,OUTBOX}

# 2. Create request file
# Path: B2Connect/collaborate/{issue-id}/@{recipient}/INBOX/
# Name: {YYYY-MM-DD}-from-{you}-{request-type}.md
# Content: Use request template from above

# 3. Post GitHub issue comment
gh issue comment {issue-number} --body "Research request posted to @{recipient} INBOX at collaborate/{issue-id}/"

# 4. Wait for response in @{recipient}/OUTBOX/
```

### For Responding to a Request

```bash
# 1. Review request in your INBOX
ls -la B2Connect/collaborate/{issue-id}/@{your-agent}/INBOX/

# 2. Read the request file
cat B2Connect/collaborate/{issue-id}/@{your-agent}/INBOX/{request-file}.md

# 3. Create response in your OUTBOX
# Path: B2Connect/collaborate/{issue-id}/@{your-agent}/OUTBOX/
# Name: {YYYY-MM-DD}-to-{requester}-{response-type}.md
# Content: Use response template from above

# 4. Delete your INBOX file (mark as processed)
rm B2Connect/collaborate/{issue-id}/@{your-agent}/INBOX/{request-file}.md

# 5. Git tracks deletion = message processed
git add -A && git commit -m "Response to {request}: INBOX file processed"
```

---

## 📞 Support & Questions

**Questions about the system?**
- See: `B2Connect/collaborate/COLLABORATION_MAILBOX_SYSTEM.md` (1400+ line reference)
- Ask: Comment on relevant GitHub issue mentioning @team-assistant

**Need to escalate?**
- If agent doesn't respond: @team-assistant posts escalation comment
- If process unclear: Ask @tech-lead or @scrum-master

**Want to suggest improvements?**
- File GitHub issue: "@process-assistant process-improvement: [description]"
- Process improvements tracked in collaborate/lessons-learned/

---

## ✅ Current Status: Issue #56

**Mailbox Setup**: ✅ Complete  
**Research Requests Posted**: ✅ Complete (Dec 30)  
**Status Dashboard**: ✅ Created (COORDINATION_SUMMARY.md)  
**Governance**: ✅ Enforced (COLLABORATION_MAILBOX_SYSTEM.md)  

**Pending Responses**:
- ⏳ @ui-expert: Design template analysis (due Dec 31 EOD)
- ⏳ @ux-expert: UX research findings (due Dec 31 EOD)

**Next Steps**:
1. **Dec 31**: Agents post responses in OUTBOX, delete INBOX files
2. **Jan 1**: @product-owner reviews responses and consolidates
3. **Jan 1+**: Phase 1 implementation begins

---

## 📚 Reference Documents

| Document | Purpose | Location |
|----------|---------|----------|
| **COLLABORATION_MAILBOX_SYSTEM.md** | Master governance rules (1400+ lines) | `/collaborate/COLLABORATION_MAILBOX_SYSTEM.md` |
| **COORDINATION_SUMMARY.md** | Issue-specific status dashboard | `/collaborate/{issue-id}/COORDINATION_SUMMARY.md` |
| **Request Template** | How to structure requests | Above section |
| **Response Template** | How to structure responses | Above section |
| **Scrum Master Instructions** | Full workflow documented | `.github/agents/scrum-master.agent.md` |

---

**Implementation Date**: 2025-12-30  
**Authority**: @process-assistant  
**Status**: ACTIVE & ENFORCED immediately  
**Next Review**: 2026-01-15 (after Issue #56 complete)

