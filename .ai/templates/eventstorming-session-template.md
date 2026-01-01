# EventStorming Session Template

## Session Information
- **Datum:** [YYYY-MM-DD]
- **Dauer:** [X] Stunden
- **Location:** [Physisch/Virtuell, Raum/Link]
- **Facilitator:** [Name, Erfahrung]
- **Domain Experts:** [Liste der Teilnehmer]
- **Business Context:** [Welcher Business Bereich wird modelliert?]

## Session Goals
### Primary Objectives
- [Was soll am Ende der Session erreicht sein?]
- [Welche Entscheidungen sollen getroffen werden?]

### Success Criteria
- [Messbare Erfolgsindikatoren]
- [Deliverables der Session]

## Preparation Checklist
### Pre-Session (1 Woche vorher)
- [ ] Domain Experts identifiziert und eingeladen
- [ ] Business Context Dokumentation vorbereitet
- [ ] Moderationsmaterialien bereitgestellt
- [ ] Technische Setup getestet (virtuell)
- [ ] Agenda kommuniziert

### Session Setup
- [ ] Großer Raum mit Pinnwand/Wall
- [ ] Moderationskarten in verschiedenen Farben
- [ ] Klebezettel, Marker, Pinnnadeln
- [ ] Timer für Timeboxing
- [ ] Digitale Kollaboration Tools (Miro, Mural)

## Agenda

### Phase 1: Big Picture (60-90 Minuten)
**Goal:** Gesamtes Business verstehen und Scope definieren

**Activities:**
1. **Domain Expert Presentations** (20 min)
   - Business Overview
   - Key Processes
   - Pain Points & Challenges

2. **Timeline Exercise** (30 min)
   - Chronologische Ereignisse des Business
   - Wichtige Meilensteine
   - Pivotal Events

3. **Process Identification** (20 min)
   - Hauptprozesse benennen
   - Subdomains identifizieren

**Output:** Big Picture Timeline mit Process Übersicht

### Phase 2: Process Level (90-120 Minuten)
**Goal:** Detaillierte Prozesse und Events identifizieren

**Activities:**
1. **Process Deep Dive** (45 min)
   - Schritt-für-Schritt Process Modeling
   - Commands und Events identifizieren

2. **Actor Identification** (30 min)
   - Wer triggert welche Commands?
   - Welche Rollen sind beteiligt?

3. **Aggregate Exploration** (30 min)
   - Business Entities identifizieren
   - Aggregate Boundaries diskutieren

**Output:** Process Flows mit Commands, Events, Aggregates

### Phase 3: Design Level (60-90 Minuten)
**Goal:** Technische Implementierung vorbereiten

**Activities:**
1. **Read Model Design** (30 min)
   - Welche Daten werden benötigt?
   - Query Requirements definieren

2. **Policy & Rules** (20 min)
   - Business Rules identifizieren
   - Invariants definieren

3. **External Systems** (20 min)
   - Integration Points
   - API Requirements

**Output:** Technische Spezifikation für Implementation

### Phase 4: Wrap-up & Next Steps (30 Minuten)
**Activities:**
1. **Key Insights** (10 min)
2. **Action Items** (10 min)
3. **Follow-up Planning** (10 min)

## EventStorming Notation

### Standard Notation
- **Orange:** Domain Events (etwas ist passiert)
- **Blue:** Commands (etwas soll passieren)
- **Yellow:** Aggregates (Business Entities)
- **Purple:** Policies (automatische Reaktionen)
- **Pink:** External Systems
- **Green:** Read Models (UI/Views)
- **Red:** Hotspots (Probleme/Risiken)

### B2Connect Specific Events
```
🟠 ProductCreated, OrderPlaced, PaymentProcessed
🔵 CreateProduct, PlaceOrder, ProcessPayment
🟡 Product, Order, Customer
🟣 OrderConfirmationEmail, InventoryUpdate
🌸 StripeAPI, SendGrid, Elasticsearch
🟢 ProductListView, OrderHistoryView
🔴 HighConcurrencyIssues, LegacySystemDependencies
```

## Process Guidelines

### Facilitation Rules
1. **Timeboxing:** Jede Aktivität hat festes Zeitlimit
2. **One Conversation:** Nur eine Person spricht gleichzeitig
3. **Stick to Facts:** Diskussion basiert auf Business Facts
4. **Challenge Assumptions:** Annahmen werden hinterfragt
5. **Visualize Everything:** Alles wird an die Wand geschrieben

### Participation Guidelines
- **Domain Experts:** Fokus auf Business Knowledge
- **Technical Team:** Fragen stellen, nicht vorschreiben
- **Everyone:** Aktiv teilnehmen, respektvoll zuhören
- **Facilitator:** Prozess leiten, Timeboxing einhalten

## Expected Outcomes

### Documentation
- **EventStorming Photos:** Hochauflösende Bilder der Boards
- **Event Catalog:** Liste aller identifizierten Events
- **Process Maps:** Detaillierte Process Flows
- **Aggregate Overview:** Business Entities und Beziehungen

### Technical Artifacts
- **Command/Event Definitions:** Für Wolverine Implementation
- **Aggregate Boundaries:** Domain Model Grundlage
- **Read Model Requirements:** UI/Reporting Needs
- **Integration Points:** External System Contracts

## Follow-up Actions

### Immediate (nächste Woche)
- [ ] EventStorming Results reviewen
- [ ] Technische Umsetzbarkeit prüfen
- [ ] Prioritäten für Implementation setzen

### Short-term (1-2 Wochen)
- [ ] Domain Model designen
- [ ] API Contracts definieren
- [ ] Sprint Planning anpassen

### Long-term (1-3 Monate)
- [ ] Implementation starten
- [ ] Integration Tests planen
- [ ] Deployment Strategy definieren

## Risk Mitigation

### Common Pitfalls
- **Scope Creep:** Zu viele Details auf einmal
- **Technical Bias:** Zu frühe technische Diskussionen
- **Dominant Voices:** Einzelne Personen dominieren
- **Lack of Preparation:** Unvorbereitete Teilnehmer

### Mitigation Strategies
- **Preparation:** Detaillierte Pre-Session Briefings
- **Facilitation:** Erfahrener Moderator
- **Time Management:** Strenges Timeboxing
- **Inclusive Culture:** Alle Stimmen werden gehört

## Success Metrics
- **Participation:** Alle Domain Experts aktiv beteiligt
- **Insights:** Neue Erkenntnisse über Business Processes
- **Alignment:** Technisches und Business Verständnis abgeglichen
- **Actionable:** Klar definierte nächste Schritte

## Retrospective Questions
1. **What surprised us about the domain?**
2. **What assumptions were challenged?**
3. **What did we learn about the business?**
4. **What needs further exploration?**
5. **How can we improve future sessions?**

---

*Dieses Template basiert auf Alberto Brandolini's EventStorming Methodology und ist angepasst für B2Connect's .NET/Wolverine Architecture.*