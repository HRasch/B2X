---
docid: UNKNOWN-013
title: Code Review.Prompt
owner: @DocMaintainer
status: Archived
created: 2026-01-08
---

---
agent: TechLead
description: Standardized code review process
---

# Code Review Request

Review the provided code changes with focus on:

## Review Checklist

### 1. Code Quality
- [ ] Code is readable and well-structured
- [ ] Naming is clear and consistent
- [ ] No unnecessary complexity
- [ ] DRY principle followed

### 2. Logic & Correctness
- [ ] Logic is correct and handles edge cases
- [ ] Error handling is appropriate
- [ ] No potential bugs or issues

### 3. Security
- [ ] No exposed secrets or credentials
- [ ] Input validation present where needed
- [ ] No SQL injection or XSS vulnerabilities
- [ ] Authorization checks in place

### 4. Performance
- [ ] No obvious performance issues
- [ ] Efficient algorithms used
- [ ] Database queries optimized

### 5. Testing
- [ ] Tests cover critical paths
- [ ] Tests are meaningful and maintainable

## Output Format

Provide review as:

```
## Summary
[One sentence overall assessment]

## Approval Status
[✅ Approved | ⚠️ Approved with Comments | ❌ Changes Requested]

## Critical Issues (Blocking)
- [Issue 1]
- [Issue 2]

## Suggestions (Non-blocking)
- [Suggestion 1]
- [Suggestion 2]

## Positive Notes
- [What was done well]
```
