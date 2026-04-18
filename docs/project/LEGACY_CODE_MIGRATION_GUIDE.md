# Legacy Code Migration Guide

## Übersicht

Dieser Leitfaden beschreibt den schrittweisen Prozess zur Migration von Legacy-Code auf moderne Standards.

## Phase 1: Automatisierte Bereinigung ✅

### Ziele:
- Automatische Formatierung anwenden
- Einfache ESLint-Regeln beheben
- Codebasis analysieren

### Schritte:
```bash
# 1. Legacy-Code analysieren
npm run legacy-cleanup

# 2. Automatische Fixes anwenden
npm run format

# 3. ESLint Auto-Fixes ausführen
cd frontend/Store && npm run lint
cd ../Admin && npm run lint
cd ../Management && npm run lint
```

### Erwartete Ergebnisse:
- ✅ Konsistente Formatierung
- ✅ Grundlegende Linting-Fehler behoben
- ✅ Analyse-Report generiert

## Phase 2: Manuelle Code-Reviews

### Priorisierte Bereiche:
1. **Sicherheitskritische Dateien** (Auth, API-Calls)
2. **Häufig geänderte Dateien** (Components, Services)
3. **Legacy-Imports** (Vue 2 Patterns, alte TypeScript-Syntax)

### Checkliste pro Datei:
- [ ] TypeScript-Strict-Mode aktiviert
- [ ] `any` Types durch spezifische Types ersetzt
- [ ] Console-Statements entfernt (außer in Tests)
- [ ] Vue 3 Composition API verwendet
- [ ] Proper Error Handling implementiert

### Zeitplan:
- **Sprint 1**: 20% der kritischen Dateien
- **Sprint 2**: 40% der häufig geänderten Dateien
- **Sprint 3**: 60% der verbleibenden Dateien
- **Sprint 4**: 80% der Legacy-Imports
- **Sprint 5**: 100% Migration abgeschlossen

## Phase 3: Strenge Regeln aktivieren

### Nach erfolgreicher Migration:
1. Legacy-Exceptions entfernen (`.editorconfig.legacy`)
2. Style-Regeln von `suggestion` auf `warning` erhöhen
3. CI/CD-Strenge erhöhen
4. Code-Reviews verschärfen

### Monitoring:
- Code-Coverage > 80%
- Zero ESLint Errors
- Zero TypeScript Errors
- Performance Benchmarks erfüllt

## Legacy-Code-Bereiche identifizieren

### Automatische Erkennung:
```bash
# Finde Dateien mit Legacy-Patterns
npm run legacy-cleanup

# Finde Dateien mit 'any' Types
grep -r ":\s*any" frontend/ --include="*.ts" --include="*.vue"

# Finde Console-Statements
grep -r "console\." frontend/src/ --include="*.ts" --include="*.vue"
```

### Manuelle Priorisierung:
1. **Hohe Priorität**: Auth, Security, API-Layer
2. **Mittlere Priorität**: UI Components, Business Logic
3. **Niedrige Priorität**: Tests, Documentation, Legacy Features

## Migration-Tools

### Automatische Tools:
- **Prettier**: Code-Formatierung
- **ESLint**: Linting und Auto-Fixes
- **Vue Migration Tool**: Vue 2 → Vue 3
- **TypeScript Compiler**: Strict Mode Checks

### Manuelle Hilfsmittel:
- **Code Search**: Legacy-Patterns finden
- **Refactoring Tools**: VS Code Refactorings
- **Pair Programming**: Wissens-Transfer

## Risiko-Management

### Rollback-Plan:
- Git Branches für jede Phase
- Feature Flags für neue Implementierungen
- Staging Environment für Tests

### Quality Gates:
- ✅ Unit Tests passieren
- ✅ Integration Tests passieren
- ✅ E2E Tests passieren
- ✅ Performance nicht verschlechtert
- ✅ Security Scans passieren

## Erfolgs-Metriken

### Quantitativ:
- Anzahl Dateien migriert: X/Y
- Code-Coverage: X%
- Build-Zeit: X Minuten
- ESLint Errors: X

### Qualitativ:
- Developer Satisfaction (Umfrage)
- Code Review Time reduziert
- Bug Rate gesunken
- Time-to-Market verbessert

## Support & Schulung

### Team-Schulung:
- Weekly Tech Talks zu neuen Patterns
- Code Examples und Best Practices
- Pair Programming Sessions

### Dokumentation:
- Migration Cookbook
- FAQ für häufige Probleme
- Code Examples für neue Patterns

## Zeitplan & Meilensteine

| Phase | Dauer | Meilenstein | Verantwortlich |
|-------|-------|-------------|----------------|
| Phase 1 | 1 Woche | Automatisierte Bereinigung | DevOps/TechLead |
| Phase 2 | 4 Wochen | Manuelle Reviews | Alle Teams |
| Phase 3 | 1 Woche | Strenge Regeln | TechLead |
| Follow-up | Laufend | Monitoring | Alle Teams |

## Notfall-Plan

Falls Migration zu komplex:
1. Legacy-Bereiche isolieren
2. Strengere Regeln nur für neue Code
3. Graduelle Migration über Monate
4. Third-Party Hilfe in Betracht ziehen

---

## Schnellstart für einzelne Dateien

```bash
# 1. Datei analysieren
npx eslint path/to/file.ts

# 2. Auto-Fixes anwenden
npx eslint path/to/file.ts --fix

# 3. TypeScript prüfen
npx vue-tsc --noEmit path/to/file.vue

# 4. Formatieren
npx prettier --write path/to/file.ts
```

---

*Dieser Leitfaden wird während der Migration aktualisiert.*