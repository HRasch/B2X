# Prompt Management Framework

## Überblick

SARAH verwaltet ein System von standardisierten, optimierten Prompts. Diese Prompts helfen Agenten:
- **Standardisiert** zu operieren (konsistent über alle Tasks)
- **Optimiert** zu arbeiten (maximale Token-Effizienz, beste Output-Quality)
- **Vorhersagbar** zu sein (konsistente Outputs für gleiche Inputs)
- **Schnell** zu werden (reduzierte Latency durch bessere Prompts)

## Prompt-Hierarchie

```
.github/prompts/
├── backend.system.md                # System-Prompts
├── frontend.system.md
├── devops.system.md
├── qa.system.md
├── security.system.md
├── techlead.system.md
├── code-review.md                   # Task-spezifische Prompts
├── architecture-design.md
├── testing.md
├── security-audit.md
├── backend-patterns.md              # Domain-spezifische Prompts
├── frontend-components.md
├── devops-infrastructure.md
├── token-efficient.md               # Kostenoptimierungs-Prompts
├── summarization.md
├── fast-analysis.md
├── REGISTRY.md                      # Zentrale Registry
└── README.md                        # Dokumentation
```

## Prompt-Typen & Zweck

### 1. System-Prompts (Agent-spezifisch)
**Zweck:** Grundsätzliche Instructions für einen Agent
**Beispiel:** Backend.prompt.md
```
You are a Backend Engineer AI Agent specialized in:
- RESTful API design
- Database optimization
- Business logic implementation

Guidelines:
- Always consider performance implications
- Document complex decisions
- Follow these API design patterns: [...]
```

**Nutzen:**
- Agent startet mit klarem Kontext
- Konsistentes Behavior über alle Tasks
- Keine repeated Context-Setting nötig

### 2. Task-Prompts
**Zweck:** Specific Prompts für häufige Task-Typen
**Beispiel:** code-review.prompt.md
```
You are performing a code review. Focus on:
- Does it follow coding standards?
- Are there security concerns?
- Is performance acceptable?
- Is it maintainable?

Use this structure for your review:
[...]
```

**Nutzen:**
- Standardisierte Code-Review Qualität
- Konsistent gefocusstes Feedback
- Agent kann prompt direkt verwenden

### 3. Domain-Prompts
**Zweck:** Domain-Expertise komprimiert in Prompts
**Beispiel:** backend-patterns.prompt.md
```
Common Backend Patterns:
1. Repository Pattern - [Description]
2. Service Layer - [Description]
3. Circuit Breaker - [Description]

When implementing backend features:
- Use these patterns as reference
- Adapt to specific requirements
- Document pattern selection
```

**Nutzen:**
- Agent hat schnellen Zugriff auf Best Practices
- Reduziert Recherche-Zeit
- Stellt sicher, dass etablierte Patterns verwendet werden

### 4. Optimization-Prompts
**Zweck:** Kostenoptimierung und Effizienz
**Beispiel:** token-efficient.prompt.md
```
Token-Efficient Prompting:
- Be concise, avoid redundancy
- Use structured output (JSON)
- Avoid verbose explanations
- Focus on essential information

For your response:
- Use max 500 tokens
- Prefer bullet points
- Remove all filler words
```

**Nutzen:**
- Reduziert API-Kosten
- Schnellere Responses
- Bessere Skalierung

## Prompt-Management Zyklus

### Phase 1: Prompt-Erstellung
```
SARAH identifiziert häufig auftretende Task-Typ oder Domain
SARAH erstellt optimierten Prompt
Dokumentiert Prompt-Rationale und Version
```

### Phase 2: Prompt-Testing
```
Agent testet Prompt mit verschiedenen Inputs
Feedback wird gesammelt
Iterationen basierend auf Feedback
```

### Phase 3: Prompt-Deployment
```
Prompt wird in `.github/prompts/` committed
Agent wird informiert über neuen Prompt
Prompt wird als "Primary" für diese Task markiert
```

