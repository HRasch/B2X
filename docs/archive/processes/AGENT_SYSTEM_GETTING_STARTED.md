# 🚀 Agent System Getting Started Guide

**Time to Set Up:** 5 minutes  
**First Use:** Immediate  
**Benefits:** Faster, more focused Copilot responses

---

## What You Just Got

You now have **12+ specialized Copilot agents** ready to use:

### Core Agents (3)
1. **🔐 Security Engineer Agent** - Build P0.1-P0.5 compliance infrastructure
2. **💻 Backend Developer Agent** - Implement features with Wolverine & compliance
3. **🧪 QA Engineer Agent** - Create & manage 52 compliance tests

### Specialized Backend/Frontend Agents (4)
4. **💼 Backend Developer - Admin Backend** - Admin API, Identity, Tenancy
5. **🛍️ Backend Developer - Store Backend** - Catalog, CMS, Theming, Search
6. **📊 Frontend Developer - Admin Frontend** - Admin Dashboard, Management UI
7. **🎨 Frontend Developer - Store Frontend** - Storefront, Product Pages, Checkout

### Leadership & Compliance Agents (2)
8. **👔 Tech Lead / Architect** - Architecture & cross-cutting concerns
9. **📋 Product Owner** - Requirements, Roadmap, Prioritization
10. **⚖️ Legal / Compliance Officer** - Regulatory compliance, risk assessment

### Stakeholder Integration Agents (5)
11. **🔗 ERP System Stakeholder** - SAP, Oracle, NetSuite, E-Rechnung integration
12. **📦 PIM System Stakeholder** - Product data, BMEcat, master data enrichment
13. **👥 CRM System Stakeholder** - Customer data, Salesforce, HubSpot integration
14. **📈 BI System Stakeholder** - Analytics, reporting, data warehouse
15. **🏪 Reseller Stakeholder** - Partner networks, white-label, multi-tenant support

Each agent has its own context with only the relevant documents, patterns, and constraints.

---

## Installation (Already Done!)

✅ Agent files created:
```
.github/
├── role-contexts/
│   ├── security-engineer-context.md
│   ├── backend-developer-context.md
│   ├── qa-engineer-context.md
│   ├── backend-admin-context.md
│   ├── backend-store-context.md
│   ├── frontend-admin-context.md
│   ├── frontend-store-context.md
│   ├── product-owner-context.md
│   ├── legal-compliance-context.md
│   ├── stakeholder-erp-context.md
│   ├── stakeholder-pim-context.md
│   ├── stakeholder-crm-context.md
│   ├── stakeholder-bi-context.md
│   ├── stakeholder-reseller-context.md
│   └── tech-lead-context.md
├── activate-agent.sh
└── AGENT_QUICK_REFERENCE.txt
```

---

## Your First Agent Usage (5 minutes)

### Step 1: Open Terminal
```bash
cd /Users/holger/Documents/Projekte/B2Connect
```

### Step 2: Activate Your Role Agent
Choose from 15 agents:

**Core Agents:**
```bash
./.github/activate-agent.sh security   # 🔐 Security Engineer
./.github/activate-agent.sh backend    # 💻 Backend Developer (General)
./.github/activate-agent.sh qa         # 🧪 QA Engineer
```

**Specialized Developers:**
```bash
./.github/activate-agent.sh backend-admin    # 💼 Admin Backend Dev
./.github/activate-agent.sh backend-store    # 🛍️ Store Backend Dev
./.github/activate-agent.sh frontend-admin   # 📊 Admin Frontend Dev
./.github/activate-agent.sh frontend-store   # 🎨 Store Frontend Dev
```

**Leadership:**
```bash
./.github/activate-agent.sh tech-lead     # 👔 Tech Lead
./.github/activate-agent.sh product-owner # 📋 Product Owner
./.github/activate-agent.sh legal         # ⚖️ Legal Officer
```

**Stakeholders:**
```bash
./.github/activate-agent.sh stakeholder-erp    # 🔗 ERP Integration
./.github/activate-agent.sh stakeholder-pim    # 📦 PIM System
./.github/activate-agent.sh stakeholder-crm    # 👥 CRM System
./.github/activate-agent.sh stakeholder-bi     # 📈 BI System
./.github/activate-agent.sh stakeholder-reseller # 🏪 Reseller Network
```

Expected output:
```
========================================
Copilot Agent Activated
========================================
✅ Role: 💼 Backend Developer - Admin Backend
✅ Context: .github/role-contexts/backend-admin-context.md

ℹ️  Reloading VS Code Copilot to apply new context...

To apply changes in VS Code:
  1. Close and reopen Copilot Chat
  2. Or reload VS Code window (Cmd+Shift+P → Developer: Reload Window)

✅ Agent context loaded!
```

### Step 3: Reload VS Code Copilot

**Option A (Easiest):**
- Close Copilot Chat panel
- Press `Cmd+K, Cmd+K` to clear chat
- Type your question

**Option B (Full Reload):**
- Press `Cmd+Shift+P`
- Type "Developer: Reload Window"
- Press Enter

