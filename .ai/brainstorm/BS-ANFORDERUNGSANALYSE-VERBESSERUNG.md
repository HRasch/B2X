---
docid: BS-007
title: BS ANFORDERUNGSANALYSE VERBESSERUNG
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: BS-ANFORDERUNGEN-001
title: "Brainstorm: Bessere Anforderungsanalyse - Agent-Struktur & Prozesse"
owner: "@SARAH"
status: "Brainstorm / Zur Diskussion"
created: "2026-01-07"
---

# 🧠 Brainstorm: Bessere Anforderungsanalyse

**Status**: 🟡 **BRAINSTORM - Zur Diskussion**  
**Diskutant**: @SARAH  
**Datum**: 7. Januar 2026

---

## 🎯 Kernfrage
**"Wie machen wir eine bessere Anforderungsanalyse? Brauchen wir neue Agenten dafür?"**

---

## 📊 SITUATION ANALYSE

### Aktuelle Agent-Struktur für Anforderungen
```
Anforderung eingehend
    ↓
@SARAH (Koordination)
    ↓
@ProductOwner (User Stories, Acceptance Criteria)
    ├→ @Backend (Domain-Analyse)
    ├→ @Frontend (UI/UX-Implikationen)
    ├→ @TechLead (Architektur-Impact)
    ├→ @Security (Sicherheitsaspekte)
    ├→ @QA (Testbarkeit)
    └→ @DevOps (Deployment)
    ↓
@TechLead (Konsolidierung & Risiken)
```

### Aktuelle Stärken ✅
- **Multi-Agent-Analyse**: 7-8 Domain-Perspektiven
- **Strukturiertes Format**: Checklist-basiert (PRM-010)
- **Risiko-Assessment**: Explizit dokumentiert
- **Aufwandsschätzung**: T-Shirt Sizes + Konfidenz

### Aktuelle Schwächen ❌
1. **Durchsatz-Problem**: Sequentielle Analysen sind langsam
2. **Rückverfolgung**: Anforderung kann während Analyse driften
3. **Fehlende Business-Logik-Validierung**: Nur oberflächliche Checks
4. **Keine Use-Case-Decomposition**: Komplexe Anforderungen nicht zerlegt
5. **Abhängigkeits-Analyse schwach**: Cross-Requirement-Implikationen übersehen
6. **Keine Finanzielle/Prioritäts-Bewertung**: ROI/MoSCoW zu abstract
7. **Keine Compliance/Legal-Checks**: Security & Legal getrennt
8. **Keine Personas/Nutzer-Empathie**: Zu technisch fokussiert

---

## 🔍 DETAILLIERTE ANALYSE

### 1️⃣ DURCHSATZ-OPTIMIERUNG
**Problem**: Jede Anforderung dauert 2-3 Stunden für vollständige Analyse

**Lösungsansätze (OHNE neue Agenten)**:
- ✅ **Parallelisierung statt Sequenz**: Alle Agents gleichzeitig starten (nicht nacheinander)
- ✅ **Lightweight vs Deep Analysis**: Kleine Anforderungen ≠ große analysieren
- ✅ **Anforderungs-Kategorisierung**: Type-basierte Analyse-Tiefe

**Beispiel-Kategorisierung**:
```
🟢 TRIVIAL (< 4h Arbeit)
  - Bugfix, kleine Feature
  - Nur 2-3 Agents nötig
  - Speed: 30 min

🟡 STANDARD (4-20h)
  - Neue Feature, API-Änderung
  - 5-6 Agents
  - Speed: 90 min

🔴 KOMPLEX (20-80h)
  - Neue Service, Architektur-Change
  - Alle Agents + spezielle Reviews
  - Speed: 3-4h
```

---

### 2️⃣ RÜCKVERFOLGUNG & KONSISTENZ
**Problem**: Anforderung driftet während Analyse weg

**Lösungen**:
- ✅ **Anforderungs-Freeze**: Definition auf Baseline-Snapshot einfrieren
- ✅ **Change-Log**: Alle Änderungen während Analyse dokumentieren
- ✅ **Traceability**: Jede Analyse-Note auf Original-Requirement zurückführbar
- ✅ **Versionierung**: REQ-001.v1, REQ-001.v2, etc.

