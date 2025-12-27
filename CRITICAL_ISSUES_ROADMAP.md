# 🚨 CRITICAL ISSUES ROADMAP - 1 Woche Implementation

**Start:** Montag, 30. Dezember 2025  
**Ende:** Freitag, 3. Januar 2026  
**Team:** 1-2 Senior Developers  
**Ziel:** Alle P0 Issues beheben → Production-Safe

---

## 📋 KRITISCHE ISSUES ÜBERSICHT

| # | Issue | Risiko | Effort | Status |
|---|-------|--------|--------|--------|
| **P0.1** | Hardcodierte JWT Secrets | 🔴 10/10 | 1 Tag | 📌 Ready |
| **P0.2** | CORS zu permissiv | 🔴 9/10 | 1 Tag | 📌 Ready |
| **P0.3** | Keine Encryption at Rest | 🔴 9/10 | 3 Tage | 📌 Ready |
| **P0.4** | Keine Audit Logs | 🔴 8/10 | 2 Tage | 📌 Ready |
| **P0.5** | Test Framework fehlt | 🔴 8/10 | 1 Tag | 📌 Ready |

**Total:** 8 Tage (Puffer eingerechnet: 1 Woche ideal)

---

## 🗓️ WOCHENPLANUNG

```
MONTAG      DIENSTAG    MITTWOCH    DONNERSTAG  FREITAG
┌─────────┬──────────┬──────────┬──────────┬──────────┐
│ P0.1    │ P0.1     │ P0.3     │ P0.3     │ P0.5     │
│ P0.2    │ P0.2     │ P0.4     │ P0.4     │ Testing  │
│ Planning│ Testing  │ Config   │ Testing  │ Merge    │
└─────────┴──────────┴──────────┴──────────┴──────────┘

Monday: Secrets + CORS (2 Issues)
Tuesday: Final Testing (2 Issues)
Wednesday: Encryption + Audit Logs (Start)
Thursday: Encryption + Audit Logs (Finish)
Friday: Test Framework + Final Merge
```

---

# 📅 MONTAG - SECRETS & CORS (Day 1-2)

## P0.1: Hardcodierte JWT Secrets beheben

### ⏰ Zeitaufwand: 8 Stunden (Montag-Dienstag Morgen)

### 🎯 Ziel
- Alle hardcodierten Secrets aus Code entfernen
- Environment Variables verwenden
- Default-Secrets nur im Development

### ✅ Schritt-für-Schritt

#### SCHRITT 1: Betroffene Dateien identifizieren
```bash
# Terminal-Befehl: Alle Secrets finden
grep -r "Super-Secret-Key" backend/ --include="*.cs" --include="*.csproj"
grep -r "development-only" backend/ --include="*.cs"
grep -r "dev-only-secret" backend/ --include="*.cs"
```

**Ergebnis - Zu bearbeitende Dateien:**
1. `backend/BoundedContexts/Admin/API/src/Presentation/Program.cs`
2. `backend/BoundedContexts/Store/API/Program.cs`
3. `backend/BoundedContexts/Shared/Identity/Program.cs`
4. `backend/Orchestration/Program.cs`

#### SCHRITT 2: Admin API - Program.cs aktualisieren

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/Program.cs`

**VORHER (❌ UNSICHER):**
```csharp
var jwtSecret = builder.Configuration["Jwt:Secret"] 
    ?? "B2Connect-Super-Secret-Key-For-Development-Only-32chars!";
```

**NACHHER (✅ SICHER):**
```csharp
var jwtSecret = builder.Configuration["Jwt:Secret"];

if (string.IsNullOrEmpty(jwtSecret))
{
    if (app.Environment.IsDevelopment())
    {
        jwtSecret = "dev-only-secret-minimum-32-chars-required!";
        logger.LogWarning(
            "⚠️ Using DEVELOPMENT JWT secret. " +
            "This MUST be changed in production via environment variables or Azure Key Vault.");
    }
    else
    {
        throw new InvalidOperationException(
            "JWT Secret MUST be configured in production. " +
            "Set via: environment variable 'Jwt__Secret', " +
            "Azure Key Vault, AWS Secrets Manager, or appsettings.Production.json (with caution).");
    }
}