### Step 4: Open Copilot Chat & Ask a Question

In VS Code, open Copilot Chat and ask a role-specific question.

---

## Agent Usage by Role

### 🔐 Security Engineer (10 min)

```bash
./.github/activate-agent.sh security
# Reload Copilot

# Ask:
"Implement P0.1 Audit Logging with SaveChangesInterceptor, 
AES-256 encryption, and tenant isolation. Include tests."
```

### 💻 Backend Developer - General (10 min)

```bash
./.github/activate-agent.sh backend
# Reload Copilot

# Ask:
"Create a Wolverine HTTP service for VAT calculation with 
FluentValidation and audit logging integration."
```

### 💼 Backend Developer - Admin (10 min)

```bash
./.github/activate-agent.sh backend-admin
# Reload Copilot

# Ask:
"Build an admin API endpoint for user role assignment with 
proper authorization checks and audit logging."
```

### 🛍️ Backend Developer - Store (10 min)

```bash
./.github/activate-agent.sh backend-store
# Reload Copilot

# Ask:
"Implement product search handler with Elasticsearch integration
and multi-language support."
```

### 📊 Frontend Developer - Admin (10 min)

```bash
./.github/activate-agent.sh frontend-admin
# Reload Copilot

# Ask:
"Create a Vue 3 admin dashboard component for managing user roles
with access control display."
```

### 🎨 Frontend Developer - Store (10 min)

```bash
./.github/activate-agent.sh frontend-store
# Reload Copilot

# Ask:
"Build an accessible product card component with price display
(incl. VAT) and add-to-cart button."
```

### 📋 Product Owner (10 min)

```bash
./.github/activate-agent.sh product-owner
# Reload Copilot

# Ask:
"Create a user story for P0.6 withdrawal period management with
acceptance criteria and compliance notes."
```

### ⚖️ Legal Officer (10 min)

```bash
./.github/activate-agent.sh legal
# Reload Copilot

# Ask:
"Review AI Act compliance requirements for fraud detection system
and generate risk assessment template."
```

### 🔗 ERP Stakeholder (10 min)

```bash
./.github/activate-agent.sh stakeholder-erp
# Reload Copilot

# Ask:
"What are the integration points needed to support E-Rechnung
ZUGFeRD 3.0 export for SAP systems?"
```

---

## Real-World Workflow

### Scenario: Building P0.6 VAT Calculation (Multi-Agent)

#### Step 1: Product Owner Creates User Story (15 min)

```bash
./.github/activate-agent.sh product-owner

# Copilot Chat:
# "Create a detailed user story for P0.6 VAT calculation 
#  with B2C and B2B variants, acceptance criteria, and compliance notes"
```

#### Step 2: Security Engineer Reviews Compliance (15 min)

```bash
./.github/activate-agent.sh security

# Copilot Chat:
# "What security measures needed for VAT calculation service?
#  Cost data encryption, audit logging, tenant isolation?"
```

#### Step 3: Backend Developer - Store Creates Service (30 min)

```bash
./.github/activate-agent.sh backend-store

# Copilot Chat:
# "Create VAT calculation service with VIES VAT-ID validation,
#  reverse charge logic, encryption for cost data, audit logging"
```

#### Step 4: Frontend Developer - Store Creates UI (30 min)

```bash
./.github/activate-agent.sh frontend-store

# Copilot Chat:
# "Build product price display component showing gross price
#  (incl. VAT) with accessibility support"
```

#### Step 5: QA Engineer Creates Tests (45 min)

```bash
./.github/activate-agent.sh qa

# Copilot Chat:
# "Create tests for P0.6 VAT calculation: B2C (19% VAT),
#  B2B (0% with VAT-ID), edge cases, VVVG compliance"
```

#### Step 6: Legal Officer Reviews Compliance (15 min)

```bash
./.github/activate-agent.sh legal

# Copilot Chat:
# "Verify P0.6 implementation meets PAngV, VVVG, and BDSG requirements
#  for German e-commerce law"
```

#### Result: Feature complete with multi-stakeholder compliance! ✅

---

## Common Questions

### Q: Can I use multiple agents in one chat?

**A:** No, keep one agent per chat session. Different agents have different constraints.

**Better:** Use agent for one component, then switch agents for next component.

### Q: What if I ask a question outside the agent's scope?

**A:** The agent will still try to help but may reference documents from other roles. Just tell Copilot to re-focus:
```
"Stay in Backend Developer - Admin context. Focus on admin API only."
```

### Q: Which agent should I start with?

**A:** Start with **Product Owner** if planning features, or **Backend Developer - Store** if building catalog features.

### Q: How do I know which agent to use?

**A:** Check the agent name and description - they match your current task:
- **Admin work?** → Use Backend/Frontend Admin agents
- **Store/catalog work?** → Use Backend/Frontend Store agents
- **Compliance?** → Use Security or Legal agents
- **Integration?** → Use Stakeholder agents
- **Planning?** → Use Product Owner agent

### Q: Can I create custom agents?

