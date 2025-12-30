# Agent Coordination Framework

## Überblick

Effiziente Abstimmung zwischen AI-Agenten im Team, optimiert für **GitHub Copilot in VS Code**. Dieses Framework nutzt die tatsächlichen Capabilities von VS Code Copilot: Custom Agents, Custom Instructions, Prompt Files und MCP Tools.

## GitHub Copilot VS Code Capabilities

### Verfügbare Customization-Optionen

| Feature | Zweck | Datei-Location |
|---------|-------|----------------|
| **Custom Instructions** | Projekt-weite Coding Guidelines | `.github/copilot-instructions.md` |
| **Path-specific Instructions** | Datei/Pfad-spezifische Regeln | `.github/instructions/*.instructions.md` |
| **Custom Agents** | Spezialisierte Rollen (z.B. SARAH) | `.github/agents/*.agent.md` |
| **Prompt Files** | Wiederverwendbare Prompts | `.github/prompts/*.prompt.md` |
| **MCP Tools** | Externe Services & Tools | `.vscode/mcp.json` |

### Realistisches Kommunikationsmodell

```
⚠️ WICHTIG: GitHub Copilot hat KEINE direkte Agent-zu-Agent Kommunikation!

Tatsächliche Kommunikation läuft:
  User → Copilot Chat → Agent Mode aktiviert → Response
  
NICHT möglich:
  Backend Agent → Frontend Agent (direkt)
```

## Grundprinzipien (VS Code Copilot-angepasst)

### Wie Agenten tatsächlich koordinieren
```
✅ User wechselt zwischen Agent-Modes (@SARAH, @Backend, etc.)
✅ Agenten nutzen gemeinsame Instructions aus .github/copilot-instructions.md
✅ Prompt Files standardisieren wiederkehrende Tasks
✅ Kontext wird über Workspace-Files geteilt (nicht über Chat)
✅ MCP Tools erweitern Capabilities
```

### Praktische Koordinationsmuster

**Pattern 1: User als Coordinator**
```
User: @SARAH "Wie soll das API-Design aussehen?"
SARAH: [Gibt Empfehlung, referenziert Guidelines]

User: @Backend "Implementiere API nach SARAH's Empfehlung"
Backend: [Implementiert, nutzt .github/instructions/backend.instructions.md]

User: @Frontend "Integriere die neue API"
Frontend: [Integriert, nutzt path-specific instructions für frontend/]
```

**Pattern 2: Shared Context über Files**
```
Statt Chat-basierte Kommunikation:
→ Agent schreibt Output in definierte Dateien
→ Nächster Agent liest diese Dateien als Kontext

Beispiel:
1. @TechLead erstellt `.ai/decisions/ADR-001.md`
2. @Backend liest ADR-001.md als Kontext für Implementation
3. Alle nutzen `.github/copilot-instructions.md` für Standards
```

**Pattern 3: Prompt Files für Standard-Tasks**
```
.github/prompts/
├── code-review.prompt.md      # Standardisierter Review-Prozess
├── api-design.prompt.md       # API Design Template
├── security-check.prompt.md   # Security Checklist
└── handover.prompt.md         # Feature Handover Template
```

## Kommunikationskanäle (VS Code Copilot)

### 1. Custom Agent Mode Switching
**Mechanismus:** User aktiviert Agent via `@AgentName` im Chat
```
@SARAH "Brauche Guidance für Feature X"
@Backend "Implementiere den Service"
@QA "Erstelle Testplan"
```

### 2. Shared Instructions (Implizite Koordination)
**Mechanismus:** Alle Agenten lesen gemeinsame Instructions
```
.github/copilot-instructions.md       → Global für alle
.github/instructions/backend.instructions.md → Für Backend-Dateien
.github/instructions/frontend.instructions.md → Für Frontend-Dateien
```

### 3. Prompt Files (Task-Koordination)
**Mechanismus:** Vordefinierte Prompts für Standard-Workflows
```
User ruft auf: /handover-backend-to-frontend
→ Prompt File gibt strukturierten Ablauf vor
→ Output ist standardisiert
```

### 4. Workspace Files (Persistent Context)
**Mechanismus:** Dokumentation in Workspace-Files für Agent-übergreifenden Kontext
```
.ai/collaboration/current-sprint.md   → Aktueller Sprint-Status
.ai/decisions/                         → Architecture Decisions
.ai/handovers/                         → Feature Handover Docs
```

