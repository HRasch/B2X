# 🔧 Security Hardening - Implementierungs-Guide

Dieser Guide behandelt die kritischen Sicherheitsprobleme und deren Lösungen.

## 🔴 P0.1: Hardcodierte JWT Secrets

### Problem
```csharp
// ❌ AKTUELL - GEFÄHRLICH
var jwtSecret = builder.Configuration["Jwt:Secret"] 
    ?? "B2X-Super-Secret-Key-For-Development-Only-32chars!";
```

Dieser Default-Secret könnte in Production verwendet werden!

### Lösung

#### Schritt 1: Program.cs aktualisieren

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/Program.cs`
**Datei:** `backend/BoundedContexts/Store/API/Program.cs`
**Datei:** `backend/BoundedContexts/Shared/Identity/Program.cs`

```csharp
// ✅ KORREKT
var jwtSecret = builder.Configuration["Jwt:Secret"];

if (string.IsNullOrEmpty(jwtSecret))
{
    if (app.Environment.IsDevelopment())
    {
        jwtSecret = "dev-only-secret-32-chars-min-length!";
        logger.LogWarning("⚠️ Using development JWT secret. This must be changed in production!");
    }
    else
    {
        throw new InvalidOperationException(
            "JWT Secret must be configured via environment variables, " +
            "Azure Key Vault, or AWS Secrets Manager in production.");
    }
}

// Key length validation
if (jwtSecret.Length < 32)
{
    throw new InvalidOperationException("JWT Secret must be at least 32 characters long");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "B2X",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "B2X",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(60) // 60 Sekunden Toleranz
        };
    });
```

#### Schritt 2: appsettings.json aktualisieren

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "Jwt": {
    "Issuer": "B2X",
    "Audience": "B2X",
    "TokenExpiration": 3600,
    "RefreshTokenExpiration": 604800
  }
}
```

#### Schritt 3: appsettings.Production.json erstellen

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "Jwt": {
    "Issuer": "B2X",
    "Audience": "B2X",
    "TokenExpiration": 900,
    "RefreshTokenExpiration": 604800
  }
}
```

#### Schritt 4: Environment Variables dokumentieren

**Datei:** `.env.example` (Projekt-Root)

```bash
# ===== DEVELOPMENT =====
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080

# JWT Configuration
JWT_SECRET=dev-secret-32-chars-minimum-required!
JWT_ISSUER=B2X
JWT_AUDIENCE=B2X

# Database
Database__Provider=inmemory

# CORS
CORS__ALLOWED_ORIGINS=http://localhost:5173,http://localhost:5174

# ===== PRODUCTION =====
# (Never commit actual production values!)
# JWT_SECRET=<from-key-vault>
# DATABASE_CONNECTION_STRING=<from-key-vault>
```

#### Schritt 5: Azure Key Vault Integration (Optional)

```csharp
// In Program.cs
if (!app.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}

// Oder AWS Secrets Manager
if (!app.Environment.IsDevelopment())
{
    var region = RegionEndpoint.USEast1;
    var secretsManager = new AmazonSecretsManagerClient(region);
    
    var secret = await secretsManager.GetSecretValueAsync(
        new GetSecretValueRequest 
        { 
            SecretId = "B2X/jwt-secret" 
        });
    
    jwtSecret = secret.SecretString;
}
```

---

## 🔴 P0.2: CORS zu permissiv

### Problem
```csharp
// ❌ AKTUELL
policy.WithOrigins(
    "http://localhost:5173",
    "http://127.0.0.1:5173",
    "https://localhost:5173"
);
```

### Lösung

#### Schritt 1: Configuration-basierte CORS

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/Program.cs`

```csharp
// ✅ KORREKT
var corsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

if (corsOrigins == null || corsOrigins.Length == 0)
{
    throw new InvalidOperationException("CORS allowed origins not configured");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminFrontend", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition", "X-Total-Count")
            .WithMaxAge(TimeSpan.FromHours(24));
    });
});
```

#### Schritt 2: appsettings nach Umgebung

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/appsettings.Development.json`

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5174",
      "http://127.0.0.1:5174",
      "http://localhost:3000"
    ]
  }
}
```

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/appsettings.Production.json`

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://admin.B2X.com"
    ]
  }
}
```

#### Schritt 3: CORS in launchSettings.json

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/Properties/launchSettings.json`

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "Cors__AllowedOrigins__0": "http://localhost:5174",
        "Cors__AllowedOrigins__1": "http://127.0.0.1:5174"
      }
    }
  }
}
```

---

## 🔴 P0.3: Keine Encryption at Rest

### Problem
Sensitive Daten (Emails, Adressen, Zahlungsdaten) sind in der Datenbank unverschlüsselt.

### Lösung: Transparent Data Encryption (TDE)

#### Schritt 1: SQL Server TDE aktivieren

```sql
-- Für SQL Server
CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'StrongPassword123!';

CREATE CERTIFICATE TDE_Cert
WITH SUBJECT = 'TDE Certificate';

CREATE ASYMMETRIC KEY TDE_Key
ENCRYPTION BY CERTIFICATE TDE_Cert;

ALTER DATABASE B2X
SET ENCRYPTION ON;
```

#### Schritt 2: Entity Framework Field-Level Encryption

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Core/Entities/User.cs`

```csharp
#nullable enable