**A:** Yes! Copy any existing agent and customize. Update `activate-agent.sh` case statement.

---

## Tips for Best Results

1. **Activate first** - Always activate agent BEFORE asking question
2. **Be specific** - Say "I'm a Backend Developer - Store..." in first message
3. **Reference patterns** - Ask Copilot to reference CheckRegistrationTypeService.cs
4. **Test generation** - Always ask for xUnit tests with 80%+ coverage
5. **Copy-paste** - Generated code is template, always review before committing
6. **Multi-agent workflow** - Use agents sequentially for complex features

---

## Next Steps

### Immediate (Now)
- [ ] Try 3-5 agents (1 hour total)
- [ ] Save quick reference card: `.github/AGENT_QUICK_REFERENCE.txt`
- [ ] Bookmark agent list: This document

### This Week
- [ ] Use agents for your first P0 component
- [ ] Try multi-agent workflow for one feature
- [ ] Share agent approach with team
- [ ] Provide feedback on agent quality

### This Month
- [ ] Integrate agents into team's daily workflow
- [ ] Create team guidelines for agent usage
- [ ] Measure improvement in code quality & speed

---

## Success Metrics

After using agents for 1 week, you should see:

| Metric | Before | After |
|--------|--------|-------|
| **Time per feature** | 2-3 hours | 1-2 hours |
| **Code quality** | Variable | Consistent |
| **Test coverage** | 60-70% | 80%+ |
| **Compliance gaps** | Found in review | Caught during generation |
| **Context switching** | Confusing | Clear |
| **Multi-agent features** | N/A | Faster with collaboration |

---

## Agent Architecture

```
┌────────────────────────────────────────┐
│  User Asks Question in Copilot Chat    │
└────────────────────────────────────────┘
         ↓
┌────────────────────────────────────────┐
│  .github/copilot-instructions.md       │
│  (loaded by activate-agent.sh)         │
└────────────────────────────────────────┘
         ↓
┌────────────────────────────────────────┐
│  Copilot Reads Role-Specific Context   │
│  Example: backend-store-context.md     │
└────────────────────────────────────────┘
         ↓
┌────────────────────────────────────────┐
│  Agent Generates Focused Response      │
│  - Wolverine pattern (not MediatR)     │
│  - Store backend focused               │
│  - Audit logging integrated            │
│  - xUnit tests included                │
│  - Onion architecture respected        │
└────────────────────────────────────────┘
         ↓
┌────────────────────────────────────────┐
│  You Copy, Review, Test, Commit!       │
│  Quality code in minutes, not hours    │
└────────────────────────────────────────┘
```

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Copilot not loading new context | Reload VS Code: `Cmd+Shift+P` → "Developer: Reload Window" |
| Context file not found | Check `.github/role-contexts/` has all agent files |
| Wrong agent active | Run `activate-agent.sh <role>` again |
| Script permission denied | Run `chmod +x .github/activate-agent.sh` |
| Copilot ignoring context | Clear chat (`Cmd+K, Cmd+K`) and ask again |
| Agent doesn't know role | Say "I'm a [role]" in your first message |

---

## Full Documentation

For detailed information, see:
- **Quick Ref:** `.github/AGENT_QUICK_REFERENCE.txt`
- **Contexts:** `.github/role-contexts/` (all agent definitions)
- **All Roles:** `copilot-contexts.json` (JSON configuration)
- **Technical Details:** `.github/copilot-instructions.md`

---

## All Available Agents (Quick List)

| Agent | Activate | Best For |
|-------|----------|----------|
| 🔐 Security Engineer | `security` | P0 compliance, encryption, audit |
| 💻 Backend Dev (General) | `backend` | General backend work |
| 💼 Backend Dev - Admin | `backend-admin` | Admin API, identity, tenancy |
| 🛍️ Backend Dev - Store | `backend-store` | Catalog, CMS, search |
| 📊 Frontend Dev - Admin | `frontend-admin` | Admin dashboard, UI |
| 🎨 Frontend Dev - Store | `frontend-store` | Storefront, product pages |
| 🧪 QA Engineer | `qa` | Testing, verification |
| 👔 Tech Lead | `tech-lead` | Architecture, code review |
| 📋 Product Owner | `product-owner` | Requirements, roadmap |
| ⚖️ Legal Officer | `legal` | Compliance, regulations |
| 🔗 ERP Stakeholder | `stakeholder-erp` | ERP integration, E-Rechnung |
| 📦 PIM Stakeholder | `stakeholder-pim` | Product data, enrichment |
| 👥 CRM Stakeholder | `stakeholder-crm` | Customer data, CRM integration |
| 📈 BI Stakeholder | `stakeholder-bi` | Analytics, reporting |
| 🏪 Reseller Stakeholder | `stakeholder-reseller` | Partner networks, white-label |

---

## Let's Go! 🚀

```bash
# Choose your role and activate
./.github/activate-agent.sh <role>

# Reload Copilot Chat in VS Code
# Then ask a role-specific question and watch the magic happen ✨
```

---

**Questions?** The agents themselves are trained to help. Just ask Copilot!