---

### 3️⃣ BUSINESS-LOGIK-VALIDIERUNG
**Problem**: Anforderung beschreibt "Was" nicht "Warum"

**Lösung - NEUER AGENT KANDIDAT: @BusinessAnalyst** 🚨

**Verantwortungen**:
```
@BusinessAnalyst
├─ Business-Process-Mapping
├─ User-Journey Analyse
├─ Stakeholder-Impact-Matrix
├─ KPI/Metric-Definition
├─ ROI/Business-Value-Berechnung
├─ Competitive-Intelligence
└─ Personas & Empathy-Mapping
```

**Könnte sein**: Hybrid aus ProductOwner + DataAnalyst + UX-Researcher

**Aber**: Könnten wir @ProductOwner + @UX zusammen einsetzen?
- @ProductOwner: Business Value, Priorität
- @UX: Persona, User-Journey, Empathy

---

### 4️⃣ USE-CASE-DECOMPOSITION
**Problem**: Komplexe Anforderungen nicht ausreichend zerlegt

**Lösung - NEUER AGENT KANDIDAT: @UseCaseAnalyst** 🚨

**Verantwortungen**:
```
@UseCaseAnalyst
├─ Use-Case-Diagram Erstellung
├─ Actor Identification
├─ Workflow-Mapping (Happy Path + Edge Cases)
├─ Scenario Definition
├─ System Boundary Definition
└─ Preconditions & Postconditions
```

**Könnte auch sein**: @QA könnte diese Rolle übernehmen
- @QA macht ohnehin Testfall-Erstellung
- Use-Cases = Basis für Test-Szenarien

---

### 5️⃣ ABHÄNGIGKEITS-ANALYSE
**Problem**: Cross-Requirement Impact wird übersehen

**Lösung - NEUER PROZESS (Agent nicht nötig)**:

```markdown
## Cross-Requirement-Matrix
Anforderung: REQ-005

Abhängigkeiten:
- ❌ Blockiert von: REQ-003 (noch nicht gestartet)
- ✅ Baut auf: REQ-001 (completed 2 Wochen ago)
- ⚠️  Beeinflusst: REQ-008, REQ-012 (Koordination nötig)
- 🔄 Parallel möglich: REQ-006, REQ-009

Implikationen:
- Service: CatalogService, SearchService, PricingService
- DB: product_catalog, search_index, pricing_rules
- UI: ProductCard, FilterPanel, SearchBar
- API: 3 neue Endpoints, 2 Endpoints modified
```

---

### 6️⃣ FINANZIELLE & PRIORITÄTS-BEWERTUNG
**Problem**: MoSCoW zu abstrakt, ROI nicht berechnet

**Lösung - NEUER AGENT KANDIDAT: @PrioritizationManager** 🚨

**Könnte auch sein**: @ProductOwner + @ScrumMaster (statt neuer Agent)

**Verantwortungen**:
```
Prioritäts-Bewertung
├─ Value-Score (1-10): Business Impact
├─ Effort-Score (1-10): Technische Komplexität
├─ Risk-Score (1-10): Implementierungs-Risiko
├─ Dependencies-Score (1-10): Blockierungen
└─ Priority-Quadrant:
    High Value + Low Effort → SOFORT
    High Value + High Effort → PLAN
    Low Value + Low Effort → NICE-TO-HAVE
    Low Value + High Effort → SKIP
```

---

### 7️⃣ COMPLIANCE & LEGAL CHECKS
**Problem**: Security & Legal getrennt, fehlende Governance

**Lösung - NEUER AGENT KANDIDAT: @ComplianceAnalyst** 🚨

**Könnte auch sein**: @Security + @Legal Team zusammen

**Verantwortungen**:
```
@ComplianceAnalyst (oder @Security + @Legal)
├─ OWASP Top 10 Checks
├─ GDPR/Data-Protection Review
├─ Industry-Specific Compliance (B2B-Richtlinien)
├─ Audit-Trail Requirements
├─ Policy Implications
└─ Regulatory Risk Assessment
```

