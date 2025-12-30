# 📋 Agent-Role Documentation Quick Reference

**How to create documentation for agent instruction files**

---

## 🎯 10-Section Template (Copy-Paste Structure)

```markdown
# [Role Name] - [Focus Area]

**Focus**: [Main responsibility]  
**Agent**: @[name]  
**Escalation**: [Problem] → @[contact]  
**Reference**: [Link to main instructions]

---

## 🎯 Your Mission
[What does this role do? 2-3 sentences]

---

## ⚡ Critical Rules
1. **Rule**: Description [with rationale]
2. **Rule**: Description [with rationale]
...

---

## 🚀 Quick Commands
```bash
command 1        # Purpose
command 2        # Purpose
```

---

## 📋 Before Implementing [X]
### Category 1
- [ ] Check 1
- [ ] Check 2

### Category 2
- [ ] Check 1

---

## 🛑 Common Mistakes
| Mistake | Prevention |
|---------|-----------|
| Do this wrong | Do this right instead |

---

## 🎯 [Pattern Name] - REQUIRED

✅ **CORRECT CODE EXAMPLE**
```csharp
// Good pattern
```

❌ **WRONG ANTI-PATTERN**
```csharp
// Bad pattern
```

---

## 📚 Reference Files
| Document | Path | When to Use |
|----------|------|-------------|
| [Name](link) | path | When you need X |

---

## 🚀 Escalation Path
- **Situation A**: Ask @agent-name
- **Situation B**: Ask @agent-name → escalate to @other

---

## 🔐 Security Checklist
Before PR: Does your code include?
- [ ] Security check 1
- [ ] Security check 2

---
```

---

## ✅ Quick Checklist (Before Submitting)

- [ ] **Structure**: All 10 sections present
- [ ] **Examples**: Every pattern has ✅ correct AND ❌ wrong
- [ ] **Clarity**: No wall of text (sections < 200 words)
- [ ] **Links**: All references are links, not plain text
- [ ] **Emojis**: Used for scanning (🎯 ⚡ 📚 🚀 🛑 ✅ ❌)
- [ ] **Code**: Copy-paste ready (proper formatting)
- [ ] **Grammar**: Proofread, bilingual if user-facing
- [ ] **Consistency**: Matches other agent docs
- [ ] **Actionable**: Reader knows exactly what to do
- [ ] **Complete**: Answers "why" for every rule

---

## 📝 Writing Tips

| Tip | Example |
|-----|---------|
| **Bold key terms** | Use **Wolverine pattern**, not "wolverine pattern" |
| **Code in backticks** | Use `IProductRepository`, not IProductRepository |
| **Link everything** | See [TESTING_GUIDE.md](path), not "TESTING_GUIDE.md" |
| **Show consequences** | "Deferred builds cause 38+ test failures" not "build early" |
| **Use tables for options** | Compare patterns side-by-side |
| **Include diagrams** | Mermaid for architecture/flow |
| **Never assume knowledge** | Explain every pattern |
| **Real code samples** | Link to actual codebase files |

---

## 🔄 Integration Path

1. **Create** documentation using this template
2. **Review** with @software-architect
3. **Update** agent instructions file (`.github/copilot-instructions-*.md`)
4. **Link** back to detailed documentation
5. **Enforce** standards via @process-assistant
6. **Measure** compliance (quarterly review)

---

## ❌ Common Mistakes to Avoid

| ❌ Mistake | ✅ Fix |
|-----------|-------|
| Wall of text | Break into sections < 200 words each |
| No code examples | Show ✅ correct AND ❌ wrong |
| "Don't do X" | "Don't do X because [consequence]" |
| Plain text links | Use `[text](path)` markdown links |
| Single example | Show 2-3 examples per pattern |
| Vague rules | "Always build first" → "Generate files → Build immediately → Fix errors → Test" |
| Missing references | Link to where to find more info |
| No anti-patterns | Show what NOT to do with explanation |
| Inconsistent formatting | Follow this template exactly |
| No version tracking | Include Last Updated date |

---

## 📊 Template Comparison

### ❌ WRONG (Wall of Text)
```markdown
# Backend Development

The backend team should follow these patterns and make sure to use the 
right architectural approach and remember that Wolverine is important 
and you should always make sure your code is tested and documented 
properly before committing...
```

### ✅ RIGHT (Clear Sections)
```markdown
# Backend Developer - Wolverine Services

**Focus**: HTTP endpoints, DDD patterns, multi-tenant safety  
**Agent**: @backend-developer  
**Escalation**: Architecture → @tech-lead  

---

## ⚡ Critical Rules
1. **Wolverine Pattern**: Plain POCO command + Service.PublicAsyncMethod()
2. **Tenant Isolation**: EVERY query filters by TenantId
3. **Build First**: Code → Build → Test → Commit

---

## 🚀 Quick Commands
```bash
dotnet build B2Connect.slnx
dotnet test backend/Domain/[Service]/tests
```
```

---

## 🎓 Examples by Role

### Backend Developer
**Must include**: Wolverine pattern, Onion architecture, tenant isolation, database design, testing

### Frontend Developer
**Must include**: Vue 3 pattern, Tailwind CSS, accessibility (WCAG), responsive design, testing

### QA Engineer
**Must include**: Test types, xUnit pattern, compliance testing, metrics, acceptance criteria

### Security Engineer
**Must include**: Encryption standards, audit logging, PII handling, security checklist, compliance

### DevOps Engineer
**Must include**: Port management, service orchestration, infrastructure, deployment, troubleshooting

---

## 📞 Questions?

| Question | Answer |
|----------|--------|
| Where does this go? | `.github/copilot-instructions-[role].md` or `.github/agents/[role].agent.md` |
| How long should it be? | 2,000-5,000 words (10-15 min read) |
| How many examples? | At least 2-3 per pattern (✅ correct + ❌ wrong) |
| Should it be bilingual? | If user-facing: YES (EN + DE) |
| Who approves it? | @software-architect (content) + @process-assistant (format) |
| How often update? | Quarterly review, or when patterns change |

---

**Use this quick reference to create documentation quickly.**  
**Full guide**: See [AGENT_ROLE_DOCUMENTATION_GUIDELINES.md](./AGENT_ROLE_DOCUMENTATION_GUIDELINES.md)

---

**Last Updated**: 29. Dezember 2025  
**Status**: ✅ READY TO USE
