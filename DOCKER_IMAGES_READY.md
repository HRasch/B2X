# ? DOCKER IMAGES GEBAUT - BEREIT ZUM START!

## ?? Status

? **Frontend Store Image gebaut:** `b2connect-frontend-store:latest` (216 MB)
? **Frontend Admin Image gebaut:** `b2connect-frontend-admin:latest` (218 MB)

---

## ?? SOFORT STARTEN

### Option 1: Mit dem vorbereiteten Script (EMPFOHLEN)

**Windows PowerShell:**
```powershell
.\scripts\start-aspire-with-frontends.ps1
```

**Linux/Mac:**
```bash
bash scripts/start-aspire-with-frontends.sh
```

Dieses Script wird:
1. ? AppHost bauen
2. ? AppHost starten
3. ? Dashboard öffnen unter http://localhost:15500

### Option 2: Manuell

**Terminal 1: Backend starten**
```bash
cd AppHost
dotnet run
```

**Warten Sie auf:** `Listening on http://localhost:15500`

**Dann öffnen Sie:**
- Dashboard: http://localhost:15500
- Store: http://localhost:5173
- Admin: http://localhost:5174

---

## ?? Was Sie im Dashboard sehen werden

```
ASPIRE DASHBOARD (http://localhost:15500)
?????????????????????????????????????????????????????
Resources              Status        Endpoints
?????????????????????????????????????????????????????
postgres              ? Healthy
redis                 ? Healthy
elasticsearch         ? Healthy
rabbitmq              ? Healthy
auth-service          ? Running     ?? 7002
tenant-service        ? Running     ?? 7003
catalog-service       ? Running     ?? 7005
theming-service       ? Running     ?? 7008
store-gateway         ? Running     ?? 8000
admin-gateway         ? Running     ?? 8080
frontend-store        ? Running     ?? 5173 ?
frontend-admin        ? Running     ?? 5174 ?
?????????????????????????????????????????????????????
```

---

## ? Wichtige Infos

### Images sind in Production-Mode (nicht Development!)

Die gebauten Images verwenden das **Production Target** aus dem Dockerfile:
```dockerfile
FROM node:20-alpine AS production
RUN npm install -g serve
COPY --from=builder /app/dist ./dist
CMD ["serve", "-s", "dist", "-l", "5173"]
```

**Das bedeutet:**
- ? **KEIN Hot-Reload** (Code-Änderungen erfordern Rebuild)
- ? **Production-ready** (optimiert für Performance)
- ? **Statische Assets** (schnell serviert)

### Falls Sie Development-Mode mit Hot-Reload brauchen

Ändern Sie die Docker Images um Development-Target zu verwenden oder starten Sie npm lokal:

```bash
# Development (mit Hot-Reload)
cd Frontend/Store && npm run dev
cd Frontend/Admin && npm run dev
```

---

## ?? Docker Images neu bauen (wenn nötig)

Falls Sie Code geändert haben:

**Store Frontend:**
```bash
docker build -t b2connect-frontend-store:latest ./Frontend/Store
```

**Admin Frontend:**
```bash
docker build -t b2connect-frontend-admin:latest ./Frontend/Admin
```

Oder beide zusammen:
```bash
docker-compose build frontend-store frontend-admin
```

---

## ? Verifikation

### 1. Images vorhanden?
```bash
docker images | findstr b2connect-frontend
```

**Sollte zeigen:**
```
b2connect-frontend-admin    latest    218MB
b2connect-frontend-store    latest    216MB
```

### 2. AppHost startet?
```bash
cd AppHost && dotnet run
```

**Sollte zeigen:**
```
Listening on http://localhost:15500
```

### 3. Services laufen?
```bash
curl http://localhost:15500
# ? Dashboard HTML laden
```

---

## ?? Nächste Schritte

1. **Starten Sie das Startup-Script:**
   ```powershell
   .\scripts\start-aspire-with-frontends.ps1
   ```

2. **Öffnen Sie den Browser:**
   ```
   http://localhost:15500
   ```

3. **Warten Sie 30-60 Sekunden** für alle Services

4. **Navigieren Sie zu den Frontends:**
   - Store: http://localhost:5173
   - Admin: http://localhost:5174

---

## ?? Häufige Probleme

### Problem: "Container exited with code 1"
**Ursache:** Port ist bereits belegt  
**Lösung:**
```bash
docker stop $(docker ps -q)
# Oder spezifisch:
docker stop b2connect-frontend-store b2connect-frontend-admin
```

### Problem: "Cannot connect to port 5173"
**Ursache:** Frontend Container läuft noch nicht  
**Lösung:** Warten Sie 30-60 Sekunden und aktualisieren Sie den Browser (F5)

### Problem: "npm run build failed"
**Ursache:** Dependencies nicht installiert  
**Lösung:** Image neu bauen:
```bash
docker build --no-cache -t b2connect-frontend-store:latest ./Frontend/Store
```

---

## ?? Wichtige Dateien

- `AppHost/Program.cs` - Backend Services + Frontends
- `AppHost/B2ConnectAspireExtensions.cs` - `AddViteContainer` Extension
- `Frontend/Store/Dockerfile` - Store Frontend Build
- `Frontend/Admin/Dockerfile` - Admin Frontend Build
- `docker-compose.yml` - Alternative Orchestrierung

---

## ?? Sie sind bereit!

**Starten Sie jetzt:**

```powershell
.\scripts\start-aspire-with-frontends.ps1
```

**Genießen Sie Ihr vollständig orchestriertes B2Connect System!** ??

---

**Status:** ? Alles ist bereit - Go live!
