# Templates

Standardisierte Vorlagen für GitHub Issues, PRs, etc.

## Verfügbare Templates

### GitHub Issues

| Template | Datei | Zweck |
|----------|-------|-------|
| **Allgemein** | [github-issue.md](README.md) | Allgemeine Issue-Vorlage mit allen Agent-Perspektiven |
| **🐛 Bug** | [github-bug.md](README.md) | Bug Reports - Fehler-Analyse & Reproduktion |
| **✨ Feature** | [github-feature-request.md](README.md) | Feature Requests - Neue Funktionen |
| **🔄 Change** | [github-change-request.md](README.md) | Change Requests - Änderungen & Anpassungen |

### Pull Requests

| Template | Datei | Zweck |
|----------|-------|-------|
| **PR** | [github-pr.md](README.md) | PR-Vorlage mit Review-Checkliste |

## Verwendung

### GitHub Integration
Platziere diese Templates in `.github/ISSUE_TEMPLATE/` oder `.github/PULL_REQUEST_TEMPLATE/` damit GitHub sie anbietet:

```bash
mkdir -p .github/ISSUE_TEMPLATE
cp .ai/templates/github-bug.md .github/ISSUE_TEMPLATE/bug.md
cp .ai/templates/github-feature-request.md .github/ISSUE_TEMPLATE/feature.md
cp .ai/templates/github-change-request.md .github/ISSUE_TEMPLATE/change.md
cp .ai/templates/github-issue.md .github/ISSUE_TEMPLATE/issue.md

mkdir -p .github/PULL_REQUEST_TEMPLATE
cp .ai/templates/github-pr.md .github/PULL_REQUEST_TEMPLATE/pull_request.md
```

### Manuell
Beim Erstellen eines Issues/PRs auf GitHub die entsprechende Vorlage kopieren.

### In VS Code
User können die Vorlagen als Snippets nutzen.

## Template-Wartung

- @ProductOwner & @TechLead: Templates regelmäßig aktualisieren
- @SARAH: Überprüft auf Konsistenz mit Guidelines
- Neue Agent-Perspektiven → Templates anpassen
