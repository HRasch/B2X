# 🐛 macOS CDP Port Conflict Fix

## Problem
Nach dem Schließen des Terminals oder des Debuggers belegt macOS Docker Content Provider (CDP) die verwendeten Ports persistent und gibt sie nicht frei. Dies führt zu Timeout-Fehlern beim Login und anderen Kommunikationsproblemen zwischen Frontend und Backend.

**Fehlerbeispiel:**
```
Operation timed out after 30 seconds with 0 bytes received
```

## Root Cause
- **CDP** (Docker Content Provider Control) ist ein macOS-Hintergrundprozess
- CDP bindet IPv6-Ports auf `[::]` (all interfaces) nach dem Start
- Wenn Services mit `http://+:PORT` starten, nutzen sie IPv4 + IPv6
- Nach dem Service-Stopp gibt CDP die IPv6-Ports nicht frei

## Lösung

### 1️⃣ Permanente Konfiguration (EMPFOHLEN)
Services wurden konfiguriert, um **nur auf IPv4 (127.0.0.1)** zu lauschen, nicht auf IPv6:

**Betroffene Dateien:**
- `backend/Orchestration/Program.cs` - Alle ASPNETCORE_URLS auf `http://127.0.0.1:PORT` geändert

**Beispiel (vorher vs. nachher):**
```csharp
// ❌ Alt (verursacht IPv6 Konflikte)
.WithEnvironment("ASPNETCORE_URLS", "http://+:7002")

// ✅ Neu (nur IPv4, kein CDP Konflikt)
.WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:7002")
```

### 2️⃣ Cleanup Script
Für hartnäckige Fälle: Cleanup-Script ausführen

```bash
# Alle CDP und B2Connect Prozesse killen
bash ./scripts/cleanup-cdp.sh
```

Das Script:
- ✓ Killtet alle `dcpctrl` Prozesse
- ✓ Killtet alle `B2Connect` Services
- ✓ Prüft, ob Ports nun frei sind

## Wann brauchst du das Cleanup Script?

1. **Nach Crash oder abnormaler Beendigung**
   ```bash
   bash ./scripts/cleanup-cdp.sh
   ```

2. **Vor einem neuen Start**
   ```bash
   bash ./scripts/cleanup-cdp.sh
   # Dann F5 zum Debuggen
   ```

3. **Wenn Ports noch belegt sind nach dem Schließen**
   ```bash
   # Prüfe welche Ports belegt sind
   lsof -i :7002  # oder andere Ports
   
   # Cleanup
   bash ./scripts/cleanup-cdp.sh
   ```

## Verifikation

### Ports sollten nur auf IPv4 lauschen:
```bash
# ✅ Korrekt - nur IPv4
tcp4       0      0  127.0.0.1.7002      *.*     LISTEN

# ❌ Problematisch - IPv6 auch
tcp6       0      0  ::1.7002            *.*     LISTEN
```

### Alle Ports prüfen:
```bash
netstat -an | grep "7002\|7003\|7004\|7005\|7008\|8000\|8080"
```

## ReverseProxy Konfiguration
Die Admin und Store Gateway ReverseProxy-Einstellungen in den `appsettings.json` Dateien verwenden noch die alten URLs (z.B. `http://localhost:7002`), das ist OK - die Services sind auf IPv4 erreichbar.

## Weitere Tipps

### Wenn Probleme persistieren:
1. **Docker Desktop neu starten**
   ```bash
   # Schließe VS Code und Docker Desktop
   # Öffne Docker Desktop neu
   ```

2. **Docker Container reinigen**
   ```bash
   docker system prune -a
   ```

3. **Nur IPv4 bei ReverseProxy-Tests verwenden**
   ```bash
   # ✓ Funktioniert
   curl http://127.0.0.1:7002/health
   
   # ✗ Könnte Probleme verursachen
   curl http://localhost:7002/health  # localhost kann IPv6 sein
   ```

## Zusammenfassung
✅ **Alle Services sind jetzt IPv4-only konfiguriert**
✅ **CDP-Konflikte sollten nicht mehr auftreten**
✅ **Cleanup-Script für hartnäckige Fälle vorhanden**

Starte das Backend neu mit `F5` und die Login-Timeouts sollten behoben sein! 🎉
