# 📚 Complete Documentation Standards - Agent Roles & Architecture

**Purpose**: Unified documentation standards for all B2Connect documentation  
**Authority**: @process-assistant (enforces), @software-architect (reviews)  
**Last Updated**: 29. Dezember 2025  
**Status**: ✅ ACTIVE

---

## 🎯 Three Types of Documentation

### 1️⃣ Agent-Role Instructions (in `.github/copilot-instructions-*.md`)

**What**: Guidance for specific agent roles (Backend Dev, Frontend Dev, QA, etc.)  
**Owner**: Role specialist + @software-architect  
**Standards**: [AGENT_ROLE_DOCUMENTATION_GUIDELINES.md](./docs/AGENT_ROLE_DOCUMENTATION_GUIDELINES.md)  
**Template**: [AGENT_ROLE_DOCUMENTATION_QUICK_REFERENCE.md](./docs/AGENT_ROLE_DOCUMENTATION_QUICK_REFERENCE.md)  

**Examples**:
- `.github/copilot-instructions-backend.md`
- `.github/copilot-instructions-frontend.md`
- `.github/copilot-instructions-qa.md`
- `.github/copilot-instructions-security.md`

**Must Include**:
- ✅ Mission/purpose statement
- ✅ 5-8 critical rules (with rationale)
- ✅ Quick commands (most frequent)
- ✅ Before-you-code checklist (10+ items)
- ✅ Common mistakes (with prevention)
- ✅ Code patterns (✅ correct, ❌ wrong)
- ✅ Reference files (with links)
- ✅ Escalation path
- ✅ Security checklist (if data-related)

**Read Time**: 15-30 minutes  
**Frequency**: Read before starting work, reference while coding

---

### 2️⃣ Architecture Documentation (in `/docs/architecture/`)

**What**: Why architectural choices were made, system design, constraints  
**Owner**: @software-architect (exclusive)  
**Standards**: [ARCHITECTURAL_DOCUMENTATION_STANDARDS.md](./docs/architecture/ARCHITECTURAL_DOCUMENTATION_STANDARDS.md)  

**Core Files**:
- `SOFTWARE_DEFINITION.md` - Vision, scope, constraints
- `DESIGN_DECISIONS.md` - Why choices were made (10 major decisions)
- `ESTIMATIONS_AND_CAPACITY.md` - Timeline, costs, capacity (Year 1-3)
- `ARCHITECTURAL_DOCUMENTATION_STANDARDS.md` - Quality standards
- `INDEX.md` - Navigation guide

**Authority**: @software-architect (exclusive write), all agents (read)  
**Changes**: Issue-review-based (only during sprint planning, not mid-sprint)  

**Must Include**:
- ✅ Clear ownership (who controls this)
- ✅ Version/date tracking
- ✅ Why choices matter (consequences)
- ✅ Trade-offs for each decision
- ✅ Growth projections
- ✅ Metrics/success criteria

**Read Time**: 5-60 minutes (depends on role)  
**Frequency**: Quarterly reviews, before major changes

---

### 3️⃣ Implementation Guides (in `/docs/guides/`, `/docs/features/`)

**What**: How to implement specific features or patterns  
**Owner**: @software-architect + specialists  
**Standards**: Feature-specific (reference architecture standards)  

**Examples**:
- `TESTING_FRAMEWORK_GUIDE.md` - How to write tests
- `TESTING_GUIDE.md` - Test patterns for each service
- `ONION_ARCHITECTURE.md` - How to structure services
- `WOLVERINE_HTTP_ENDPOINTS.md` - How to create endpoints

