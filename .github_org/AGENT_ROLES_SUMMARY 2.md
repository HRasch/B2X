# 🎯 B2Connect Agent Roles Summary

**Last Updated:** 28. Dezember 2025  
**Total Agents:** 15  
**Status:** All activated and ready to use

---

## Quick Activation Reference

### Core Technical Roles (3)

| Role | Command | Focus |
|------|---------|-------|
| 🔐 Security Engineer | `./activate-agent.sh security` | Encryption, audit logging, compliance (P0.1-P0.5, P0.7) |
| 💻 Backend Developer (General) | `./activate-agent.sh backend` | General backend work, microservices |
| 🧪 QA Engineer | `./activate-agent.sh qa` | Testing, automation, 52 compliance tests |

### Specialized Developer Roles (4)

| Role | Command | Focus |
|------|---------|-------|
| 💼 Backend Dev - Admin | `./activate-agent.sh backend-admin` | Admin API, Identity service, Tenancy, user management |
| 🛍️ Backend Dev - Store | `./activate-agent.sh backend-store` | Catalog, CMS, Theming, Search, Localization |
| 📊 Frontend Dev - Admin | `./activate-agent.sh frontend-admin` | Admin dashboard, management UI, forms, tables |
| 🎨 Frontend Dev - Store | `./activate-agent.sh frontend-store` | Storefront, product pages, checkout, cart, UX |

### Leadership Roles (3)

| Role | Command | Focus |
|------|---------|-------|
| 👔 Tech Lead / Architect | `./activate-agent.sh tech-lead` | Architecture, design patterns, code review, standards |
| 📋 Product Owner | `./activate-agent.sh product-owner` | Requirements, roadmap, prioritization, user stories |
| ⚖️ Legal / Compliance Officer | `./activate-agent.sh legal` | Regulations, compliance, risk assessment, audit |

### Stakeholder Integration Roles (5)

| Role | Command | Focus |
|------|---------|-------|
| 🔗 ERP System | `./activate-agent.sh stakeholder-erp` | SAP, Oracle, NetSuite, E-Rechnung, ZUGFeRD, UBL |
| 📦 PIM System | `./activate-agent.sh stakeholder-pim` | Product data, BMEcat, master data, enrichment |
| 👥 CRM System | `./activate-agent.sh stakeholder-crm` | Customer data, Salesforce, HubSpot, sync |
| 📈 BI System | `./activate-agent.sh stakeholder-bi` | Analytics, reporting, data warehouse, dashboards |
| 🏪 Reseller Network | `./activate-agent.sh stakeholder-reseller` | Partner networks, white-label, multi-tenant |

---

## By Domain

### Admin Backend Development

**Primary Agent:** `backend-admin` (💼)

Includes:
- Admin API development
- Identity & Authentication services
- Tenancy & multi-tenant management
- User role & permission system
- Admin-specific features

**When to use:**
- Building admin API endpoints
- Implementing user management
- Creating identity services
- Setting up tenant isolation
- Admin dashboard backend

**Example:**
```bash
./activate-agent.sh backend-admin
# Ask: "Create admin API endpoint for assigning user roles with authorization checks"
```

### Admin Frontend Development

**Primary Agent:** `frontend-admin` (📊)

Includes:
- Admin dashboard components
- User/role/permission management UI
- Configuration interfaces
- Admin-specific layouts
- Forms and data tables

**When to use:**
- Building admin dashboard
- Creating user management interface
- Building configuration UIs
- Admin-specific components

**Example:**
```bash
./activate-agent.sh frontend-admin
# Ask: "Create admin dashboard component for managing user roles and permissions"
```

### Store Backend Development

**Primary Agent:** `backend-store` (🛍️)

Includes:
- Store API (public endpoints)
- Catalog service (products, categories)
- CMS service (content, pages)
- Theming service (designs, templates)
- Search service (Elasticsearch)
- Localization service (i18n)

**When to use:**
- Building product catalog features
- Creating content management
- Implementing search functionality
- Adding multi-language support
- Theming & customization

**Example:**
```bash
./activate-agent.sh backend-store
# Ask: "Implement product search handler with Elasticsearch and filters"
```

