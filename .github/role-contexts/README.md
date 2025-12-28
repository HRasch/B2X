# 🤖 Copilot Agent System

**Version:** 1.0 | **Status:** Active | **Last Updated:** 28. Dezember 2025

---

## Overview

This directory contains **role-specific Copilot agent contexts** that improve focus, performance, and team coordination. Each agent is tailored to a specific role with only the relevant documents, patterns, and constraints.

```
🔐 Security Engineer    💻 Backend Developer    🧪 QA Engineer
    P0.1-P0.5                  P0.1, P0.6               52 Tests
    Encryption                 Wolverine                 Compliance
    Audit Logging              CQRS                      Gate Control
    Incident Response          E-Commerce                Verification
```

---

## Quick Start

### 1. Activate Your Agent

```bash
# From repo root
chmod +x .github/activate-agent.sh
./.github/activate-agent.sh <role>
```

**Available Roles:**
- `security` - Security Engineer (P0.1-P0.5)
- `backend` - Backend Developer (Features + Wolverine)
- `qa` - QA Engineer (52 Compliance Tests)
- `frontend` - Frontend Developer (Vue.js + WCAG)
- `devops` - DevOps Engineer (Infrastructure)
- `product` - Product Owner (Prioritization)
- `legal` - Legal/Compliance (Regulations)
- `tech-lead` - Tech Lead (Architecture)

### 2. Reload Copilot Chat

In VS Code:
```
Cmd+K, Cmd+K (Clear chat) → or reload VS Code window
```

The new context is now active!

### 3. Start Working

Ask Copilot directly what you need:
```
"I'm a Backend Developer. Help me implement P0.6 VAT calculation with Wolverine."
```

Copilot will use role-specific context to generate focused, accurate code.

---

## What Each Agent Does

### 🔐 Security Engineer Agent
**Focus:** P0.1-P0.5 Compliance Infrastructure

- ✅ Enforces AES-256 encryption for all PII
- ✅ Ensures audit logging on every write
- ✅ Verifies tenant isolation
- ✅ Prevents hardcoded secrets
- ✅ Implements NIS2 incident response

**Documents:** `security-engineer-context.md`  
**Use When:** Building encryption, audit logging, incident response systems

---

### 💻 Backend Developer Agent
**Focus:** Feature Implementation with Compliance

- ✅ Uses Wolverine pattern (NOT MediatR!)
- ✅ Enforces Onion Architecture
- ✅ Integrates audit logging & encryption
- ✅ Validates all inputs with FluentValidation
- ✅ Ensures tenant isolation in queries

**Documents:** `backend-developer-context.md`  
**Use When:** Building APIs, services, business logic

---

### 🧪 QA Engineer Agent
**Focus:** 52 Compliance Tests & Verification

- ✅ Writes tests for all 4 P0 components
- ✅ Ensures P0.6 (E-Commerce), P0.7 (AI Act), P0.8 (BITV), P0.9 (E-Rechnung)
- ✅ Verifies compliance gate criteria
- ✅ Automates test execution

**Documents:** `qa-engineer-context.md`  
**Use When:** Writing tests, verifying features, managing test gates

---

### 🎨 Frontend Developer Agent *(Coming Soon)*
**Focus:** Vue.js + WCAG Accessibility

- ✅ Enforces WCAG 2.1 AA compliance
- ✅ Keyboard navigation & screen reader support
- ✅ Tailwind CSS best practices
- ✅ Vue 3 Composition API patterns
- ✅ i18n/localization

---

### ⚙️ DevOps Engineer Agent *(Coming Soon)*
**Focus:** Infrastructure & Scaling

- ✅ Aspire orchestration
- ✅ Network segmentation & DDoS
- ✅ Auto-scaling configuration
- ✅ Database replication & failover
- ✅ CI/CD pipeline setup

---

### 📋 Product Owner Agent *(Coming Soon)*
**Focus:** Prioritization & Go/No-Go Decisions

- ✅ Feature prioritization matrix
- ✅ Phase gate criteria
- ✅ Stakeholder communication
- ✅ Budget & timeline tracking

---

### ⚖️ Legal/Compliance Agent *(Coming Soon)*
**Focus:** Regulatory Compliance & Risk

- ✅ NIS2 requirements
- ✅ GDPR/AI Act interpretation
- ✅ Legal document review
- ✅ Incident notification procedures

---

### 👔 Tech Lead Agent *(Coming Soon)*
**Focus:** Architecture & Code Quality

- ✅ Architecture decisions
- ✅ Code review standards
- ✅ Design patterns
- ✅ Performance optimization

---

## File Structure

```
.github/
├── role-contexts/                    # Agent contexts
│   ├── security-engineer-context.md  # 🔐 Security focus
│   ├── backend-developer-context.md  # 💻 Backend focus
│   ├── qa-engineer-context.md        # 🧪 QA focus
│   ├── frontend-developer-context.md # 🎨 Frontend (planned)
│   ├── devops-engineer-context.md    # ⚙️ DevOps (planned)
│   ├── product-owner-context.md      # 📋 Product (planned)
│   ├── legal-compliance-context.md   # ⚖️ Legal (planned)
│   └── tech-lead-context.md          # 👔 Tech Lead (planned)
│
├── copilot-instructions.md           # 📌 Global instructions (swapped by agents)
├── copilot-instructions.md.bak       # Backup of original
└── activate-agent.sh                 # 🚀 Agent activation script
```

