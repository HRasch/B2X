---
docid: QS-AI-DOCS
title: AI-Ready Documentation Quick Start
category: Quick Start Guides
status: Active
created: 2026-01-08
---

# AI-Ready Documentation Quick Start

**DocID**: QS-AI-DOCS  
**Purpose**: Quick reference for creating AI-friendly documentation using templates

---

## 🚀 In 60 Seconds

### What You're Creating
Documentation that **AI assistants can use** for:
- 💬 Customer support chatbots
- 👣 Step-by-step guidance systems
- 📊 Sales enablement
- 🎓 User onboarding
- 📱 Mobile app help

### Three Key Files
1. **TPL-DEVDOC-001** - Template for technical/developer docs
2. **TPL-USERDOC-001** - Template for user/end-user docs
3. **GL-051** - Rules for making docs AI-friendly

### One Rule
**Use structured sections + DocID links + metadata**

---

## 📋 Quick Decision Tree

```
What am I documenting?

├─ How users DO something (create account, place order)
│  └─ Use: TPL-USERDOC-001
│     Type: USERDOC-HOW-*
│     Example: USERDOC-HOW-001-create-account.md
│
├─ How developers BUILD something (architecture, patterns)
│  └─ Use: TPL-DEVDOC-001
│     Type: DEVDOC-GUIDE-*
│     Example: DEVDOC-GUIDE-001-architecture-overview.md
│
├─ Reference material (screen fields, API endpoints)
│  ├─ If users: TPL-USERDOC-001
│  │  Type: USERDOC-SCREEN-*
│  │
│  └─ If developers: TPL-DEVDOC-001
│     Type: DEVDOC-API-*
│
└─ Questions people ask (FAQs)
   ├─ If users: TPL-USERDOC-001
   │  Type: USERDOC-FAQ-*
   │
   └─ If developers: TPL-DEVDOC-001
      Type: DEVDOC-FAQ-*
```

---

## 📝 Step-by-Step: Create Your First AI-Ready Doc

### Step 1: Pick Your Template (30 seconds)

**Are users or developers reading this?**

- **Users** (end-users, customers, sales team) → Use **TPL-USERDOC-001**
- **Developers** (software engineers, architects) → Use **TPL-DEVDOC-001**

### Step 2: Copy Template (15 seconds)

```bash
# For user docs
cp .ai/templates/TPL-USERDOC-001-USER-DOCS-TEMPLATE.md \
   docs/user/howto/USERDOC-HOW-001-my-task.md

# For developer docs
cp .ai/templates/TPL-DEVDOC-001-DEVELOPER-DOCS-TEMPLATE.md \
   docs/developer/guides/DEVDOC-GUIDE-001-my-feature.md
```

### Step 3: Update YAML Front-Matter (2 minutes)

```yaml
---
docid: USERDOC-HOW-001-my-task
title: How to Do My Task
category: user/howto
type: How-To Guide
audience: End Users
technical_level: Beginner
status: Active
last_updated: 2026-01-08

ai_metadata:
  use_cases:
    - customer_support
    - user_onboarding
  time_to_read_minutes: 5
  time_to_complete_minutes: 10
  includes_screenshots: true
  step_count: 7

related:
  - USERDOC-HOW-002
  - USERDOC-FAQ-001

keywords:
  - task
  - workflow
  - basic
---
```

### Step 4: Fill Content (10-20 minutes)

**For USERDOC docs** - Follow this order:
1. Overview (2 paragraphs max)
2. Quick steps (for experienced users)
3. Before you start (checklist)
4. Step-by-step guide (numbered, 7-15 steps)
5. Screenshots (at least 3)
6. Troubleshooting (common problems)
7. Related articles

**For DEVDOC docs** - Follow this order:
1. Overview (what & why)
2. Core concepts (define terms)
3. Architecture (design & diagrams)
4. Code examples (both simple & advanced)
5. Common patterns (recurring solutions)
6. Troubleshooting (debug issues)
7. Related resources

### Step 5: Add DocID Links (5 minutes)

✅ **Good** - Links using DocID format:
```markdown
See [USERDOC-HOW-002] for next steps
Learn more: [DEVDOC-GUIDE-001]
Questions? Check [USERDOC-FAQ-001]
```