### Store Frontend Development

**Primary Agent:** `frontend-store` (🎨)

Includes:
- Store/storefront UI
- Product page components
- Shopping cart & checkout
- Customer-facing features
- Accessibility (WCAG 2.1 AA)

**When to use:**
- Building product pages
- Creating checkout flow
- Building product cards
- Implementing accessibility
- Customer UX features

**Example:**
```bash
./activate-agent.sh frontend-store
# Ask: "Create accessible product card with price (incl. VAT) and add-to-cart button"
```

---

## By Function

### Compliance & Security

**Security Engineer** (`security`)
- P0.1: Audit Logging
- P0.2: Encryption at Rest (AES-256)
- P0.3: Incident Response
- P0.5: Key Management
- P0.7: AI Act Compliance

**Legal Officer** (`legal`)
- P0.6: E-Commerce Legal (B2B/B2C)
- P0.8: BITV Accessibility
- P0.9: E-Rechnung
- Regulatory compliance
- Risk assessment

**QA Engineer** (`qa`)
- 52 compliance tests
- P0.6, P0.7, P0.8, P0.9 verification
- Automation testing
- Performance testing

### Development

**Backend Developers**
- `backend` (general)
- `backend-admin` (admin-specific)
- `backend-store` (store-specific)

**Frontend Developers**
- `frontend-admin` (admin UI)
- `frontend-store` (storefront UI)

### Leadership

**Tech Lead** (`tech-lead`)
- Architecture decisions
- Code review
- Standards & patterns
- Cross-cutting concerns

**Product Owner** (`product-owner`)
- Requirements definition
- Roadmap planning
- Prioritization
- User stories

### Integration

**Stakeholders**
- `stakeholder-erp`: ERP system integration
- `stakeholder-pim`: Product data management
- `stakeholder-crm`: Customer relationship management
- `stakeholder-bi`: Business intelligence
- `stakeholder-reseller`: Partner/reseller networks

---

## Common Workflows

### Feature Development (Single Agent)

**Simple Feature = One Agent**

```bash
# Feature: Product search
./activate-agent.sh backend-store
# Ask: "Implement Elasticsearch-based product search with filters"
```

### Feature Development (Multi-Agent)

**Complex Feature = Multiple Agents (Sequential)**

```bash
# Step 1: Product Owner defines story
./activate-agent.sh product-owner
# Ask: "Create user story for P0.6 VAT calculation with acceptance criteria"

# Step 2: Security Engineer reviews
./activate-agent.sh security
# Ask: "What security measures for VAT service with cost data?"

# Step 3: Backend Store Dev implements
./activate-agent.sh backend-store
# Ask: "Implement VAT calculation with VIES validation and encryption"

# Step 4: Frontend Store Dev builds UI
./activate-agent.sh frontend-store
# Ask: "Build price display showing gross price with VAT"

# Step 5: QA Engineer tests
./activate-agent.sh qa
# Ask: "Create tests for P0.6 VAT: B2C (19%), B2B (0% with VAT-ID)"

# Step 6: Legal reviews
./activate-agent.sh legal
# Ask: "Verify P0.6 meets PAngV, VVVG, and German e-commerce law"
```

### Integration Project

```bash
# Step 1: Understand requirements
./activate-agent.sh product-owner
# Ask: "What does ERP integration involve?"

# Step 2: Architecture review
./activate-agent.sh tech-lead
# Ask: "Design ERP integration architecture"

# Step 3: Backend implementation
./activate-agent.sh backend-store
# Ask: "Implement E-Rechnung ZUGFeRD export for orders"

# Step 4: Integration stakeholder
./activate-agent.sh stakeholder-erp
# Ask: "What SAP/Oracle requirements for ZUGFeRD import?"

# Step 5: QA verification
./activate-agent.sh qa
# Ask: "Create P0.9 E-Rechnung tests for ZUGFeRD validation"
```

---

## Agent Capabilities Matrix