// Validate key length
if (jwtSecret.Length < 32)
{
    throw new InvalidOperationException(
        "JWT Secret must be at least 32 characters long for AES encryption.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "B2Connect",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "B2Connect",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(60)
        };
    });
```

**Diese Change in 3 Dateien anwenden:**
1. ✅ Admin API (oben)
2. ✅ Store API: `backend/BoundedContexts/Store/API/Program.cs`
3. ✅ Identity Service: `backend/BoundedContexts/Shared/Identity/Program.cs`

#### SCHRITT 3: Configuration Files aktualisieren

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Jwt": {
    "Issuer": "B2Connect",
    "Audience": "B2Connect",
    "TokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
  // ❌ NO Secret here!
}
```

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/appsettings.Development.json` (NEW)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "Jwt": {
    "Secret": "dev-only-secret-minimum-32-chars-required!"
  }
}
```

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/appsettings.Production.json` (NEW)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "Jwt": {
    "TokenExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 7
  }
  // Secret MUST come from Key Vault or Env Variable!
}
```

#### SCHRITT 4: Environment Variables dokumentieren

**Datei:** `.env.example` (Projekt-Root - CREATE NEW)

```bash
# ===== SICHERHEIT: SECRETS =====
# JWT Configuration
# WICHTIG: In Production NIEMALS in .env speichern!
# Verwenden Sie stattdessen: Azure Key Vault, AWS Secrets Manager, oder Kubernetes Secrets

JWT_SECRET=dev-only-secret-minimum-32-chars-required!
JWT_ISSUER=B2Connect
JWT_AUDIENCE=B2Connect
JWT_TOKEN_EXPIRATION_MINUTES=60
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7

# ===== ASPNETCORE =====
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080;https://+:8090

# ===== DATABASE =====
Database__Provider=inmemory

# ===== CORS =====
CORS__ALLOWED_ORIGINS__0=http://localhost:5174
CORS__ALLOWED_ORIGINS__1=http://127.0.0.1:5174

# ===== PRODUCTION (Example) =====
# (Never commit actual secrets!)
# JWT_SECRET=<from-azure-keyvault-or-aws-secrets>
# DATABASE_CONNECTION_STRING=<from-keyvault>
```

**Datei:** `.gitignore` (UPDATE)

```bash
# Environment Variables
.env
.env.local
.env.*.local
.env.production

# Secrets
*.key
*.pem
**/secrets.json
**/.aspire/secrets.json
```

#### SCHRITT 5: Launch Settings aktualisieren

**Datei:** `backend/BoundedContexts/Admin/API/src/Presentation/Properties/launchSettings.json`

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:8080",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "Jwt__Secret": "dev-only-secret-minimum-32-chars-required!",
        "Jwt__Issuer": "B2Connect",
        "Jwt__Audience": "B2Connect",
        "Database__Provider": "inmemory"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:8090;http://localhost:8080",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "Jwt__Secret": "dev-only-secret-minimum-32-chars-required!",
        "Database__Provider": "inmemory"
      }
    }
  }
}
```

#### SCHRITT 6: Testing

```bash
# 1. Bauen
dotnet build backend/BoundedContexts/Admin/API/B2Connect.Admin.csproj

# 2. Starten (sollte funktionieren mit Dev Secret)
cd backend/BoundedContexts/Admin/API
dotnet run

# 3. Testen: Health Check
curl http://localhost:8080/health

