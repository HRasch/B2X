# Agent Context Optimization Guidelines

## Ziel
Minimaler Token-Verbrauch bei maximaler Effektivität der Agent-Definitionen.

## Metriken

| Metrik | Ziel | Warnung | Kritisch |
|--------|------|---------|----------|
| Zeilen pro Agent | <35 | 35-50 | >50 |
| Gesamt (alle Agents) | <350 | 350-500 | >500 |
| Tokens pro Agent | <600 | 600-900 | >900 |

## Optimierungsprinzipien

### 1. Struktur über Prosa
```markdown
❌ Vermeiden:
"The Backend Agent is responsible for designing and implementing 
server-side logic, APIs, and databases. It ensures systems are 
scalable, reliable, and maintainable."

✅ Besser:
## Role
Server-side logic, APIs, databases.
```

### 2. Bullet Points statt Fließtext
```markdown
❌ Vermeiden:
The agent should validate all inputs, use parameterized queries
to prevent SQL injection, and implement proper error handling.

✅ Besser:
- Input validation on all endpoints
- Parameterized queries (SQL injection)
- Proper error handling
```

### 3. Redundanz eliminieren
- Gemeinsame Standards → `.github/copilot-instructions.md`
- Detaillierte Guidelines → `.ai/guidelines/`
- Nur agent-spezifisches in `.agent.md`

### 4. Koordination kompakt
```markdown
## Coordinates With
- @Backend: API contracts
- @Security: Auth flows
```

### 5. Outputs statt Prozesse
```markdown
❌ Vermeiden:
"Reviews code focusing on security, participates in threat modeling,
conducts security audits..."

✅ Besser:
## Outputs
- Security reviews
- Threat models
- Audit reports
```

## Review-Checklist

Bei jeder Agent-Änderung prüfen:

- [ ] Unter 35 Zeilen?
- [ ] Keine Wiederholung von `copilot-instructions.md`?
- [ ] Bullet Points statt Prosa?
- [ ] Nur essentielle Information?
- [ ] `Coordinates With` Sektion vorhanden?
- [ ] `Outputs` klar definiert?

## Regelmäßige Reviews

SARAH führt Context-Reviews durch bei:
- Neuen Agent-Definitionen
- Größeren Agent-Updates
- Quartalsweise für alle Agenten

## Workflows & Prompts

**Detailed Optimization Process:**
- Prompt: [context-optimization.prompt.md](../../github/prompts/context-optimization.prompt.md)
- Workflow: [context-optimization.workflow.md](../workflows/context-optimization.workflow.md)
- Logs: `.ai/logs/context-audit-*.md` (monthly)

**Usage:**
```
User: @Agent "Optimize your context for efficiency"
Agent: Follows context-optimization.prompt.md
Output: Reduced token usage, clearer hierarchy
```

## Aktuelle Statistik

```
Agent            | Zeilen | Status
-----------------|--------|--------
SARAH            |   39   | ✅ OK
Architect        |   32   | ✅ OK
Backend          |   30   | ✅ OK
DevOps           |   32   | ✅ OK
ScrumMaster      |   32   | ✅ OK
Security         |   32   | ✅ OK
Frontend         |   31   | ✅ OK
QA               |   31   | ✅ OK
TechLead         |   31   | ✅ OK
DevelopmentTeam  |   29   | ✅ OK
ProductOwner     |   28   | ✅ OK
-----------------|--------|--------
GESAMT           |  347   | ✅ OK (Target: <350)
