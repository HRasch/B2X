# Agent Directives & Instructions

## Überblick

SARAH kann offizielle Anweisungen und Directives an andere Agenten ausgeben, um koordinierte Aktion und Task-Ausführung sicherzustellen.

## Directive-Typen

### 1. Task Assignments
**Zweck:** Zuweisung spezifischer Aufgaben an Agenten
**Beispiel:** "Backend-Agent: Implementiere diese API basierend auf der Spezifikation"
**Bindung:** Agent folgt Anweisung innerhalb seiner Expertise

### 2. Priority Adjustments
**Zweck:** Änderung von Prioritäten für aktuelle Arbeiten
**Beispiel:** "QA-Agent: Priorisiere Security-Testing über Feature-Testing diesen Sprint"
**Bindung:** Agent passt Fokus entsprechend an

### 3. Process Changes
**Zweck:** Temporäre oder dauerhafte Änderungen am Workflow
**Beispiel:** "Alle Agents: Verwende das neue Code-Review Template ab sofort"
**Bindung:** Agent implementiert Änderung sofort

### 4. Coordination Instructions
**Zweck:** Koordination zwischen mehreren Agenten
**Beispiel:** "Backend & Frontend: Agreed auf dieses API-Design; handelt danach"
**Bindung:** Agent koordiniert mit anderen Agents gemäß Anweisung

### 5. Standards Enforcement
**Zweck:** Durchsetzung von Standards bei Non-Compliance
**Beispiel:** "Security-Agent: Review alle Credentials in Code-Base"
**Bindung:** Agent folgt sofort

### 6. Crisis Management
**Zweck:** Handlungsanweisungen in Notsituationen
**Beispiel:** "DevOps: Aktiviere Disaster Recovery Plan"
**Bindung:** Agent handelt sofort

## Directive Format

```
[SARAH DIRECTIVE]
To: [Agent(s)]
Type: [Task Assignment / Priority Adjustment / Process Change / Coordination / Standards Enforcement / Crisis]
Priority: [CRITICAL / HIGH / MEDIUM / LOW]
Deadline: [ggf. Datum/Uhrzeit]
Action: [Konkrete Anweisung]
Rationale: [Begründung]
Acceptance: [Bestätigung erforderlich? ja/nein]
```

## Compliance & Authority

- Directives von SARAH sind bindend für alle Agenten
- Agenten folgen Directives innerhalb ihrer Expertise und Verantwortung
- Bei Konflikten zwischen Directives und Agent-Guidelines kontaktiert Agent SARAH für Clarification
- Alle Directives werden dokumentiert in `.ai/collaboration/directives-log.md`

## Beispiel-Szenarien

1. **Feature Development Coordination**: SARAH koordiniert mehrere Agents für komplexes Feature
2. **Emergency Response**: SARAH gibt sofortige Anweisungen bei kritischen Issues
3. **Standards Rollout**: SARAH ordnet Implementierung neuer Guidelines an
4. **Performance Optimization**: SARAH dirigiert Agents zu Optimierungsaufgaben
5. **Project Restructuring**: SARAH leitet Agent-Reorganisation bei Projektänderungen
