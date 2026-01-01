# Knowledge Consolidation Framework

## Überblick

SARAH evaluiert nach jeder Directive, Entscheidung und gelerntem Wissen, ob Agent-Instructions und Prompts aktualisiert werden sollten. Dies macht Wissen **nachhaltig** und **embedded** in Agent-Behavior.

## Rationale

### Problem: Temporäre vs. Nachhaltige Directives
```
Temporärer Ansatz:
1. SARAH gibt Directive: "Alle Tests müssen XYZ Format haben"
2. Agent folgt Directive während Task
3. Nach Task: Agent vergisst Detail → Wiederholung nächstes Mal

Nachhaltiger Ansatz:
1. SARAH gibt Directive: "Alle Tests müssen XYZ Format haben"
2. Agent folgt Directive
3. SARAH aktualisiert QA Agent Instructions: "Testing Standard: XYZ Format"
4. Nachfolgende Generationen von Tasks verwenden automatisch neues Standard
```

## Knowledge Consolidation Prozess

### Phase 1: Directive Execution
```
SARAH gibt Directive an Agent(en)
Agenten führen aus und berichten Ergebnisse
```

### Phase 2: Learning Evaluation
SARAH bewertet nach Completion:
- ✅ Was funktionierte gut?
- ✅ Was ist neues erkanntes Pattern?
- ✅ Was sollte Standard sein?
- ✅ Welche Agenten sollten davon wissen?

### Phase 3: Knowledge Classification
SARAH klassifiziert gelernte Erkenntnisse als:

**A) Agent-spezifisches Wissen**
→ Update in Agent Instructions (`.github/agents/Agent.agent.md`)
```
Beispiel:
Learning: "Backend APIs sollten immer JSON-Schema haben"
Update: Backend.agent.md → "API Design" Sektion
```

**B) Domain-übergreifendes Wissen**
→ Update in Guidelines (`.ai/guidelines/`)
```
Beispiel:
Learning: "Code-Review müssen Security-Aspekte checken"
Update: `.ai/guidelines/quality/code-review-standards.md`
```

**C) Prompt-Optimierungen**
→ Update in Prompts (`.github/prompts/`)
```
Beispiel:
Learning: "Bessere Token-Effizienz mit [Struktur]"
Update: System Prompts aktualisieren
```

**D) Workflow-Verbesserungen**
→ Update in Workflows (`.ai/workflows/`)
```
Beispiel:
Learning: "Dieser Koordinations-Ansatz ist 3x schneller"
Update: Workflow-Definition optimieren
```

**E) Permission-Adjustments**
→ Update in Permissions (`.ai/permissions/`)
```
Beispiel:
Learning: "Agent braucht diese Permission für häufige Tasks"
Update: Neue delegierte Permission erteilen
```

### Phase 4: Implementation Plan
SARAH erstellt Update-Plan:
```
Was wird aktualisiert?
Welche Datei(en)?
Welcher Agent sollte über Update informiert werden?
Braucht es Quality-Gate Review? (JA für Guidelines, Security, Prompts)
Rückwärts-Kompatibilität? (Alte Directives funktionieren noch?)
```

### Phase 5: Update Execution
```
SARAH führt Update aus (mit QG-Review wenn nötig)
Betroffene Agenten werden benachrichtigt
Updates werden dokumentiert in `.ai/collaboration/learning-log.md`
```

## Consolidation Katalog

### Beispiel 1: Testing-Standard Consolidation
```
Directive: "Alle Unit-Tests müssen >80% Coverage haben"
↓ (Nach Testing von 10 Features)
Learning: "Diese Coverage schafft gute Balance"
↓
Consolidation:
- Update: QA.agent.md → "Testing Standards"
- Update: `.ai/guidelines/quality/testing-standards.md`
- Notify: QA Agent
- Log: Learning-Log Eintrag
```

### Beispiel 2: API-Design Consolidation
```
Directive: "Verwende Standardized Error Response Format"
↓ (Nach API-Review mit Security Agent)
Learning: "Format ist sicher und konsistent"
↓
Consolidation:
- Update: Backend.agent.md → "API Design"
- Update: `.github/prompts/api-design-template.md`
- Update: `.ai/guidelines/architecture/api-standards.md`
- Notify: Backend Agent
- Log: Learning-Log Eintrag
```

### Beispiel 3: Security Consolidation
```
Directive: "Prüfe auf SQL Injection in allen Queries"
↓ (Nach Security Review)
Learning: "Pattern erkannt: [Spezifische Checks]"
↓
Consolidation:
- Update: Security.agent.md → "Code Review Focus"
- Update: `.ai/guidelines/security/injection-prevention.md`
- Update: `.github/prompts/security-review-checklist.md`
- Notify: Security Agent
- Log: Learning-Log Eintrag
```

## Sustainability Prinzipien

1. **Aus Einzelnen zu Patterns** - Ein gutes Beispiel wird zum Standard
2. **Aus Directives zu Instructions** - Ad-hoc Anweisungen werden zu Agent-Verhalten
3. **Aus Erkenntnissen zu Guidelines** - Lessons Learned sind dokumentierte Standards
4. **Aus Prompts zu Templates** - Wirksame Prompts werden wiederverwendbar
5. **Aus Trials zu Policies** - Bewährte Workflows werden Standardprozesse

## Trigger für Consolidation

SARAH evaluiert Consolidation nach:
- ✅ Abschluss wichtiger Projekte/Features
- ✅ Erfolgreiche Wiederholung eines Patterns (2+ mal)
- ✅ Agent-Feedback über konsistentes Problem
- ✅ Security/Quality Incident (→ sofort konsolidieren)
- ✅ Monatliche Review-Zyklen
- ✅ Auf Anfrage eines Agenten

## Dokumentation & Audit

Alle Consolidations werden dokumentiert:
- `.ai/collaboration/learning-log.md` - Was wurde gelernt?
- `.ai/collaboration/consolidation-log.md` - Was wurde aktualisiert?
- Audit-Trail in relevanten Guideline/Prompt-Dateien

## Anti-Patterns

⚠️ SARAH vermeidet:
- ❌ "Nur Directive geben, nie konsolidieren" → Wissen geht verloren
- ❌ "Alles konsolidieren" → Zu häufige Änderungen, Instabilität
- ❌ "Alte Instructions vergessen" → Verwirrung, Inkonsistenz
- ❌ "Prompts ohne Basis ändern" → Unpredictable Behavior