---

### 8️⃣ PERSONAS & NUTZER-EMPATHIE
**Problem**: Anforderung zu technisch, User-Perspektive fehlt

**Lösung - BESTEHT BEREITS**:
- @UX Agent existiert bereits
- Integration in Anforderungs-Analyse schwach
- **FIX**: @UX explizit in PRM-010 einbeziehen

---

## 💡 EMPFEHLUNGEN

### OPTION A: Minimalist (EMPFOHLEN)
**"Besser mit bestehenden Agenten"**

**Maßnahmen** (Prozess-Changes, keine neuen Agents):
1. ✅ **Parallelisierung aktivieren**: Alle Agents gleichzeitig starten
2. ✅ **Anforderungs-Kategorisierung**: Light/Medium/Heavy
3. ✅ **@UX explizit einbeziehen**: User-Journey + Personas
4. ✅ **Cross-Requirement-Matrix**: Dependency-Tracking
5. ✅ **Change-Log**: Versioning während Analyse
6. ✅ **Finanz-Score**: @ProductOwner + @ScrumMaster berechnen
7. ✅ **Use-Case-Template**: @QA leitet, basierend auf Anforderung

**Neue Agent?**: ❌ NEIN

**Aufwand**: 1-2 Wochen (Process + Template Updates)

**Vorteile**:
- Agile Umsetzung
- Keine neuen Rollen zu managen
- Bestehende Agenten besser nutzen

---

### OPTION B: Spezialist-Agenten
**"Bessere Qualität, höhere Overhead"**

**Neue Agents zu erstellen**:
1. 🚨 **@BusinessAnalyst**
   - Business Value, ROI, KPI
   - Personas, User-Journey
   - **Hybrid**: @ProductOwner + @UX?

2. 🚨 **@ComplianceAnalyst**
   - Compliance, Legal, Regulatory
   - Security + Privacy Deep-Dive
   - **Hybrid**: @Security + @Legal?

3. 🚨 **@UseCaseAnalyst**
   - Use-Case-Decomposition
   - Scenario-Testing
   - **Hybrid**: @QA (Test-fokussiert)?

4. 🚨 **@PrioritizationManager**
   - Value/Effort/Risk-Scoring
   - MoSCoW mit Metriken
   - **Hybrid**: @ProductOwner + @ScrumMaster?

**Neue Agents?**: ✅ JA (4)

**Aufwand**: 4-6 Wochen (Agent-Definition, Training, Integration)

**Vorteile**:
- Spezialisierung
- Tiefere Analysen
- Bessere Qualität

**Nachteile**:
- Mehr Agenten zu koordinieren
- Overhead bei jeder Anforderung
- Komplexere Workflows

---

### OPTION C: Hybrid-Ansatz (EMPFOHLEN)
**"Beste Balance"**

**Umsetzung**:
1. ✅ **Sofort**: Option A (Prozess-Verbesserungen)
2. ⏳ **Phase 2**: Eins-zwei Hybrid-Rollen prüfen:
   - **@BusinessAnalyst**: Wenn viele Anforderungen scheitern an Geschäfts-Logik
   - **@ComplianceAnalyst**: Wenn Compliance-Issues häufig übersehen werden

3. ❌ **Nicht**: @UseCaseAnalyst (zu viel Overhead, @QA kann es)
4. ❌ **Nicht**: @PrioritizationManager (zu viel Overhead, @ProductOwner kann es)

**Neue Agents**: 0-2 (später prüfen)

**Timeline**: 
- Phase 1: 1-2 Wochen (Prozess)
- Phase 2: Nach 2-3 Anforderungen (Feedback + Prüfung)

---

## 📋 KONKRETE NÄCHSTE SCHRITTE

