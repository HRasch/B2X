# 🤖 Copilot Agents Setup & Registration Guide

**Last Updated:** 28. Dezember 2025  
**Status:** Ready for VS Code Integration  
**Total Agents:** 17

---

## 🚀 Quick Start

### 1. Install Required Extensions

```bash
# Ensure these are installed:
- GitHub Copilot (GitHub.copilot)
- GitHub Copilot Chat (GitHub.copilot-chat)
- C# Extension (ms-dotnettools.csharp)
- Vue Extension (Vue.volar)
```

The `.vscode/extensions.json` file contains recommended extensions. VS Code will prompt you to install them.

### 2. Open Copilot Chat

**Keyboard Shortcut:** `Cmd + Shift + I` (macOS) or `Ctrl + Shift + I` (Windows/Linux)

Or click the **Copilot Chat** icon in the Activity Bar (left sidebar).

### 3. Select Your Agent

In Copilot Chat, use the `@` mention to select an agent:

```
@security-engineer Help me implement audit logging
@backend-developer Create a Wolverine HTTP handler for product checkout
@frontend-developer Make this component WCAG 2.1 AA accessible
@qa-engineer Run the P0.6 E-Commerce compliance tests
@tech-lead Review architecture decisions for the new API
```

---

## 📋 Available Agents

### **Technical Roles (9)**

#### 🔐 Security Engineer
```
@security-engineer
ID: security-engineer
Focus: P0.1-P0.5, P0.7 (Encryption, Audit, Incident Response, Keys, AI Act)
```
**Example Prompts:**
- "Implement AES-256 encryption for user email"
- "Design audit logging system for CRUD operations"
- "Set up NIS2-compliant incident detection"
- "Document AI Act risk assessment for fraud detection"

---

#### ⚙️ DevOps Engineer
```
@devops-engineer
ID: devops-engineer
Focus: P0.3, P0.4, P0.5 (Infrastructure, Networking, Monitoring, Keys)
```
**Example Prompts:**
- "Configure network segmentation in AWS VPC"
- "Set up monitoring and alerting for incident detection"
- "Implement Azure KeyVault integration for secret rotation"
- "Debug Aspire orchestration port conflicts"

---

#### 💻 Backend Developer (General)
```
@backend-developer
ID: backend-developer
Focus: Wolverine APIs, CQRS, Onion Architecture, microservices
```
**CRITICAL:** Use ONLY Wolverine pattern (NOT MediatR)

**Example Prompts:**
- "Create Wolverine HTTP handler for POST /products"
- "Implement VAT calculation with reverse charge logic"
- "Add audit logging to product update handler"
- "Design aggregate root for order management"

---

#### 💼 Backend Dev - Admin
```
@backend-admin
ID: backend-admin
Focus: Identity, Tenancy, RBAC, JWT, user management
```
**Example Prompts:**
- "Implement JWT token generation and refresh"
- "Add tenant isolation to product queries"
- "Create role-based authorization for admin endpoints"
- "Design password reset flow with email verification"

---

#### 🛍️ Backend Dev - Store
```
@backend-store
ID: backend-store
Focus: Catalog, CMS, Theming, Search, Localization
```
**Example Prompts:**
- "Create product catalog API with caching"
- "Implement Elasticsearch product search"
- "Design multi-language content management"
- "Add theme management for white-label customization"

---

#### 🎨 Frontend Developer
```
@frontend-developer
ID: frontend-developer
Focus: Vue.js 3, Tailwind CSS, WCAG 2.1 AA (CRITICAL)
```
**CRITICAL:** BITV 2.0 / WCAG 2.1 AA is MANDATORY (deadline: 28. Juni 2025)

**Example Prompts:**
- "Create accessible product card with keyboard navigation"
- "Make this form WCAG 2.1 AA compliant with proper ARIA labels"
- "Implement responsive product grid (mobile-first)"
- "Add dark mode support with Tailwind CSS"

---

#### 📊 Frontend Dev - Admin
```
@frontend-admin
ID: frontend-admin
Focus: Admin dashboard, data tables, forms, management UI
```
**Example Prompts:**
- "Create admin table for user management"
- "Build product edit form with validation"
- "Design order management interface"
- "Create settings page for tenant configuration"

