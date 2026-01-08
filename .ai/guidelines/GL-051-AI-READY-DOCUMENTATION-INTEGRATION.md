---
docid: GL-088
title: GL 051 AI READY DOCUMENTATION INTEGRATION
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: GL-051
title: AI-Ready Documentation Integration Guide
category: guidelines
type: Guideline
owner: @SARAH
status: Active
created: 2026-01-08
---

# AI-Ready Documentation Integration Guide

**DocID**: GL-051  
**Purpose**: Ensure documentation templates enable seamless AI assistant integration for customer support, sales, and user guidance.

---

## Overview

This guideline ensures that all documentation created from templates (TPL-DEVDOC-001, TPL-USERDOC-001) is:
- **Machine-readable**: Structured data for AI parsing
- **Consistently referenced**: DocID-based linking for knowledge graphs
- **AI-trainable**: Quality content for LLM fine-tuning
- **Context-aware**: Metadata enables intelligent routing
- **Scalable**: Supports multiple AI use cases

---

## AI Use Cases

### 1. Customer Support Chatbot 🤖
**Purpose**: Answer customer questions automatically

**Requires**:
- **USERDOC-HOW-***: Step-by-step instructions
- **USERDOC-FAQ-***: Frequently asked questions
- **USERDOC-SCREEN-***: UI/interface guidance
- Structured troubleshooting with clear problem/solution mapping

**AI Processing**:
```
User Input: "How do I create a product?"
  ↓
AI Retrieval: Search USERDOC-HOW-* with keyword "product creation"
  ↓
AI Generation: Extract steps 1-7 from [USERDOC-HOW-001]
  ↓
User Output: Numbered steps with screenshots
```

**Template Requirements**:
- ✅ Clear step numbering
- ✅ Expected results after each step
- ✅ FAQ section for follow-up questions
- ✅ Screenshots with annotations
- ✅ Related articles for escalation

---

### 2. Step-by-Step Guidance System 👣
**Purpose**: Guide users through complex workflows

**Requires**:
- **USERDOC-HOW-***: Detailed step-by-step guides
- **USERDOC-PROC-***: Process documentation
- **USERDOC-SCREEN-***: Screen explanations
- Context about user's current location

**AI Processing**:
```
System Context: User on Product Form, needs help
  ↓
AI Retrieval: Load [USERDOC-SCREEN-001] + [USERDOC-HOW-001]
  ↓
AI Generation: Current step = Step 3 (Fill in Basic Information)
  ↓
User Output: Next action recommendation with explanation
```

**Template Requirements**:
- ✅ "Before you start" section with prerequisites
- ✅ Each step has clear input/output
- ✅ Screenshots show current UI state
- ✅ Troubleshooting linked inline
- ✅ Progress indicator (Step X of Y)

---

### 3. Sales Enablement Assistant 📊
**Purpose**: Support sales team with product/feature information

**Requires**:
- **USERDOC-FEAT-***: Feature descriptions for customers
- **USERDOC-HOW-***: Quick demonstrations
- **DEVDOC-GUIDE-***: Technical capabilities
- Benefits vs. competitor features

**AI Processing**:
```
Sales Input: "What can the system do for inventory?"
  ↓
AI Retrieval: Search [USERDOC-FEAT-*] for "inventory"
  ↓
AI Generation: Extract benefits + [USERDOC-HOW-002] for demo
  ↓
Sales Output: Feature summary + quick demo guide
```

**Template Requirements**:
- ✅ USERDOC-FEAT-* with clear benefits
- ✅ Quick steps section for fast demos
- ✅ Related how-tos for deeper dive
- ✅ Common use cases in FAQ
- ✅ Links to technical capability docs

---

### 4. User Onboarding Assistant 🎓
**Purpose**: Help new users learn system during first days

**Requires**:
- **USERDOC-START-***: Getting started guides
- **USERDOC-HOW-***: Core workflows
- **USERDOC-SCREEN-***: Screen references
- Progressive complexity increase

**AI Processing**:
```
New User: Day 1
  ↓
AI Lesson Plan: [USERDOC-START-001] + [USERDOC-HOW-001] + [USERDOC-HOW-002]
  ↓
AI Generation: Day 1 tasks with links to guidance
  ↓
User Output: "Today, learn: 1. Basics (5 min) 2. First task (10 min)"

Next Day → More complex features
```

