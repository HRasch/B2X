# 🟡 P1 High-Priority Issues - Implementation Plan

**Gesamtstatus**: ⏳ Ready for Implementation  
**Geschätzter Aufwand**: 4-5 Tage  
**Priorität nach**: P0 (eben abgeschlossen)

---

## 📋 P1 Issues Overview

| # | Issue | Risiko | Aufwand | Status |
|---|-------|--------|---------|--------|
| **P1.1** | Keine Rate Limiting | 🟠 7/10 | 1-2 Tage | 📌 Ready |
| **P1.2** | HTTPS nicht erzwungen | 🟠 8/10 | 0.5 Tage | 📌 Ready |
| **P1.3** | Keine Security Headers | 🟠 6/10 | 0.5 Tage | 📌 Ready |
| **P1.4** | Unzureichende Input Validation | 🟠 7/10 | 1-2 Tage | 📌 Ready |
| **P1.5** | Sensitive Data in Logs | 🟠 6/10 | 0.5 Tage | 📌 Ready |

**Total**: ~4-5 Tage

---

## 🚨 P1.1: Keine Rate Limiting

### Risiko-Bewertung
- **Schweregrad**: HIGH (7/10)
- **CVSS Score**: 6.5
- **CWE**: CWE-307 (Improper Restriction of Input)
- **Angriffsszenario**: Brute-Force auf Login-Endpoint

### Problem
```
❌ Auth-Endpoints haben KEINE Rate-Limiting
❌ 10.000 Login-Versuche in Sekunden möglich
❌ Anfällig für Brute-Force und DDoS
```

### Attack Demo
```bash
# Brute-force: 10.000 Versuche ohne Schutz
for i in {1..10000}; do
  curl -X POST http://localhost:8080/api/auth/login \
    -d '{"email":"admin@b2connect.com","password":"attempt'$i'"}' &
done

# Resultat: Erfolgreicher Login ohne Verzögerung
```

### Lösung
Implementierung mit `AspNetCoreRateLimit` NuGet Package:

**Schritt 1**: NuGet Package installieren
```bash
dotnet add package AspNetCoreRateLimit
```

**Schritt 2**: Configuration in `appsettings.json`
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "HttpStatusCode": 429,
    "RealIpHeader": "X-Real-IP",
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      },
      {
        "Endpoint": "POST:/api/auth/login",
        "Period": "5m",
        "Limit": 5
      },
      {
        "Endpoint": "POST:/api/auth/register",
        "Period": "1h",
        "Limit": 3
      }
    ],
    "EndpointWhitelist": ["GET:/health", "GET:/swagger"]
  }
}
```

**Schritt 3**: Program.cs aktualisieren
```csharp
using AspNetCoreRateLimit;