| Capability | Security | Backend | Frontend | QA | Product | Tech Lead | Legal | Stakeholders |
|-----------|:--------:|:-------:|:--------:|:--:|:-------:|:---------:|:-----:|:------------:|
| **Compliance** | ✅✅✅ | ✅ | ✅ | ✅✅ | ✅ | ✅ | ✅✅✅ | ✅ |
| **Architecture** | ✅ | ✅ | ✅ | - | - | ✅✅✅ | - | - |
| **Code Gen** | ✅✅ | ✅✅✅ | ✅✅✅ | ✅ | - | ✅ | - | - |
| **Testing** | ✅ | ✅ | ✅ | ✅✅✅ | - | ✅ | - | - |
| **Requirements** | - | - | - | - | ✅✅✅ | ✅ | ✅ | ✅ |
| **Integration** | - | ✅ | - | - | - | ✅ | - | ✅✅✅ |
| **Legal/Compliance** | ✅✅ | - | - | ✅ | - | - | ✅✅✅ | ✅ |

---

## Configuration Files

All agent configurations defined in:

**JSON Config:**
- `.github/copilot-contexts.json` - Master configuration for all 15 agents

**Context Files:**
- `.github/role-contexts/` - Individual context markdown files

**Activation Script:**
- `.github/activate-agent.sh` - Bash script to switch agents

**Quick Reference:**
- `.github/AGENT_QUICK_REFERENCE.txt` - Command reference card

---

## Tips for Maximum Effectiveness

### ✅ DO

1. **Activate agent first** - Always run `.github/activate-agent.sh <role>` before asking
2. **Identify yourself** - Say "I'm a [role]" in first message
3. **Ask specifically** - Reference existing patterns, files, or components
4. **Expect tests** - Always ask for xUnit tests with 80%+ coverage
5. **Use sequentially** - Switch agents for different aspects of complex work
6. **Reference docs** - Ask agent to reference specific documentation
7. **Clear cache** - Use `Cmd+K, Cmd+K` to clear Copilot chat between agents

### ❌ DON'T

1. **Don't ask unrelated questions** - Stay within agent's scope
2. **Don't use multiple agents in one chat** - Confuses context
3. **Don't mix patterns** - Don't ask Backend Dev about frontend styling
4. **Don't assume capabilities** - Not all agents can generate code
5. **Don't forget to test** - Always run generated code before committing
6. **Don't mix languages** - Stick to the agent's primary language (C# vs Vue, etc.)

---

## Role Onboarding

**New team member?** Here's the quickest path:

1. **First Day:**
   - [ ] Read this file (5 min)
   - [ ] Try Backend Dev agent (10 min)
   - [ ] Try QA agent (10 min)

2. **Second Day:**
   - [ ] Try specialized agent for your role (10 min)
   - [ ] Work on first task using agent (2 hours)
   - [ ] Ask questions in Copilot Chat

3. **First Week:**
   - [ ] Use agents for all development tasks
   - [ ] Try multi-agent workflow once
   - [ ] Provide feedback on agent quality

---

## Feedback & Improvements

If an agent:
- **Generates bad code** → Check activation, scope, and specificity of question
- **Ignores your instructions** → Clear cache and reload VS Code
- **Doesn't know your role** → Explicitly state "I'm a [role]" in first message
- **Missing documentation** → File issue to update context files

---

## Success Metrics

After using specialized agents for 1 week:

| Metric | Baseline | Target |
|--------|----------|--------|
| Time per feature | 2-3 hours | 1-2 hours |
| Code review cycles | 3-4 rounds | 1-2 rounds |
| Test coverage | 60-70% | 80%+ |
| Bug escape rate | 5-10% | <2% |
| Compliance gaps | Review time | Build time |

---

## Next Steps

1. **👉 Choose your role** from the table above
2. **👉 Run activation** `./activate-agent.sh <role>`
3. **👉 Reload Copilot** in VS Code
4. **👉 Ask a question** and get specialized help!

---

**Document Owner:** Architecture Team  
**Last Review:** 28. Dezember 2025  
**Next Update:** 15. Januar 2026

**Questions?** Check `.github/AGENT_QUICK_REFERENCE.txt` or ask Copilot directly!
