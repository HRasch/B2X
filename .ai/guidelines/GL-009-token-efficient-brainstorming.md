# 🧠 Token-Efficient Brainstorming Strategy

**DocID**: `GL-009`  
**Status**: ✅ Active  
**Owner**: @SARAH  
**Created**: 3. Januar 2026

---

## Problem

Hoher Token-Verbrauch bei Brainstorming führt zu:
- Rate-Limits bei kostenlosen Modellen
- Langsame Antwortzeiten
- Ineffiziente Kommunikation

| Ineffizient | Warum teuer |
|---|---|
| Offene Fragen ("Was denkst du?") | Lange, ausschweifende Antworten |
| Kontext-Wiederholungen | Jede Nachricht wiederholt Infos |
| Prosa-Antworten | 500+ Wörter statt 50 |
| Explorative Diskussion | Viele Hin-und-Her Nachrichten |

---

## Lösung: Structured Brainstorming Protocol

### 1. Numbered-Options Format

```
❌ INEFFIZIENT:
"Wie sollen wir das API designen?"
→ 500 Wörter Antwort

✅ EFFIZIENT:
"API Design - gib mir 3 Optionen als Bullets mit je 1 Satz Pro/Con"
→ 60 Wörter Antwort
```

### 2. Constraint-First Prompting

```
❌ INEFFIZIENT:
"Erkläre mir die Architektur-Optionen"

✅ EFFIZIENT:
"Architektur-Optionen: max 5 Bullets, je max 10 Wörter"
```

### 3. Decision-Tree statt Diskussion

```
❌ INEFFIZIENT:
Offenes Brainstorming (10+ Messages)

✅ EFFIZIENT - Binary Questions:
"REST oder GraphQL?" → "REST"
"Sync oder Async?" → "Async"
"Cache ja/nein?" → "Ja, Redis"
→ 3 Messages statt 10
```

### 4. Template-basierte Antworten

Fordere dieses Format für alle Brainstorming-Antworten:

```markdown
## [Topic]
**Empfehlung**: [1 Satz]
**Optionen**: 
1. [Option] - [Pro] / [Con]
2. [Option] - [Pro] / [Con]
3. [Option] - [Pro] / [Con]
**Next**: [Entscheidung oder Frage]
```

---

## Praktische Prompt-Templates

### Feature-Brainstorming

```
Feature: [Name]
Gib mir: 3 Implementierungs-Optionen
Format: Numbered list, je 2 Sätze max
Empfehlung: Markiere beste Option mit ⭐
```

### Architektur-Entscheidungen

```
Entscheidung: [Topic]
Constraint: Max 100 Wörter Antwort
Format: 
- Option A: [1 Satz]
- Option B: [1 Satz]  
- Empfehlung: [A/B + 1 Satz warum]
```

### Problem-Analyse

```
Problem: [Beschreibung]
Analysiere in diesem Format:
- Ursache: [1 Satz]
- Lösung: [1 Satz]
- Aufwand: [S/M/L]
```

### Quick Decision

```
Frage: [Ja/Nein Frage]
Antwort: [Ja/Nein] + 1 Satz Begründung
```

---

## Token-Spar-Regeln

| Regel | Ersparnis | Beispiel |
|---|---|---|
| "Max X Wörter" im Prompt | ~60% | "Max 50 words" |
| Bullet-Format fordern | ~40% | "Bullets only" |
| Binary Questions nutzen | ~70% | "A oder B?" |
| Template vorgeben | ~50% | "Format: ..." |
| Erklärungen skippen | ~30% | "No explanation" |

---

## Quick-Reference Phrases

Füge diese Phrases zu deinen Prompts hinzu:

```
# Effizienz-Booster

"Bullets only, no prose"
"Max 50 words"
"3 options, 1 sentence each"
"Yes/No + 1 reason"
"Table format preferred"
"Skip explanation, just answer"
"Empfehlung mit ⭐ markieren"
"No context repeat needed"
"Binary: A or B?"
```

---

## Beispiel-Session

### Ineffiziente Session (vor GL-009)

```
User: "Wie soll die Authentifizierung funktionieren?"
AI: [300 Wörter über verschiedene Auth-Methoden...]

User: "Und was ist mit OAuth?"
AI: [200 Wörter über OAuth Details...]

User: "Welche Libraries gibt es?"
AI: [250 Wörter über Libraries...]

→ 750+ Wörter, 3 Nachrichten, wenig konkret
```

### Effiziente Session (mit GL-009)

```
User: "Auth-Methode: JWT vs OAuth vs Session? 
       3 Bullets, je Pro/Con, Empfehlung mit ⭐"

AI: 
- JWT: Stateless, skalierbar / Token-Größe
- OAuth: Standard, 3rd-party / Komplexität  
- Session: Einfach / Server-State ⭐ JWT für API

User: "JWT Library: 3 Optionen, 1 Satz je"

AI:
1. System.IdentityModel - Built-in, keine Deps
2. IdentityServer - Full-featured, komplex
3. JWT.io - Lightweight ⭐ System.IdentityModel

→ 80 Wörter, 2 Nachrichten, klare Entscheidung
```

---

## Integration mit anderen Guidelines

- **[GL-006]** Token Optimization: Allgemeine Token-Regeln
- **[GL-007]** Rate Limits: API-Limit Vermeidung
- **[GL-008]** Agent Consolidation: Weniger Agenten = weniger Tokens

---

## Checkliste vor Brainstorming

- [ ] Constraint definiert? (Max Wörter/Bullets)
- [ ] Format vorgegeben? (Table/Bullets/Template)
- [ ] Binary Question möglich?
- [ ] Empfehlung angefordert?
- [ ] "No explanation" wenn nicht nötig?

---

**Agents**: @SARAH | **Owner**: @SARAH