// Add Rate Limiting Services
builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();
builder.Services.Configure<IpRateLimitOptions>(
    builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(
    builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ... weitere Services ...

var app = builder.Build();

// Use Rate Limiting Middleware (BEFORE routing)
app.UseIpRateLimiting();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
```

**Schritt 4**: Endpoints markieren
```csharp
app.MapPost("/api/auth/login", LoginAsync)
    .RequireRateLimiting("auth-limit")
    .WithName("Login")
    .WithOpenApi();
```

---

## 🔒 P1.2: HTTPS nicht erzwungen

### Risiko-Bewertung
- **Schweregrad**: HIGH (8/10)
- **CVSS Score**: 7.4
- **CWE**: CWE-295 (Improper SSL/TLS Verification)
- **Angriffsszenario**: Man-in-the-Middle (MITM)

### Problem
```
❌ Keine HSTS Header
❌ Keine HTTPS Umleitung
❌ Browser erkennt HTTP als sicher
```

### Attack Demo
```
1. Angreifer intercepted HTTP Request
2. Modifiziert Response um malicious Script zu injizieren
3. Stiehlt Session Tokens und Credentials
4. Ohne HSTS = Browser versucht HTTP wieder nächstes Mal
```

### Lösung

**Program.cs aktualisieren**:
```csharp
// Add HTTPS Redirection
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(options =>
    {
        options.MaxAge(TimeSpan.FromDays(365));
        options.IncludeSubDomains();
        options.Preload();
    });
    
    app.UseHttpsRedirection();
}

// Add Security Headers
app.Use(async (context, next) =>
{
    if (!context.Request.IsHttps && !app.Environment.IsDevelopment())
    {
        context.Response.Redirect(
            "https://" + context.Request.Host + context.Request.Path);
        return;
    }
    
    await next();
});
```

**launchSettings.json** (Development):
```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:8090;http://localhost:8080",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## 🛡️ P1.3: Keine Security Headers

### Risiko-Bewertung
- **Schweregrad**: MEDIUM-HIGH (6/10)
- **OWASP**: Top 10 - A04:2021

### Problem
```
❌ X-Content-Type-Options: nosniff (fehlend)
❌ X-Frame-Options: DENY (fehlend)
❌ Content-Security-Policy (fehlend)
❌ X-XSS-Protection (fehlend)
❌ Referrer-Policy (fehlend)
```

### Lösung

**Middleware für Security Headers** (neue Datei):
```csharp
// File: backend/shared/B2Connect.Shared.Infrastructure/Middleware/SecurityHeadersMiddleware.cs

namespace B2Connect.Infrastructure.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // X-Content-Type-Options: Prevent MIME type sniffing
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

        // X-Frame-Options: Prevent Clickjacking
        context.Response.Headers.Add("X-Frame-Options", "DENY");

        // X-XSS-Protection: Legacy XSS protection
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

        // Referrer-Policy: Control referrer information
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

        // Content-Security-Policy: Prevent XSS, Clickjacking, etc.
        var csp = "default-src 'self'; " +
                  "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                  "style-src 'self' 'unsafe-inline'; " +
                  "img-src 'self' data: https:; " +
                  "font-src 'self' data:; " +
                  "connect-src 'self' https:; " +
                  "frame-ancestors 'none'; " +
                  "base-uri 'self'; " +
                  "form-action 'self'";

        context.Response.Headers.Add("Content-Security-Policy", csp);

        // Permissions-Policy: Restrict browser features
        context.Response.Headers.Add(
            "Permissions-Policy", 
            "geolocation=(), microphone=(), camera=(), payment=()"
        );

        _logger.LogDebug("Security headers applied");

        await _next(context);
    }
}
```

**In Program.cs registrieren**:
```csharp
using B2Connect.Infrastructure.Middleware;

// Add Security Headers Middleware (EARLY - vor anderen Middleware)
app.UseMiddleware<SecurityHeadersMiddleware>();

// ... andere Middleware ...
app.UseAuthentication();
app.UseAuthorization();
```

---

## ✓ P1.4: Unzureichende Input Validation

### Risiko-Bewertung
- **Schweregrad**: HIGH (7/10)
- **OWASP**: Top 10 - A03:2021 (Injection)

### Problem
```
❌ Keine zentralisierte Input Validation
❌ Anfällig für SQL Injection
❌ XSS durch unsanitized Input
```

### Lösung - FluentValidation

**Schritt 1**: NuGet installieren
```bash
dotnet add package FluentValidation.AspNetCore
```

**Schritt 2**: Validator erstellen
```csharp
// File: backend/BoundedContexts/Shared/Identity/src/Application/Validators/LoginRequestValidator.cs

namespace B2Connect.Identity.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase")
            .Matches(@"[0-9]").WithMessage("Password must contain digit");
    }
}
```

**Schritt 3**: In Program.cs registrieren
```csharp
builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssembly(typeof(Program).Assembly);
```

**Schritt 4**: Im Controller verwenden
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login(
    [FromBody] LoginRequest request,
    IValidator<LoginRequest> validator)
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
        return BadRequest(validationResult.Errors);

    // ... process login ...
}
```

---

## 📝 P1.5: Sensitive Data in Logs

### Risiko-Bewertung
- **Schweregrad**: MEDIUM (6/10)
- **Compliance**: GDPR, PCI-DSS

### Problem
```
❌ Passwords in Logs
❌ Credit Card Data in Logs
❌ PII (Email, Phone) unmasked
❌ API Keys in Debug Logs
```

### Lösung - Serilog Enrichers

**Schritt 1**: Sensitive Data Enricher
```csharp
// File: backend/shared/B2Connect.Shared.Infrastructure/Logging/SensitiveDataEnricher.cs

