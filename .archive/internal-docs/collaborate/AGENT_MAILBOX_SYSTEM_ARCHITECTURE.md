# Agent Mailbox System Architecture

**How the B2Connect agent coordination system works**

---

## System Overview

The Agent Mailbox System enables asynchronous, self-documenting coordination between agents through **structured GitHub folders** instead of scattered comments.

### Why Not GitHub Comments?

| Aspect | GitHub Comments | Mailbox System |
|--------|---|---|
| **Discoverability** | Buried in issue | Centralized folder |
| **Tracking** | Mixed with other comments | Clear request/response pairs |
| **History** | Hard to audit | Full git history |
| **Cleanup** | Clutters issue | Delete when processed |
| **Scale** | 100 comments = chaos | 100 messages = organized |
| **Intent** | Mixed (questions, feedback, questions) | Clear (request vs response) |

---

## Architecture

### Folder Structure

```
collaborate/                                 # Central coordination hub
├── issue/                                   # Per-issue coordination
│   ├── {ISSUE_ID}/
│   │   ├── COORDINATION_SUMMARY.md          # Daily summary (maintained by @team-assistant)
│   │   ├── @agent-1/                        # Agent's personal mailbox
│   │   │   ├── YYYY-MM-DD-from-{sender}-{type}.md    # Request TO this agent
│   │   │   └── agent-1-response-YYYY-MM-DD-{type}.md # Response FROM this agent
│   │   └── @agent-2/
│   │       ├── YYYY-MM-DD-from-{sender}-{type}.md
│   │       └── agent-2-response-YYYY-MM-DD-{type}.md
│   │
│   └── {ISSUE_ID}/
│       └── ...
│
├── sprint/                                  # Sprint planning & retros
│   ├── {SPRINT_NUMBER}/
│   │   ├── planning/
│   │   │   └── ISSUE_ASSIGNMENTS.md
│   │   ├── execution/
│   │   │   └── DAILY_STANDUP.md
│   │   └── retrospective/
│   │       └── LEARNINGS.md
│
├── lessons-learned/                         # Retrospectives & validated learnings
│   ├── {YYYY-MM-DD}-{topic}.md              # Individual learning
│   └── consolidated-sprint-{N}.md           # Sprint consolidation
│
└── README.md                                # Navigation index
```

### Key Principles

1. **One Inbox Per Agent** (`@agent-name/` folder)
   - All requests TO this agent in their folder
   - All responses FROM this agent in their folder
   - Makes coordination personal and clear

2. **Request → Response → Delete**
   - Create request in recipient's folder
   - Recipient creates response in same folder
   - Recipient deletes request (marks processed)
   - Full git history preserved

3. **Per-Issue Organization**
   - Coordination grouped by issue
   - Not by sprint or timeline
   - Easy to reference: "See collaborate/issue/56/"

4. **Self-Documenting**
   - Folder structure shows who's working on what
   - File names show request type and date
   - No need for external status documents

---

## Workflow

### Request Lifecycle

```
1. Sender creates request
   ├─ Location: collaborate/issue/{ID}/@{RECIPIENT}/
   ├─ File: YYYY-MM-DD-from-{SENDER}-{TYPE}.md
   ├─ Content: Clear request, acceptance criteria
   └─ Git: Commit with "add request" message

2. Recipient reviews request
   ├─ Location: Same folder
   ├─ Time: Usually < 24h

3. Recipient creates response
   ├─ Location: Same folder (@recipient/)
   ├─ File: {AGENT}-response-YYYY-MM-DD-{TYPE}.md
   ├─ Content: Deliverable addressing request
   └─ Git: Commit with "response to" message

4. Recipient deletes request
   ├─ Action: rm original request file
   ├─ Git: Commit with "mark request processed"
   └─ Result: Marks as complete
```

### Example Timeline

```
Issue #56: Store Frontend Modernization
==========================================

collaborate/issue/56/
├── @backend-developer/
│   ├── 2025-12-30-from-product-owner-api-spec.md
│   ├── backend-developer-response-2025-12-31-api-spec.md
│   └── [request deleted after response]
│
├── @frontend-developer/
│   ├── 2025-12-31-from-backend-api-ready.md
│   └── frontend-developer-response-2025-12-31-confirmed.md
│
└── @qa-engineer/
    ├── 2025-12-31-from-product-owner-test-plan.md
    └── [awaiting response...]
```

---

## Communication Patterns

### Pattern 1: Information Request

**Sender → Recipient**: "I need X information/design/spec from you"

```markdown
# API Specification Request

**From**: @product-owner  
**To**: @backend-developer  
**Due**: 2025-12-31 EOD  
**Issue**: #56

## What I Need

Design endpoint for product search

## Acceptance Criteria

- [ ] Endpoint designed
- [ ] Schemas documented
- [ ] Error handling defined
- [ ] Examples provided

---

**Response Format**: OpenAPI spec or markdown
```

### Pattern 2: Feedback/Review

**Sender → Recipient**: "Review this code/design/document"

```markdown
# Code Review Request

**From**: @product-owner  
**To**: @tech-lead  
**Due**: 2025-12-30 EOD  
**Issue**: #56

## What I Need Reviewed

Pull request: #245  
Topic: Product search API implementation

## Questions

- Does this follow our Wolverine patterns?
- Any security concerns?
- Performance OK?

---

**Response Format**: Approval + feedback
```

### Pattern 3: Clarification

**Sender → Recipient**: "Answer this question"

```markdown
# Clarification: Error Handling

**From**: @frontend-developer  
**To**: @backend-developer  
**Due**: 2025-12-30 EOD  
**Issue**: #56

## Question

How should we handle 422 validation errors from the API?

Should we:
- [ ] Show field-level errors inline?
- [ ] Show toast notification?
- [ ] Both?

---

**Response Format**: Clear answer + example code
```

