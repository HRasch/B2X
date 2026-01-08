---
docid: TPL-015
title: TPL DEVDOC 001 DEVELOPER DOCS TEMPLATE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: TPL-DEVDOC-001
title: Developer Documentation Template
category: Documentation Templates
type: Developer Documentation (DEVDOC-*)
created: 2026-01-08
---

# Developer Documentation Template

Use this template for all **DEVDOC-*** (Developer Documentation) files.

---

## Template Usage

1. Copy this file
2. Rename: `DEVDOC-[CATEGORY]-[NUMBER]-[short-title].md`
   - Example: `DEVDOC-ARCH-001-system-architecture.md`
3. Update the YAML front-matter
4. Fill in all sections
5. Remove instruction comments (lines starting with `<!--`)

---

```yaml
---
docid: DEVDOC-ARCH-001-system-architecture
title: System Architecture Overview
category: developer/architecture
type: Architecture
audience: Developers, Architects
technical_level: Advanced
status: Active
last_updated: 2026-01-08
version: 1.0
language: en

# Keywords for search/AI indexing
keywords: 
  - architecture
  - system-design
  - microservices

# Related documents for cross-referencing
related:
  - DEVDOC-ARCH-002
  - ADR-001
  - KB-006

# Machine-readable metadata for AI assistants
ai_metadata:
  use_cases:
    - system_understanding
    - onboarding
    - architecture_review
  difficulty_level: advanced
  time_to_read_minutes: 15
  includes_code_examples: true
  includes_diagrams: true
---
```

---

## Document Structure

### 1. 🎯 Overview (Required)
**Purpose**: Quick summary of what this document covers

```markdown
## Overview

This document describes [WHAT]. It covers:
- Point 1
- Point 2
- Point 3

**Intended Audience**: [WHO should read this?]
**Prerequisites**: [What should reader know first?]
**Reading Time**: ~15 minutes
```

**Tips for AI**:
- Keep concise and scannable
- List key points with bullets
- Specify audience level

---

### 2. 📋 Table of Contents (Recommended)
**Purpose**: Help readers/AI navigate long documents

```markdown
## Table of Contents

1. [Overview](#overview)
2. [Core Concepts](#core-concepts)
3. [Architecture](#architecture)
4. [Code Examples](#code-examples)
5. [Common Patterns](#common-patterns)
6. [Troubleshooting](#troubleshooting)
7. [Related Resources](#related-resources)
```

