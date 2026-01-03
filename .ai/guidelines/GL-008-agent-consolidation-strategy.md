# 🎯 Agent Consolidation Strategy (Rate-Limit Optimierung)

**DocID**: `GL-008`  
**Status**: ✅ Active (Opt-In)  
**Owner**: @SARAH  
**Created**: 3. Januar 2026  
**Rollback**: Siehe Abschnitt "Rollback to Full Agent Mode"

---

## Übersicht

Diese Strategie reduziert die Anzahl aktiver Agenten von 16 auf 3-5, um Rate-Limits zu vermeiden.

**WICHTIG**: Diese Strategie ist **opt-in** und kann jederzeit deaktiviert werden.

---

## Tier-System

### Tier 1 - Primär (Immer verfügbar)

| Konsolidierter Agent | Ersetzt | Aufgaben |
|---|---|---|
| `@SARAH` | - | Coordination, Routing, Quality-Gate |
| `@Dev` | @Backend + @Frontend + @TechLead | Full-Stack Development, Code Quality |
| `@Quality` | @QA + @Security | Testing, Security, Compliance |

### Tier 2 - On-Demand (Nur bei Bedarf)

| Agent | Aktivieren wenn |
|---|---|
| `@Architect` | ADRs, Major Design Decisions, Service Boundaries |
| `@DevOps` | Deployment, Infrastructure, CI/CD Changes |
| `@Legal` | GDPR, Licensing, Contractual Review |

### Tier 3 - Archiviert (Selten, File-basiert)

Diese Agenten werden NICHT gelöscht, aber ihre Arbeit erfolgt über `.ai/` Files statt interaktiv:

| Agent | Alternative |
|---|---|
| @ScrumMaster | `.ai/sprint/` Files |
| @ProductOwner | `.ai/requirements/` Files |
| @UX, @UI | `.ai/design/` Files |
| @SEO, @GitManager, @Enventa, @DocMaintainer | On-demand nur |

---

## Konsolidierte Agent-Definitionen

### @Dev (Full-Stack)

```yaml
# Kombiniert: @Backend + @Frontend + @TechLead
Expertise:
  - .NET 10, Wolverine CQRS, EF Core, PostgreSQL
  - Vue.js 3, TypeScript, Tailwind CSS, Pinia
  - Code Quality, Reviews, Refactoring
  
Owns:
  - API layer, Services, Repositories
  - UI Components, State Management
  - Code Standards, Best Practices
```

### @Quality (QA + Security)

```yaml
# Kombiniert: @QA + @Security
Expertise:
  - Unit/Integration/E2E Testing
  - Security Audits, OWASP Compliance
  - Vulnerability Assessment
  
Owns:
  - Test Coverage, Test Strategy
  - Security Reviews, Auth/Authz
  - Compliance Verification
```

---

## Workflow-Regeln

### Aktive Strategie (Consolidation Mode)

```
┌─────────────────────────────────────────────────────┐
│  CONSOLIDATION MODE (Rate-Limit Optimiert)          │
├─────────────────────────────────────────────────────┤
│                                                     │
│  Regel 1: Max 2 Agenten pro Session                 │
│  Regel 2: @SARAH immer zuerst für Routing           │
│  Regel 3: @Dev für alle Implementation              │
│  Regel 4: @Quality für alle Reviews/Tests           │
│  Regel 5: Tier-2 Agenten nur on-demand              │
│  Regel 6: Tier-3 über Files, nicht Chat             │
│                                                     │
└─────────────────────────────────────────────────────┘
```

### Beispiel-Workflows

**Feature Implementation:**
```
User → @SARAH "Neues Feature X"
     → @SARAH erstellt Plan in .ai/requirements/
     → @Dev implementiert (Backend + Frontend)
     → @Quality reviewed (Tests + Security)
     → Done (3 Agenten statt 6+)
```

**Bug Fix:**
```
User → @Dev "Bug in Component Y"
     → @Dev analysiert und fixt
     → @Quality verifiziert
     → Done (2 Agenten)
```

**Architecture Decision:**
```
User → @SARAH "Brauche ADR für Z"
     → @Architect (Tier-2 aktiviert)
     → ADR in .ai/decisions/
     → @Dev implementiert
     → Done (3 Agenten)
```

---

## Aktivierung

### Consolidation Mode aktivieren

Füge zu `.github/copilot-instructions.md` hinzu:

```markdown
## Active Strategy
**Mode**: Consolidation (GL-008)
- Use @Dev instead of @Backend/@Frontend/@TechLead
- Use @Quality instead of @QA/@Security
- Max 2 agents per session
```

### Agenten-Mapping (für User)

| Wenn du brauchst... | Nutze stattdessen |
|---|---|
| @Backend | @Dev |
| @Frontend | @Dev |
| @TechLead | @Dev |
| @QA | @Quality |
| @Security | @Quality |
| @ProductOwner | @SARAH + `.ai/requirements/` |
| @ScrumMaster | `.ai/sprint/` Files |

---

## 🔄 Rollback to Full Agent Mode

### Sofort-Rollback (1 Minute)

Entferne/kommentiere in `.github/copilot-instructions.md`:

```markdown
## Active Strategy
<!-- DISABLED: Consolidation Mode
**Mode**: Consolidation (GL-008)
...
-->
**Mode**: Full Agent Team (Default)
```

### Was passiert beim Rollback:

1. ✅ Alle 16 Agenten wieder verfügbar
2. ✅ Keine Agent-Definitionen gelöscht
3. ✅ Volle Spezialisierung wieder aktiv
4. ⚠️ Rate-Limits können wieder auftreten

### Wann Rollback sinnvoll:

- Premium/Paid Copilot Plan (keine Rate-Limits)
- Komplexes Feature mit vielen Domains
- Compliance-Audit (braucht @Legal separat)
- Design-Sprint (braucht @UX + @UI separat)

---

## Monitoring

### Erfolgsmetriken

| Metrik | Vorher | Nachher | Messung |
|---|---|---|---|
| Aktive Agenten/Session | 6+ | 2-3 | Manuell |
| Rate-Limit Errors/Tag | 3-5 | 0-1 | Error Logs |
| Session-Dauer | Unlimited | 45 Min + Pause | Timer |

### Rate-Limit Check

Wenn Rate-Limit auftritt:
1. Warte 15-30 Minuten
2. Prüfe ob Consolidation Mode aktiv
3. Reduziere auf 1 Agent (@Dev only)

---

## Nicht betroffen

Diese Elemente bleiben unverändert:

- ✅ Agent-Definitionen in `.github/agents/` (nicht gelöscht)
- ✅ Prompts in `.github/prompts/`
- ✅ Instructions in `.github/instructions/`
- ✅ `.ai/` Struktur und Dokumentation

---

## Zusammenfassung

| Aspekt | Consolidation Mode | Full Mode |
|---|---|---|
| Agenten aktiv | 3-5 | 16 |
| Rate-Limit Risiko | Niedrig | Hoch |
| Spezialisierung | Konsolidiert | Voll |
| Rollback | Sofort möglich | - |

**Empfehlung**: Consolidation Mode für tägliche Arbeit, Full Mode für komplexe Multi-Domain Features.

---

**Agents**: @SARAH, @CopilotExpert | **Owner**: @SARAH