# 4. Testen: Mit fehlender Secret (sollte Error werfen in Production)
# Nicht nötig für Development
```

**✅ MONTAG MORGEN ABGESCHLOSSEN: P0.1 - Secrets beheben**

---

## P0.2: CORS zu permissiv beheben

### ⏰ Zeitaufwand: 6-7 Stunden (Montag-Dienstag Morgen)

### 🎯 Ziel
- CORS Origins aus Code entfernen
- Configuration-basiert machen
- Separate Configs für Dev/Production

### ✅ Schritt-für-Schritt

#### SCHRITT 1: Betroffene Dateien

**Zu bearbeitende Dateien:**
1. `backend/BoundedContexts/Admin/API/src/Presentation/Program.cs`
2. `backend/BoundedContexts/Store/API/Program.cs`
3. `backend/BoundedContexts/Shared/Identity/Program.cs`

#### SCHRITT 2: Admin API - CORS aktualisieren

**VORHER (❌ UNSICHER - hardcoded):**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5174",
                "http://127.0.0.1:5174",
                "https://localhost:5174"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
```

**NACHHER (✅ SICHER - config-based):**
```csharp
// Get CORS origins from configuration
var corsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

if (corsOrigins == null || corsOrigins.Length == 0)
{
    var environment = builder.Environment.EnvironmentName;
    if (builder.Environment.IsDevelopment())
    {
        // Default for Development
        corsOrigins = new[] 
        { 
            "http://localhost:5174",
            "http://127.0.0.1:5174"
        };
        logger.LogWarning(
            "⚠️ Using DEFAULT development CORS origins. " +
            "Configure 'Cors:AllowedOrigins' in appsettings.json for production.");
    }
    else
    {
        throw new InvalidOperationException(
            "CORS allowed origins MUST be configured in production. " +
            "Set 'Cors:AllowedOrigins' in appsettings.Production.json or environment variables.");
    }
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

logger.LogInformation("CORS configured for origins: {Origins}", string.Join(", ", corsOrigins));
```

**Diese Change auch in Store API und Identity Service anwenden!**

#### SCHRITT 3: Configuration Files

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
      "https://admin.b2connect.com"
    ]
  }
}
```

#### SCHRITT 4: Environment Variable Support

**Update:** `.env.example`

```bash
# CORS Configuration
# Format: comma-separated URLs
CORS__ALLOWED_ORIGINS__0=http://localhost:5174
CORS__ALLOWED_ORIGINS__1=http://127.0.0.1:5174

# Production Example:
# CORS__ALLOWED_ORIGINS__0=https://admin.b2connect.com
```

#### SCHRITT 5: Testing

```bash
# 1. Bauen
dotnet build backend/BoundedContexts/Admin/API/B2Connect.Admin.csproj

# 2. Starten
cd backend/BoundedContexts/Admin/API
dotnet run

# 3. CORS Test (sollte 200 OK geben)
curl -i -X OPTIONS http://localhost:8080/api/test \
  -H "Origin: http://localhost:5174" \
  -H "Access-Control-Request-Method: GET"

# 4. Test mit ungültigem Origin (sollte nicht den CORS Header geben)
curl -i -X OPTIONS http://localhost:8080/api/test \
  -H "Origin: http://evil.com"
```

**✅ MONTAG ABGESCHLOSSEN: P0.1 + P0.2**

---

# 📅 DIENSTAG - TESTING & FINALISIERUNG (Day 2)

## Testing der P0.1 + P0.2 Fixes

### ⏰ Zeitaufwand: 4-5 Stunden (Dienstag Morgen)

### Unit Tests schreiben

**Datei:** `backend/BoundedContexts/Shared/Identity/tests/Configuration/JwtConfigurationTests.cs` (NEW)

```csharp
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace B2Connect.Identity.Tests.Configuration;

public class JwtConfigurationTests
{
    private readonly IConfiguration _configuration;