## Praktische Workflows (VS Code Copilot)

### Workflow 1: Feature Handover (Backend → Frontend)

**VS Code Copilot Implementierung:**
```
Schritt 1: @Backend erstellt Handover-Dokument
→ Output: .ai/handovers/feature-xyz-handover.md

Schritt 2: User attachet Dokument für @Frontend
→ @Frontend liest Kontext aus Handover-Dokument

Schritt 3: @Frontend erstellt Integration
→ Nutzt .github/instructions/frontend.instructions.md für Standards
```

**Prompt File:** `.github/prompts/feature-handover.prompt.md`
```markdown
---
mode: agent
agent: Backend
---
Erstelle ein Feature Handover Dokument für das Feature {{feature_name}}:

1. **API Endpoints** - Alle relevanten Endpoints mit Methods
2. **Request/Response Schemas** - Vollständige JSON Schemas
3. **Error Handling** - Alle Error Codes mit Beschreibungen
4. **Authentifizierung** - Auth-Requirements
5. **Beispiele** - curl/fetch Beispiele
6. **Known Issues** - Bekannte Einschränkungen

Speichere in: .ai/handovers/{{feature_name}}-handover.md
```

### Workflow 2: Code Review

**VS Code Copilot Implementierung:**
```
Schritt 1: User öffnet PR/geänderte Files
Schritt 2: @QA oder @Security mit Files als Kontext
→ Agent nutzt path-specific instructions automatisch
→ Agent gibt strukturiertes Review-Feedback
```

**Path-specific Instructions:** `.github/instructions/code-review.instructions.md`
```markdown
---
applyTo: "**/*.{ts,js,py}"
---
Bei Code Reviews prüfe:
- [ ] Security: Input Validation, Auth, Secrets
- [ ] Performance: N+1 Queries, Memory Leaks
- [ ] Error Handling: Try/Catch, Logging
- [ ] Tests: Unit Tests vorhanden und sinnvoll
- [ ] Style: Projekt-Conventions eingehalten
```

### Workflow 3: Architecture Decision

**VS Code Copilot Implementierung:**
```
Schritt 1: @Architect erstellt ADR Draft
→ Output: .ai/decisions/ADR-XXX-draft.md

Schritt 2: User fragt verschiedene Agents nach Input
→ @Backend: "Review ADR-XXX aus Backend-Perspektive"
→ @Security: "Review ADR-XXX aus Security-Perspektive"
→ @DevOps: "Review ADR-XXX aus Infra-Perspektive"

Schritt 3: @Architect finalisiert ADR mit allen Inputs
→ Output: .ai/decisions/ADR-XXX.md (final)
```

**Prompt File:** `.github/prompts/adr-review.prompt.md`
```markdown
---
mode: agent
---
Analysiere den ADR Draft in .ai/decisions/{{adr_file}} aus deiner Domain-Perspektive:

1. **Auswirkungen auf deine Domain**
2. **Risiken und Bedenken**
3. **Alternative Vorschläge** (falls relevant)
4. **Empfehlung** (Approve / Request Changes / Reject)
5. **Begründung**
```

### Workflow 4: Security Review

**VS Code Copilot Implementierung:**
```
Schritt 1: User attachet relevante Files
Schritt 2: @Security analysiert mit Security Instructions
→ .github/instructions/security.instructions.md wird automatisch angewandt

Schritt 3: Security erstellt Report
→ Output: Inline-Kommentare oder .ai/security/review-{{date}}.md
```

**Path-specific Instructions:** `.github/instructions/security.instructions.md`
```markdown
---
applyTo: "**/*"
---
Security-Prüfpunkte:
- Keine Secrets im Code (API Keys, Passwords)
- Input Validation auf allen Endpoints
- SQL Injection Prevention (Parameterized Queries)
- XSS Prevention (Output Encoding)
- CSRF Protection
- Authentication & Authorization korrekt
- Sensitive Data Handling (Encryption, Masking)
```

### Workflow 5: Bug Triage

**VS Code Copilot Implementierung:**
```
Schritt 1: User beschreibt Bug mit Kontext
Schritt 2: @SARAH empfiehlt zuständigen Agent
→ "Bug X betrifft API Authentication → @Backend"

Schritt 3: Zuständiger Agent analysiert
→ @Backend: "Analysiere Bug mit diesem Stacktrace"

Schritt 4: Agent erstellt Fix oder Workaround
```

