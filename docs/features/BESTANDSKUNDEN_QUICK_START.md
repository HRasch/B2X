# Bestandskunden-Registrierung: Quick-Start Implementation (2-3 Tage)

**Zieltermin:** KW 2 2026  
**Geschätzter Aufwand:** 40-48 Stunden  
**Team:** Backend (1), Frontend (1), QA (0.5)

---

## 🎯 Übersicht

```
Bestandskunde (im ERP)
         ↓
Kundennummer oder E-Mail eingeben
         ↓
ERP-Daten validieren
         ↓
Daten zur Bestätigung anzeigen
         ↓
Passwort setzen + Registrieren
         ↓
Account erstellt, E-Mail-Bestätigung
```

---

## 📋 Implementierungs-Checkliste (Parallel)

### Backend (Entwickler 1) - 2,5 Tage

#### Tag 1: Setup & Datenmodell (4h)

- [ ] **1.1** Entities erstellen (30 min)
  - `UserRegistration` Entity + Enums
  - Datei: `backend/Domain/Identity/B2Connect.Identity.Core/Entities/UserRegistration.cs`
  - Copy-Paste aus [BESTANDSKUNDEN_IMPLEMENTIERUNGS_SCAFFOLD.md](../features/BESTANDSKUNDEN_IMPLEMENTIERUNGS_SCAFFOLD.md) Sektion "1. Entities"

- [ ] **1.2** Repository Interfaces (20 min)
  - `IUserRegistrationRepository`
  - `IErpCustomerService`
  - Datei: `backend/Domain/Identity/B2Connect.Identity.Core/Interfaces/`

- [ ] **1.3** Datenbank Migrations (1h)
  ```bash
  cd backend/Domain/Identity
  dotnet ef migrations add AddUserRegistration -p B2Connect.Identity.Infrastructure -s B2Connect.Identity.API
  dotnet ef database update
  ```

- [ ] **1.4** Tests schreiben (2h)
  - Entity Tests
  - Repository Mock-Tests

#### Tag 2: ERP-Integration & Handler (6h)

- [ ] **2.1** ERP-Service Implementation (2h)
  - `SapCustomerService` oder `CsvCustomerService` (für lokale Tests)
  - Datei: `backend/Domain/Identity/B2Connect.Identity.Infrastructure/ExternalServices/SapCustomerService.cs`
  - Copy-Paste aus Scaffold

- [ ] **2.2** CQRS Handler implementieren (2h)
  - `CheckRegistrationTypeCommand`
  - `CheckRegistrationTypeCommandHandler`
  - Copy-Paste + anpassen an lokale Struktur

- [ ] **2.3** Validator (1h)
  - `CheckRegistrationTypeCommandValidator`

- [ ] **2.4** Handler Tests (1h)
  - Unit Tests mit Mocks
  - Happy Path + Error Cases

#### Tag 3: API Endpoints & Integration (4h)

- [ ] **3.1** RegistrationController erstellen (1h)
  - POST `/api/registration/check-type`
  - Copy-Paste aus Scaffold

- [ ] **3.2** Request/Response DTOs (30 min)

- [ ] **3.3** Fehlerbehandlung Middleware (1h)
  - Rate Limiting
  - Exception Handling
  - Logging

- [ ] **3.4** Lokale Tests (1.5h)
  ```bash
  dotnet test backend/Domain/Identity/tests/ -v minimal
  ```

- [ ] **3.5** API Tests mit Postman/curl (30 min)
  ```bash
  # Check Customer Number
  curl -X POST http://localhost:7002/api/registration/check-type \
    -H "Content-Type: application/json" \
    -H "X-Tenant-ID: 550e8400-e29b-41d4-a716-446655440000" \
    -d '{"value":"KD-12345","type":"customer_number"}'
  ```

---

### Frontend (Entwickler 2) - 2,5 Tage (parallel)

#### Tag 1: State Management & Components (6h)

- [ ] **1.1** Pinia Store erstellen (1h)
  - `registrationStore.ts`
  - Copy-Paste aus Scaffold
  - State, Computed, Actions

- [ ] **1.2** Komponenten erstellen (3h)
  - `RegistrationForm.vue`
  - `ExistingCustomerStep.vue`
  - `CustomerDataConfirmation.vue`
  - Copy-Paste + anpassen

- [ ] **1.3** Styling (Tailwind) (1h)
  - Responsive Design
  - Dark Mode Support (optional)

- [ ] **1.4** API Service Integration (1h)
  - `registrationService.ts`
  - HTTP Client Setup
  - Error Handling

#### Tag 2: Integration & Testing (4h)

- [ ] **2.1** Router Setup (30 min)
  ```typescript
  // src/router/index.ts
  {
    path: '/register',
    name: 'register',
    component: RegistrationForm
  },
  {
    path: '/registration-complete',
    name: 'registration-complete',
    component: RegistrationSuccess
  }
  ```

- [ ] **2.2** Validierung & UX-Verbesserungen (1h)
  - Form Validation
  - Error Messages
  - Loading States
  - Password Strength Indicator