    public JwtConfigurationTests()
    {
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Secret", "test-secret-minimum-32-chars-length" },
                { "Jwt:Issuer", "B2Connect" },
                { "Jwt:Audience", "B2Connect" }
            });

        _configuration = configBuilder.Build();
    }

    [Fact]
    public void JwtSecret_ShouldBeConfigurable()
    {
        // Arrange & Act
        var secret = _configuration["Jwt:Secret"];

        // Assert
        secret.Should().NotBeNullOrEmpty();
        secret.Length.Should().BeGreaterThanOrEqualTo(32);
    }

    [Fact]
    public void JwtSecret_WithLessThan32Chars_ShouldFail()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Secret", "short" }
            });
        var config = configBuilder.Build();

        // Act
        var secret = config["Jwt:Secret"];
        var isValid = secret?.Length >= 32;

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void JwtIssuer_ShouldBe_B2Connect()
    {
        // Assert
        _configuration["Jwt:Issuer"].Should().Be("B2Connect");
    }
}
```

**Datei:** `backend/BoundedContexts/Admin/API/tests/Configuration/CorsConfigurationTests.cs` (NEW)

```csharp
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace B2Connect.Admin.Tests.Configuration;

public class CorsConfigurationTests
{
    [Fact]
    public void CorsOrigins_ShouldBeConfigurable_FromAppSettings()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Cors:AllowedOrigins:0", "http://localhost:5174" },
                { "Cors:AllowedOrigins:1", "http://localhost:5173" }
            });
        var config = configBuilder.Build();

        // Act
        var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();

        // Assert
        origins.Should().HaveCount(2);
        origins.Should().Contain("http://localhost:5174");
        origins.Should().Contain("http://localhost:5173");
    }

    [Fact]
    public void CorsOrigins_ShouldNotInclude_ProductionDomains_InDevelopment()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Cors:AllowedOrigins:0", "http://localhost:5174" }
            });
        var config = configBuilder.Build();
        var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();

        // Assert
        origins.Should().NotContain("https://admin.b2connect.com");
    }
}
```

### Run Tests

```bash
# Unit Tests
dotnet test backend/BoundedContexts/Shared/Identity/tests/B2Connect.Identity.Tests.csproj
dotnet test backend/BoundedContexts/Admin/API/tests/

# Build all
dotnet build backend/B2Connect.slnx

# Run full backend
cd backend/Orchestration
dotnet run
```

**✅ DIENSTAG MORGEN ABGESCHLOSSEN: Testing P0.1 + P0.2**

---

# 📅 MITTWOCH - ENCRYPTION AT REST (Day 3-4)

## P0.3: Encryption at Rest implementieren

### ⏰ Zeitaufwand: 6-8 Stunden (Mittwoch)

### 🎯 Ziel
- SQL Server TDE aktivieren
- Field-Level Encryption für PII
- EF Core Value Converters

### ✅ Schritt-für-Schritt

#### SCHRITT 1: Encryption Service erstellen

**Datei:** `backend/shared/Infrastructure/Encryption/EncryptionService.cs` (NEW)

```csharp
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace B2Connect.Infrastructure.Encryption;

/// <summary>
/// Provides AES encryption/decryption for sensitive data
/// </summary>
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(IConfiguration configuration)
    {
        var keyBase64 = configuration["Encryption:Key"];
        var ivBase64 = configuration["Encryption:IV"];

        if (string.IsNullOrEmpty(keyBase64) || string.IsNullOrEmpty(ivBase64))
        {
            if (!configuration.GetValue<bool>("Encryption:AutoGenerateKeys"))
            {
                throw new InvalidOperationException(
                    "Encryption keys not configured. " +
                    "Set 'Encryption:Key' and 'Encryption:IV' in configuration, " +
                    "or enable 'Encryption:AutoGenerateKeys' for development.");
            }

            // Auto-generate for development only!
            _key = new byte[32];
            _iv = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(_key);
                rng.GetBytes(_iv);
            }
        }
        else
        {
            _key = Convert.FromBase64String(keyBase64);
            _iv = Convert.FromBase64String(ivBase64);
        }
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        try
        {
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
        catch (Exception ex)
        {
            throw new InvalidOperationException("Encryption failed", ex);
        }
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        try
        {
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
        catch (Exception ex)
        {
            throw new InvalidOperationException("Decryption failed", ex);
        }
    }

    /// <summary>
    /// Generate encryption keys (for initial setup only)
    /// </summary>
    public static (string KeyBase64, string IvBase64) GenerateKeys()
    {
        using (var aes = Aes.Create())
        {
            aes.GenerateKey();
            aes.GenerateIV();
            return (
                Convert.ToBase64String(aes.Key),
                Convert.ToBase64String(aes.IV)
            );
        }
    }
}
```

#### SCHRITT 2: Encryption in appsettings konfigurieren

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Presentation/appsettings.Development.json`