namespace B2X.Identity.Core.Entities;

public class User : BaseEntity
{
    [Encrypted] // Custom attribute für Encryption
    public string Email { get; set; } = string.Empty;
    
    [Encrypted]
    public string? PhoneNumber { get; set; }
    
    [Encrypted]
    public string FirstName { get; set; } = string.Empty;
    
    [Encrypted]
    public string LastName { get; set; } = string.Empty;
    
    // Non-encrypted fields
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
```

#### Schritt 3: Encryption Service

**Datei:** `backend/shared/Infrastructure/Encryption/EncryptionService.cs`

```csharp
using System.Security.Cryptography;
using System.Text;

namespace B2X.Infrastructure.Encryption;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(IConfiguration config)
    {
        var keyBase64 = config["Encryption:Key"] 
            ?? throw new InvalidOperationException("Encryption key not configured");
        var ivBase64 = config["Encryption:IV"]
            ?? throw new InvalidOperationException("Encryption IV not configured");
        
        _key = Convert.FromBase64String(keyBase64);
        _iv = Convert.FromBase64String(ivBase64);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        using (var aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        using (var aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}
```

#### Schritt 4: EF Core Value Converter

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Infrastructure/Data/IdentityDbContext.cs`

```csharp
public class IdentityDbContext : DbContext
{
    private readonly IEncryptionService _encryptionService;

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, 
        IEncryptionService encryptionService) : base(options)
    {
        _encryptionService = encryptionService;
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Verschlüssle Email
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasConversion(
                v => _encryptionService.Encrypt(v),
                v => _encryptionService.Decrypt(v)
            );

        // Verschlüssle PhoneNumber
        modelBuilder.Entity<User>()
            .Property(u => u.PhoneNumber)
            .HasConversion(
                v => v == null ? null : _encryptionService.Encrypt(v),
                v => v == null ? null : _encryptionService.Decrypt(v)
            );
    }
}
```

#### Schritt 5: appsettings.json

```json
{
  "Encryption": {
    "Key": "YOUR_BASE64_ENCRYPTED_KEY_HERE",
    "IV": "YOUR_BASE64_IV_HERE"
  }
}
```

---

## 🔴 P0.4: Keine Audit Logging

### Lösung

**Datei:** `backend/shared/Infrastructure/Audit/AuditInterceptor.cs`

```csharp
#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

namespace B2X.Infrastructure.Audit;

public interface IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditInterceptor> _logger;

    public AuditInterceptor(IHttpContextAccessor httpContextAccessor, 
        ILogger<AuditInterceptor> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var userId = GetCurrentUserId();
        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditableEntity auditable)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    auditable.CreatedAt = now;
                    auditable.CreatedBy = userId ?? "System";
                    _logger.LogInformation(
                        "Entity created: {EntityType} by {UserId}",
                        entry.Entity.GetType().Name,
                        userId);
                    break;

                case EntityState.Modified:
                    auditable.ModifiedAt = now;
                    auditable.ModifiedBy = userId ?? "System";
                    _logger.LogInformation(
                        "Entity modified: {EntityType} by {UserId}",
                        entry.Entity.GetType().Name,
                        userId);
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    auditable.DeletedAt = now;
                    auditable.DeletedBy = userId ?? "System";
                    _logger.LogInformation(
                        "Entity soft-deleted: {EntityType} by {UserId}",
                        entry.Entity.GetType().Name,
                        userId);
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private string? GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
            return null;

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)
            ?? httpContext.User.FindFirst("sub");

        return userIdClaim?.Value;
    }
}
```

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Core/Entities/User.cs`

```csharp
using B2X.Infrastructure.Audit;

namespace B2X.Identity.Core.Entities;

public class User : BaseEntity, IAuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "System";
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
```

**Dependency Injection in Program.cs:**

```csharp
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddDbContext<IdentityDbContext>((sp, options) =>
{
    options.UseSqlServer(connectionString);
    options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
});
```

---

## 🟡 P1.1: Rate Limiting

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Presentation/Program.cs`

```csharp
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();

builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.ClientIdHeader = "X-Client-ID";
    options.GeneralRules = new List<RateLimitRule>
    {
        new()
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100,  // 100 requests per minute
            CounterKey = "{0}"
        }
    };
    
    options.EndpointWhitelist = new List<string> { "GET:/health" };
});

builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

var app = builder.Build();

app.UseIpRateLimiting();

// Custom rate limiter for login endpoint
app.MapPost("/api/auth/login", async context =>
{
    var rateLimitService = context.RequestServices.GetRequiredService<IIpRateLimitService>();
    var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var key = $"login:{clientId}";
    
    var counter = await rateLimitService.CheckLoginAttemptAsync(clientId);
    if (counter > 5)  // Max 5 login attempts per minute
    {
        context.Response.StatusCode = 429;
        await context.Response.WriteAsJsonAsync(new { error = "Too many login attempts" });
        return;
    }
    
    // Login logic...
});
```

---

## Checkpoint: Alle P0 Issues behoben?

```bash
✅ JWT Secrets über Environment Variables
✅ CORS nach Umgebung konfiguriert
✅ Encryption at Rest implementiert
✅ Audit Logging aktiviert
✅ Rate Limiting für kritische Endpoints
```

**Nächste Schritte:** Testing & GDPR Implementation