---

## Team Coordination

### @team-assistant Responsibilities

Maintains `COORDINATION_SUMMARY.md` in each issue folder:

```markdown
# Issue #56 Coordination Summary

**Last Updated**: 2025-12-31 10:00  
**Status**: In Progress

## Pending Requests

| Agent | From | Type | Due | Status |
|-------|------|------|-----|--------|
| @backend-developer | product-owner | API spec | EOD | ✅ Done |
| @frontend-developer | backend-dev | API ready | EOD | ⏳ In Progress |
| @qa-engineer | product-owner | Test plan | EOD | 🔴 Not Started |

## Responses Posted

| Agent | Type | Posted | Status |
|-------|------|--------|--------|
| @backend-developer | API spec | 2025-12-31 | ✅ Processed (request deleted) |
| @frontend-developer | Confirmed | 2025-12-31 | ✅ Processed |

## Next Steps

- @qa-engineer: Start test plan today
- @frontend-developer: Continue implementation
- @product-owner: Review responses in agent folders

## Escalations

None (all on track)

---

Updated by @team-assistant daily
```

### Escalation (If Request Unanswered > 48h)

```markdown
# Escalation Notice

**Request**: API specification from @backend-developer  
**Date Sent**: 2025-12-28  
**Status**: ⏳ UNANSWERED > 48h  
**Location**: collaborate/issue/56/@backend-developer/

## Action

1. Comment in GitHub issue: "@backend-developer - request waiting in collaborate/issue/56/"
2. Tag @tech-lead if truly blocked
3. Consider moving to GitHub if async coordination not working

---

@tech-lead: Any guidance? Should we move to real-time discussion?
```

---

## Integration with GitHub Issues

### When to Use Mailbox vs GitHub

| Type | Channel |
|------|---------|
| Feature request | Create GitHub issue |
| Design feedback | GitHub PR comments |
| **Agent-to-agent request** | **Mailbox** ✅ |
| Code review | GitHub PR |
| Team decision | GitHub issue discussion |
| **Quick async Q&A** | **Mailbox** ✅ |
| Blocker notice | GitHub + Slack |
| Daily standup | GitHub issue |

### Linking GitHub to Mailbox

**In GitHub Issue**:
```markdown
## Coordination

Agent requests being tracked in:
- collaborate/issue/56/

See specific agent folders for coordination.
```

**In Mailbox Request**:
```markdown
**Issue**: #56
**GitHub PR**: #245 (if applicable)
```

---

## Rules & Constraints

### Mailbox Rules

1. ✅ **Use structured file naming** (`YYYY-MM-DD-from-{sender}-{type}.md`)
2. ✅ **One request per file** (don't combine multiple asks)
3. ✅ **Delete request after responding** (marks processed)
4. ✅ **Keep requests short** (<300 words, link to GitHub for details)
5. ✅ **Set clear deadlines** (EOD, next day, end of week)
6. ✅ **Respond within 48h** (or escalate to @tech-lead)
7. ✅ **Include acceptance criteria** (checkboxes)

### Anti-Patterns

- ❌ **Don't** create requests without deadlines (ambiguous)
- ❌ **Don't** write novel-length requests (use GitHub for discussion)
- ❌ **Don't** leave requests unanswered > 48h without escalation
- ❌ **Don't** forget to delete processed requests (breaks cleanup)
- ❌ **Don't** use for feedback better suited to code review
- ❌ **Don't** archive old requests (keep only active issue folders)

---

## Benefits

### For Agents

- ✅ Clear personal inbox (see what's requested from you)
- ✅ Asynchronous (respond on your schedule)
- ✅ Self-documenting (file names are clear)
- ✅ Traceable (full git history)
- ✅ No context switching (check folder when free)

### For Teams

- ✅ Centralized coordination (not scattered in comments)
- ✅ Auditable (full git history preserved)
- ✅ Scalable (100+ messages stay organized)
- ✅ Searchable (grep for requests/responses)
- ✅ Referenceable (link to specific coordination)

### For Management

- ✅ Visibility (see who's coordinating on what)
- ✅ Transparency (all coordination documented)
- ✅ Metrics (track coordination velocity)
- ✅ Retrospectives (easy to analyze coordination patterns)

---

## Troubleshooting

### "I Can't Find My Requests"

Check folders in order:
1. `collaborate/issue/{CURRENT_ISSUE}/@your-agent/`
2. `collaborate/issue/` (check all open issues)
3. Search: `grep -r "To.*@your-agent" collaborate/`

### "Request Went Unanswered"

1. Check if it's been > 48h
2. If yes → Comment in GitHub issue with @mention
3. Tag @tech-lead if blocking progress
4. Consider moving to real-time (Slack/call)

### "My Folder Is Getting Too Big"

Normal! Old requests are deleted after response. If old responses accumulate:
1. Archive to `collaborate/lessons-learned/` if valuable
2. Delete if reference no longer needed
3. Clean up as part of sprint retro

---

## Related Documentation

- **Quick Start**: [AGENT_MAILBOX_QUICK_START.md](./AGENT_MAILBOX_QUICK_START.md)
- **Communication Rules**: [PLAIN_COMMUNICATION_RULE.md](./PLAIN_COMMUNICATION_RULE.md)
- **Team Coordination**: [.github/agents/team-assistant.agent.md](../../.github/agents/team-assistant.agent.md)
- **Sprint Planning**: [collaborate/sprint/](./sprint/)

---

**Last Updated**: 30. Dezember 2025  
**System**: Agent Mailbox (Production)  
**Maintenance**: @team-assistant (daily summary)