❌ **Bad** - Generic links:
```markdown
See the next guide for next steps
Learn more in the documentation
Check the FAQ
```

### Step 6: Test & Review (10 minutes)

**Checklist**:
- [ ] Tested with actual user/reader
- [ ] All links work (internal DocID links + external links)
- [ ] Screenshots current and clear
- [ ] Steps actually work (if step-by-step)
- [ ] No broken formatting

### Step 7: Commit (1 minute)

```bash
git add docs/user/howto/USERDOC-HOW-001-my-task.md
git commit -m "docs(user): Add USERDOC-HOW-001 - How to Do My Task

Clear, step-by-step guide for [audience] to [outcome]

- 7 numbered steps with screenshots
- Troubleshooting section with 3 common problems
- Related to [USERDOC-HOW-002] and [USERDOC-FAQ-001]
- Time to complete: ~10 minutes"
```

---

## 🎯 Template Sections at a Glance

### TPL-USERDOC-001 Sections

| Section | When Needed | AI Use |
|---------|-------------|--------|
| Overview | Always | Summarize for users |
| Quick Links | Recommended | Route user quickly |
| Quick Steps | Recommended | Fast path for experts |
| Before You Start | Recommended | Set expectations |
| Step-by-Step Guide | Always | Main content for chatbots |
| Screenshots | Recommended | Visual learners + context |
| Troubleshooting | Recommended | Problem diagnosis |
| Get Help | Recommended | Escalation routing |
| FAQ | Recommended | Q&A chatbot content |
| Related Articles | Always | Learning paths + knowledge graph |

### TPL-DEVDOC-001 Sections

| Section | When Needed | AI Use |
|---------|-------------|--------|
| Overview | Always | Summarize for developers |
| Table of Contents | Recommended | Navigate long docs |
| Core Concepts | Recommended | Vocabulary building |
| Architecture | Varies | System understanding |
| Code Examples | Always | Pattern recognition |
| Common Patterns | Recommended | Implementation guidance |
| Getting Started | Recommended | Onboarding devs |
| Troubleshooting | Recommended | Debug assistance |
| Related Resources | Always | Knowledge graph |

---

## 📚 Examples by Type

### USERDOC Example: How-To Guide

**DocID**: `USERDOC-HOW-001-create-account`  
**Audience**: New users, customers  
**AI Use**: Customer support bot, onboarding  
**Structure**:
- 5-minute read time
- 7 numbered steps
- 2 screenshots
- Troubleshooting for "email already exists" and "invalid password"
- Related: [USERDOC-HOW-002], [USERDOC-SCREEN-001], [USERDOC-FAQ-001]

### DEVDOC Example: Architecture Guide

**DocID**: `DEVDOC-ARCH-001-system-architecture`  
**Audience**: Developers, architects  
**AI Use**: Onboarding engineers, code review  
**Structure**:
- 15-minute read time
- 3 core concepts defined
- System diagram (ASCII + real diagram)
- 5 code examples (simple to advanced)
- 3 common patterns with links to real code
- Troubleshooting: common setup issues

### USERDOC Example: Screen Reference

**DocID**: `USERDOC-SCREEN-001-product-form`  
**Audience**: End users, support team  
**AI Use**: Context-aware help, field-level guidance  
**Structure**:
- Screenshot of full form
- Table of all fields: Name | Purpose | Example | Required?
- Inline help for complex fields
- Link to related how-tos
- Link to FAQ for field-specific questions

---

## 🤖 AI Integration Checklist

Before using your docs with AI assistants, verify:

### Structure ✓
- [ ] Sections follow template order
- [ ] Numbered steps where applicable
- [ ] Tables for data (not paragraphs)
- [ ] Clear headings (## Section, ### Subsection)

### References ✓
- [ ] All internal links use DocID format [DOCID]
- [ ] Related articles section populated
- [ ] 3-5 outbound DocID links minimum
- [ ] No broken links

### Metadata ✓
- [ ] YAML front-matter complete
- [ ] `ai_metadata.use_cases` populated
- [ ] `time_to_*` fields estimated
- [ ] `keywords` added for retrieval
- [ ] `related` section has 3-5 DocIDs

### Content ✓
- [ ] Screenshots current (dated within 3 months)
- [ ] Examples include real values (not "enter something")
- [ ] Troubleshooting has problem/cause/solution structure
- [ ] Expected results stated after each step
- [ ] Language is simple (no jargon without explanation)

### Maintenance ✓
- [ ] Document marked as "Active"
- [ ] `last_updated` date is recent (< 30 days)
- [ ] Version number present
- [ ] Change log included

---

## 🔗 DocID Format Reference

### User Documentation (USERDOC-*)

```
USERDOC-[CATEGORY]-[NUMBER]-[slug-title]

CATEGORIES:
  START   = Getting Started guides
  FEAT    = Feature descriptions
  HOW     = How-To guides
  SYS     = System overview
  PROC    = Process guides
  SCREEN  = Screen references
  FAQ     = Frequently asked questions

EXAMPLES:
  USERDOC-START-001-getting-started
  USERDOC-HOW-001-create-product
  USERDOC-SCREEN-001-product-form
  USERDOC-FAQ-001-common-questions
```

### Developer Documentation (DEVDOC-*)

```
DEVDOC-[CATEGORY]-[NUMBER]-[slug-title]

CATEGORIES:
  ARCH    = Architecture
  API     = API reference
  GUIDE   = Developer guides
  FEAT    = Feature docs
  HOW     = How-To guides
  FAQ     = FAQ

EXAMPLES:
  DEVDOC-ARCH-001-system-architecture
  DEVDOC-API-001-product-api
  DEVDOC-GUIDE-001-getting-started
  DEVDOC-HOW-001-implement-feature
```

---

## 💡 Pro Tips

### Tip 1: Start Small
Don't write the 50-page guide first. Start with:
- One how-to (10 minutes)
- One FAQ (5 minutes)
- One screen reference (5 minutes)

Then expand based on support tickets.

### Tip 2: Use Real Examples
❌ Bad: "Enter a product name"  
✅ Good: "Enter a product name (example: 'Blue T-Shirt')"

AI learns better with concrete examples.

### Tip 3: Link Strategically
Every doc should have 3-5 outbound DocID links:
- 1 "previous" (what comes before)
- 2-3 "related" (similar topics)
- 1 "next" (what comes after)

This creates learning paths for AI.

### Tip 4: Metadata is for Machines
YAML front-matter looks boring, but it:
- Routes docs to correct AI system
- Estimates user skill level
- Enables smart filtering
- Improves retrieval accuracy

Don't skip it!

### Tip 5: Screenshot Maintenance
Set a reminder every 3 months to:
1. Compare screenshots to current UI
2. Update if changed
3. Bump version number
4. Update `last_updated` date

Old screenshots confuse AI.

### Tip 6: Use Tables for Data
❌ Instead of: "Enter the field, which represents..."  
✅ Use a table:
```markdown
| Field | What to Enter | Example |
|-------|---|---|
| **Name** | Product name | "Blue T-Shirt" |
| **SKU** | Unique code | "SHIRT-BLUE-001" |
```

Tables are easier for AI to parse.

---

## 📞 Need Help?

### Questions About:
- **How to create USERDOC** → See [TPL-USERDOC-001]
- **How to create DEVDOC** → See [TPL-DEVDOC-001]
- **AI integration rules** → See [GL-051]
- **Directory structure** → See [GL-050]
- **Documentation framework** → See [GL-049]

### Getting Templates
```bash
# Templates are in
.ai/templates/TPL-DEVDOC-001-*.md
.ai/templates/TPL-USERDOC-001-*.md
```

### Registering New Docs
Update: `.ai/DOCUMENT_REGISTRY.md`
```markdown
| `USERDOC-HOW-001` | My new guide | `docs/user/howto/USERDOC-HOW-001-*.md` | Active |
```

---

## ✅ Success Criteria

You've created **AI-ready documentation** when:

✅ Sections follow template structure  
✅ All internal links use DocID format  
✅ YAML metadata complete with `ai_metadata`  
✅ Examples include real values  
✅ Screenshots current (< 3 months old)  
✅ Troubleshooting has problem/solution structure  
✅ Related articles linked (3-5 minimum)  
✅ Maintenance schedule documented  

---

**Quick Start Version**: 1.0  
**Last Updated**: 2026-01-08  
**Related**: [TPL-DEVDOC-001], [TPL-USERDOC-001], [GL-051], [GL-050], [GL-049]