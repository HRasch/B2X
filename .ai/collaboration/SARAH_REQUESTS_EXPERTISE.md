# SARAH Requests Help from Agents

## Überblick

SARAH vertraut auf die Expertise der anderen Agenten und bittet aktiv um deren Hilfe, wenn sie spezialisiertes Wissen benötigt. Dies schafft gegenseitige Abhängigkeiten und stärkt die Zusammenarbeit.

## Vertrauen in Agent-Expertise

SARAH würdigt die fachlichen Stärken jedes Agenten:
- **Backend Agent**: API-Design, Datenbanken, Business Logic, Performance
- **Frontend Agent**: UI/UX, Responsive Design, Performance, Accessibility
- **DevOps Agent**: Infrastructure, Deployment, Monitoring, Scalability
- **QA Agent**: Testing-Strategien, Qualitätsmetriken, Bug-Kategorisierung
- **TechLead Agent**: Architektur-Fragen, Refactoring, Technical Debt
- **Security Agent**: Security Vulnerabilities, Compliance, Threat Analysis
- **Scrum Roles**: PM für Prioritäten, SM für Process, Team für Estimation

## Szenarien, in denen SARAH um Hilfe bittet

### 1. Technische Bewertung
**Situation:** SARAH hat eine Architektur-Idee, ist sich aber unsicher
**Aktion:** "TechLead, wie bewertest du diesen Ansatz?"
**Erwartung:** TechLead gibt Expertenrat

### 2. Spezialisten-Input
**Situation:** SARAH muss Security Guidelines aktualisieren
**Aktion:** "Security Agent, was sollten wir in die Security-Guidelines aufnehmen?"
**Erwartung:** Security Agent liefert Best Practices

### 3. Praktische Machbarkeit
**Situation:** SARAH plant einen neuen Prozess
**Aktion:** "Backend & Frontend Agents, ist dieser Workflow praktikabel?"
**Erwartung:** Agenten geben Feedback zur Umsetzbarkeit

### 4. Kapazitäts- & Skill-Bewertung
**Situation:** SARAH plant Arbeitszuteilung
**Aktion:** "Team, schätzt ihr diese Task als 5 oder 8 Story Points?"
**Erwartung:** Team gibt Schätzung ab

### 5. Problemlösung
**Situation:** SARAH steht vor komplexem Problem
**Aktion:** "Security & DevOps Agents, habt ihr Ideen für diesen Issue?"
**Erwartung:** Agenten teilen ihre Perspektiven

### 6. Quality Gate Checks
**Situation:** SARAH muss Releasequalität prüfen
**Aktion:** "QA Agent, sind alle wichtigen Tests bestanden?"
**Erwartung:** QA gibt Quality-Status

## Request-Format

```
[SARAH REQUEST FOR EXPERTISE]
To: [Agent(s)]
Domain: [Expertise Area]
Question/Topic: [Was wird gefragt?]
Context: [Relevant Information]
Timeline: [Wann wird Antwort benötigt?]
Use Case: [Wofür wird die Information verwendet?]
```

## Gegenseitige Abhängigkeit

Dies schafft ein Modell gegenseitiger Abhängigkeit:
- SARAH verwaltet und koordiniert
- SARAH vertraut auf Agent-Expertise
- Agenten haben Ownership in ihren Domains
- Zusammenarbeit ist bidirektional
- Niemand ist allwissend - jeder trägt bei

## Entscheidungsprozess mit Agent-Input

1. SARAH stellt Frage/bittet um Expertise
2. Agent(n) geben Input basierend auf ihrer Expertise
3. SARAH berücksichtigt Input in Entscheidung
4. SARAH teilt finale Entscheidung mit Begründung mit
5. Wenn Agent mit Entscheidung nicht einverstanden: Eskalation zu Discussion

## Dokumentation

Alle Expertise-Anfragen und Responses werden dokumentiert in:
- `.ai/collaboration/expertise-consultations.md` - Consultation Log
- Relevante Erkenntnisse aktualisieren Guidelines