---

#### 🎨 Frontend Dev - Store
```
@frontend-store
ID: frontend-store
Focus: Storefront, product pages, checkout, cart, customer UX
```
**Example Prompts:**
- "Create product listing page with filters"
- "Design shopping cart with quantity management"
- "Build checkout flow with payment integration"
- "Add AGB (terms) & withdrawal info display"

---

#### 🧪 QA Engineer
```
@qa-engineer
ID: qa-engineer
Focus: Testing, automation, compliance verification (52 tests)
```
**Example Prompts:**
- "Generate xUnit test for product creation handler"
- "Write Playwright E2E test for checkout flow"
- "Run accessibility audit with axe"
- "Check P0.6 E-Commerce compliance tests status"

---

### **Leadership Roles (3)**

#### 👔 Tech Lead / Architect
```
@tech-lead
ID: tech-lead
Focus: Architecture, patterns, code review, standards, all P0 components
```
**Example Prompts:**
- "Review architecture for new microservice"
- "Enforce Wolverine pattern compliance across team"
- "Analyze performance bottlenecks"
- "Plan compliance integration for feature X"

---

#### 📋 Product Owner
```
@product-owner
ID: product-owner
Focus: Requirements, roadmap, prioritization, user stories, go/no-go decisions
```
**Example Prompts:**
- "Define user story with acceptance criteria for feature X"
- "Review Phase 0-3 timeline and dependencies"
- "Check phase go/no-go gates"
- "Prioritize features vs compliance requirements"

---

#### ⚖️ Legal / Compliance Officer
```
@legal-compliance
ID: legal-compliance
Focus: Regulations (NIS2, GDPR, AI Act), legal review, risk assessment
```
**Example Prompts:**
- "Explain NIS2 incident notification requirements"
- "Review feature for GDPR compliance"
- "Assess AI Act risk for fraud detection system"
- "Determine withdrawal period calculation for B2C orders"

---

### **Stakeholder Integration Roles (5)**

#### 🔗 ERP System
```
@stakeholder-erp
ID: stakeholder-erp
Focus: enventa Trade ERP integration, Specializing in the plumbing, heating, and air conditioning (HVAC), building services, steel trading, and bending industries.
```

#### 📦 PIM System
```
@stakeholder-pim
ID: stakeholder-pim
Focus: Product data, BMEcat, master data enrichment
```

#### 👥 CRM System
```
@stakeholder-crm
ID: stakeholder-crm
Focus: Customer data, Salesforce, HubSpot integration
```

#### 📈 BI System
```
@stakeholder-bi
ID: stakeholder-bi
Focus: Analytics, reporting, data warehouse
```

#### 🏪 Reseller Network
```
@stakeholder-reseller
ID: stakeholder-reseller
Focus: White-label, partner networks, multi-tenant customization
```

---

## 🔧 Using Copilot Agents in VS Code

### Method 1: Direct Mention (Easiest)

```
@backend-developer Implement user registration handler

This will use the Backend Developer agent for context.
```

### Method 2: Create a Chat Thread

1. Open **Copilot Chat** (`Cmd + Shift + I`)
2. Mention agent: `@backend-developer`
3. Ask your question
4. Continue conversation in same thread (agent context persists)

### Method 3: Inline Chat

1. Select code in editor
2. Press `Cmd + I` (macOS) or `Ctrl + I` (Windows)
3. Type `/agent @backend-developer` (if supported)
4. Ask your question

### Method 4: Quick Actions

Use agent shortcuts defined in `.vscode/copilot-agents.json`:

```json
"shortcuts": {
  "security": "@security-engineer",
  "backend": "@backend-developer",
  "frontend": "@frontend-developer",
  "qa": "@qa-engineer",
  "tech-lead": "@tech-lead"
}
```

---

## 📚 What Each Agent Has Access To

All agents have access to:
- ✅ `.github/copilot-instructions.md` (core B2Connect patterns)
- ✅ Role-specific documentation in `docs/by-role/`
- ✅ Project files matching their context
- ✅ `copilot-contexts.json` (file inclusion rules)

### Role-Specific Documentation