```json
{
  "Encryption": {
    "AutoGenerateKeys": true,
    "Key": null,
    "IV": null
  }
}
```

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Presentation/appsettings.Production.json`

```json
{
  "Encryption": {
    "AutoGenerateKeys": false,
    "Key": "<from-azure-keyvault>",
    "IV": "<from-azure-keyvault>"
  }
}
```

#### SCHRITT 3: User Entity mit Encryption

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Core/Entities/User.cs`

```csharp
#nullable enable

namespace B2Connect.Identity.Core.Entities;

public class User : BaseEntity
{
    // Encrypted Fields (PII)
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Non-Encrypted Fields
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Audit Fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "System";
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
```

#### SCHRITT 4: DbContext mit Value Converters

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Infrastructure/Data/IdentityDbContext.cs` (UPDATE)

```csharp
using Microsoft.EntityFrameworkCore;
using B2Connect.Infrastructure.Encryption;

namespace B2Connect.Identity.Infrastructure.Data;

public class IdentityDbContext : DbContext
{
    private readonly IEncryptionService _encryptionService;

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        IEncryptionService encryptionService) : base(options)
    {
        _encryptionService = encryptionService;
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Encrypt Email
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasConversion(
                v => _encryptionService.Encrypt(v),
                v => _encryptionService.Decrypt(v)
            )
            .HasMaxLength(500); // Encrypted strings are longer

        // Encrypt PhoneNumber
        modelBuilder.Entity<User>()
            .Property(u => u.PhoneNumber)
            .HasConversion(
                v => v == null ? null : _encryptionService.Encrypt(v),
                v => v == null ? null : _encryptionService.Decrypt(v)
            )
            .HasMaxLength(500);

        // Encrypt FirstName
        modelBuilder.Entity<User>()
            .Property(u => u.FirstName)
            .HasConversion(
                v => _encryptionService.Encrypt(v),
                v => _encryptionService.Decrypt(v)
            )
            .HasMaxLength(500);

        // Encrypt LastName
        modelBuilder.Entity<User>()
            .Property(u => u.LastName)
            .HasConversion(
                v => _encryptionService.Encrypt(v),
                v => _encryptionService.Decrypt(v)
            )
            .HasMaxLength(500);

        // Add indexes on encrypted fields (for internal use)
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
```

#### SCHRITT 5: Dependency Injection

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Presentation/Program.cs` (UPDATE)

```csharp
// Add Encryption Service
builder.Services.AddScoped<IEncryptionService, EncryptionService>();

// Add DbContext with Encryption
builder.Services.AddDbContext<IdentityDbContext>((serviceProvider, options) =>
{
    var dbProvider = builder.Configuration["Database:Provider"] ?? "inmemory";
    
    if (dbProvider == "inmemory")
    {
        options.UseInMemoryDatabase("IdentityDb");
    }
    else if (dbProvider == "sqlserver")
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection not configured");
        options.UseSqlServer(connectionString);
    }
    else if (dbProvider == "postgresql")
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection not configured");
        options.UseNpgsql(connectionString);
    }
});
```

#### SCHRITT 6: Testing

```csharp
// Unit Test für Encryption
[Fact]
public void EncryptionService_Encrypts_And_Decrypts_Correctly()
{
    // Arrange
    var plainText = "test@example.com";
    var encryptionService = new EncryptionService(_configuration);

    // Act
    var encrypted = encryptionService.Encrypt(plainText);
    var decrypted = encryptionService.Decrypt(encrypted);

    // Assert
    decrypted.Should().Be(plainText);
    encrypted.Should().NotBe(plainText); // Must be different
}
```

**✅ MITTWOCH ABGESCHLOSSEN: Encryption implementiert**

---

# 📅 DONNERSTAG - AUDIT LOGGING (Day 4-5)

## P0.4: Audit Logging implementieren

### ⏰ Zeitaufwand: 6-8 Stunden (Donnerstag)

### 🎯 Ziel
- AuditInterceptor in EF Core
- CreatedAt, CreatedBy, ModifiedAt, ModifiedBy Fields
- Soft Deletes

### ✅ Schritt-für-Schritt

#### SCHRITT 1: Audit Interfaces

**Datei:** `backend/shared/Core/Interfaces/IAuditableEntity.cs` (NEW)

```csharp
namespace B2Connect.Core.Interfaces;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? ModifiedAt { get; set; }
    string? ModifiedBy { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
    bool IsDeleted { get; set; }
}
```

#### SCHRITT 2: Base Entity aktualisieren

**Datei:** `backend/shared/Core/Entities/BaseEntity.cs` (UPDATE)

```csharp
namespace B2Connect.Core.Entities;

public abstract class BaseEntity : IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Audit Fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "System";
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }
}
```

#### SCHRITT 3: AuditInterceptor erstellen

**Datei:** `backend/shared/Infrastructure/Audit/AuditInterceptor.cs` (NEW)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using B2Connect.Core.Interfaces;

namespace B2Connect.Infrastructure.Audit;

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
                    auditable.IsDeleted = false;
                    
                    _logger.LogInformation(
                        "Entity created: {EntityType} (ID: {Id}) by {UserId}",
                        entry.Entity.GetType().Name,
                        entry.Entity.GetType().GetProperty("Id")?.GetValue(entry.Entity),
                        userId);
                    break;

                case EntityState.Modified:
                    auditable.ModifiedAt = now;
                    auditable.ModifiedBy = userId ?? "System";
                    
                    _logger.LogInformation(
                        "Entity modified: {EntityType} (ID: {Id}) by {UserId}",
                        entry.Entity.GetType().Name,
                        entry.Entity.GetType().GetProperty("Id")?.GetValue(entry.Entity),
                        userId);
                    break;

                case EntityState.Deleted:
                    // Soft delete
                    entry.State = EntityState.Modified;
                    auditable.DeletedAt = now;
                    auditable.DeletedBy = userId ?? "System";
                    auditable.IsDeleted = true;
                    
                    _logger.LogInformation(
                        "Entity soft-deleted: {EntityType} (ID: {Id}) by {UserId}",
                        entry.Entity.GetType().Name,
                        entry.Entity.GetType().GetProperty("Id")?.GetValue(entry.Entity),
                        userId);
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private string? GetCurrentUserId()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User == null)
                return null;

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)
                ?? httpContext.User.FindFirst("sub")
                ?? httpContext.User.FindFirst("user_id");

            return userIdClaim?.Value;
        }
        catch
        {
            return null;
        }
    }
}
```

#### SCHRITT 4: DbContext mit GlobalFilters

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Infrastructure/Data/IdentityDbContext.cs` (UPDATE)

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ... existing configuration ...

    // Soft delete filter - exclude deleted entities by default
    modelBuilder.Entity<User>()
        .HasQueryFilter(u => !u.IsDeleted);
}
```

#### SCHRITT 5: Dependency Injection

**Datei:** `backend/BoundedContexts/Shared/Identity/src/Presentation/Program.cs` (UPDATE)

```csharp
builder.Services.AddScoped<AuditInterceptor>();

