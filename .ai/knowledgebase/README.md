# Knowledge Base Index

Die Wissensdatenbank ermöglicht Assistenten, ihren Kontext bei Bedarf zu erweitern und auf zentrale Erkenntnisse zuzugreifen.

## Struktur

### [architecture/](architecture/)
- Systemarchitektur und Design-Dokumentation
- Component-Übersichten und Abhängigkeitsdiagramme
- Architektur-Entscheidungen (ADRs)
- Scalability und Performance-Considerations

### [patterns/](patterns/)
- Code-Patterns und Conventions
- Design-Patterns für häufige Probleme
- Anti-Patterns und Fallstricke
- Refactoring-Strategien

### [best-practices/](best-practices/)
- Team-Standards und Guidelines
- Code-Style und Formatierung
- Testing-Strategien
- Security und Performance Best Practices

### [tools-and-tech/](tools-and-tech/)
- Verwendete Technologien und Frameworks
- Tool-Setup und Konfiguration
- Libraries und Dependencies
- Integration mit externen Services
- **[Mermaid Documentation](./tools-and-tech/MERMAID_DOCUMENTATION.md)** - VS Code & GitHub diagram support

### [domain-knowledge/](domain-knowledge/)
- Business-Logik und Geschäfts-Kontexte
- Fachliche Anforderungen
- Workflow-Beschreibungen
- Häufig gestellte Fragen zum Projekt

### [software/](software/) — NEW
- 📦 Dependency Upgrade Documentation
- Version Changelogs & Migration Guides
- Breaking Changes & Migration Paths
- Security Fixes & CVE Information
- Performance Improvements Tracking

**See [INDEX.md](INDEX.md) for central overview and tag-based search.**

## Verwendung durch Assistenten

Assistenten verwenden diese Wissensdatenbank, um:

1. **Kontext zu erweitern**: Bei Fragen zu Architektur, Patterns oder Best Practices
2. **Konsistenz zu wahren**: Einhaltung von etablierten Standards
3. **Schneller zu werden**: Direkter Zugriff auf bewährte Lösungen
4. **Bessere Entscheidungen zu treffen**: Auf Basis dokumentierter Erfahrungen
5. **Dependencies aktuell zu halten**: Versions- und Migration Information zentral verfügbar

## Workflow: Neue Dependency-Version Dokumentieren

```
1. User: @Architect "Recherchiere {Software} von v{old} zu v{new}"
2. Architect researches & creates: software/{name}/{old}--to--{new}.md
3. Architect: Updates INDEX.md with version info & tags
4. SARAH: Notifies all agents with .ai/knowledgebase link
```

**Workflow-Details:** Siehe [Workflow: Dependency Upgrade](../workflows/dependency-upgrade.workflow.md)

**Prompt:** Siehe [dependency-upgrade-research.prompt.md](.../../github/prompts/dependency-upgrade-research.prompt.md)

## Wartung

- Wissensdatenbank sollte regelmäßig aktualisiert werden
- Neue Erkenntnisse aus Projekte sollten hier dokumentiert werden
- Veraltete Informationen sollten gepflegt oder entfernt werden
- Dependency-Dokumentation sollte nach Updates erstellt werden