**Tips for AI**:
- Use consistent heading structure
- Use internal links (#anchors)
- Helps AI understand document structure

---

### 3. 🔑 Core Concepts (Recommended)
**Purpose**: Define key terms and concepts

```markdown
## Core Concepts

### Bounded Context
A bounded context is [definition]. Key characteristics:
- Feature 1
- Feature 2

**Related Concept**: [Link to DEVDOC or KB]
```

**Tips for AI**:
- Define domain-specific terms
- Link to related concepts
- Make it easy to build mental models

---

### 4. 🏗️ Architecture/Design (Varies by topic)
**Purpose**: Explain structure, design patterns, system layout

```markdown
## Architecture

### High-Level Design
[Description]

### Diagram
```
[ASCII diagram or reference to external diagram]
```

### Components
| Component | Responsibility | Technology |
|-----------|---|---|
| Service A | Does X | .NET |
| Service B | Does Y | Node.js |

**Design Decisions**: [Link to ADR-*]
**Rationale**: [Why was this design chosen?]
```

**Tips for AI**:
- Use structured formats (tables, lists)
- Include diagrams for visual learners + AIs
- Reference architectural decisions (ADRs)
- Explain rationale, not just structure

---

### 5. 💻 Code Examples (Recommended)
**Purpose**: Show practical implementation

```markdown
## Code Examples

### Example 1: Basic Usage

\`\`\`csharp
// C# example
public class MyService
{
    public async Task DoSomething()
    {
        // Implementation
    }
}
\`\`\`

**Explanation**: This example shows [WHAT and WHY]

### Example 2: Advanced Pattern

\`\`\`csharp
// More complex example
\`\`\`

**Where to find**: See `src/services/MyService.cs` (line 42)
```

**Tips for AI**:
- Include language specification (csharp, typescript, etc.)
- Add explanatory comments in code
- Link to actual source files
- Show both simple and advanced patterns

---

### 6. 🔄 Common Patterns (Recommended)
**Purpose**: Share recurring patterns and best practices

```markdown
## Common Patterns

### Pattern 1: [Name]

**When to use**: [Describe scenario]

**Implementation**:
[Code or description]

**Example**: [Link to file or real usage in codebase]

**See also**: [DEVDOC-*, ADR-*, KB-*]
```

**Tips for AI**:
- Pattern names should be meaningful
- Include "when to use" for decision-making
- Link to real examples in codebase
- Reference related patterns

---

### 7. 🚀 Getting Started (Recommended)
**Purpose**: Help developers start working with this topic

```markdown
## Getting Started

### Prerequisites
- Requirement 1
- Requirement 2

### Step-by-Step
1. First step
2. Second step
3. Third step

### Verification
How to verify it's working:
\`\`\`bash
# Command to verify
\`\`\`

### Next Steps
- [DEVDOC-GUIDE-001] for deeper learning
- [DEVDOC-HOW-001] for step-by-step guide
```

**Tips for AI**:
- Number steps for clarity
- Include verification/success criteria
- Link to next learning resources
- Make it copy-paste ready where possible

---

### 8. ⚠️ Troubleshooting (Recommended)
**Purpose**: Help with common problems

```markdown
## Troubleshooting

### Problem 1: [Error message or symptom]

**Symptoms**:
- Sign 1
- Sign 2

**Causes**:
- Possible cause 1
- Possible cause 2

**Solution**:
1. Check X
2. Try Y
3. If still broken, see [DEVDOC-HOW-002]

**Related Issue**: [GitHub issue link if applicable]
```

**Tips for AI**:
- Match against actual error messages
- Structure: Problem → Symptoms → Causes → Solutions
- Link to detailed guides for complex fixes

---

### 9. 📚 Related Resources (Required)
**Purpose**: Connect to related knowledge

```markdown
## Related Resources

### Documentation
- [DEVDOC-ARCH-002] - Related architecture topic
- [DEVDOC-API-001] - API endpoint reference
- [DEVDOC-GUIDE-001] - Developer guide

### Architecture Decisions
- [ADR-001] - Why we chose this approach
- [ADR-020] - Related decision

### Knowledgebase
- [KB-006] - Wolverine Patterns
- [KB-011] - Design Patterns

### How-To Guides
- [DEVDOC-HOW-001] - How to implement this
- [DEVDOC-HOW-002] - Common task with this feature

### Code References
- `src/Domain/Catalog/` - Implementation location
- `backend/Gateway/Store/API/` - API handlers
```

**Tips for AI**:
- Categorize related resources
- Use full DocID references
- Include file paths for context
- Help AI understand knowledge graph

---

### 10. 📝 Metadata & Footer (Required)
**Purpose**: Track document info

```markdown
---

## Document Metadata

| Property | Value |
|----------|-------|
| **DocID** | DEVDOC-ARCH-001 |
| **Status** | Active |
| **Last Updated** | 2026-01-08 |
| **Author** | @TechLead |
| **Reviewers** | @Architect, @Backend |
| **Version** | 1.0 |

### Change History
| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-01-08 | Initial version |
| 1.1 | 2026-01-15 | Added examples |

### Contributing
Found an issue? Have improvements?
- Update this document
- Add your name to reviewers
- Create PR with clear description
```

**Tips for AI**:
- Machine-readable metadata helps indexing
- Change history helps AI understand evolution
- Makes it easy to find original author

---

## ✅ Quality Checklist

Before publishing, verify:

- [ ] YAML front-matter complete and valid
- [ ] DocID assigned and unique
- [ ] Title is descriptive
- [ ] Overview section present and clear
- [ ] Core concepts defined
- [ ] Code examples include language specification
- [ ] All internal links use DocID format
- [ ] All external links tested
- [ ] Related resources section complete
- [ ] No broken links
- [ ] Metadata section present
- [ ] Document ready for AI consumption

---

## 🤖 AI Assistant Notes

This template is designed for:
- **Helpdesk Systems**: Answer common questions with precise references
- **Step-by-Step Guidance**: Structured sections enable clear instructions
- **Technical Support**: Troubleshooting section with clear problem/solution mapping
- **Training**: Core concepts and getting started sections support learning
- **Code Generation**: Code examples and patterns inform AI assistance

**Key for AI Integration**:
1. Consistent structure enables reliable parsing
2. DocID references create knowledge graph
3. Code examples are copy-paste compatible
4. Metadata enables smart filtering and ranking
5. Troubleshooting section enables diagnosis

---

## Example: Completed Document

See [DEVDOC-ARCH-001-example.md](./DEVDOC-ARCH-001-example.md) for a filled-out example.

---

**Template Version**: 1.0  
**Last Updated**: 2026-01-08  
**Maintained by**: @DocMaintainer