- [ ] **2.3** Unit Tests (1h)
  ```bash
  cd Frontend/Store
  npm run test -- src/stores/registrationStore.spec.ts
  npm run test -- src/components/RegistrationForm.spec.ts
  ```

- [ ] **2.4** Lokale manuellen Tests (1.5h)
  - Happy Path
  - Error Scenarios
  - Browser DevTools prüfen

---

### Integration & QA (0,5 Tage) - parallel mit Tagen 2-3

- [ ] **4.1** E2E Tests schreiben (2h)
  - Playwright Tests
  - Happy Path
  - Error Cases
  ```bash
  cd Frontend/Store
  npm run test:e2e
  ```

- [ ] **4.2** Cross-Browser Tests (30 min)
  - Chrome, Firefox, Safari

- [ ] **4.3** Performance Testing (30 min)
  - API Response Time
  - Frontend Load Time
  - Network Throttling

---

## 🚀 Schnelle Implementierung - Punkt für Punkt

### Phase 1: Backend Setup (Dag 1, Morgen)

**Zeit: 4 Stunden**

```bash
# 1. Entities erstellen
code backend/Domain/Identity/B2Connect.Identity.Core/Entities/UserRegistration.cs
# → Copy-Paste aus SCAFFOLD, Sektion 1

# 2. Interfaces erstellen
code backend/Domain/Identity/B2Connect.Identity.Core/Interfaces/
# → IUserRegistrationRepository.cs
# → IErpCustomerService.cs

# 3. DbContext erweitern
code backend/Domain/Identity/B2Connect.Identity.Infrastructure/Data/IdentityDbContext.cs
# Hinzufügen:
# public DbSet<UserRegistration> UserRegistrations => Set<UserRegistration>();

# 4. Migration erstellen
cd backend/Domain/Identity
dotnet ef migrations add AddUserRegistration \
  -p B2Connect.Identity.Infrastructure \
  -s B2Connect.Identity.API

# 5. Testen
dotnet test backend/Domain/Identity/tests/ -v minimal
```

### Phase 2: ERP-Service (Tag 1/2, Nachmittag)

**Zeit: 3 Stunden**

```bash
# 1. ERP Service implementieren (oder CSV-Fallback für Tests)
code backend/Domain/Identity/B2Connect.Identity.Infrastructure/ExternalServices/SapCustomerService.cs
# → Copy-Paste aus SCAFFOLD, Sektion 5

# 2. Registrierung in DI Container
code backend/Domain/Identity/B2Connect.Identity.API/Program.cs
# Hinzufügen:
services.AddScoped<IErpCustomerService, SapCustomerService>();
# Oder für lokale Tests:
# services.AddScoped<IErpCustomerService, CsvCustomerService>();

# 3. HttpClient konfigurieren
# In Program.cs:
services.AddHttpClient<SapCustomerService>()
    .ConfigureHttpClient(client =>
    {
        var sapUrl = configuration["Erp:Endpoint"];
        client.BaseAddress = new Uri(sapUrl);
        client.DefaultRequestHeaders.Add("Authorization", 
            $"Basic {Convert.ToBase64String(...)}");
    });

# 4. Testen
dotnet test backend/Domain/Identity/tests/ -v minimal
```

### Phase 3: CQRS Handler (Tag 2, Morgen)

**Zeit: 3 Stunden**

```bash
# 1. Commands & Queries erstellen
code backend/Domain/Identity/B2Connect.Identity.Application/Features/Registration/Commands/CheckRegistrationTypeCommand.cs
# → Copy-Paste aus SCAFFOLD

# 2. Handler implementieren
code backend/Domain/Identity/B2Connect.Identity.Application/Features/Registration/Handlers/CheckRegistrationTypeCommandHandler.cs
# → Copy-Paste aus SCAFFOLD

# 3. Validator erstellen
code backend/Domain/Identity/B2Connect.Identity.Application/Features/Registration/Validators/CheckRegistrationTypeCommandValidator.cs

# 4. In MediatR registrieren
code backend/Domain/Identity/B2Connect.Identity.API/Program.cs
# Hinzufügen:
services.AddMediatR(typeof(Program).Assembly);

# 5. Testen
dotnet test backend/Domain/Identity/tests/ -v minimal
```

### Phase 4: API Controller (Tag 2, Nachmittag)

**Zeit: 2 Stunden**

```bash
# 1. Controller erstellen
code backend/BoundedContexts/Store/API/Controllers/RegistrationController.cs
# → Copy-Paste aus SCAFFOLD, Sektion 6

# 2. Route in Program.cs registrieren
code backend/BoundedContexts/Store/API/Program.cs
# app.MapControllers();

# 3. Lokal testen
dotnet run --project backend/BoundedContexts/Store/API

# In anderer Console:
curl -X POST http://localhost:8000/api/registration/check-type \
  -H "Content-Type: application/json" \
  -H "X-Tenant-ID: 550e8400-e29b-41d4-a716-446655440000" \
  -d '{"value":"KD-12345","type":"customer_number"}'
```

### Phase 5: Frontend Store & Komponente (Tag 1-2, parallel)

**Zeit: 6 Stunden**

