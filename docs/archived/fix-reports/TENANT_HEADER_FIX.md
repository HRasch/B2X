# ? X-Tenant-ID Header Fix für Admin Frontend

## ?? Problem

Login schlägt fehl mit:
```json
{"success":false,"error":"Missing required header: X-Tenant-ID"}
```

## ? Lösung

Das Backend erwartet einen `X-Tenant-ID` Header im GUID-Format bei **allen** Requests, einschließlich Login.

### Änderungen:

1. **Frontend/Admin/.env** - Default Tenant GUID hinzugefügt:
   ```env
   VITE_DEFAULT_TENANT_ID=00000000-0000-0000-0000-000000000001
   ```

2. **Frontend/Admin/src/main.ts** - Initialisiert Tenant ID beim App-Start:
   ```typescript
   const DEFAULT_TENANT_ID = import.meta.env.VITE_DEFAULT_TENANT_ID || "00000000-0000-0000-0000-000000000001";
   if (!localStorage.getItem("tenantId")) {
     localStorage.setItem("tenantId", DEFAULT_TENANT_ID);
   }
   ```

3. **Frontend/Admin/src/services/api/auth.ts** - Sendet X-Tenant-ID beim Login:
   ```typescript
   const tenantId = localStorage.getItem("tenantId") || DEFAULT_TENANT_ID;
   const response = await axios.post<LoginResponse>(
     `${baseURL}/api/auth/login`,
     credentials,
     {
       headers: { 
         "Content-Type": "application/json",
         "X-Tenant-ID": tenantId,
       },
     }
   );
   ```

4. **Frontend/Admin/src/services/client.ts** - Sendet X-Tenant-ID bei allen API-Calls:
   ```typescript
   const tenantId = localStorage.getItem("tenantId");
   if (tenantId) {
     config.headers["X-Tenant-ID"] = tenantId;
   }
   ```

---

## ?? So testen Sie

1. **localStorage leeren** (optional für Clean State):
   ```javascript
   localStorage.clear()
   ```

2. **Frontend neu starten**:
   ```bash
   cd Frontend/Admin
   npm run dev
   ```

3. **Login versuchen** - Der X-Tenant-ID Header sollte jetzt gesendet werden

---

## ?? Header Flow

```
1. App Start
   ??> main.ts setzt localStorage.tenantId = "00000000-0000-0000-0000-000000000001"

2. Login Request
   ??> auth.ts liest tenantId aus localStorage
   ??> Sendet Header: X-Tenant-ID: 00000000-0000-0000-0000-000000000001

3. Nach Login
   ??> Speichert user.tenantId in localStorage (falls vorhanden)

4. Alle weiteren Requests
   ??> client.ts Interceptor fügt X-Tenant-ID Header hinzu
```

---

## ?? Wichtig: Backend Tenant-Validierung

Das Backend (`ValidateTenantAttribute.cs`) validiert:
- Header muss vorhanden sein
- Muss gültiges GUID-Format sein
- Darf nicht Guid.Empty sein

Der Default-Tenant `00000000-0000-0000-0000-000000000001` ist ein gültiges GUID.

---

## ?? Multi-Tenant Architektur

In einer echten Multi-Tenant Umgebung:

1. **Tenant Selection** - User wählt Tenant vor Login
2. **Tenant from URL** - Tenant aus Subdomain/Path extrahieren
3. **Tenant from Token** - Nach Login aus JWT Token lesen

Für Development verwenden wir einen festen Default-Tenant.