**Template Requirements**:
- ✅ Time estimates for each guide
- ✅ Difficulty levels clearly marked
- ✅ Prerequisites listed upfront
- ✅ Progressive learning path in related articles
- ✅ Quick wins for motivation

---

## Machine-Readable Metadata Requirements

### YAML Front-Matter (Required for AI)

```yaml
---
docid: USERDOC-HOW-001
title: How to Create a Product
audience: End Users, Business Users, Sales Teams
technical_level: Beginner  # beginner | intermediate | advanced
status: Active
created: 2026-01-08
last_updated: 2026-01-08
version: 1.0

# CRITICAL FOR AI: Use cases this document supports
ai_metadata:
  use_cases:
    - customer_support      # ChatBot can use
    - step_by_step_guidance # Wizard can use
    - sales_enablement      # Sales team guide
    - user_onboarding       # New user training
    
  # Time to read/complete - helps prioritize info
  time_to_read_minutes: 5
  time_to_complete_minutes: 10
  
  # Content type flags - guides AI processing
  includes_screenshots: true
  includes_video_link: false
  includes_code_examples: false
  step_count: 7
  
  # Difficulty for AI - helps with user matching
  difficulty_level: beginner
  
  # Domain tags for knowledge graph
  domains:
    - product_management
    - catalog
    - ecommerce
  
  # For multi-language support
  languages: ["en"]  # Add: de, fr, es, it, pt, nl, pl
  
  # For content filtering - what this doc teaches
  teaches:
    - product_creation
    - basic_workflow
    - form_filling

# Related documents - creates knowledge graph edges
related:
  - USERDOC-HOW-002           # Next logical step
  - USERDOC-SCREEN-001        # Reference material
  - USERDOC-FAQ-001           # Common questions
  - USERDOC-PROC-001          # Process context

# Keywords for retrieval
keywords:
  - product
  - creation
  - catalog
  - getting-started
  - how-to

# Search terms customers actually use
search_terms:
  - how do i create a product
  - add new product
  - create item
  - new catalog entry
---
```

### Why Each Field Matters for AI

| Field | AI Purpose | Example |
|-------|-----------|---------|
| `docid` | Unique reference in knowledge graph | USERDOC-HOW-001 |
| `use_cases` | Route to correct AI system | [support, onboarding] |
| `time_*` | Prioritize info, set expectations | 5 min read, 10 min task |
| `technical_level` | Match to user skill | beginner, advanced |
| `includes_*` | Determine presentation format | screenshots needed |
| `step_count` | Track progress in wizard | Show "Step 3 of 7" |
| `domains` | Filter by topic area | product_management |
| `teaches` | Learning outcome tracking | [form_filling] |
| `related` | Create learning paths | Next: USERDOC-HOW-002 |
| `keywords` | Content retrieval | [product, creation] |
| `search_terms` | Natural language matching | "how do i create" |

---

## Content Structure for AI Processing

### Rule 1: Structured Sections ✅ YES / ❌ NO

**✅ Good - AI can parse**:
```markdown
## Step 1: Log In
1. Open browser
2. Go to website
3. Enter credentials

✅ You're now logged in
```

**❌ Bad - AI struggles**:
```markdown
## Getting Started
The first thing you should probably do is log in to the system 
by going to the website and entering your information when 
you see the login screen, which should appear...
```

**Rule**: Use numbered lists, clear headers, explicit success states.

---

### Rule 2: Explicit Problem/Solution Mapping ✅ YES / ❌ NO

**✅ Good - AI can diagnose**:
```markdown
### Problem: "Price must be higher than cost"

**Causes**:
- Price < Cost
- Decimal mismatch

**Solution**:
1. Check price field
2. Check cost field
3. Ensure price > cost
```

**❌ Bad - AI can't diagnose**:
```markdown
### Pricing Issues
Sometimes there are errors with pricing if you're not careful
with the numbers. Make sure everything is right.
```

**Rule**: Problem → Symptoms → Causes → Solutions (always in this order).

---

### Rule 3: Consistent Cross-References ✅ YES / ❌ NO

**✅ Good - Creates knowledge graph**:
```markdown
See [USERDOC-HOW-002] for next steps
Related: [USERDOC-FAQ-001]
Reference: [USERDOC-SCREEN-001]
```

