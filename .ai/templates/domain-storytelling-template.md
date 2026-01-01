# Domain Storytelling Template

## Session Information
- **Story Title:** [Beschreibender Titel der Domain Story]
- **Datum:** [YYYY-MM-DD]
- **Storyteller:** [Domain Expert Name]
- **Facilitator:** [Moderator Name]
- **Participants:** [Liste aller Teilnehmer]
- **Domain Context:** [Welcher Business Bereich?]

## Story Overview
### Business Context
[Warum erzählen wir diese Story? Welches Problem löst sie?]

### Story Scope
[Welcher Prozess/Ablauf wird beschrieben?]

### Expected Outcome
[Was soll durch diese Story erreicht werden?]

## Story Elements

### Actors (Personen/Rollen)
- **🏢 [Actor Name]:** [Rolle und Verantwortlichkeiten]
- **🏢 [Actor Name]:** [Rolle und Verantwortlichkeiten]
- **🔧 [System Actor]:** [Automatisierte Prozesse]

### Work Objects (Geschäftsobjekte)
- **📄 [Object Name]:** [Beschreibung und Zweck]
- **📄 [Object Name]:** [Beschreibung und Zweck]
- **💾 [Data Object]:** [Persistente Daten]

### Activities (Aktivitäten)
1. **[Activity Name]** - [Kurze Beschreibung]
2. **[Activity Name]** - [Kurze Beschreibung]
3. **[Activity Name]** - [Kurze Beschreibung]

## Story Narrative

### Szene 1: [Szenen Titel]
**Setting:** [Wo und wann spielt die Szene?]

**Actors involved:**
- [Actor 1] - [Rolle in dieser Szene]
- [Actor 2] - [Rolle in dieser Szene]

**Sequence of Activities:**
1. [Actor] tut [Activity] mit [Work Object]
2. [Actor] erhält [Result/Information]
3. [Actor] entscheidet [Decision]
4. [Actor] führt [Next Activity] aus

**Key Business Rules:**
- [Rule 1]: [Beschreibung]
- [Rule 2]: [Beschreibung]

### Szene 2: [Szenen Titel]
[Wie oben strukturiert]

### Szene 3: [Szenen Titel]
[Wie oben strukturiert]

## Domain Story Visualisierung

```
🏢 Kunde                    📄 Bestellung
    │                           │
    │ 1. Produkt auswählen      │
    │──────────────────────────►│
    │                           │
    │ 2. Bestellung aufgeben    │
    │──────────────────────────►│
    │                           │
🔧 Bestell-System              │
    │                           │
    │ 3. Zahlung verarbeiten    │
    │──────────────────────────►│
    │                           │
    │ 4. Bestellung bestätigen  │
    │──────────────────────────►│
```

## Business Rules & Constraints

### Explicit Rules
1. **[Rule Name]**: [Detaillierte Beschreibung]
   - **Context:** [Wann gilt diese Regel?]
   - **Enforcement:** [Wie wird sie durchgesetzt?]
   - **Exceptions:** [Wann gilt sie nicht?]

2. **[Rule Name]**: [Detaillierte Beschreibung]
   - **Context:** [Wann gilt diese Regel?]
   - **Enforcement:** [Wie wird sie durchgesetzt?]
   - **Exceptions:** [Wann gilt sie nicht?]

### Implicit Assumptions
- [Annahme 1]: [Begründung und Validierung]
- [Annahme 2]: [Begründung und Validierung]

## Critical Incidents & Edge Cases

### Happy Path Scenario
[Normale Ablauf ohne Probleme]

### Alternative Paths
1. **[Scenario Name]**: [Beschreibung]
   - **Trigger:** [Was löst dieses Szenario aus?]
   - **Resolution:** [Wie wird es gelöst?]

2. **[Scenario Name]**: [Beschreibung]
   - **Trigger:** [Was löst dieses Szenario aus?]
   - **Resolution:** [Wie wird es gelöst?]

### Error Scenarios
1. **[Error Case]**: [Beschreibung]
   - **Cause:** [Was verursacht den Fehler?]
   - **Impact:** [Welche Auswirkungen hat es?]
   - **Recovery:** [Wie wird der Fehler behoben?]

## Questions & Clarifications

### Open Questions
- [ ] [Frage 1] - [Kontext]
- [ ] [Frage 2] - [Kontext]
- [ ] [Frage 3] - [Kontext]

### Clarifications Needed
- [ ] [Unklarheit 1] - [Von wem klären?]
- [ ] [Unklarheit 2] - [Von wem klären?]

## Technical Implications

### Data Requirements
- **[Entity Name]**: [Benötigte Attribute/Properties]
- **[Entity Name]**: [Benötigte Attribute/Properties]

### System Interactions
- **[System A]** ↔ **[System B]**: [Art der Interaktion]
- **[System C]** → **[External API]**: [Integration Details]

### Performance Considerations
- **[High Volume Scenario]**: [Performance Requirements]
- **[Concurrent Access]**: [Concurrency Requirements]

## Action Items

### Immediate Actions
- [ ] **[Owner]**: [Task] - **[Deadline]**
- [ ] **[Owner]**: [Task] - **[Deadline]**

### Follow-up Activities
- [ ] **[Owner]**: [Task] - **[Deadline]**
- [ ] **[Owner]**: [Task] - **[Deadline]**

## Validation Checklist

### Story Completeness
- [ ] Alle wichtigen Actors identifiziert
- [ ] Alle Work Objects benannt
- [ ] Activities sind logisch sequenziert
- [ ] Business Rules dokumentiert

### Business Alignment
- [ ] Story wurde von Domain Expert validiert
- [ ] Technische Machbarkeit geprüft
- [ ] Business Value ist klar

### Technical Readiness
- [ ] Systemgrenzen definiert
- [ ] Integration Points identifiziert
- [ ] Datenmodel skizziert

## Story Acceptance
- **Storyteller:** ____________________ Datum: ________
- **Facilitator:** ____________________ Datum: ________
- **Technical Reviewer:** ____________________ Datum: ________

---

*Dieses Domain Storytelling Template hilft dabei, komplexe Business Prozesse zu verstehen und in technische Lösungen zu überführen.*