---

## Workflow 6: Anforderungsanalyse (Requirements Analysis)

Die Analyse von Anforderungen erfordert die koordinierte Abstimmung mehrerer Agenten, um verschiedene Perspektiven einzuholen und ein vollständiges Bild zu erhalten.

### Beteiligte Agenten & Verantwortlichkeiten

| Agent | Perspektive | Prüft auf |
|-------|-------------|-----------|
| `@ProductOwner` | Business | Klarheit, Vollständigkeit, Priorisierung, User Value |
| `@Architect` | Systemarchitektur | System Design, Scalability, Patterns, Technical Debt |
| `@TechLead` | Code Quality | Codequalität, Mentoring, Best Practices |
| `@Backend` | Backend | API Design, Datenmodell, Business Logic |
| `@Frontend` | Frontend | UX Machbarkeit, UI Components, State |
| `@Security` | Security | Sicherheitsimplikationen, Compliance |
| `@QA` | Testbarkeit | Akzeptanzkriterien, Testszenarien |
| `@DevOps` | Operations | Deployment, Monitoring, Skalierung |
| `@SARAH` | Koordination | Abstimmung, Konflikte, Vollständigkeit |

### Anforderungsanalyse-Workflow

```
┌────────────────────────────────────────────────────────────────────┐
│                    REQUIREMENTS ANALYSIS WORKFLOW                   │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│  Phase 1: Initiale Erfassung                                       │
│  ──────────────────────────                                        │
│  User → @ProductOwner: "Analysiere diese Anforderung"              │
│  Output: .ai/requirements/REQ-XXX-initial.md                       │
│                                                                    │
│  Phase 2: Multi-Perspektiven-Analyse (Parallel möglich)            │
│  ─────────────────────────────────────────────────────             │
│  User → @Architect: "Prüfe REQ-XXX auf Systemdesign-Implikationen" │
│  User → @Backend:   "Prüfe REQ-XXX aus Backend-Sicht"              │
│  User → @Frontend:  "Prüfe REQ-XXX aus Frontend-Sicht"             │
│  User → @Security:  "Prüfe REQ-XXX auf Security-Implikationen"     │
│  User → @QA:        "Prüfe REQ-XXX auf Testbarkeit"                │
│  User → @DevOps:    "Prüfe REQ-XXX auf Ops-Implikationen"          │
│                                                                    │
│  Output: .ai/requirements/REQ-XXX-analysis.md (konsolidiert)       │
│                                                                    │
│  Phase 3: Konsolidierung & Konfliktlösung                          │
│  ────────────────────────────────────────                          │
│  User → @SARAH: "Konsolidiere Analysen für REQ-XXX"                │
│  Output: .ai/requirements/REQ-XXX-consolidated.md                  │
│                                                                    │
│  Phase 4: Finale Spezifikation                                     │
│  ─────────────────────────────                                     │
│  User → @ProductOwner: "Finalisiere REQ-XXX"                       │
│  Output: .ai/requirements/REQ-XXX-final.md                         │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

### Analyse-Dokument Struktur

**Phase 1 Output:** `.ai/requirements/REQ-XXX-initial.md`
```markdown
# REQ-XXX: [Titel]

## Anforderungsbeschreibung
[Original-Anforderung vom User/Stakeholder]

## User Story
Als [Rolle] möchte ich [Funktion], damit [Nutzen].

## Akzeptanzkriterien (vorläufig)
- [ ] Kriterium 1
- [ ] Kriterium 2

## Offene Fragen
- [ ] Frage 1
- [ ] Frage 2

## Abhängigkeiten (bekannt)
- [Abhängigkeit 1]