**❌ Bad - AI can't follow**:
```markdown
See the next guide for more info
Check the FAQ for questions
Look at the screen reference
```

**Rule**: Always use `[DOCID]` format, never generic "the next guide".

---

### Rule 4: Data Entry Examples with Values ✅ YES / ❌ NO

**✅ Good - AI has concrete examples**:
```markdown
| Field | What to enter | Example |
|-------|---|---|
| **Product Name** | Name for customers | "Blue T-Shirt" |
| **SKU** | Product code | "SHIRT-BLUE-001" |
| **Price** | Sale price | "$19.99" |
```

**❌ Bad - AI over-generalizes**:
```markdown
Fill in all the fields with appropriate information
```

**Rule**: Use tables with examples for every data field.

---

### Rule 5: Inline Tips & Warnings ✅ YES / ❌ NO

**✅ Good - AI understands context**:
```markdown
**💡 Tip**: Use clear names. Avoid special characters.
**⚠️ Important**: Price must be > Cost
**🎯 Pro Tip**: Pick all categories so customers find you
```

**❌ Bad - AI misses nuance**:
```markdown
It's probably good to use clear names
Make sure the pricing makes sense
```

**Rule**: Use emoji + bold for emphasis so AI recognizes importance.

---

## DocID-Based Knowledge Graph

### How AI Uses DocID Links

```
Customer: "How do I set pricing?"

AI Search:
  └─ Find all docs with "pricing" keyword
     ├─ USERDOC-HOW-003 (How to set pricing)
     ├─ USERDOC-FAQ-001 (Pricing questions)
     └─ USERDOC-SCREEN-001 (Pricing field reference)

AI Reasoning:
  ├─ User is new (check related docs)
  ├─ Related: USERDOC-HOW-001 (prerequisites)
  └─ Next: USERDOC-HOW-004 (discounts)

AI Generation:
  "First, complete [USERDOC-HOW-001] 
   Then follow [USERDOC-HOW-003] steps 1-3
   Common questions: [USERDOC-FAQ-001]"
```

### Creating Strong Links

**Guideline**: Every doc should have 3-5 outbound links via DocID

```markdown
## Related Articles
- [USERDOC-HOW-002] - Next logical task (progression)
- [USERDOC-FAQ-001] - Common questions (support)
- [USERDOC-SCREEN-001] - Reference material (depth)
- [USERDOC-PROC-001] - Process context (understanding)
```

---

## Content Quality Standards for AI

### 1. Accuracy ⚠️ Critical

**Requirement**: No outdated information

- [ ] Screenshots match current UI
- [ ] Steps work in current version
- [ ] Links not broken
- [ ] Examples still valid
- [ ] Maintenance schedule documented

**How AI uses it**:
- If doc is marked as "updated 2025-12", AI deprioritizes it
- Old screenshots cause AI confusion about current UI

---

### 2. Clarity 📝 Critical

**Requirement**: Simple language, no jargon

- [ ] Explain technical terms
- [ ] Use active voice
- [ ] Short sentences
- [ ] Numbered lists for sequences
- [ ] Plain English (translate jargon)

**How AI uses it**:
- Clear docs enable accurate summarization
- Jargon confuses embeddings
- Complex sentences get misinterpreted

---

### 3. Completeness 🎯 Critical

**Requirement**: Answer full question, no "see documentation"

- [ ] Step-by-step guide included
- [ ] Troubleshooting section complete
- [ ] Expected result stated
- [ ] Next steps provided
- [ ] Prerequisites listed

**How AI uses it**:
- Incomplete docs require AI to chain multiple documents
- Each link increases error risk
- Complete docs reduce support escalation

---

### 4. Consistency ↔️ High Priority

**Requirement**: Same concepts explained same way

- [ ] Terminology used consistently
- [ ] Examples follow same format
- [ ] Screenshots use same style
- [ ] Table layouts match across docs
- [ ] Related docs have same structure

**How AI uses it**:
- Consistent patterns enable reliable extraction
- Different explanations confuse embeddings
- Consistent structure reduces parsing errors

---

### 5. Freshness 🔄 Medium Priority

**Requirement**: Regularly updated

- [ ] Reviewed every 3 months
- [ ] Screenshots updated when UI changes
- [ ] Version number incremented
- [ ] Change log maintained
- [ ] Author feedback incorporated

