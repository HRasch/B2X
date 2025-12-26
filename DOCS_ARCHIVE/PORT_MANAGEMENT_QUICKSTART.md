# Port Verfügbarkeitsprüfung für Aspire - Schnellstart

## 🚀 Sofort Einsatzbereit

Das System prüft beim Start **automatisch alle Ports**. Sie müssen nichts Besonderes tun!

```bash
# Einfach Aspire starten
./aspire-start.sh Development Debug

# Das System prüft automatisch:
# ✓ Port 5200 (AppHost) - Fallback wenn belegt
# ✓ Port 5500 (Dashboard) - Fallback wenn belegt
# ✓ Ports 9001-9004 (Services) - Fallback wenn belegt
```

## 🛠️ Manuelle Port-Verwaltung

Für volle Kontrolle steht Ihnen das Port-Management-Tool zur Verfügung:

### Status prüfen
```bash
./check-ports.sh --check
```
Zeigt welche Ports verfügbar sind und welche belegt.

### Ports freigeben
```bash
./check-ports.sh --free
```
Beendet alle Prozesse auf belegten Ports und gibt diese frei.

### Echtzeit-Überwachung
```bash
./check-ports.sh --monitor
```
Überwacht die Ports in Echtzeit (aktualisiert alle 3 Sekunden).

## 📋 Typische Workflows

### Workflow 1: Schnellstart (Recommended)
```bash
# Port-Status prüfen (optional)
./check-ports.sh --check

# Aspire starten (mit automatischer Port-Fallback)
./aspire-start.sh Development Debug

# Fertig! Dashboard ist verfügbar unter:
# http://localhost:5200 (oder alternativer Port)
```

### Workflow 2: Fehlersuche bei Port-Konflikten
```bash
# 1. Status aller Ports sehen
./check-ports.sh --check

# 2. Problematische Ports freigeben
./check-ports.sh --free

# 3. Aspire starten
./aspire-start.sh Development Debug
```

### Workflow 3: Debug + Echtzeit-Überwachung
```bash
# Terminal 1: Überwache Ports live
./check-ports.sh --monitor

# Terminal 2: Starte Aspire
./aspire-start.sh Development Debug

# Terminal 3: Weitere Tests/Development
# ...
```

## 🔍 Ausgabe verstehen

### `--check` Ausgabe
```
✓ AppHost (Port 5200): AVAILABLE       ← Grün = Verfügbar
✗ CatalogService (Port 9001): IN USE   ← Rot = Belegt
   Process: dotnet (12345)              ← Welcher Prozess

Summary:
  Available: 5/6
  In Use:    1/6
```

### `--free` Ausgabe
```
[*] Attempting to free port 9001 (Catalog Service)...
    Sending SIGTERM to PID 12345...
[✓] Port 9001 freed                     ← Erfolgreich freigegeben
```

## 🎯 Häufig gestellte Fragen

**F: Was ist, wenn ein Port nicht freigegeben werden kann?**  
A: Das ist sehr selten. Falls es passiert:
```bash
# Manuell prüfen welcher Prozess den Port belegt
lsof -i :9001

# Manuell beenden (falls nötig)
kill -9 <PID>
```

**F: Kann ich andere Ports verwenden?**  
A: Ja! Beim Start angeben:
```bash
./aspire-start.sh Development Debug 5300
```
Das System nutzt dann Port 5300 als AppHost-Port.

**F: Was ist mit Netcat?**  
A: Wird für Port-Prüfung verwendet. Falls nicht installiert:
```bash
# macOS
brew install netcat

# Linux (Debian/Ubuntu)
sudo apt-get install netcat-openbsd

# Linux (RedHat/CentOS)
sudo yum install nmap-ncat
```

**F: Funktioniert automatische Fallback immer?**  
A: Ja, solange weniger als 100 aufeinanderfolgende Ports belegt sind. Das ist äußerst unwahrscheinlich.

## 📚 Weitere Ressourcen

- [PORT_MANAGEMENT.md](PORT_MANAGEMENT.md) - Vollständige Dokumentation
- [ASPIRE_SETUP_QUICKSTART.md](ASPIRE_SETUP_QUICKSTART.md) - Aspire-Setup
- [ASPIRE_SERVICE_DISCOVERY.md](ASPIRE_SERVICE_DISCOVERY.md) - Service-Discovery

## ✅ Checkliste für Entwickler

- [ ] `netcat` installiert (`brew install netcat`)
- [ ] `check-ports.sh --check` läuft ohne Fehler
- [ ] `./aspire-start.sh Development Debug` startet erfolgreich
- [ ] Dashboard ist erreichbar unter angezeigte URL
- [ ] Alle Microservices sind erreichbar

## 🎨 Color-Codes Übersicht

| Symbol | Farbe | Bedeutung |
|--------|-------|-----------|
| ✓ | 🟢 Grün | Port verfügbar |
| ✗ | 🔴 Rot | Port belegt |
| [*] | 🟡 Gelb | Info/Aktion |
| [✓] | 🟢 Grün | Erfolgreich |
| [✗] | 🔴 Rot | Fehler |

## 💡 Pro-Tipps

1. **Regelmäßige Überprüfung**: Nutze `--monitor` im Hintergrund während der Entwicklung
2. **Automation**: Nutze `--free` bevor Du wichtige Tests startest
3. **Debugging**: Wenn unerwartete Fehler auftreten, prüfe zuerst die Ports
4. **Performance**: Geschlossene Ports = schnellerer Start

---

**Viel Erfolg bei der Entwicklung!** 🚀
