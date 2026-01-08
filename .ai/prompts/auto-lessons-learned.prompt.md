---
docid: UNKNOWN-170
title: Auto Lessons Learned.Prompt
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
agent: TechLead
description: Auto-generate lessons learned entry after bug fix
---

# Auto Lessons Learned Generation

Generate a lessons learned entry for the knowledge base after resolving a bug.

## Bug Fix Information Required
- Bug Description:
- Root Cause:
- Solution Applied:
- Files Modified:
- Time to Fix:
- Complexity: [Low/Medium/High]

## Lesson Template

### [Bug Category]: [Brief Description]

**Issue**: [What was the problem?]

**Root Cause**: [Why did it happen?]

**Lesson**: [Key takeaway to prevent similar issues]

**Solution**: [How it was fixed - with code examples]

```[language]
// Code example of the fix
[code here]
```

**Benefits**:
- **[Benefit 1]**: [Explanation]
- **[Benefit 2]**: [Explanation]

**Prevention**: [How to avoid this in the future]

## Output Format

```
## [Generated Lesson Title]

**Issue**: [One sentence description]

**Root Cause**: [Technical explanation]

**Lesson**: [Key principle learned]

**Solution**: [Code example]

**Benefits**:
- [List 2-3 benefits]

**Prevention**: [Future avoidance strategy]
```

## Integration
- [ ] Add to `.ai/knowledgebase/lessons.md`
- [ ] Update relevant patterns in `.ai/knowledgebase/patterns-antipatterns.md`
- [ ] Notify team via Slack/Teams if critical lesson