**How AI uses it**:
- Old content gets lower rank in retrieval
- Maintenance dates signal reliability
- Recent updates indicate accuracy

---

## AI Training & Fine-Tuning

### Preparing Content for LLM Fine-Tuning

**Structure for AI training**:

```markdown
# [DOCID] - [Title]

## Context
[What is this about? Why important?]

## Problem
[What question does this answer?]

## Solution
[How to solve it - step by step]

## Examples
[Real examples from your system]

## Related
[Links to dependent docs: DOCID]

## Common Mistakes
[What NOT to do]

## Verification
[How to know it worked]
```

**Why this structure**:
- Problem/Solution pairs work well for LLM training
- Related links create knowledge graph
- Examples improve generation quality
- Common mistakes reduce hallucination

---

### Content Filtering for Training

**Include in training data** ✅:
- USERDOC-HOW-* (task-oriented)
- USERDOC-SCREEN-* (reference)
- USERDOC-FAQ-* (Q&A pairs)
- DEVDOC-GUIDE-* (capability explanations)
- ADR-* (decision context)

**Exclude from training** ❌:
- Status documents (temporary)
- Brainstorm docs (unfinalized)
- Archive documents (obsolete)
- Internal meeting notes
- Draft/WIP documents

---

## Implementation Checklist

### Before Deploying AI Assistant

- [ ] **Documentation Complete**
  - [ ] All use-case docs have required YAML metadata
  - [ ] All docs follow template structure
  - [ ] All cross-references use DocID format
  - [ ] No broken internal links
  
- [ ] **Content Quality**
  - [ ] Screenshots updated for current UI
  - [ ] Steps tested with real user
  - [ ] Examples with actual values
  - [ ] Troubleshooting covers 80% of issues
  
- [ ] **Knowledge Graph**
  - [ ] All USERDOC-HOW-* are indexed
  - [ ] Related articles section complete
  - [ ] FAQ sections populated
  - [ ] Learning paths defined in related articles
  
- [ ] **AI Metadata**
  - [ ] All docs have valid YAML front-matter
  - [ ] use_cases field populated
  - [ ] time_to_* estimated
  - [ ] Keywords for retrieval included
  
- [ ] **Support Systems**
  - [ ] Metadata tags match your support system
  - [ ] DocID routing configured
  - [ ] Escalation paths defined
  - [ ] Fallback to human support configured

---

## Maintenance & Evolution

### Quarterly Review Process

**Every 3 months**:
1. Run audit script: `scripts/docs-audit.sh`
2. Check for outdated screenshots (compare UI)
3. Review support tickets for uncovered questions
4. Add new FAQ entries from support
5. Update modification dates
6. Version bump if changes significant

**Process**:
```bash
# Audit all docs
scripts/docs-audit.sh > audit-report.csv

# Identify docs > 90 days old
grep "last_updated.*2025" audit-report.csv

# Update those docs
# Increment version
# Update last_updated date in YAML
```

---

### AI Performance Monitoring

**Track these metrics**:

| Metric | Goal | How to Measure |
|--------|------|---|
| Doc Retrieval Accuracy | >95% | % of AI responses with correct DocID |
| Step Completion Rate | >90% | % of users completing guided steps |
| FAQ Hit Rate | >80% | % of questions answered from FAQ |
| Escalation Rate | <10% | % of issues requiring human help |
| User Satisfaction | >4.5/5 | Post-interaction rating |

**Feedback Loop**:
1. Track which docs get asked about most
2. Identify missing docs or sections
3. Update content based on patterns
4. Re-train LLM with updated content
5. Monitor improvement metrics

---

## Summary: AI-Ready Documentation Means

✅ **Structured**: Numbered lists, clear sections, tables  
✅ **Referenced**: All links use DocID format  
✅ **Complete**: Answers question fully, no "see documentation"  
✅ **Consistent**: Same concepts explained same way  
✅ **Metadata-Rich**: YAML front-matter with AI fields  
✅ **Connected**: Related articles create knowledge graph  
✅ **Fresh**: Maintained on regular schedule  
✅ **Tested**: Steps work with current system  

---

**Guideline Version**: 1.0  
**Last Updated**: 2026-01-08  
**Owner**: @SARAH  
**Related**: [GL-049], [GL-050], [TPL-DEVDOC-001], [TPL-USERDOC-001]