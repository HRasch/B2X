# AI Evaluation Waivers & Compliance Log

**Status**: Phase 1 Active  
**Owner**: @Security, @QA  
**Last Updated**: 11. Januar 2026

---

## Overview

This document tracks waivers for AI evaluation thresholds that fall below policy standards. Safety violations (harmful content, protected material) **cannot be waivered** and must block merges.

**Quality waivers** may be granted for:
- Relevance score 0.65–0.70 (threshold 0.70)
- Completeness score 0.70–0.75 (threshold 0.75)
- NLP scores within 5% of threshold

**Non-waiverable violations**:
- ❌ Harmful content detected (ContentHarmEvaluator)
- ❌ Protected material detected (ProtectedMaterialEvaluator)
- ❌ SQL injection or XSS patterns in code generation

---

## Waiver Template

```markdown
## Waiver: [Feature/Test Name]

| Field | Value |
|-------|-------|
| **Test Name** | (e.g., `QualityAndSafetyEvaluationTests::ChatCompletion_ShouldReturnRelevantResponse`) |
| **Evaluator** | (e.g., `RelevanceEvaluator`, `BleuEvaluator`) |
| **Score** | (e.g., `0.68`) |
| **Threshold** | (e.g., `0.70`) |
| **Gap** | (e.g., `-0.02` = 2% below) |
| **Reason** | (Brief explanation of why waiver is justified) |
| **Evidence** | (Link to PR, issue, or test data) |
| **Owner** | (GitHub handle) |
| **Expiry** | (Date; must be reviewed within 30 days) |
| **Approved By** | (Security/QA approval) |

**Mitigation**:
- [ ] Will improve in next release
- [ ] Known LLM variance; acceptable risk
- [ ] Temporary pending model update
```

---

## Active Waivers

*None currently*

---

## Expired/Resolved Waivers

*Log entries will be moved here after expiry*

---

## Non-Waiverable Violations Log

Violations of safety constraints that must block merges:

| Date | Component | Violation | Action | Status |
|------|-----------|-----------|--------|--------|
| - | - | - | - | - |

---

## Policy & Enforcement

### Waiver Approval Chain

1. **Author**: Requests waiver in PR with justification
2. **@QA**: Reviews quality threshold waivers
3. **@Security**: Reviews all waivers (especially safety-adjacent)
4. **Merge**: Only after approval from both

### SLA

| Severity | Waiver SLA | Action If Unresolved |
|----------|-----------|---------------------|
| 🟡 Yellow (quality 2–5% below) | 14 days | Auto-escalation issue created |
| 🟠 Orange (quality >5% below) | 7 days | PR blocked pending resolution |
| 🔴 Red (safety violation) | 0 days | **Immediate block, no waiver** |

### Metrics

- **Waiver count**: Tracked weekly
- **Average age**: Should not exceed 7 days
- **Expiry rate**: Target 80% resolved/expired per month

---

## References

- **Policy**: See `src/tests/AI.Evaluation/README.md` for thresholds
- **Tests**: `src/tests/AI.Evaluation/QualityAndSafetyEvaluationTests.cs`
- **CI**: `.github/workflows/ai-eval.yml`
- **ADR**: Evaluation strategy (see `.ai/decisions/ADR-*.md`)