namespace B2Connect.Infrastructure.Logging;

public class SensitiveDataEnricher : ILogEventEnricher
{
    private static readonly string[] SensitiveKeys = new[]
    {
        "password", "secret", "token", "apikey", "creditcard",
        "ssn", "email", "phone", "firstname", "lastname"
    };

    public void Enrich(LogEvent logEvent, ILogEventPropertyValueFactory propertyValueFactory)
    {
        if (logEvent.Properties == null)
            return;

        var sensitiveFound = false;
        foreach (var key in logEvent.Properties.Keys.ToList())
        {
            if (SensitiveKeys.Any(sk => 
                key.Contains(sk, StringComparison.OrdinalIgnoreCase)))
            {
                logEvent.Properties[key] = 
                    new ScalarValue("[REDACTED]");
                sensitiveFound = true;
            }
        }

        if (sensitiveFound)
        {
            logEvent.MessageTemplate = 
                new TextValueFormatter().Format(logEvent.MessageTemplate);
        }
    }
}
```

**Schritt 2**: In Serilog konfigurieren
```csharp
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .Enrich.With<SensitiveDataEnricher>()
        .WriteTo.Console(
            outputTemplate: 
                "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .ReadFrom.Configuration(context.Configuration);
});
```

---

## 📊 Implementation Timeline

### Tag 1: Rate Limiting & HTTPS
- [ ] P1.1: Rate Limiting Service konfigurieren
- [ ] P1.1: Endpoints markieren
- [ ] P1.2: HSTS + HTTPS Redirection
- [ ] P1.2: Tests schreiben

### Tag 2: Security Headers & Logging
- [ ] P1.3: SecurityHeadersMiddleware erstellen
- [ ] P1.3: In alle APIs registrieren
- [ ] P1.5: SensitiveDataEnricher implementieren
- [ ] P1.5: Tests für Log Redaction

### Tag 3: Input Validation
- [ ] P1.4: FluentValidation installieren
- [ ] P1.4: Validator für Key Endpoints schreiben
- [ ] P1.4: AutoValidation in Program.cs
- [ ] P1.4: Tests

### Tag 4: Integration & Testing
- [ ] Alle P1 Issues testen
- [ ] E2E Tests für Rate Limiting
- [ ] Load Testing
- [ ] Dokumentation aktualisieren

### Tag 5: Code Review & Deployment
- [ ] Code Review durchführen
- [ ] Fixes aus Review
- [ ] Staging Deployment
- [ ] Production Deployment

---

## ✅ Acceptance Criteria

### P1.1: Rate Limiting
- [ ] Login-Endpoint erlaubt max 5 Versuche pro 5 Minuten
- [ ] Andere Endpoints: 100 pro Minute
- [ ] 429 Response bei Limit-Überschreitung
- [ ] Rate-Limit Headers in Response
- [ ] Tests: Verify Blocking nach Limit

### P1.2: HTTPS
- [ ] HSTS Header present (max-age: 31536000)
- [ ] HTTP redirects zu HTTPS
- [ ] Development: HTTP erlaubt
- [ ] Production: HTTPS only

### P1.3: Security Headers
- [ ] X-Content-Type-Options: nosniff
- [ ] X-Frame-Options: DENY
- [ ] CSP: default-src 'self'
- [ ] Verifizierbar mit Browser DevTools

### P1.4: Input Validation
- [ ] Email validation: RFC 5322
- [ ] Password: Min 8 Chars, Upper+Lower+Digit
- [ ] Keine SQL Injection möglich
- [ ] Keine XSS durch Input möglich

### P1.5: Sensitive Data Logging
- [ ] Passwords: [REDACTED]
- [ ] Emails: [REDACTED]
- [ ] Tokens: [REDACTED]
- [ ] Verifizierbar in Log Viewer

---

## 🚀 Getting Started

Starten Sie mit **P1.1 Rate Limiting** - das hat höchste Priorität für Sicherheit.

Dann die anderen P1 Issues in Reihenfolge umsetzen.

Alle P1 Issues sollten bis Ende dieser Woche erledigt sein!

---

**Nächster Schritt**: `dotnet add package AspNetCoreRateLimit` zum ersten API hinzufügen und P1.1 beginnen! 🚀