**Must Include**:
- ✅ Step-by-step instructions
- ✅ Code examples (copy-paste ready)
- ✅ Common gotchas (prevent mistakes)
- ✅ Links to reference implementations
- ✅ Success criteria (how to know you're done)

**Read Time**: 20-60 minutes  
**Frequency**: When implementing new patterns

---

## 🗂️ Documentation Organization

```
B2Connect/
├── .github/
│   ├── copilot-instructions.md              ← Main reference (all roles)
│   ├── copilot-instructions-backend.md      ← Backend Developer
│   ├── copilot-instructions-frontend.md     ← Frontend Developer
│   ├── copilot-instructions-qa.md           ← QA Engineer
│   ├── copilot-instructions-security.md     ← Security Engineer
│   ├── copilot-instructions-devops.md       ← DevOps Engineer
│   ├── copilot-instructions-quickstart.md   ← 5-minute overview
│   └── agents/                              ← Agent-specific files
│       ├── scrum-master.agent.md
│       ├── process-assistant.agent.md
│       └── ...
│
├── docs/
│   ├── AGENT_ROLE_DOCUMENTATION_GUIDELINES.md  ← THIS GUIDE
│   ├── AGENT_ROLE_DOCUMENTATION_QUICK_REFERENCE.md ← Quick template
│   ├── architecture/
│   │   ├── SOFTWARE_DEFINITION.md           ← What B2Connect is
│   │   ├── DESIGN_DECISIONS.md              ← Why choices were made
│   │   ├── ESTIMATIONS_AND_CAPACITY.md      ← Timeline & costs
│   │   ├── ARCHITECTURAL_DOCUMENTATION_STANDARDS.md ← Quality standards
│   │   └── INDEX.md                         ← Navigation
│   │
│   ├── guides/
│   │   ├── TESTING_FRAMEWORK_GUIDE.md       ← How to test
│   │   ├── TESTING_GUIDE.md                 ← Test patterns
│   │   └── ...
│   │
│   └── features/
│       ├── FEATURE_1.md
│       ├── FEATURE_2.md
│       └── ...
│
├── APPLICATION_SPECIFICATIONS.md            ← Feature requirements
├── PROJECT_DASHBOARD.md                     ← Overall status
└── README.md                                ← Getting started
```

---

## 📋 Quality Standards (Universal)

### Writing Style
- **Length**: 2,000-5,000 words (agent docs), varies (architecture/guides)
- **Audience**: Developers with 1-3 years experience (minimum)
- **Tone**: Professional, direct, action-oriented (no fluff)
- **Examples**: EVERY concept needs 2-3 working code examples
- **Clarity**: Readable in native language (EN/DE if user-facing)
- **Proof**: Reviewed before merge (no grammar errors, no broken links)

### Formatting Standards
✅ **REQUIRED**:
- **Bold** for critical concepts (`**rule**`)
- `Code` for class/method names
- [Links](path) for all references
- Tables for comparisons
- Mermaid diagrams for architecture
- Emojis for visual scanning (🎯 ⚡ 📚 🚀 🛑 ✅ ❌)
- Section headings with emoji
- Clear hierarchy (H2 → H3 → H4 max)

❌ **FORBIDDEN**:
- Wall of text (sections > 200 words)
- Nested bullet points (> 2 levels)
- Plain text links (use markdown)
- Code without output/result
- Vague rules ("be careful" → "do X because...")
- Inconsistent formatting

### Code Quality

**Every code example must have**:
- ✅ **Correct pattern** with annotation
- ❌ **Wrong anti-pattern** with explanation
- Reference to codebase file
- Output/result comments

**Format**:
```markdown
✅ **CORRECT**
```csharp
// Code
```

❌ **WRONG**
```csharp
// Bad code
```

**Why**: [Explanation]  
**Reference**: [Link to file]
```

### Governance

**Who can write documentation?**
- ✅ @software-architect (all types)
- ✅ Role specialists (agent-role docs only)
- ✅ All agents (feature-specific guides with approval)

**Who reviews documentation?**
- ✅ @software-architect (content accuracy)
- ✅ @process-assistant (format/standards compliance)
- ✅ Grammar reviewer (bilingual if EN/DE)

**Who can modify published documentation?**
- ✅ @software-architect (architecture docs - exclusive)
- ✅ @process-assistant (format/structure)
- ✅ Original author (minor updates, approved by @software-architect)
- ❌ Others (request changes, don't modify directly)

---

## 📖 Documentation by Audience

### For New Developers
**Start with**: [copilot-instructions-quickstart.md](./.github/copilot-instructions-quickstart.md) (5 min)  
**Then read**: Role-specific instructions (15-30 min)  
**Reference**: Implementation guides as needed

### For Tech Leads / Architects
**Start with**: [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md) (10 min)  
**Then read**: [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md) (15 min)  
**Then read**: [ESTIMATIONS_AND_CAPACITY.md](./docs/architecture/ESTIMATIONS_AND_CAPACITY.md) (10 min)  
**Reference**: Implementation guides for deep dives

### For QA / Testing
**Start with**: [copilot-instructions-qa.md](./.github/copilot-instructions-qa.md) (15 min)  
**Then read**: [TESTING_FRAMEWORK_GUIDE.md](./docs/TESTING_FRAMEWORK_GUIDE.md) (20 min)  
**Reference**: Feature-specific test patterns

### For Managers / PMs
**Start with**: [PROJECT_DASHBOARD.md](./PROJECT_DASHBOARD.md) (10 min)  
**Then read**: [ESTIMATIONS_AND_CAPACITY.md](./docs/architecture/ESTIMATIONS_AND_CAPACITY.md) (15 min)  
**Then read**: [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md) (8 min)

---

## ✅ Complete Quality Checklist

Before merging ANY documentation:

### Content
- [ ] Clear purpose/scope stated upfront
- [ ] Audience identified (who reads this?)
- [ ] EVERY concept has 2-3 examples
- [ ] Anti-patterns shown with explanations
- [ ] All rules have rationale/consequences
- [ ] All claims backed by evidence/links
- [ ] Cross-references to related docs
- [ ] No contradictions with other docs

### Formatting
- [ ] Header section complete (title, owner, date, status)
- [ ] Section headers with emoji (🎯 ⚡ 📚 🚀 🛑)
- [ ] Code examples in ```bash``` or ```csharp``` blocks
- [ ] Tables for comparisons/options
- [ ] Links work (test each one)
- [ ] No wall of text (sections < 200 words)
- [ ] Bold for critical concepts
- [ ] Emoji for visual scanning

### Governance
- [ ] Owner identified (@software-architect, @role, etc.)
- [ ] Version number (semantic versioning)
- [ ] Last updated date
- [ ] Status (✅ ACTIVE, 🟡 DRAFT, 🔴 DEPRECATED)
- [ ] Authority specified (who can modify)

### Completeness
- [ ] All sections present (per template)
- [ ] No TODO comments
- [ ] All questions answered
- [ ] References to other docs
- [ ] Bilingual if user-facing (EN + DE)

### Accuracy
- [ ] No grammar/spelling errors
- [ ] Code examples compile/run
- [ ] Technical accuracy verified
- [ ] Links tested (no 404s)
- [ ] Consistent terminology
- [ ] No deprecated patterns referenced

---

## 🔄 Documentation Lifecycle

### 1. Create
- Write following appropriate template
- Include all required sections
- Add 2-3 examples per concept

### 2. Review
- @software-architect reviews content
- @process-assistant reviews format
- Grammar reviewer checks EN/DE

### 3. Merge
- All feedback addressed
- Approved by reviewers
- Merged to appropriate location

### 4. Monitor
- Link checking (quarterly)
- Content accuracy (quarterly)
- Usage feedback (collect)

### 5. Update
- Quarterly reviews scheduled
- Keep current with architecture changes
- Archive deprecated docs

### 6. Retire
- Move to archive if deprecated
- Link replacements from old doc
- Update all references

---

## 📊 Documentation Metrics (Track Monthly)

| Metric | Target | Owner |
|--------|--------|-------|
| % documentation current | 100% | @software-architect |
| % links working | 100% | @process-assistant |
| % code examples tested | 100% | Author |
| Avg time to find info | < 5 min | All |
| Grammar errors | 0 | Reviewer |
| Broken cross-references | 0 | @process-assistant |
| Docs reviewed/updated | 25% per quarter | @software-architect |
| Agent satisfaction | 90%+ | Survey |

---

## 🎯 Getting Started

### To Create Agent-Role Documentation
1. Read [AGENT_ROLE_DOCUMENTATION_GUIDELINES.md](./docs/AGENT_ROLE_DOCUMENTATION_GUIDELINES.md) (20 min)
2. Copy template from [AGENT_ROLE_DOCUMENTATION_QUICK_REFERENCE.md](./docs/AGENT_ROLE_DOCUMENTATION_QUICK_REFERENCE.md)
3. Fill in 10 sections
4. Run quality checklist above
5. Submit PR with @software-architect + @process-assistant
6. Address feedback
7. Merge when approved

### To Create Architecture Documentation
1. Read [ARCHITECTURAL_DOCUMENTATION_STANDARDS.md](./docs/architecture/ARCHITECTURAL_DOCUMENTATION_STANDARDS.md) (15 min)
2. Work with @software-architect (exclusive authority)
3. Follow standards exactly
4. Include decision rationale
5. Add version/date/owner
6. Submit with approval from @software-architect

### To Update Existing Documentation
1. Identify what changed
2. Update document with rationale
3. Bump version number (semantic versioning)
4. Update last-modified date
5. Test all links
6. Commit with full explanation

---

## 📞 Questions?

| Question | Answer |
|----------|--------|
| "Where should this doc go?" | Agent-role → `.github/`, Architecture → `/docs/architecture/`, Implementation → `/docs/guides/` |
| "How long should it be?" | 2,000-5,000 words (agent), varies (others) |
| "Should it be bilingual?" | YES if user-facing (EN + DE) |
| "Who approves it?" | @software-architect (content) + @process-assistant (format) |
| "Can I modify published docs?" | NO (request changes via issue) |
| "How often update?" | Quarterly reviews, or when patterns change |

---

## 🎓 Summary

**B2Connect has three documentation types**:

1. **Agent-Role Instructions** (`.github/copilot-instructions-*.md`)
   - For specific roles (Backend Dev, Frontend Dev, QA, etc.)
   - Standard template with 10 required sections
   - 15-30 min read
   - Reviewed before merge

2. **Architecture Documentation** (`/docs/architecture/`)
   - Why architectural choices were made
   - Only @software-architect can modify
   - Reviewed quarterly
   - Issue-review-based changes (not mid-sprint)

3. **Implementation Guides** (`/docs/guides/`, `/docs/features/`)
   - How to implement specific patterns
   - Referenced from agent docs
   - Ownership varies
   - Updated as patterns evolve

**All documentation**:
- ✅ Follows quality standards
- ✅ Includes code examples (✅ correct, ❌ wrong)
- ✅ Has links to references
- ✅ Reviewed before merge
- ✅ Tracked for currency
- ✅ Kept in sync

**Use this guide to create documentation that agents actually read, understand, and follow.**

---

**Created**: 29. Dezember 2025  
**Status**: ✅ ACTIVE  
**Owner**: @process-assistant (enforces), @software-architect (reviews)  
**Next Review**: Quarterly (with architecture updates)