---

## How It Works

### Step 1: Agent Selection
```bash
./.github/activate-agent.sh backend
```

### Step 2: Context Swap
- Backs up current context
- Copies `backend-developer-context.md` → `copilot-instructions.md`

### Step 3: Copilot Reloads
- VS Code detects file change
- Copilot reads new context
- New instructions loaded

### Step 4: Focused Chat
```
User: "I'm a Backend Developer. Help me create a Product service with Wolverine."

Copilot thinks:
  ✓ I'm in Backend Developer context
  ✓ Only read Backend-specific docs
  ✓ Enforce Wolverine pattern (NOT MediatR!)
  ✓ Include audit logging integration
  ✓ Ensure tenant isolation
  ✓ Add FluentValidation

Response: [Focused, accurate code for backend service]
```

---

## Benefits

| Benefit | Impact |
|---------|--------|
| **Focused Context** | Each agent reads only 3-5 critical documents (not 50+) |
| **Faster Responses** | Smaller context = faster token processing |
| **Better Quality** | Agent enforces role-specific standards |
| **Clear Ownership** | Each P0 component has assigned agent |
| **Team Coordination** | Security → Backend → QA pipeline |
| **Knowledge Sharing** | Junior devs learn patterns from agent-generated code |

---

## Usage Examples

### Example 1: Security Engineer Creating Audit Logging

```bash
# Activate security context
./.github/activate-agent.sh security

# Open VS Code Copilot Chat
# Ask: "Implement P0.1 Audit Logging with AES-256 encryption and tenant isolation"

# Copilot generates:
# ✅ AuditLogEntry entity
# ✅ SaveChangesInterceptor
# ✅ AES encryption service
# ✅ Tenant-safe queries
# ✅ Unit tests with encryption/isolation verification
```

### Example 2: Backend Developer Creating E-Commerce Feature

```bash
# Activate backend context
./.github/activate-agent.sh backend

# Open VS Code Copilot Chat
# Ask: "Create a VAT calculation service for P0.6 using Wolverine pattern"

# Copilot generates:
# ✅ Wolverine HTTP handler (NOT MediatR!)
# ✅ Onion architecture with Core/Application/Infrastructure
# ✅ FluentValidation for inputs
# ✅ Audit logging integration
# ✅ xUnit tests with 80%+ coverage
# ✅ Tenant isolation in queries
```

### Example 3: QA Engineer Creating Compliance Tests

```bash
# Activate QA context
./.github/activate-agent.sh qa

# Open VS Code Copilot Chat
# Ask: "Create test for P0.6 - VAT Calculation B2B Reverse Charge"

# Copilot generates:
# ✅ xUnit test with Arrange/Act/Assert
# ✅ VIES API mocking
# ✅ Assertion for 0% VAT when VAT-ID valid
# ✅ Integration test with DbContext
# ✅ Test name following naming convention
```

---

## Best Practices

### ✅ DO

- ✅ Activate correct agent BEFORE asking complex questions
- ✅ Ask role-specific questions ("I'm a Backend Developer...")
- ✅ Reference role-specific documents when clarifying
- ✅ Switch agents between different P0 components
- ✅ Use agent suggestions as templates (copy & adapt)

### ❌ DON'T

- ❌ Ask security questions in backend context
- ❌ Ask frontend questions in backend context
- ❌ Ignore agent constraints (e.g., "use MediatR" despite agent enforcing Wolverine)
- ❌ Mix contexts (don't ask multiple roles in one chat)
- ❌ Use global copilot-instructions.md directly (always use agents)

---

## Restoring Global Context

If you need to restore the original global context:

```bash
# Restore backup
cp .github/copilot-instructions.md.bak .github/copilot-instructions.md

# Reload Copilot Chat in VS Code
```

---

## Creating New Agents

To create a new agent for a different role:

1. **Create context file:** `.github/role-contexts/[role]-context.md`
2. **Follow template:** Use existing agents as examples
3. **Update activate-agent.sh:** Add new role to case statement
4. **Test:** Run `./activate-agent.sh [role]` and verify Copilot loads correctly

---

## Troubleshooting

### Copilot Not Detecting New Context?

1. Close Copilot Chat
2. Reload VS Code: `Cmd+Shift+P` → "Developer: Reload Window"
3. Reopen Copilot Chat

### Wrong Context Loaded?

```bash
# Check current context
head -5 .github/copilot-instructions.md

# Should show role name in header
# If not, reactivate: ./activate-agent.sh <role>
```

### Context File Not Found?

```bash
# Verify files exist
ls -la .github/role-contexts/

# Should list all agent context files
# If missing, recreate from backup or git restore
```

---

## Contact & Support

- **Questions?** → Ask in Copilot chat (it will know!)
- **Bug?** → Create issue with `[Agent]` prefix
- **New role?** → Submit PR with new context file

---

## Future Enhancements

- [ ] VS Code extension for role switching (quick pick menu)
- [ ] Role-specific command palette filters
- [ ] Automatic agent selection based on open file
- [ ] Agent-specific snippets & templates
- [ ] Team analytics (which agents used most)
- [ ] Context caching for faster switching
- [ ] Multi-agent conversations (agents can talk to each other)

---

**Ready? Activate your agent:**
```bash
./.github/activate-agent.sh <your-role>
```