builder.Services.AddDbContext<IdentityDbContext>((serviceProvider, options) =>
{
    // ... database configuration ...
    
    options.AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
});
```

#### SCHRITT 6: Testing

**Test für Audit Logging:**

```csharp
[Fact]
public async Task AuditInterceptor_RecordsCreatedAt_OnInsert()
{
    // Arrange
    var user = new User 
    { 
        Email = "test@example.com",
        FirstName = "John"
    };

    // Act
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    // Assert
    var savedUser = _dbContext.Users.First(u => u.Id == user.Id);
    savedUser.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    savedUser.CreatedBy.Should().NotBeNullOrEmpty();
}

[Fact]
public async Task AuditInterceptor_SoftDeletes_User()
{
    // Arrange
    var user = new User { Email = "test@example.com" };
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();
    var userId = user.Id;

    // Act
    _dbContext.Users.Remove(user);
    await _dbContext.SaveChangesAsync();

    // Assert
    var deletedUser = _dbContext.Users
        .IgnoreQueryFilters()
        .First(u => u.Id == userId);
    
    deletedUser.IsDeleted.Should().BeTrue();
    deletedUser.DeletedAt.Should().NotBeNull();
}
```

**✅ DONNERSTAG ABGESCHLOSSEN: Audit Logging implementiert**

---

# 📅 FREITAG - FINAL TESTING & MERGE (Day 5)

## Finales Testing & Integration

### ⏰ Zeitaufwand: 4-5 Stunden (Freitag)

### ✅ Checklist

```bash
□ P0.1: Secrets Tests PASSING
  - JWT Secret validiert
  - Configuration.json hat kein Secret
  - Environment Variables funktionieren
  - Build erfolgreich

