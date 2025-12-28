# ? CORS Fix für Aspire Dynamic Ports

## ?? Problem

```
Access to XMLHttpRequest at 'http://localhost:8080/api/auth/login' from origin 
'http://localhost:54977' has been blocked by CORS policy
```

Aspire verwendet **dynamische Ports** für Frontend-Anwendungen (z.B. 54977 statt 5174).
Die festen CORS-Origins (`localhost:5174`) funktionieren nicht.

## ? Lösung

In **Development Mode** erlauben wir alle `localhost` Origins:

### backend/Gateway/Admin/Program.cs

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // In development, allow any localhost origin (Aspire uses dynamic ports)
            policy
                .SetIsOriginAllowed(origin => 
                {
                    if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    {
                        return uri.Host == "localhost" || uri.Host == "127.0.0.1";
                    }
                    return false;
                })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition", "X-Total-Count");
        }
        else
        {
            // In production, use strict origins from configuration
            policy
                .WithOrigins(corsOrigins!)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition", "X-Total-Count");
        }
    });
});
```

### backend/Gateway/Store/API/Program.cs

Gleiche Änderung für `AllowStoreFrontend` Policy.

---

## ?? Sicherheit

### Development Mode
- ? Alle `localhost` und `127.0.0.1` Origins erlaubt
- ? Flexibel für Aspire dynamische Ports
- ?? NUR für lokale Entwicklung!

### Production Mode
- ? Nur konfigurierte Origins erlaubt
- ? Streng kontrolliert
- ? Konfiguration via Environment Variables

---

## ?? Wie Aspire Ports zuweist

```
Aspire Port Assignment:
????????????????????????????????????????
? Frontend Store                       ?
? Konfiguriert: 5173                   ?
? Zugewiesen:   54977 (dynamisch)      ?
????????????????????????????????????????
? Frontend Admin                       ?
? Konfiguriert: 5174                   ?
? Zugewiesen:   54978 (dynamisch)      ?
????????????????????????????????????????
```

Aspire kann die konfigurierten Ports überschreiben, wenn:
- Port bereits belegt ist
- Proxy-Modus aktiviert ist
- Service Discovery aktiv ist

---

## ?? Testen

1. **AppHost starten:**
   ```bash
   cd AppHost && dotnet run
   ```

2. **Browser DevTools öffnen** (F12)

3. **Network Tab überprüfen:**
   - Preflight OPTIONS Request sollte `200 OK` sein
   - Response Headers sollten enthalten:
     - `Access-Control-Allow-Origin: http://localhost:XXXXX`
     - `Access-Control-Allow-Credentials: true`

4. **Login versuchen:**
   - Kein CORS-Fehler mehr
   - Request wird durchgelassen

---

## ?? Geänderte Dateien

- ? `backend/Gateway/Admin/Program.cs` - CORS für dynamische Ports
- ? `backend/Gateway/Store/API/Program.cs` - CORS für dynamische Ports

---

## ?? Alternative: Feste Ports in Aspire

Falls Sie feste Ports bevorzugen, deaktivieren Sie den Proxy:

```csharp
// AppHost/Program.cs
var frontendAdmin = builder
    .AddViteApp("frontend-admin", "../Frontend/Admin")
    .WithHttpEndpoint(port: 5174, name: "http", isProxied: false)  // isProxied: false!
    ...
```

Aber die `SetIsOriginAllowed` Lösung ist flexibler und empfohlen.

---

## ? Status

Build erfolgreich! CORS sollte jetzt funktionieren.

Starten Sie den AppHost neu:
```bash
cd AppHost && dotnet run
```
