# Agent Mailbox Quick Start

**How to coordinate agent-to-agent work in B2Connect**

---

## The System (30 seconds)

Agents coordinate by leaving requests/responses in **personal folders**.

```
collaborate/issue/56/
├── @backend-developer/
│   ├── 2025-12-30-from-product-owner-api-request.md  ← REQUEST TO ME
│   └── backend-developer-response-2025-12-30-api-design.md  ← MY RESPONSE
│
├── @frontend-developer/
│   ├── 2025-12-30-from-backend-api-spec.md  ← REQUEST TO ME
│   └── frontend-developer-response-2025-12-30-ready.md  ← MY RESPONSE
```

## How to Request Something (2 minutes)

### Step 1: Find the Right Folder
```bash
cd collaborate/issue/{ISSUE_NUMBER}/@{RECIPIENT_AGENT}/
```

### Step 2: Create Request File
**File name format**: `YYYY-MM-DD-from-{SENDER}-{TYPE}.md`

Example:
```bash
2025-12-30-from-product-owner-api-spec-request.md
```

### Step 3: Write Request (Template)
```markdown
# API Specification Request

**From**: @product-owner  
**To**: @backend-developer  
**Due**: 2025-12-31 EOD  
**Issue**: #56

## What I Need

Design REST endpoint for product search:
- Input: category, price range, sort
- Output: products with ratings
- Pagination support

## Acceptance Criteria

- [ ] Endpoint designed
- [ ] Request/response schemas defined
- [ ] Error cases documented
- [ ] Example calls provided

## Deliverable Format

OpenAPI spec (YAML) or detailed markdown

---

**Request posted to**: collaborate/issue/56/@backend-developer/
**Delete this file when responding** (marks request processed)
```

### Step 4: Add to Git & Notify
```bash
git add collaborate/issue/56/@backend-developer/2025-12-30-from-product-owner-api-spec-request.md
# Mention in issue comment: "@backend-developer see request in collaborate/"
```

## How to Respond (2 minutes)

### Step 1: Find Your Folder
```bash
cd collaborate/issue/{ISSUE_NUMBER}/@{YOUR_AGENT}/
```

### Step 2: Create Response File
**File name format**: `{AGENT-NAME}-response-YYYY-MM-DD-{TYPE}.md`

Example:
```bash
backend-developer-response-2025-12-30-api-spec.md
```

### Step 3: Write Response (Template)
```markdown
# API Specification Response

**From**: @backend-developer  
**To**: @product-owner  
**Fulfills**: Request from 2025-12-30  
**Status**: ✅ Complete

## API Endpoints

```
GET /api/products/search?category={}&priceMin={}&priceMax={}&sort={}
Response: { products: [...], total: N, page: N }
```

[Full specification...]

---

**Posted to**: collaborate/issue/56/@backend-developer/
**Original request deleted**: ✅ (marks as processed)
```

### Step 4: Add to Git & Notify
```bash
git add collaborate/issue/56/@backend-developer/backend-developer-response-2025-12-30-api-spec.md
# Delete original request (marks as processed)
rm collaborate/issue/56/@backend-developer/2025-12-30-from-product-owner-api-spec-request.md
git add -u
# Mention in issue comment: "@product-owner response ready in collaborate/"
```

## When to Use Mailbox vs GitHub

| Situation | Use |
|-----------|-----|
| Feature request | GitHub issue |
| Design feedback | GitHub PR comments |
| Agent-to-agent request | **Mailbox** ✅ |
| Team standup | GitHub issue |
| Code review | GitHub PR |
| Quick async Q&A | **Mailbox** ✅ |
| Urgent blocker | GitHub + Slack |

## Real Example

**Scenario**: Frontend dev needs API spec from backend dev for Issue #56

```
Day 1 (Monday):
  @product-owner creates issue #56
  Creates request: collaborate/issue/56/@backend-developer/
    File: 2025-12-30-from-product-owner-api-request.md
    Content: "Design product search endpoint"

Day 2 (Tuesday):
  @backend-developer creates response: collaborate/issue/56/@backend-developer/
    File: backend-developer-response-2025-12-30-api-design.md
    Content: Full OpenAPI spec + examples
  Deletes original request (marks processed)

Day 3 (Wednesday):
  @frontend-developer creates request: collaborate/issue/56/@backend-developer/
    File: 2025-12-30-from-frontend-error-handling.md
    Content: "How should we handle 422 validation errors?"

Day 3 (Wednesday, later):
  @backend-developer responds: collaborate/issue/56/@backend-developer/
    File: backend-developer-response-2025-12-30-error-handling.md
    Content: "Return structured error with field names"
  Deletes request
```

## Folder Structure

```
collaborate/
├── issue/                      ← Agent coordination by issue
│   ├── 56/                     ← Issue #56
│   │   ├── @backend-developer/
│   │   ├── @frontend-developer/
│   │   ├── @qa-engineer/
│   │   └── COORDINATION_SUMMARY.md
│   └── 57/
│
├── sprint/                     ← Sprint planning/retros
│   ├── 1/
│   │   ├── planning/
│   │   ├── execution/
│   │   └── retrospective/
│   └── 2/
│
├── lessons-learned/            ← Retrospectives & learnings
│   ├── consolidated-sprint-1.md
│   └── 2025-12-30-issue-56-learnings.md
│
└── README.md                   ← Main navigation
```

## Tips

- ✅ **Keep requests specific** (max 200 words)
- ✅ **Clear acceptance criteria** (checkboxes)
- ✅ **Set realistic deadlines** (EOD, next day, end of week)
- ✅ **Delete request after responding** (cleanup, marks processed)
- ✅ **Link to GitHub issue** in request/response

- ❌ **Don't** leave requests unanswered > 48h (escalate in GitHub)
- ❌ **Don't** write novel-length requests (link to GitHub for details)
- ❌ **Don't** keep archive of old requests (only current issue folders)

## Escalation

If request unanswered > 48h:
1. Comment in GitHub issue: "@agent-name - request waiting in collaborate/issue/{N}/@your-agent/"
2. Tag @tech-lead if truly blocked
3. Move work to GitHub PR if async isn't working

---

**Related**:
- [System Architecture](./AGENT_MAILBOX_SYSTEM_ARCHITECTURE.md)
- [Plain Communication Rule](./PLAIN_COMMUNICATION_RULE.md)