```bash
# 1. Store erstellen
code Frontend/Store/src/stores/registrationStore.ts
# → Copy-Paste aus SCAFFOLD

# 2. Komponente erstellen
code Frontend/Store/src/components/RegistrationForm.vue
# → Copy-Paste aus SCAFFOLD

# 3. Router einrichten
code Frontend/Store/src/router/index.ts
# Hinzufügen:
{
  path: '/register',
  name: 'register',
  component: () => import('@/components/RegistrationForm.vue')
}

# 4. Lokal testen
cd Frontend/Store
npm install
npm run dev
# → http://localhost:5173/register

# 5. Manuell testen
# - Kundennummer eingeben → Daten sollten geladen werden
# - Passwort eingeben → Registrieren
# - Erfolgreich-Seite sollte erscheinen
```

### Phase 6: Unit Tests (Tag 2-3)

**Zeit: 2 Stunden**

```bash
# Backend Unit Tests
cd backend/Domain/Identity/tests
dotnet test B2Connect.Identity.Tests.csproj -v minimal --logger:"console;verbosity=normal"

# Frontend Unit Tests
cd Frontend/Store
npm run test src/stores/registrationStore.spec.ts
npm run test src/components/RegistrationForm.spec.ts
```

### Phase 7: E2E Tests (Tag 3, Morgen)

**Zeit: 1.5 Stunden**

```bash
# Datei erstellen
code Frontend/Store/tests/e2e/registration.spec.ts
# → Copy-Paste aus SCAFFOLD

# Testen
npx playwright test tests/e2e/registration.spec.ts --headed
```

---

## 🔧 Häufige Probleme & Lösungen

### Problem: "ERP nicht erreichbar"
**Lösung**: Für lokale Entwicklung `CsvCustomerService` verwenden
```csharp
// Program.cs
#if DEBUG
    services.AddScoped<IErpCustomerService, CsvCustomerService>();
#else
    services.AddScoped<IErpCustomerService, SapCustomerService>();
#endif
```

**CSV-Datei:** `backend/data/customers.csv`
```csv
CustomerId,CompanyName,ContactFirstName,ContactLastName,Email,Phone
KD-12345,Musterfirma GmbH,Max,Mustermann,max@musterfirma.de,+49 89 123456
KD-12346,Beispiel AG,Erika,Müller,erika@beispiel.de,+49 30 987654
```

### Problem: "CORS Error" beim Frontend → Backend aufrufen
**Lösung**: CORS in Backend konfigurieren
```csharp
// Program.cs
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins("http://localhost:5173", "http://localhost:5174")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

app.UseCors();
```

### Problem: "Tenant-ID fehlt"
**Lösung**: Middleware zum Extrahieren der Tenant-ID
```csharp
app.Use(async (context, next) =>
{
    var tenantId = context.Request.Headers["X-Tenant-ID"].ToString();
    if (string.IsNullOrEmpty(tenantId))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { error = "Missing Tenant-ID" });
        return;
    }
    
    context.Items["TenantId"] = Guid.Parse(tenantId);
    await next();
});
```

---

## 📊 Geschätzte Zeiten pro Aufgabe

| Aufgabe | Backend | Frontend | QA | Gesamt |
|---------|---------|----------|-----|--------|
| Setup & Datenmodell | 4h | - | - | 4h |
| ERP-Integration | 3h | - | - | 3h |
| CQRS Handler | 3h | - | - | 3h |
| API Endpoints | 2h | - | - | 2h |
| Pinia Store | - | 2h | - | 2h |
| Vue Komponenten | - | 3h | - | 3h |
| Integration | - | 1h | 1h | 2h |
| Tests | 2h | 2h | 1h | 5h |
| **Gesamt** | **14h** | **8h** | **2h** | **24h** |

**Gesamt: 3 Tage mit 2 Entwicklern (parallel arbeiten)**

---

## ✅ Akzeptanzkriterien

- [ ] Bestandskunde kann sich mit Kundennummer registrieren (< 2 Min)
- [ ] E-Mail wird automatisch aus ERP geladen
- [ ] Duplikate werden erkannt und verhindert
- [ ] Fehlerbehandlung ist robust
- [ ] API antwortet < 500ms
- [ ] Alle Unit Tests grün
- [ ] E2E Tests bestehen
- [ ] Code-Review erfolgreich

---

## 📞 Supportquellen

- **Scaffold-Code**: [BESTANDSKUNDEN_IMPLEMENTIERUNGS_SCAFFOLD.md](../features/BESTANDSKUNDEN_IMPLEMENTIERUNGS_SCAFFOLD.md)
- **Spezifikation**: [BESTANDSKUNDEN_VEREINFACHTE_REGISTRIERUNG.md](../features/BESTANDSKUNDEN_VEREINFACHTE_REGISTRIERUNG.md)
- **Copilot Instructions**: `.github/copilot-instructions.md`
- **Architecture**: `docs/architecture/DDD_BOUNDED_CONTEXTS.md`

---

**Start-Zeit:** Montag, KW 2 2026  
**End-Zeit:** Mittwoch, KW 2 2026  
**Status:** 🟢 Ready to Implement