## Priorität (vorgeschlagen)
[Must/Should/Could/Won't] - [Begründung]

---
Erstellt: [Datum] | Agent: @ProductOwner | Status: Initial
```

**Phase 2 Output:** Agent-spezifische Analyse-Sektionen
```markdown
## TechLead Analyse
### Machbarkeit: [Hoch/Mittel/Niedrig]
### Architektur-Impact
- [Impact 1]
### Technische Schulden-Risiko
- [Risiko]
### Empfehlung
[Empfehlung]

## Backend Analyse
### Betroffene Services
- [Service 1]
### Datenmodell-Änderungen
- [Änderung 1]
### API-Änderungen
- [Endpoint]
### Aufwandsschätzung
[S/M/L/XL]
### Empfehlung
[Empfehlung]

## Frontend Analyse
### Betroffene Components
- [Component 1]
### UX Considerations
- [Consideration]
### State Management Impact
- [Impact]
### Aufwandsschätzung
[S/M/L/XL]
### Empfehlung
[Empfehlung]

## Security Analyse
### Sicherheitsimplikationen
- [Implikation 1]
### Compliance-Relevanz
- [Compliance-Punkt]
### Empfohlene Maßnahmen
- [Maßnahme 1]
### Security Sign-off Required
[Ja/Nein]

## QA Analyse
### Testbarkeit: [Hoch/Mittel/Niedrig]
### Vorgeschlagene Testszenarien
- [Szenario 1]
### Automatisierbarkeit
[Hoch/Mittel/Niedrig]
### Akzeptanzkriterien (ergänzt)
- [ ] Neues Kriterium

## DevOps Analyse
### Deployment Impact
- [Impact]
### Monitoring Requirements
- [Requirement]
### Skalierungs-Überlegungen
- [Überlegung]
### Infrastructure Changes
[Ja/Nein] - [Details]
```

**Phase 3 Output:** `.ai/requirements/REQ-XXX-consolidated.md`
```markdown
# REQ-XXX: [Titel] - Konsolidierte Analyse

## Zusammenfassung
[Executive Summary aller Perspektiven]

## Konsens-Punkte
- [Punkt 1: Alle Agenten stimmen zu]
- [Punkt 2: Alle Agenten stimmen zu]

## Identifizierte Konflikte
| # | Konflikt | Beteiligte | SARAH Empfehlung |
|---|----------|------------|------------------|
| 1 | [Konflikt] | Backend vs Security | [Empfehlung] |

## Risiken (priorisiert)
| Risiko | Quelle | Schwere | Mitigation |
|--------|--------|---------|------------|
| [Risiko 1] | @Security | Hoch | [Mitigation] |

## Gesamtaufwand
| Agent | Schätzung | Konfidenz |
|-------|-----------|-----------|
| Backend | M | Hoch |
| Frontend | S | Mittel |
| DevOps | S | Hoch |
| **Gesamt** | **M-L** | **Mittel** |

## Empfehlung
[SARAH's finale Empfehlung: Proceed / Adjust / Reject]

## Nächste Schritte
1. [Schritt 1]
2. [Schritt 2]

---
Konsolidiert: [Datum] | Agent: @SARAH
```

### Schnell-Analyse (Fast Track)

Für kleinere Anforderungen, die keine vollständige Multi-Agent-Analyse benötigen:

```
┌────────────────────────────────────────────────────┐
│              FAST TRACK ANALYSIS                   │
├────────────────────────────────────────────────────┤
│                                                    │
│  User → @SARAH: "Quick Analysis für [Anforderung]" │
│                                                    │
│  SARAH:                                            │
│  1. Identifiziert betroffene Domains               │
│  2. Empfiehlt relevante Agenten (2-3 max)          │
│  3. Gibt initiale Einschätzung                     │
│                                                    │
│  User konsultiert nur empfohlene Agenten           │
│                                                    │
└────────────────────────────────────────────────────┘
```

### Workflow 7: Dependency Upgrade Research

**Anwendungsfall:** Eine Softwareabhängigkeit (Node.js, React, PostgreSQL, etc.) hat eine neuere Version. Recherchiere die Dokumentation und aktualisiere die Knowledgebase.

**VS Code Copilot Implementierung:**
```
Schritt 1: User startet Recherche
User: @Architect "Recherchiere React von v17 zu v18"
→ Output: `.ai/knowledgebase/software/react/v17--to--v18.md`

Schritt 2: Researcher (Architect oder Subagent) fetcht Doku
Aufgaben:
  - Official Changelog
  - Migration Guide
  - Breaking Changes
  - Performance Improvements
  - Security Updates

Schritt 3: Zusammenfassung & Optimierung
→ Bullets statt Prosa
→ Tabellen für Vergleiche
→ Links statt Content-Kopien
→ Max 1 KB pro Feature
→ Agent-spezifische Sektionen

Schritt 4: Index Update
→ `.ai/knowledgebase/INDEX.md` aktualisiert
→ Tags vergeben (breaking-changes, security, etc.)

Schritt 5: SARAH Distribution
→ Alle Agents informieren
→ Verwandte Issues aktualisieren
```

**Dateistruktur:**
```
.ai/knowledgebase/
├── INDEX.md                         # Zentrale Übersicht & Tags
└── software/
    ├── nodejs/
    │   ├── v16--to--v18.md
    │   └── v20.md
    ├── react/
    │   └── v17--to--v18.md
    └── [mehr Software]
```

**Token-Optimierungsrichtlinien:**
- ✓ Bullets vor Prosa
- ✓ Vergleichstabellen nutzen
- ✓ Code-Beispiele nur für kritische Changes
- ✓ Links statt komplette Docs
- ✓ <50 Wörter pro Item
- ✓ Kurze Überschriften-Hierarchie

**Agent-spezifische Inhalte:**
```markdown
## Agent-specific Notes

### @Architect
[System Design Impact, Integration]

### @Backend
[API/Data Changes]

### @Frontend
[UI Component Updates]

### @Security
[CVE Fixes, Security Improvements]

### @DevOps
[Deployment Changes]
```

**Prompt File:** `.github/prompts/dependency-upgrade-research.prompt.md`

### Abstimmungs-Matrix

Wann welche Agenten einbeziehen:

| Anforderungstyp | PO | Tech | BE | FE | Sec | QA | Ops |
|-----------------|:--:|:----:|:--:|:--:|:---:|:--:|:---:|
| Neues Feature | ✅ | ✅ | ✅ | ✅ | ⚡ | ✅ | ⚡ |
| API Änderung | ⚡ | ✅ | ✅ | ✅ | ✅ | ⚡ | ⚡ |
| UI Änderung | ✅ | ⚡ | ⚡ | ✅ | ⚡ | ✅ | ❌ |
| Security Feature | ⚡ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Performance | ⚡ | ✅ | ✅ | ✅ | ❌ | ✅ | ✅ |
| Infrastruktur | ⚡ | ✅ | ⚡ | ❌ | ✅ | ⚡ | ✅ |
| Bug Fix | ❌ | ⚡ | ⚡ | ⚡ | ⚡ | ✅ | ⚡ |

**Legende:** ✅ Required | ⚡ Optional/Bei Bedarf | ❌ Nicht nötig

## VS Code Copilot Best Practices

### Pattern 1: Kontext über Attachments
```
Statt: Agent A sendet Nachricht an Agent B
Nutze: User attachet relevante Files beim Wechsel zu Agent B

Beispiel:
@Backend mit angehängtem `api-spec.yaml`
→ Backend hat sofort den nötigen Kontext
```

### Pattern 2: Prompt Files für Konsistenz
```
Statt: Jedes mal andere Formulierung
Nutze: Standardisierte Prompt Files

.github/prompts/
├── code-review.prompt.md
├── security-audit.prompt.md
├── performance-check.prompt.md
└── documentation.prompt.md
```

### Pattern 3: Path-specific Instructions
```
Automatische Domain-Zuordnung über Glob-Patterns:

.github/instructions/backend.instructions.md
---
applyTo: "src/api/**,src/services/**,src/models/**"
---

.github/instructions/frontend.instructions.md
---
applyTo: "src/components/**,src/pages/**,src/hooks/**"
---
```

### Pattern 4: Workspace Files als "Shared Memory"
```
Agenten "kommunizieren" über persistente Files:

.ai/
├── decisions/       → ADRs (Architecture Decisions)
├── handovers/       → Feature Handover Docs
├── reviews/         → Code/Security Review Reports
├── status/          → Sprint/Task Status
└── context/         → Shared Context Documents
```

### Pattern 5: Agent Mode für Domain-Expertise
```
@SARAH   → Koordination, Guidelines, Permissions
@Backend → API, Database, Services
@Frontend → UI, Components, State
@DevOps  → CI/CD, Infrastructure
@Security → Vulnerabilities, Auth, Compliance
@QA      → Testing, Quality, Bug Analysis
```

## Agent-Domain Matrix (VS Code Copilot)

| Agent | Aktivierung | Path-specific Files | Primäre Aufgaben |
|-------|-------------|---------------------|------------------|
| **SARAH** | `@SARAH` | Alle | Koordination, Guidelines, Permissions |
| **Architect** | `@Architect` | `docs/architecture/**` | System Design, ADRs, Patterns |
| **Backend** | `@Backend` | `src/api/**, src/services/**` | APIs, Services, Database |
| **Frontend** | `@Frontend` | `src/components/**, src/pages/**` | UI, Components, State |
| **DevOps** | `@DevOps` | `.github/**, Dockerfile, *.yml` | CI/CD, Infrastructure |
| **Security** | `@Security` | Alle (Security Focus) | Auth, Vulnerabilities |
| **QA** | `@QA` | `**/*.test.*, **/*.spec.*` | Tests, Quality |
| **TechLead** | `@TechLead` | `src/**` | Code Quality, Mentoring |
| **ProductOwner** | `@ProductOwner` | `docs/requirements/**` | Requirements, Stories |
| **ScrumMaster** | `@ScrumMaster` | `.ai/workflows/**` | Process, Workflows |

## Empfohlene Dateistruktur

```
.github/
├── copilot-instructions.md         # Global für alle Agents
├── agents/
│   ├── SARAH.agent.md
│   ├── Backend.agent.md
│   ├── Frontend.agent.md
│   ├── DevOps.agent.md
│   ├── Security.agent.md
│   ├── QA.agent.md
│   ├── TechLead.agent.md
│   ├── ProductOwner.agent.md
│   └── ScrumMaster.agent.md
├── instructions/
│   ├── backend.instructions.md     # applyTo: src/api/**, src/services/**
│   ├── frontend.instructions.md    # applyTo: src/components/**, src/pages/**
│   ├── testing.instructions.md     # applyTo: **/*.test.*, **/*.spec.*
│   ├── security.instructions.md    # applyTo: **/* (Security-Fokus)
│   └── devops.instructions.md      # applyTo: .github/**, Dockerfile
└── prompts/
    ├── code-review.prompt.md
    ├── feature-handover.prompt.md
    ├── security-audit.prompt.md
    ├── adr-create.prompt.md
    └── bug-analysis.prompt.md

.ai/
├── decisions/                       # Architecture Decision Records
├── handovers/                       # Feature Handover Documents
├── reviews/                         # Code/Security Reviews
├── status/                          # Sprint/Task Status
└── context/                         # Shared Context Documents
```

## Eskalation & SARAH Support

### Wann @SARAH einbeziehen:
```
✅ Konflikte zwischen Domain-Empfehlungen
✅ Unklare Zuständigkeiten
✅ Permission-Anfragen
✅ Guideline-Änderungen erforderlich
✅ Quality-Gate Entscheidungen
✅ Neue Agent/Workflow Anforderungen
```

### Eskalations-Workflow:
```
Schritt 1: User beschreibt Situation für @SARAH
Schritt 2: SARAH analysiert und gibt Guidance
Schritt 3: SARAH aktualisiert ggf. Guidelines/Permissions
Schritt 4: User führt mit empfohlenem Agent fort
```

## Einschränkungen von VS Code Copilot

### Was NICHT möglich ist:
```
❌ Direkte Agent-zu-Agent Kommunikation
❌ Parallele Agent-Ausführung in einer Anfrage
❌ Automatische Agent-Delegation
❌ Persistenter Chat-Kontext zwischen Sessions
❌ Agent-übergreifendes Memory
```

### Workarounds:
```
→ User als Coordinator (Agent-Switching)
→ Workspace Files als "Shared Memory"
→ Path-specific Instructions als implizite Koordination
→ Prompt Files für standardisierte Workflows
→ MCP Tools für externe Integrationen
```

## Anti-Patterns vermeiden

❌ **Vermeiden:**
- Annahme von direkter Agent-Kommunikation
- Vergessen von Kontext-Attachments
- Ignorieren von Path-specific Instructions
- Fehlende Dokumentation in Workspace Files
- Überspringen der SARAH-Guidance bei Unklarheiten

✅ **Stattdessen:**
- User koordiniert zwischen Agents
- Relevante Files immer attachen
- Domain-spezifische Instructions nutzen
- Wichtige Entscheidungen in .ai/ dokumentieren
- @SARAH für Koordination und Guidance nutzen