- **Security Engineer:** `docs/by-role/SECURITY_ENGINEER.md`
- **DevOps Engineer:** `docs/by-role/DEVOPS_ENGINEER.md`
- **Backend Developer:** `docs/by-role/BACKEND_DEVELOPER.md`
- **Frontend Developer:** `docs/by-role/FRONTEND_DEVELOPER.md`
- **QA Engineer:** `docs/by-role/QA_ENGINEER.md`
- **Tech Lead:** `docs/by-role/TECH_LEAD.md`
- **Product Owner:** `docs/by-role/PRODUCT_OWNER.md`
- **Legal/Compliance:** `docs/by-role/LEGAL_COMPLIANCE.md`

---

## ⚡ Common Use Cases

### Backend Developer: Create New Handler

```
@backend-developer

I need to create a Wolverine HTTP handler for:
- Command: UpdateProductCommand
- Fields: Sku, Name, Price
- Validation: FluentValidation
- Audit: Log all changes
- Encryption: Supplier names

Generate the complete handler with tests.
```

### Frontend Developer: Accessible Component

```
@frontend-developer

Create a product filter component that:
- Is keyboard navigable (TAB, ENTER)
- Has ARIA labels for screen readers
- Mobile responsive (Tailwind CSS)
- WCAG 2.1 AA compliant
```

### QA Engineer: Compliance Testing

```
@qa-engineer

Help me run P0.6 E-Commerce compliance tests:
1. What tests exist?
2. How do I execute them?
3. What are the acceptance criteria?
4. Generate a test report template.
```

### Tech Lead: Architecture Review

```
@tech-lead

Review this service architecture:
- Is it following Onion Architecture?
- Any security gaps?
- Performance concerns?
- Compliance integration needed?

Suggest improvements.
```

---

## 🔍 Checking Agent Context

To see what files an agent has access to, ask:

```
@security-engineer What files do you have access to?
@backend-developer What's in your context?
```

---

## 🚨 Troubleshooting

### Agent Not Found

If you see "Agent not found" error:

1. Ensure `.vscode/copilot-agents.json` exists
2. Verify Copilot Chat extension is installed and enabled
3. Try refreshing VS Code: `Cmd + Shift + P` → "Developer: Reload Window"

### Wrong Context

If agent seems to have wrong context:

1. Check that `.vscode/copilot-contexts.json` is properly configured
2. Check file inclusion/exclusion rules
3. Clear Copilot Chat cache: Delete `.vscode/extensions/github.copilot-chat` folder
4. Reload VS Code

### Copilot Chat Not Responding

1. Check GitHub Copilot extension is activated
2. Ensure you're signed in to GitHub
3. Verify API key is valid
4. Check internet connection
5. Try restarting VS Code

---

## 📖 Additional Resources

- [Copilot Instructions](./copilot-instructions.md) - Core architecture patterns
- [Agent Roles Summary](./AGENT_ROLES_SUMMARY.md) - Quick reference
- [Role-Based Documentation](../docs/by-role/) - Detailed role guides
- [Application Specifications](../docs/APPLICATION_SPECIFICATIONS.md) - Full requirements

---

## ✅ Checklist: Onboarding New Team Member

- [ ] VS Code installed with Copilot Chat extension
- [ ] GitHub Copilot extension installed and signed in
- [ ] `.vscode/copilot-agents.json` loaded
- [ ] Understand available agents (review this document)
- [ ] Try mentioning `@backend-developer` in Copilot Chat
- [ ] Read role-specific documentation (`docs/by-role/[YOUR_ROLE].md`)
- [ ] Bookmark agent selection guide (this document)
- [ ] Start using agents for development tasks

---

## 🎯 Next Steps

1. **Right Now:** Install Copilot Chat extension if not already installed
2. **Next:** Open Copilot Chat and try: `@backend-developer Hello! What can you help me with?`
3. **Then:** Read your role-specific documentation
4. **Finally:** Start using agents for development tasks!

---

**Questions?** Ask your Tech Lead or check the [Copilot Instructions](./copilot-instructions.md)

---

*Agent registration configured for B2Connect DDD Microservices architecture with Wolverine, Onion Architecture, and compliance-first development approach.*
