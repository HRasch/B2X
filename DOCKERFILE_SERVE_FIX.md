# ? DOCKERFILE FIX - Serve Command Error behoben

## ?? Problem
```
ArgError: unknown or unexpected option: -H
```

Der `serve` Command in der Production Stage kannte die `-H` Flag nicht.

## ? Lösung

Beide Dockerfiles wurden korrigiert:
- `Frontend/Store/Dockerfile` ? Fixed
- `Frontend/Admin/Dockerfile` ? Fixed

**Änderung:**
```dockerfile
# ? ALT (nicht funktionierend)
CMD ["serve", "-s", "dist", "-l", "5174", "-H", "0.0.0.0"]

# ? NEU (funktionierend)
CMD ["serve", "-s", "dist", "-l", "5174"]
```

Die `serve` Package hört standardmäßig auf `0.0.0.0` (alle Interfaces), daher ist die `-H` Flag nicht nötig.

---

## ?? Docker Images neu gebaut

? `b2connect-frontend-store:latest` - Fresh build  
? `b2connect-frontend-admin:latest` - Fresh build  

---

## ?? Jetzt können Sie starten

```powershell
# Windows PowerShell
.\scripts\start-aspire-with-frontends.ps1
```

Oder manuell:
```bash
cd AppHost && dotnet run
```

---

## ?? Erwartetes Ergebnis

Frontends sollten jetzt **ohne Fehler** starten:
- ? `frontend-store` läuft auf Port 5173
- ? `frontend-admin` läuft auf Port 5174
- ? Im Aspire Dashboard sichtbar
- ? Über http://localhost:5173 und http://localhost:5174 erreichbar

---

## ? Status

**Docker Images:** ? Fixed und neu gebaut  
**Dockerfiles:** ? Korrigiert  
**Ready to Start:** ? JA!

**Los geht's!** ??
