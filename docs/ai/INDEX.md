# 🧠 AI Knowledge Base Index

**Purpose**: Fast lookup table for AI agents - check this FIRST before answering questions  
**Last Updated**: 30. Dezember 2025  
**Maintained By**: @process-assistant (EXCLUSIVE authority - see [GOVERNANCE_RULES.md](../../.github/docs/processes/GOVERNANCE/GOVERNANCE_RULES.md))  
**Rule**: When a trigger keyword appears in user query → READ the linked file BEFORE responding

---

## 🔍 Trigger Keywords → Documentation Lookup

| User Asks About... | Read This File First | Lines |
|-------------------|---------------------|-------|
| **Models, LLM, Claude, GPT, Gemini, Grok, Anthropic, OpenAI** | [COPILOT_MODELS_REFERENCE.md](./COPILOT_MODELS_REFERENCE.md) | 545 |
| **C# 14, extension members, field keyword, span, lambda** | [C#14_FEATURES_REFERENCE.md](./C#14_FEATURES_REFERENCE.md) | 902 |
| **daisyUI, components, UI, Tailwind, buttons, forms, cards** | [DAISYUI_V5_COMPONENTS_REFERENCE.md](./DAISYUI_V5_COMPONENTS_REFERENCE.md) | 870 |
| **Agent definition, YAML, frontmatter, tools, .agent.md** | [COPILOT_AGENT_DEFINITION.md](./COPILOT_AGENT_DEFINITION.md) | 362 |
| **Dependencies, packages, versions, NuGet, npm** | [DEPENDENCY_DOCUMENTATION_INDEX.md](./DEPENDENCY_DOCUMENTATION_INDEX.md) | 263 |
| **CSS variables, custom properties, theming** | [CSS_CUSTOM_PROPERTIES_COMPREHENSIVE_GUIDE.md](./CSS_CUSTOM_PROPERTIES_COMPREHENSIVE_GUIDE.md) | - |
| **New features, implementation guide** | [NEW_FEATURES_IMPLEMENTATION_GUIDE.md](./NEW_FEATURES_IMPLEMENTATION_GUIDE.md) | - |

---

## 📊 Quick Facts (Memorize These)

### GitHub Copilot Models (Dec 2025)
- **17+ models** from OpenAI, Anthropic, Google, xAI
- **Best free**: Claude Haiku 4.5 (0.33x), GPT-5 mini (0x)
- **Best balanced**: Claude Sonnet 4 (1x), GPT-5.1-Codex (1x)
- **Best premium**: Claude Opus 4.5 (3x)
- **Fastest**: Grok Code Fast 1 (0.25x)

### C# 14 Features (Nov 2024)
- **Extension Members**: Add methods/properties to existing types
- **field Keyword**: Auto-property backing field access
- **Null-Conditional Assignment**: `obj?.Property = value;`
- **Implicit Span Conversions**: Zero-copy memory operations

### daisyUI v5 Components
- **Forms**: Input, Select, Checkbox, Toggle, File Input
- **Navigation**: Navbar, Breadcrumbs, Tabs, Menu, Dock
- **Feedback**: Alert, Toast, Modal, Loading, Progress
- **Data Display**: Table, Card, Badge, Avatar, Status

### B2Connect Tech Stack
- **Backend**: .NET 10, Wolverine, EF Core, PostgreSQL 16
- **Frontend**: Vue.js 3.5, Tailwind CSS 4.1, daisyUI 5.5
- **Build**: Vite, TypeScript
- **Orchestration**: .NET Aspire

---

## 🎯 Decision Tree

```
User Question About...
│
├─► AI Models / LLM Selection
│   └─► READ: COPILOT_MODELS_REFERENCE.md
│
├─► C# Code Patterns / .NET Features
│   └─► READ: C#14_FEATURES_REFERENCE.md
│
├─► Frontend Components / UI Design
│   └─► READ: DAISYUI_V5_COMPONENTS_REFERENCE.md
│
├─► Agent Configuration / YAML
│   └─► READ: COPILOT_AGENT_DEFINITION.md
│
├─► Package Versions / Updates
│   └─► READ: DEPENDENCY_DOCUMENTATION_INDEX.md
│
└─► Architecture / Wolverine / DDD
    └─► READ: ../.github/copilot-instructions.md
```

---

## ⚠️ Common Mistakes to Avoid

| Wrong Answer | Correct Source |
|--------------|----------------|
| "GPT-4 is the main model" | COPILOT_MODELS_REFERENCE.md → 17+ models available |
| "Use MediatR for CQRS" | copilot-instructions.md → Use Wolverine, NOT MediatR |
| "Bootstrap for components" | DAISYUI_V5_COMPONENTS_REFERENCE.md → Use daisyUI |
| "C# 13 features" | C#14_FEATURES_REFERENCE.md → C# 14 is current |

---

## 📁 Full File List

| File | Purpose | Audience |
|------|---------|----------|
| `COPILOT_MODELS_REFERENCE.md` | AI model selection guide | All agents |
| `C#14_FEATURES_REFERENCE.md` | C# 14 language features | Backend devs |
| `DAISYUI_V5_COMPONENTS_REFERENCE.md` | UI component library | Frontend devs |
| `COPILOT_AGENT_DEFINITION.md` | Agent YAML format | Process assistant |
| `DEPENDENCY_DOCUMENTATION_INDEX.md` | Package version index | DevOps, Tech Lead |
| `DEPENDENCY_UPDATES_AND_NEW_FEATURES.md` | Detailed update analysis | Senior devs |
| `DEPENDENCY_UPDATES_QUICK_REFERENCE.md` | Quick version lookup | All devs |
| `NEW_FEATURES_IMPLEMENTATION_GUIDE.md` | Feature implementation | All devs |
| `CSS_CUSTOM_PROPERTIES_COMPREHENSIVE_GUIDE.md` | CSS theming guide | Frontend devs |
| `AGENT_ROLE_DOCUMENTATION_GUIDELINES.md` | Agent documentation standards | Process assistant |
| `AGENT_ROLE_DOCUMENTATION_QUICK_REFERENCE.md` | Quick agent docs reference | All agents |

---

## 🔄 Maintenance

**Owner**: @process-assistant (EXCLUSIVE authority)

**Update Schedule**:
- **Daily**: Check trigger keywords accuracy, update quick facts if needed
- **Weekly**: Verify all cross-references work, add new files to index
- **Monthly**: Deep audit against external sources (GitHub docs, C# docs, etc.)

**To Request Changes**: File GitHub issue with `@process-assistant` tag

---

**Rule**: Always check this index FIRST. If trigger keywords match, READ the linked file BEFORE answering.
