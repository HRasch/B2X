# Agent Permissions Directory

Dieses Verzeichnis enthält Permission-Definitionen für Agenten im Projekt, verwaltet von SARAH.

## Struktur

- `AGENT_MANAGEMENT.md` - Zentrale Permission-Richtlinien
- `agent-permissions.yml` - Maschinenlesbare Permission-Definitionen
- `audit-log.md` - Audit-Trail für Permission-Änderungen

## Permission-Typen

### Absolut (Nur SARAH)
- Agent erstellen/modifizieren/löschen
- Guidelines ändern
- Neue Permissions definieren

### Delegiert (Von SARAH gewährt)
- Aufgabenspezifische Autorität
- Eingeschränkte Änderungsrechte
- Temporäre oder dauerhafte Permissions

## Permission-Anfrage Prozess

1. Agent/Team stellt Anfrage an SARAH
2. SARAH evaluiert Anfrage und Auswirkungen
3. SARAH dokumentiert Decision in audit-log.md
4. SARAH gewährt Permission mit Bedingungen oder lehnt ab
5. Permission wird in agent-permissions.yml dokumentiert

## Beispiele delegierter Permissions

- Backend-Engineer: Kann Coding-Standards für Backend aktualisieren
- DevOps-Engineer: Kann Deployment-Guidelines ändern
- QA-Specialist: Kann Testing-Guidelines pflegen
- TechLead: Kann Architecture-Guidelines erweitern