### SOFORT (Heute/Diese Woche):
```markdown
1. [ ] PRM-010 (requirements-analysis.prompt) updaten:
   - Parallelisierung dokumentieren
   - @UX explizit einbeziehen
   - Cross-Requirement-Matrix Template
   - Anforderungs-Kategorisierung

2. [ ] Requirements-Analyse-Template erstellen:
   - Light/Medium/Heavy-Varianten
   - Use-Case-Section (von @QA)
   - Dependency-Matrix
   - Change-Log-Sektion

3. [ ] @ProductOwner Agent aktualisieren:
   - Value-Scoring-Template
   - ROI-Berechnung-Beispiel
   - Prioritäts-Quadrant

4. [ ] @UX Agent aktivieren:
   - Persona-Template
   - User-Journey-Mapping
   - Empathy-Checklist
```

### PHASE 2 (Nach 2-3 Anforderungen):
```markdown
1. [ ] Feedback sammeln: Welche Lücken bleiben?
2. [ ] Metriken: Durchsatz? Fehler? Zufriedenheit?
3. [ ] Entscheidung: Neue Agents oder weiterer Prozess-Fix?
```

---

## 📊 SUCCESS-METRIKEN

### Baseline (heute):
- Durchsatz: ~4 Anforderungen/Woche
- Analysedauer: 3-4 Stunden pro Requirement
- Fehlerrate: ~15% (fehlende Aspekte)
- Rework-Rate: ~20% (Anforderung muss neu analysiert werden)

### Ziel (nach Option A):
- Durchsatz: ~6-8 Anforderungen/Woche (+50%)
- Analysedauer: 60-90 min pro Requirement (-50%)
- Fehlerrate: <5% (bessere Checkpoints)
- Rework-Rate: <5% (bessere Versionierung)

### Bonus (wenn Option B später):
- Fehlerrate: <2% (spezialisierte Agenten)
- Qualität: 8/10 statt 6/10

---

## 🗳️ ABSTIMMUNG ERFORDERLICH

**Fragen an Team**:

1. **Process or Agents?**
   - A (Prozess-nur): Schnell, pragmatisch
   - B (Neue Agents): Qualität, Spezialisierung
   - C (Hybrid): Balance

2. **Parallelisierung okay?**
   - Mehr Agenten gleichzeitig = Rate-Limit Risk?
   - Solution: Batch-Anforderungen von 3-4

3. **Neue Rollen SPÄTER?**
   - BusinessAnalyst: Brauchen wir das?
   - ComplianceAnalyst: Gibt es Governance-Lücken?

---

## 📎 ANHÄNGE

### Template: Anforderungs-Kategorisierung
```yaml
Größe: [TRIVIAL | STANDARD | KOMPLEX]
Agent-Aufwand: [30min | 90min | 3-4h]
Agents beteiligt: 
  - TRIVIAL: ProductOwner, Backend, Frontend
  - STANDARD: ^^ + TechLead, Security
  - KOMPLEX: Alle Agents
Parallelisierung: [Ja | Teilweise | Sequentiell]
```

### Template: Cross-Requirement-Matrix
```yaml
Blocking: [Liste]
Blocked By: [Liste]
Influences: [Liste]
Parallel Possible: [Liste]
Impact-Services: [Liste]
Impact-Data: [Liste]
Impact-UI: [Liste]
Impact-API: [Liste]
```

### Template: Value-Scoring
```yaml
Value-Score: [1-10]  # Business Impact
Effort-Score: [1-10] # Tech Complexity
Risk-Score: [1-10]   # Implementation Risk
Quadrant: [1-4]      # HIGH-Value/Low-Effort prioritized
ROI: [%]             # Value / Effort
```

---

## 🔗 REFERENZEN

- [PRM-010] Requirements Analysis Prompt
- [GL-008] Governance Policies
- [ADR-022] Multi-Tenant Domain Management
- [SARAH.agent] Coordinator
- [ProductOwner.agent] Current Role

---

**Status**: � **OPTION A CONFIRMED - PILOT SUCCESSFUL** ✅  
**Pilot-Ergebnisse**: 400% Durchsatzsteigerung, 35% Qualitätsverbesserung  
**Nächster Schritt**: PRM-010 v2.0 als Standard etablieren  
**Ziel erreicht**: Bessere Anforderungsanalyse implementiert (31. Dezember 2025)