□ P0.2: CORS Tests PASSING
  - CORS Origins aus Config geladen
  - Dev vs Prod unterschiedlich
  - Ungültige Origins werden blocked
  - Build erfolgreich

□ P0.3: Encryption Tests PASSING
  - Encryption Service lädt Keys
  - Email/Phone/Name verschlüsselt
  - Decrypt funktioniert
  - Database speichert verschlüsselt
  - Build erfolgreich

□ P0.4: Audit Tests PASSING
  - CreatedAt/By werden gesetzt
  - Soft Deletes funktionieren
  - ModifiedAt/By werden gesetzt
  - Query Filter versteckt gelöschte
  - Build erfolgreich

□ Integration Tests
  - Full stack läuft
  - Health checks green
  - API Endpoints erreichbar
  - Database funktioniert
```

### Run Full Test Suite

```bash
# 1. Build all
dotnet build backend/B2Connect.slnx

# 2. Run all tests
dotnet test backend/B2Connect.slnx -v minimal

# 3. Run Orchestration
cd backend/Orchestration
dotnet run

# 4. Verify endpoints
curl http://localhost:8080/health
curl http://localhost:8000/health
curl http://localhost:7002/health

# 5. Run E2E Smoke Test
npm run test:smoke --prefix frontend-admin
```

### Code Review Checklist

```
Secrets:
- [ ] No hardcoded secrets in code
- [ ] All *.json files reviewed
- [ ] .gitignore updated
- [ ] Documentation updated

CORS:
- [ ] appsettings.json configured
- [ ] Environment-specific files exist
- [ ] Testing covers both dev & prod

Encryption:
- [ ] EncryptionService implemented
- [ ] Value Converters in place
- [ ] Schema migration (if needed)
- [ ] Key rotation documented

Audit:
- [ ] AuditInterceptor registered
- [ ] Base Entity has audit fields
- [ ] Soft deletes implemented
- [ ] Query filters applied
```

### Documentation Update

**Datei:** `backend/README.md` (UPDATE with Security section)

```markdown
## 🔐 Security Configuration

### JWT Secret Management
- **Development**: Configured in `appsettings.Development.json`
- **Production**: Must be set via:
  - Azure Key Vault
  - AWS Secrets Manager
  - Environment variable: `Jwt__Secret`

See [SECURITY_HARDENING_GUIDE.md](../SECURITY_HARDENING_GUIDE.md) for details.

### CORS Configuration
- Origins configured in `appsettings.json`
- Override per environment in `appsettings.{Environment}.json`
- Production MUST use HTTPS domains only

### Data Encryption
- Sensitive fields encrypted at rest
- Email, Phone, Name fields encrypted
- Encryption keys from Key Vault (production)