### Phase 4: Prompt-Monitoring
```
SARAH misst Prompt-Performance:
- Output-Qualität
- Token-Effizienz
- Response-Zeit
- Agent-Satisfaction
```

### Phase 5: Prompt-Optimization
```
Nach gesammelten Daten optimiert SARAH Prompts:
- Bessere Structure
- Klarere Instructions
- Token-Reduktion
- Qualitäts-Verbesserung
```

## Prompt-Best-Practices (von SARAH durchgesetzt)

### ✅ DO: Strukturierte Prompts
```
Gut:
"As a Backend Engineer:
1. Design the API endpoint
2. Consider security
3. Optimize for performance
Return as JSON: {endpoint, security, performance}"

Schlecht:
"Design an API endpoint"
```

### ✅ DO: Rollen-Klarheit
```
Gut:
"You are a Security Specialist focused on: [specific focus]
Your task: [specific task]"

Schlecht:
"Review this code"
```

### ✅ DO: Output-Format definieren
```
Gut:
"Provide your review as:
- Finding 1: [description]
- Severity: [high/medium/low]
- Recommendation: [action]"

Schlecht:
"Tell me what's wrong"
```

### ✅ DO: Context-Effizienz
```
Gut:
"Key constraints: [list]
Success criteria: [list]"

Schlecht:
"[20 paragraphs of background]"
```

### ❌ DON'T: Redundante Instructions
```
Schlecht:
"Please, if you don't mind, could you possibly consider...
and maybe also think about..."

Gut:
"Focus on [specific focus]"
```

## Prompt-Versionierung

Prompts werden versioniert:
```
backend-api-design.prompt.v1.md
backend-api-design.prompt.v2.md  (improved structure)
backend-api-design.prompt.v3.md  (added security focus)
```

SARAH maintains:
- `current` - aktuelle empfohlene Version
- `archive` - alte Versionen für Rollback
- `changelog` - Was hat sich geändert?

## Prompt-Registry

SARAH maintains zentrale Registry (`.github/prompts/REGISTRY.md`):
```
| Prompt Name | Type | Agent | Version | Last Updated | Performance |
|---|---|---|---|---|---|
| backend-api-design | task | Backend | v3 | 2025-12-30 | ⭐⭐⭐⭐⭐ |
| code-review | task | All | v2 | 2025-12-28 | ⭐⭐⭐⭐ |
| security-audit | task | Security | v1 | 2025-12-15 | ⭐⭐⭐ |
```

## Prompt-Optimization Metriken

SARAH tracked:
- **Quality Score** - Wie gut ist Output?
- **Token Efficiency** - Tokens pro Task
- **Response Time** - Wie schnell?
- **Agent Satisfaction** - Mag Agent diesen Prompt?
- **Consistency** - Konsistenz über Inputs

## Anti-Patterns

⚠️ SARAH vermeidet:
- ❌ Zu lange Prompts (reduziert trotz mehr Context)
- ❌ Mehrdeutige Instructions
- ❌ Fehlende Output-Format Definition
- ❌ Veraltete Prompts (nicht aktualisieren)
- ❌ Agent-übergreifende Konfusion (falscher Prompt)

## Prompt-Sharing

Agenten können Prompts sharen und reuse:
```
Code Review Prompt → Backend NUTZT IT
                  → Frontend NUTZT IT
                  → QA NUTZT IT
                  
= Ein guter Prompt für alle
= Konsistente Qualität
= Kostenersparnis
```

## Integration mit Knowledge Base

Prompts sind mit Knowledgebase integriert:
```
Prompt kann reference auf `.ai/knowledgebase/patterns/`
Prompt kann reference auf `.ai/guidelines/`
Prompt kann reference auf `.ai/knowledgebase/best-practices/`

Beispiel:
"Review API design against patterns documented in
.ai/knowledgebase/patterns/api-patterns.md"
```
