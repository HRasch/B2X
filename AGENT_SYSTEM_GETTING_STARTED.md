# 🚀 Agent System Getting Started Guide

**Time to Set Up:** 5 minutes  
**First Use:** Immediate  
**Benefits:** Faster, more focused Copilot responses

---

## What You Just Got

You now have **3 specialized Copilot agents** ready to use:

1. **🔐 Security Engineer Agent** - Build P0.1-P0.5 compliance infrastructure
2. **💻 Backend Developer Agent** - Implement features with Wolverine & compliance
3. **🧪 QA Engineer Agent** - Create & manage 52 compliance tests

Each agent has its own context with only the relevant documents, patterns, and constraints.

---

## Installation (Already Done!)

✅ Agent files created:
```
.github/
├── role-contexts/
│   ├── security-engineer-context.md
│   ├── backend-developer-context.md
│   └── qa-engineer-context.md
├── activate-agent.sh
└── AGENT_QUICK_REFERENCE.txt
```

---

## Your First Agent Usage (5 minutes)

### Step 1: Open Terminal
```bash
cd /Users/holger/Documents/Projekte/B2Connect
```

### Step 2: Activate Backend Developer Agent
```bash
./.github/activate-agent.sh backend
```

Expected output:
```
========================================
Copilot Agent Activated
========================================
✅ Role: 💻 Backend Developer
✅ Context: .github/role-contexts/backend-developer-context.md

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

In VS Code, open Copilot Chat and ask:

```
I'm a Backend Developer. Create a simple Product service using Wolverine pattern 
with validation and audit logging for P0.6. Include xUnit tests.
```

Watch Copilot generate focused, compliant code! 🎯

---

## Try All 3 Agents

### Agent 1: Security Engineer (10 min)

```bash
./.github/activate-agent.sh security
# Reload Copilot

# Ask:
"Implement P0.1 Audit Logging with SaveChangesInterceptor, 
AES-256 encryption, and tenant isolation. Include tests."
```

### Agent 2: Backend Developer (10 min)

```bash
./.github/activate-agent.sh backend
# Reload Copilot

# Ask:
"Create a Wolverine HTTP service for VAT calculation with 
FluentValidation and audit logging integration."
```

### Agent 3: QA Engineer (10 min)

```bash
./.github/activate-agent.sh qa
# Reload Copilot

# Ask:
"Write xUnit tests for P0.6 withdrawal period validation. 
Include tests for within 14 days (pass) and after 14 days (fail)."
```

---

## How Each Agent Helps

### 🔐 Security Agent
- **Enforces** AES-256 encryption for all PII
- **Ensures** audit logging on every write
- **Prevents** hardcoded secrets
- **Verifies** tenant isolation

Use when: Building security/compliance features

### 💻 Backend Agent
- **Enforces** Wolverine pattern (not MediatR!)
- **Ensures** Onion architecture
- **Integrates** audit logging & encryption
- **Validates** all inputs

Use when: Building business features

### 🧪 QA Agent
- **Writes** all 52 compliance tests
- **Verifies** P0.6, P0.7, P0.8, P0.9
- **Gates** all deployments on test passing
- **Reports** test coverage

Use when: Creating tests & verifying compliance

---

## Real-World Workflow

### Scenario: Building P0.6 VAT Calculation

#### Step 1: Security Engineer Creates Encryption (Day 1)

```bash
./.github/activate-agent.sh security

# Copilot Chat:
# "Create an encryption service for cost data in Product entity using AES-256."
# → Generates AES encryption service, tests, KeyVault integration
```

#### Step 2: Backend Developer Creates Service (Day 2)

```bash
./.github/activate-agent.sh backend

# Copilot Chat:
# "Create a Wolverine HTTP handler for VAT calculation using the encryption service."
# → Generates handler, DTOs, validators, audit logging
```

#### Step 3: QA Engineer Creates Tests (Day 3)

```bash
./.github/activate-agent.sh qa

# Copilot Chat:
# "Create tests for P0.6 VAT calculation: B2C (19% VAT), B2B (0% with VAT-ID)."
# → Generates unit & integration tests, assertions
```

#### Result: Feature complete with security & compliance built-in! ✅

---

## Common Questions

### Q: Can I use multiple agents in one chat?

**A:** No, keep one agent per chat session. Different agents have different constraints.

**Better:** Use agent for one P0 component, then switch agents for next component.

### Q: What if I ask a question outside the agent's scope?

**A:** The agent will still try to help but may reference documents from other roles. Just tell Copilot to re-focus:
```
"Stay in Backend Developer context. Use Wolverine pattern only."
```

### Q: Can I restore the original global context?

**A:** Yes!
```bash
cp .github/copilot-instructions.md.bak .github/copilot-instructions.md
```

### Q: Which agent should I use first?

**A:** Start with **Backend Developer** if building features, or **Security Engineer** if building P0 infrastructure.

### Q: Can I create custom agents?

**A:** Yes! Copy any existing agent and customize. Update `activate-agent.sh` case statement.

---

## Tips for Best Results

1. **Activate first** - Always activate agent BEFORE asking question
2. **Be specific** - Say "I'm a Backend Developer..." in your first message
3. **Reference patterns** - Ask Copilot to reference CheckRegistrationTypeService.cs
4. **Test generation** - Always ask for xUnit tests with 80%+ coverage
5. **Copy-paste** - Generated code is template, always review before committing

---

## Next Steps

### Immediate (Now)
- [ ] Try all 3 agents (30 min total)
- [ ] Save quick reference card: `.github/AGENT_QUICK_REFERENCE.txt`
- [ ] Bookmark agent README: `.github/role-contexts/README.md`

### This Week
- [ ] Use agents for your first P0 component
- [ ] Share agent approach with team
- [ ] Provide feedback on agent quality

### This Month
- [ ] Create additional agents (Frontend, DevOps, Legal)
- [ ] Integrate with team's Copilot workflow
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
│  Example: backend-developer-context.md │
└────────────────────────────────────────┘
         ↓
┌────────────────────────────────────────┐
│  Agent Generates Focused Response      │
│  - Wolverine pattern (not MediatR)     │
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
| Context file not found | Check `.github/role-contexts/` exists with all agent files |
| Wrong agent active | Run `activate-agent.sh <role>` again |
| Script permission denied | Run `chmod +x .github/activate-agent.sh` |
| Copilot ignoring context | Clear chat (`Cmd+K, Cmd+K`) and ask again |

---

## Full Documentation

For detailed information, see:
- **Overview:** `.github/role-contexts/README.md`
- **Quick Ref:** `.github/AGENT_QUICK_REFERENCE.txt`
- **Security Agent:** `.github/role-contexts/security-engineer-context.md`
- **Backend Agent:** `.github/role-contexts/backend-developer-context.md`
- **QA Agent:** `.github/role-contexts/qa-engineer-context.md`

---

## Let's Go! 🚀

```bash
# Activate your first agent
./.github/activate-agent.sh backend

# Reload Copilot Chat in VS Code
# Then ask a question and watch the magic happen ✨
```

---

**Questions?** The agents themselves are trained to help. Just ask Copilot!