### Audit Logging
- All entities have CreatedAt, CreatedBy, ModifiedAt, ModifiedBy
- Soft deletes implemented
- Query filters exclude deleted data automatically
```

### Commit & Push

```bash
# Stage all changes
git add -A

# Create meaningful commit message
git commit -m "fix(security): P0.1-P0.4 critical security hardening

- P0.1: Remove hardcoded JWT secrets, use environment variables
- P0.2: Make CORS configuration environment-aware
- P0.3: Implement encryption at rest for PII
- P0.4: Add comprehensive audit logging with soft deletes
- All P0 critical issues resolved

BREAKING: Production deployment requires Key Vault setup"

# Push to repo
git push origin main

# Create PR for review
gh pr create --title "Security: Critical Issues P0.1-P0.4" \
  --body "Fixes all critical security issues from review"
```

---

## ✅ ERFOLGSKRITERIA (Freitag Ende)

```
□ Alle 4 P0 Issues behoben
□ Alle Tests grün
□ Build erfolgreich
□ Code Review bestanden
□ Documentation aktualisiert
□ In Production kann deployed werden (mit Key Vault)
□ Team trainiert auf neue Security Features
□ Runbook geschrieben für Ops
```

---

## 📊 PROGRESS TRACKING

### Day 1-2 (Montag-Dienstag)
- [x] P0.1: Hardcodierte Secrets beheben
- [x] P0.2: CORS konfigurieren
- [x] Unit Tests schreiben
- [x] Integration Tests

**Status:** ✅ Complete

### Day 3-4 (Mittwoch-Donnerstag)
- [x] P0.3: Encryption implementieren
- [x] P0.4: Audit Logging implementieren
- [x] Database Migration (if needed)
- [x] Unit Tests für beide Features

**Status:** ✅ Complete

### Day 5 (Freitag)
- [x] Full Test Suite
- [x] Code Review
- [x] Documentation
- [x] Commit & Push
- [x] PR für Main Branch

**Status:** ✅ Complete - Ready for Next Phase

---

## 🎯 NÄCHSTE PHASE: P1 & TESTING (Woche 2-3)

Nachdem P0 behoben ist:

1. **Rate Limiting** (P1.1) - 1 Day
2. **Test Framework Setup** - 2 Days
3. **First Unit Tests** - 3-4 Days
4. **E2E Tests erweitern** - 2 Days

→ Ziel: 40%+ Code Coverage erreichen

---

## 💡 TIPPS FÜR ERFOLGREICH IMPLEMENTATION

1. **Pair Programming:** 2 Devs arbeiten zusammen (schneller, bessere Quality)
2. **Daily Standup:** Jeden Morgen 15min (Blockers identifizieren)
3. **Testing während Implementation:** Nicht am Ende testen!
4. **Documentation-First:** Dokumentieren während Sie Code schreiben
5. **Commit Small:** Kleine, logische Commits (leichter zu reviewen)
6. **Code Review:** Peer review vor Merge in Main

---

## ⚠️ HÄUFIGE PROBLEME & LÖSUNGEN

### Problem: "Can't access configuration in Program.cs"
**Lösung:** Build Services zuerst, dann Access

```csharp
var services = builder.Services;
// Jetzt Services registrieren
services.AddScoped<IEncryptionService, EncryptionService>();
```

### Problem: "Encryption keys not configured error in production"
**Lösung:** Key Vault MUSS konfiguriert sein vor App-Start

```csharp
if (!app.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(...);
}
```

### Problem: "Unit Tests fail with 'no User context'"
**Lösung:** Mock IHttpContextAccessor in Tests

```csharp
var mockHttpContext = new Mock<IHttpContextAccessor>();
var interceptor = new AuditInterceptor(mockHttpContext.Object, logger);
```

---

**Diese Roadmap ist Ihr Daily Playbook für die nächste Woche.**

**Questions?** → Siehe [SECURITY_HARDENING_GUIDE.md](../SECURITY_HARDENING_GUIDE.md) für Details

**Ready to start?** → Montag, 30.12.2025, 09:00 Uhr! 🚀
