# Port Management für B2Connect Aspire

## Überblick

Das System überprüft jetzt automatisch beim Start von Aspire, ob die erforderlichen Ports verfügbar sind. Falls Ports belegt sind, werden automatisch alternative Ports gesucht und verwendet.

## Automatische Port-Prüfung beim Aspire-Start

Das `aspire-start.sh` Skript führt jetzt automatisch folgende Prüfungen durch:

### Geprüfte Ports
- **AppHost Dashboard**: Port 5200 (Standard)
- **Aspire Dashboard**: Port 5500 (Standard)
- **CatalogService**: Port 9001 (Standard)
- **AuthService**: Port 9002 (Standard)
- **SearchService**: Port 9003 (Standard)
- **OrderService**: Port 9004 (Standard)

### Automatische Fallback-Strategie

Wenn ein Port bereits in Verwendung ist:
1. Das Skript erkennt den belegten Port
2. Es sucht nach dem nächsten freien Port
3. Weist diesen alternativen Port dem Service zu
4. Zeigt die neue Port-Zuordnung an

**Beispiel-Ausgabe:**
```
[!] AppHost Port 5200 is in use
[✓] Using alternative AppHost Port: 5201
[✓] CatalogService Port 9001 is available
```

## Port-Checker Tool

Ein neues Hilfsskript `check-ports.sh` steht zur Verfügung für Port-Management:

### Verwendung

```bash
# Standard: Prüfe Port-Verfügbarkeit
./check-ports.sh --check

# Frei besetzte Ports und starte neu
./check-ports.sh --free

# Überwache Ports in Echtzeit
./check-ports.sh --monitor

# Hilfe anzeigen
./check-ports.sh --help
```

### Kommandos im Detail

#### `--check` (Standard)
Zeigt den aktuellen Status aller Ports:
- ✓ Verfügbar (grün)
- ✗ Belegt (rot mit Prozess-Info)
- Zusammenfassung der Verfügbarkeit

**Beispiel:**
```bash
$ ./check-ports.sh --check
✓ AppHost (Port 5200): AVAILABLE
✗ CatalogService (Port 9001): IN USE
   Process: node (12345)
✓ AuthService (Port 9002): AVAILABLE
...
```

#### `--free`
Gibt belegte Ports frei:
1. Zeigt aktuellen Status
2. Sendet SIGTERM an Prozesse
3. Wartet kurz
4. Falls nötig, sendet SIGKILL
5. Zeigt Ergebnis

**Beispiel:**
```bash
$ ./check-ports.sh --free
[*] Attempting to free port 9001 (CatalogService)...
    Sending SIGTERM to PID 12345...
[✓] Port 9001 freed
```

#### `--monitor`
Zeigt Ports in Echtzeit an (aktualisiert alle 3 Sekunden):
```bash
$ ./check-ports.sh --monitor
✓ AppHost (Port 5200): AVAILABLE
✗ CatalogService (Port 9001): IN USE
   node (12345)
✓ AuthService (Port 9002): AVAILABLE
...
(Updated every 3 seconds - Press Ctrl+C to exit)
```

## Workflow für die Entwicklung

### Normales Starten

```bash
# Einfach starten - Aspire prüft automatisch Ports
./aspire-start.sh Development Debug
```

Das System wird:
- Alle erforderlichen Ports prüfen
- Bei Konflikten automatisch alternative Ports verwenden
- Eine Zusammenfassung mit allen verwendeten Ports anzeigen

### Fehlersuche bei Port-Konflikten

```bash
# 1. Status aller Ports prüfen
./check-ports.sh --check

# 2. Problematische Ports ermitteln
# Suche nach "IN USE" Einträgen

# 3. Prozesse beenden
./check-ports.sh --free

# 4. Aspire starten
./aspire-start.sh Development Debug
```

### Echtzeit-Überwachung

```bash
# Terminal 1: Überwache Ports
./check-ports.sh --monitor

# Terminal 2: Starte Aspire
./aspire-start.sh Development Debug
```

## Voraussetzungen

Das System benötigt `netcat` (nc) für Port-Prüfungen:

### Installation

**macOS:**
```bash
brew install netcat
```

**Linux (Debian/Ubuntu):**
```bash
sudo apt-get install netcat-openbsd
```

**Linux (RedHat/CentOS):**
```bash
sudo yum install nmap-ncat
```

Falls `netcat` nicht installiert ist, zeigt Aspire eine Warnung, aber mit Fallback-Funktion.

## Troubleshooting

### "Port XYZ is in use"
1. Führe `./check-ports.sh --check` aus
2. Identifiziere die blockierenden Prozesse
3. Nutze `./check-ports.sh --free` zum Beenden
4. Starte Aspire erneut

### Prozess lässt sich nicht beenden
Falls `--free` fehlschlägt:
```bash
# Manuelle Überprüfung
lsof -i :9001

# Manuelles Beenden (wenn nötig)
kill -9 <PID>
```

### "Could not find available port"
Wenn das System keinen freien Port findet (theoretisch):
- Über 100 aufeinanderfolgende Ports sind belegt
- Überprüfe Systemlast: `netstat -tuln`
- Beende unnötige Services

## Beispiel: Kompletter Startup-Prozess

```bash
#!/bin/bash

# 1. Prüfe Ports
./check-ports.sh --check

# 2. Falls Konflikte, bereinigen
if [ $? -ne 0 ]; then
    ./check-ports.sh --free
fi

# 3. Starte Aspire (mit automatischer Port-Fallback)
./aspire-start.sh Development Debug

# 4. Öffne Dashboard
sleep 2
open http://localhost:5200
```

## Umgebungsvariablen

Für benutzerdefinierte Ports beim Start:

```bash
# AppHost auf Port 5300 starten
./aspire-start.sh Development Debug 5300
```

Das System prüft dann automatisch Port 5300 und weist alternative Ports zu, falls nötig.

## Integrierte Features

✓ Automatische Port-Verfügbarkeitsprüfung  
✓ Intelligente Fallback-Port-Zuweisung  
✓ Echtzeit-Port-Überwachung  
✓ Prozess-Information bei belegten Ports  
✓ Sichere Prozessbeendigung (SIGTERM → SIGKILL)  
✓ Farbcodierte Ausgabe für bessere Lesbarkeit  
✓ Detaillierte Fehlermeldungen  

## Weitere Informationen

Siehe auch:
- [ASPIRE_SETUP_QUICKSTART.md](../ASPIRE_SETUP_QUICKSTART.md) - Schnellstart-Guide
- [ASPIRE_SERVICE_DISCOVERY.md](../ASPIRE_SERVICE_DISCOVERY.md) - Service-Discovery
- [aspire-start.sh](../aspire-start.sh) - Hauptstartskript
