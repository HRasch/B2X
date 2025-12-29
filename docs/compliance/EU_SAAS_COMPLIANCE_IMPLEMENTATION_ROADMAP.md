# 🔒 EU SaaS Compliance Implementation Roadmap

**Version:** 1.0 | **Status:** Active | **Last Updated:** 28. Dezember 2025

---

## Executive Summary

B2Connect ist eine **Multi-Tenant SaaS Plattform für 100+ europäische Shops** mit 1.000+ gleichzeitigen Nutzern. Diese Roadmap übersetzt **EU Compliance-Anforderungen (NIS2, GDPR, DORA, eIDAS)** in konkrete technische Implementierungsschritte.

**Philosophie:** Technische Lösung zuerst → dann Business Features. Ohne funktionierende Compliance-Infrastruktur kein vertrauenswürdiges SaaS Produkt.

---

## 📋 Regulatory Framework Overview

### Applicable Regulations

| Regulation | Scope | Deadline | Impact | Priority |
|-----------|-------|----------|--------|----------|
| **NIS2** | Supply Chain Security, Incident Response | Oct 2025* | Service Disruption Liability | 🔴 P0 |
| **GDPR** | Data Protection, Privacy, Right to Forget | Active | €20M+ Fines | 🔴 P0 |
| **DORA** | Operational Resilience, ICT Risk | Jan 2025 | Service Continuity | 🔴 P0 |
| **eIDAS 2.0** | Digital Signatures, Electronic ID | Nov 2024 | Legal Validity | 🟡 P1 |
| **TISAX** | Telecom Security (DE/AT) | Ongoing | Enterprise Contracts | 🟡 P1 |
| **NIS Directive** | Legacy (pre-NIS2) | Phased Out | Transition to NIS2 | 🟢 P2 |
| **EU AI Act** | AI Risk Management, Transparency, Audit | €30M fines for high-risk violations | 🔴 P0 |

*NIS2 Umstellung: Phase 1 Oct 2025 (große Unternehmen), Phase 2 Oct 2026 (alle anderen)  
*AI Act: Ab 12. Mai 2026 (Compliance Window jetzt offen für Planung)

---

## 🛍️ E-Commerce Legal Compliance (B2B & B2C)

### Regulatory Framework für Online-Handel in der EU

| Regulation | Scope | Impact | Priority |
|-----------|-------|--------|----------|
| **PAngV** (Preisangaben-VO) | Transparente Preisangabe, Rabatte | Unzulässige Werbung möglich | 🔴 P0 |
| **VVVG** (Verbraucher-VO) | 14-Tage Widerrufsrecht, AGB | Bußgelder 5.000-300.000€ | 🔴 P0 |
| **TMG** (Telemediengesetz) | Impressum, Datenschutz, NutzerInnen-Daten | Sperrung möglich | 🔴 P0 |
| **AStV** (Umsatzsteuer-Systemrichtlinie) | Reverse Charge (B2B), VAT-ID-Validierung | Nachzahlung + Strafzins | 🔴 P0 |
| **VerpackG** (Verpackungsgesetz) | Registrierungspflicht, Rückgabepflicht | Zertifizierung erforderlich | 🔴 P0 |
| **BattG** (Batteriegesetz) | Altbatterien-Rückgabe | Rückgabestellen pflicht | 🟡 P1 |
| **ElektroG** (E-Schrott-Gesetz) | Elektroschrott-Rückgabe | Rückgabestellen pflicht | 🟡 P1 |
| **ODR-VO** (Online Dispute Resolution) | Außergerichtliche Streitbeilegung | Kontakt zu Schlichter pflicht | 🔴 P0 |
| **eIDAS** (Electronic IDentification) | Digitale Signaturen, eSeals | Rechtsverbindlichkeit | 🟡 P1 |

### B2B vs B2C Spezifische Anforderungen

#### B2C (Business to Consumer)
```
Widerrufsrecht:
  ✅ 14 Tage Widerrufsrecht (ab Lieferung)
  ✅ Widerrufsformular bereitstellen
  ✅ Retouren-Versandkosten-Regelung
  ✅ Rückerstattung innerhalb 14 Tage

Preisangabe:
  ✅ Endpreis (inklusive MwSt)
  ✅ Versandkosten sichtbar (vor Checkout)
  ✅ Rabatte müssen gekennzeichnet sein
  ✅ Ursprungspreis sichtbar wenn rabattiert

Verbraucherschutz:
  ✅ AGB akzeptieren (Checkbox vor Bestellung)
  ✅ Datenschutzerklärung verlinkt
  ✅ Impressum verlinkt
  ✅ E-Mail-Adresse für Support sichtbar

Rechnungen:
  ✅ Rechnungsnummer eindeutig pro Shop
  ✅ Rechnungsdaten 10 Jahre archiviert
  ✅ Digitale Signatur auf Rechnungen möglich
```

#### B2B (Business to Business)
```
Umsatzsteuer:
  ✅ VAT-ID-Validierung (VIES-Lookup)
  ✅ Reverse Charge (Verkäufer zahlt keine MwSt wenn Käufer VAT-ID hat)
  ✅ Steuernummern validieren
  ✅ Intra-EU-Lieferung korrekt abrechnen

Geschäftsbedingungen:
  ✅ AGB angepasst für B2B (andere Gewährleistung)
  ✅ Zahlungsbedingungen flexibel (z.B. 30 Tage)
  ✅ Lieferbedingungen (INCOTERMS) wählbar
  ✅ Mindestbestellmenge möglich

Rechnungen:
  ✅ Umsatzsteuer-ID statt Name/Adresse
  ✅ Rechnungsnummer eindeutig
  ✅ PDF-Export & eInvoicing (UBL/ZUGFeRD)
  ✅ EDI-Integration für große Partner

Dokumentation:
  ✅ Export-Dokumente (für EU-externe Länder)
  ✅ Zollanmeldungen automatisch
  ✅ EORI-Nummern validieren
```

---

## 🎯 Implementation Strategy

### Phasing Approach

```
┌─────────────────────────────────────────────────────────┐
│ COMPLIANCE-FIRST vs. FEATURE-FIRST                      │
├─────────────────────────────────────────────────────────┤
│ WIR WÄHLEN: COMPLIANCE-FIRST (with parallel features)   │
│                                                         │
│ Phase 0: Compliance Foundation (parallel to MVP)        │
│   → Audit Logging, Encryption, Incident Response       │
│   → THEN Phase 1 Features können sicher deployed werden │
│                                                         │
│ Phase 1: MVP Features + Compliance Integration          │
│   → Each Feature = Feature Code + Security Code         │
│   → Deployment nur wenn Compliance ✅                    │
│                                                         │
│ Phase 2: Scale with Built-in Compliance                │
│   → Auto-Scaling respects Security Boundaries           │
│   → Compliance Monitoring at Scale                      │
│                                                         │
│ Phase 3: Advanced Compliance                            │
│   → Penetration Testing Automation                      │
│   → Compliance Certification (ISO27001, SOC2)           │
└─────────────────────────────────────────────────────────┘
```

---

## 📍 PHASE 0: COMPLIANCE FOUNDATION (Weeks 1-6, Parallel to MVP)

### Overview

Diese Phase legt die **Sicherheits-Infrastruktur** für alle folgenden Phasen. Ohne diese Grundlage können keine Features sicher deployed werden.

**Team:** 1-2 Security Engineers + 1 DevOps Engineer  
**Duration:** 6 Wochen (parallel zu MVP Backend)  
**Go/No-Go Gate:** Alle P0 Items ✅ bevor Phase 1 Features deployiert werden

---

### 🔓 P0.1: Audit Logging System (Immutable, Encrypted)

**Regulatory Driver:** NIS2 Art. 21, GDPR Art. 32(c), DORA Art. 6

**Business Impact:** 
- ❌ Ohne Audit Logs: NIS2 Non-Compliance, Enterprise Contracts unmöglich
- ✅ Mit Audit Logs: Incident Response nachweisbar, GDPR Right-to-Audit erfüllt

**Technical Requirements:**

| Requirement | Details | Acceptance Criteria |
|-------------|---------|-------------------|
| **All CRUD Operations Logged** | CREATE, UPDATE, DELETE (not READ initially) | 100% coverage of business entities |
| **Immutability** | Once written, logs cannot be modified/deleted | Tamper detection on retrieval |
| **Encryption at Rest** | AES-256-GCM with rotating keys | No plaintext logs on disk |
| **Retention** | Minimum 7 years for audit purposes | Configurable per tenant |
| **Tenant Isolation** | Logs cannot leak across tenants | Tenant filter on all log queries |
| **Performance Impact** | < 10ms overhead per operation | Measured at 100 req/sec load |
| **SIEM Integration** | Export to centralized logging | Syslog or JSON feed available |

**Implementation Tasks:**

```csharp
// 1. Create AuditLogEntry Entity
namespace B2Connect.Shared.Infrastructure.Audit;

public class AuditLogEntry
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }        // Tenant Isolation
    public Guid UserId { get; set; }           // WHO made the change
    public string EntityType { get; set; }     // Product, User, Order, etc.
    public string EntityId { get; set; }       // Which specific record
    public string Action { get; set; }         // CREATE, UPDATE, DELETE
    public string? OldValues { get; set; }     // JSON snapshot before
    public string? NewValues { get; set; }     // JSON snapshot after
    public DateTime CreatedAt { get; set; }    // IMMUTABLE timestamp
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    
    // Security: Hash of all fields for tamper detection
    public string Hash { get; set; }           // SHA-256(all fields)
}

// 2. Create AuditInterceptor (EF Core)
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IAuditLogService _auditService;
    private readonly ITenantContext _tenantContext;
    private readonly IUserContext _userContext;
    
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct)
    {
        var context = eventData.Context!;
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();
        
        foreach (var entry in entries)
        {
            await _auditService.LogAsync(
                tenantId: _tenantContext.TenantId,
                userId: _userContext.UserId,
                entityType: entry.Entity.GetType().Name,
                entityId: GetEntityId(entry),
                action: entry.State.ToString(),
                oldValues: SerializeValues(entry.OriginalValues),
                newValues: SerializeValues(entry.CurrentValues)
            );
        }
        
        return await base.SavingChangesAsync(eventData, result, ct);
    }
}

// 3. Register in DI
builder.Services.AddScoped<IAuditInterceptor, AuditInterceptor>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Add to all DbContexts:
builder.Services.AddScoped(sp => {
    var options = new DbContextOptionsBuilder<MyDbContext>();
    options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
    return new MyDbContext(options.Options);
});
```

**Testing Acceptance Criteria:**

```csharp
[Fact]
public async Task CreateProduct_LogsAuditEntry()
{
    // Arrange
    var product = new Product { Sku = "TEST-001", Name = "Test" };
    
    // Act
    await _context.Products.AddAsync(product);
    await _context.SaveChangesAsync();
    
    // Assert - Verify Audit Log Created
    var auditLog = await _context.AuditLogs
        .FirstAsync(x => x.EntityType == "Product" && x.Action == "CREATE");
    
    Assert.NotNull(auditLog);
    Assert.Equal(_currentTenantId, auditLog.TenantId);  // Tenant isolation
    Assert.NotEmpty(auditLog.NewValues);                 // JSON captured
    Assert.NotNull(auditLog.Hash);                       // Tamper detection
}

[Fact]
public async Task AuditLog_CannotBeModified_AfterCreation()
{
    // Arrange
    var log = new AuditLogEntry { /* ... */ };
    await _context.AuditLogs.AddAsync(log);
    await _context.SaveChangesAsync();
    var originalHash = log.Hash;
    
    // Act - Try to modify
    log.Action = "HACKED";
    
    // Assert - Should fail
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(
        () => _context.SaveChangesAsync()
    );
    Assert.Contains("immutable", exception.Message);
}
```

**Definition of Done:**
- [ ] AuditLogEntry Entity implemented
- [ ] EF Core Interceptor logs all CRUD ops
- [ ] Tests: 5+ test cases covering CRUD scenarios
- [ ] Performance: < 10ms overhead per operation
- [ ] Encryption: AES-256 at rest, rotation policy
- [ ] Tenant isolation verified (cross-tenant data leak impossible)
- [ ] SIEM integration prepared (Syslog format defined)

**Effort:** 40 hours (1 Security Engineer, 1 week)

---

### 🔐 P0.2: Encryption at Rest (AES-256-GCM)

**Regulatory Driver:** NIS2 Art. 21(4), GDPR Art. 32(1)(b), DORA Art. 10

**Business Impact:**
- ❌ Ohne Verschlüsselung: Enterprise Kunden lehnen ab ("No encryption = no contract")
- ✅ Mit Verschlüsselung: Compliance Badge für Vertrieb

**PII Fields to Encrypt:**

```csharp
public class User
{
    public Guid Id { get; set; }
    
    // Encrypted Fields (sensitive data)
    public string EmailAddressEncrypted { get; set; }      // encrypted: user@example.com
    public string PhoneNumberEncrypted { get; set; }       // encrypted: +49123456789
    public string FirstNameEncrypted { get; set; }         // encrypted: "John"
    public string LastNameEncrypted { get; set; }          // encrypted: "Doe"
    public string DateOfBirthEncrypted { get; set; }       // encrypted: "1990-01-15"
    public string AddressEncrypted { get; set; }           // encrypted: "Main St. 123"
    
    // Non-encrypted (ok to be visible)
    public string Username { get; set; }                   // visible: "john.doe"
    public bool IsActive { get; set; }                     // visible: true
    public DateTime CreatedAt { get; set; }                // visible: 2025-12-28
}

public class Product
{
    public Guid Id { get; set; }
    
    // Encrypted (cost data)
    public string CostPriceEncrypted { get; set; }        // encrypted: internal cost
    public string SuppliersEncrypted { get; set; }         // encrypted: supplier names
    
    // Non-encrypted (customer-facing)
    public string Sku { get; set; }                        // visible: "PROD-001"
    public string Name { get; set; }                       // visible: "Product Name"
    public decimal PublicPrice { get; set; }               // visible: 99.99
}
```

**Implementation Tasks:**

```csharp
// 1. Create Encryption Service
public interface IEncryptionService
{
    string Encrypt(string plaintext);
    string Decrypt(string ciphertext);
    void RotateKey();  // Annually for NIS2 compliance
}

public class AesEncryptionService : IEncryptionService
{
    private readonly string _encryptionKey;
    private readonly string _encryptionIv;
    
    public string Encrypt(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext)) return plaintext;
        
        using (var aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_encryptionKey);
            aes.GenerateIV();  // Random IV per encryption
            
            using (var encryptor = aes.CreateEncryptor())
            using (var ms = new MemoryStream())
            {
                // Write IV to output (needed for decryption)
                ms.Write(aes.IV, 0, aes.IV.Length);
                
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plaintext);
                }
                
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
    
    public string Decrypt(string ciphertext)
    {
        if (string.IsNullOrEmpty(ciphertext)) return ciphertext;
        
        using (var aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_encryptionKey);
            
            var buffer = Convert.FromBase64String(ciphertext);
            aes.IV = buffer.Take(aes.IV.Length).ToArray();
            
            using (var decryptor = aes.CreateDecryptor())
            using (var ms = new MemoryStream(buffer, aes.IV.Length, buffer.Length - aes.IV.Length))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}

// 2. EF Core Value Converters
modelBuilder.Entity<User>()
    .Property(u => u.EmailAddressEncrypted)
    .HasConversion(
        v => _encryptionService.Encrypt(v),
        v => _encryptionService.Decrypt(v)
    );

modelBuilder.Entity<User>()
    .Property(u => u.PhoneNumberEncrypted)
    .HasConversion(
        v => string.IsNullOrEmpty(v) ? null : _encryptionService.Encrypt(v),
        v => string.IsNullOrEmpty(v) ? null : _encryptionService.Decrypt(v)
    );

// 3. Key Rotation Policy
public class KeyRotationService : IHostedService
{
    private readonly IEncryptionService _encryption;
    private readonly ILogger<KeyRotationService> _logger;
    
    public async Task StartAsync(CancellationToken ct)
    {
        // Check monthly, rotate if > 365 days old
        using var timer = new PeriodicTimer(TimeSpan.FromDays(30));
        
        while (await timer.WaitForNextTickAsync(ct))
        {
            if (IsKeyOlderThan(days: 365))
            {
                _logger.LogInformation("Rotating encryption key (annual rotation)");
                await _encryption.RotateKey();
            }
        }
    }
    
    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}

// 4. Register in DI
builder.Services.AddSingleton<IEncryptionService, AesEncryptionService>();
builder.Services.AddHostedService<KeyRotationService>();
```

**Testing Acceptance Criteria:**

```csharp
[Fact]
public void Encrypt_ProducesBase64String()
{
    var plaintext = "sensitive@example.com";
    
    var encrypted = _encryptionService.Encrypt(plaintext);
    
    Assert.NotEqual(plaintext, encrypted);
    Assert.True(IsBase64String(encrypted));
}

[Fact]
public void Decrypt_RecoversOriginalValue()
{
    var plaintext = "sensitive@example.com";
    
    var encrypted = _encryptionService.Encrypt(plaintext);
    var decrypted = _encryptionService.Decrypt(encrypted);
    
    Assert.Equal(plaintext, decrypted);
}

[Fact]
public void SameValue_ProducesDifferentCiphertexts_DueToRandomIV()
{
    var plaintext = "sensitive@example.com";
    
    var encrypted1 = _encryptionService.Encrypt(plaintext);
    var encrypted2 = _encryptionService.Encrypt(plaintext);
    
    Assert.NotEqual(encrypted1, encrypted2);  // Different IV each time
}

[Fact]
public async Task User_SavesWithEncryptedEmail()
{
    var user = new User { EmailAddressEncrypted = "test@example.com" };
    
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    
    // Verify stored as encrypted
    var storedEncrypted = await _context.Users
        .AsNoTracking()
        .Select(u => u.EmailAddressEncrypted)
        .FirstAsync();
    
    Assert.NotEqual("test@example.com", storedEncrypted);
    Assert.True(IsBase64String(storedEncrypted));
}

[Fact]
public async Task QueryingUser_DecryptsAutomatically()
{
    var user = new User { EmailAddressEncrypted = "test@example.com" };
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    
    var retrieved = await _context.Users.FirstAsync();
    
    Assert.Equal("test@example.com", retrieved.EmailAddressEncrypted);
}
```

**Definition of Done:**
- [ ] AesEncryptionService implemented
- [ ] EF Core Value Converters for all PII fields
- [ ] Key Rotation Service (annual rotation)
- [ ] Tests: Encryption, Decryption, different IVs, DB round-trip
- [ ] Performance: < 5ms per encryption operation
- [ ] Key Storage: In Azure KeyVault or Vault (not hardcoded)
- [ ] HTTPS/TLS 1.3 enforced for all API calls

**Effort:** 35 hours (1 Security Engineer, 1 week)

---

### 🚨 P0.3: Incident Response System (< 24h Notification)

**Regulatory Driver:** NIS2 Art. 23, GDPR Art. 33, DORA Art. 19

**Business Impact:**
- ❌ Ohne IRS: Non-compliance fines up to €20M (GDPR)
- ✅ Mit IRS: Automated detection + response < 24h notification

**Incidents to Detect:**

1. **Security Incidents** (NIS2 Art. 23)
   - Unauthorized access attempts (>5 failed logins from same IP)
   - Data exfiltration (unusual volume downloads)
   - Malware/injection attempts (detected via WAF)
   - Privilege escalation (non-admin → admin)

2. **Availability Incidents** (DORA Art. 6)
   - Service down (> 5 minutes)
   - Response time degradation (> 2x baseline)
   - Database failures
   - External service failures (payment gateway, etc.)

3. **Data Protection Incidents** (GDPR Art. 33)
   - PII data access violations
   - Encryption key compromise
   - Audit log tampering

**Implementation Tasks:**

```csharp
// 1. Incident Detection Service
public interface IIncidentDetectionService
{
    Task DetectSecurityIncidentsAsync(CancellationToken ct);
    Task DetectAvailabilityIncidentsAsync(CancellationToken ct);
    Task<IEnumerable<SecurityIncident>> GetIncidentsAsync(
        Guid tenantId,
        DateTime from,
        DateTime to);
}

public class SecurityIncident
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Type { get; set; }              // "UnauthorizedAccess", "DataExfiltration", etc.
    public string Severity { get; set; }           // "Low", "Medium", "High", "Critical"
    public string Description { get; set; }
    public DateTime DetectedAt { get; set; }
    public DateTime? NotifiedAt { get; set; }     // When NIS2 notification was sent
    public string NotificationRef { get; set; }   // Reference number for authorities
    public string Status { get; set; }             // "Detected", "Investigating", "Resolved"
    public string? RootCause { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();  // Additional data
}

// 2. Detection Rules
public class IncidentDetectionRules
{
    private readonly IMetricsService _metrics;
    private readonly IIncidentRepository _incidents;
    private readonly IAlertService _alerts;
    
    public async Task DetectFailedLoginAttackAsync()
    {
        // Alert if > 5 failed logins in 10 minutes from same IP
        var failedLogins = await _metrics.GetFailedLoginsAsync(
            last: TimeSpan.FromMinutes(10),
            groupBy: "IpAddress"
        );
        
        foreach (var ip in failedLogins.Where(x => x.Count > 5))
        {
            await _incidents.CreateAsync(new SecurityIncident
            {
                Type = "BruteForceAttack",
                Severity = "High",
                Description = $"Multiple failed logins from {ip.IpAddress}",
                Context = new { FailedAttempts = ip.Count, IpAddress = ip.IpAddress }
            });
            
            await _alerts.SendAsync(
                recipients: _config.SecurityTeamEmail,
                subject: "🚨 INCIDENT: Potential Brute Force Attack",
                body: $"IP {ip.IpAddress} attempted {ip.Count} failed logins in last 10 minutes"
            );
        }
    }
    
    public async Task DetectDataExfiltrationAsync()
    {
        // Alert if unusual volume of data downloads
        var dailyBaseline = await _metrics.GetAverageDailyDownloadVolumeAsync(
            days: 30
        );
        
        var todayVolume = await _metrics.GetTodayDownloadVolumeAsync();
        
        if (todayVolume > dailyBaseline * 3)  // 3x normal
        {
            await _incidents.CreateAsync(new SecurityIncident
            {
                Type = "DataExfiltration",
                Severity = "Critical",
                Description = $"Abnormal data download volume: {todayVolume} vs {dailyBaseline} baseline",
                Context = new { TodayVolume = todayVolume, Baseline = dailyBaseline }
            });
            
            await _alerts.SendAsync(
                recipients: _config.SecurityTeamEmail,
                subject: "🚨 CRITICAL: Potential Data Exfiltration",
                body: $"Download volume is 3x higher than normal. Immediate investigation required."
            );
        }
    }
}

// 3. NIS2 Notification Service (< 24h)
public class Nis2NotificationService
{
    private readonly INotificationRepository _notifications;
    private readonly ILogger<Nis2NotificationService> _logger;
    
    public async Task NotifySecurityIncidentAsync(
        SecurityIncident incident,
        CancellationToken ct)
    {
        // Log the incident for NIS2 notification
        var notification = new Nis2Notification
        {
            IncidentId = incident.Id,
            TenantId = incident.TenantId,
            Type = incident.Type,
            IncidentOccurredAt = incident.DetectedAt,
            NotificationSentAt = DateTime.UtcNow,
            Recipients = GetCompetentAuthorities(incident.TenantId),
            DocumentationReference = GenerateDocumentation(incident)
        };
        
        await _notifications.CreateAsync(notification, ct);
        
        // Send to authorities (via certified email, etc.)
        await SendToAuthoritiesAsync(notification, ct);
        
        _logger.LogCritical(
            "NIS2 Incident Notification sent: {IncidentId} at {Time}",
            incident.Id,
            notification.NotificationSentAt
        );
    }
    
    private List<string> GetCompetentAuthorities(Guid tenantId)
    {
        // NIS2: Must notify competent authority in country where tenant is located
        var tenant = _tenants.Get(tenantId);
        
        return tenant.Country switch
        {
            "DE" => new() { "bsi@bsi.bund.de" },  // Germany: BSI
            "AT" => new() { "incident@nics.gv.at" },  // Austria
            "FR" => new() { "cert@ssi.gouv.fr" },  // France
            _ => throw new NotSupportedException($"No authority configured for {tenant.Country}")
        };
    }
}

// 4. Register as Hosted Service
builder.Services.AddHostedService<IncidentDetectionService>();
builder.Services.AddScoped<IIncidentDetectionService, IncidentDetectionService>();
builder.Services.AddScoped<Nis2NotificationService>();
```

**Testing Acceptance Criteria:**

```csharp
[Fact]
public async Task DetectBruteForceAttack_When5FailedLoginsIn10Min()
{
    // Arrange
    var ip = "192.168.1.1";
    for (int i = 0; i < 6; i++)
    {
        await RecordFailedLoginAsync(ip);
    }
    
    // Act
    await _detectionService.DetectSecurityIncidentsAsync(CancellationToken.None);
    
    // Assert
    var incident = await _incidents.GetAsync(x => 
        x.Type == "BruteForceAttack" && 
        x.Context["IpAddress"].ToString() == ip
    );
    
    Assert.NotNull(incident);
    Assert.Equal("High", incident.Severity);
}

[Fact]
public async Task NotifyAuthorities_Within24Hours()
{
    // Arrange
    var incident = await CreateSecurityIncidentAsync();
    var detectionTime = incident.DetectedAt;
    
    // Act
    await _notificationService.NotifySecurityIncidentAsync(incident, CancellationToken.None);
    
    // Assert
    var notification = await _notifications.GetAsync(incident.Id);
    Assert.NotNull(notification.NotificationSentAt);
    Assert.True(
        (notification.NotificationSentAt - detectionTime).TotalHours <= 24,
        "NIS2 notification must be sent within 24 hours"
    );
}
```

**Definition of Done:**
- [ ] SecurityIncident entity created
- [ ] Detection rules for brute force, data exfiltration, availability
- [ ] NIS2NotificationService with < 24h timer
- [ ] Tests: Detection accuracy, notification timing
- [ ] Authorities configured per country (DE, AT, FR minimum)
- [ ] Alert channels configured (Email, Slack, PagerDuty)
- [ ] Incident tracking dashboard for management

**Effort:** 45 hours (1 Security Engineer + 1 DevOps, 1.5 weeks)

---

### 🛡️ P0.4: Network Segmentation & DDoS Protection

**Regulatory Driver:** NIS2 Art. 21(3), DORA Art. 10

**Business Impact:**
- ❌ Ohne Segmentation: Breaches spread across all systems
- ✅ Mit Segmentation: Breaches isolated to compromised component

**Architecture:**

```
┌─────────────────────────────────────────────────────────┐
│ LOAD BALANCER (AWS ALB / Azure Load Balancer)           │
│   - DDoS Protection (AWS Shield, Azure DDoS Protection) │
│   - WAF (AWS WAF, Azure WAF)                            │
│   - Rate Limiting (per IP, per tenant)                  │
└─────────────────────────────────────────────────────────┘
         ↓ (only HTTPS/TLS 1.3)
┌─────────────────────────────────────────────────────────┐
│ API GATEWAY (YARP reverse proxy)                        │
│   - Request validation                                  │
│   - Authentication check (JWT)                          │
│   - Tenant routing                                      │
│   - Rate limiting enforcement                          │
└─────────────────────────────────────────────────────────┘
         ↓ (mTLS between services)
┌─────────────────────────────────────────────────────────┐
│ SERVICE LAYER (Microservices)                           │
│   - Identity Service (port 7002, private network only)  │
│   - Catalog Service (port 7005, private network only)   │
│   - Order Service (port 7006, private network only)     │
│                                                         │
│   STORAGE LAYER (Private subnet, no internet access)    │
│   - PostgreSQL (port 5432)                              │
│   - Redis (port 6379)                                   │
│   - Elasticsearch (port 9200)                           │
└─────────────────────────────────────────────────────────┘
```

**Implementation Tasks:**

```yaml
# AWS VPC Configuration
Resource "aws_vpc" "b2connect" {
  cidr_block           = "10.0.0.0/16"
  enable_dns_hostnames = true
}

# Public Subnet (Load Balancer only)
resource "aws_subnet" "public" {
  vpc_id            = aws_vpc.b2connect.id
  cidr_block        = "10.0.1.0/24"
  availability_zone = "eu-central-1a"
}

# Private Subnet (Services)
resource "aws_subnet" "private_services" {
  vpc_id            = aws_vpc.b2connect.id
  cidr_block        = "10.0.2.0/24"
  availability_zone = "eu-central-1a"
}

# Private Subnet (Databases)
resource "aws_subnet" "private_databases" {
  vpc_id            = aws_vpc.b2connect.id
  cidr_block        = "10.0.3.0/24"
  availability_zone = "eu-central-1a"
}

# Security Groups
resource "aws_security_group" "alb" {
  vpc_id = aws_vpc.b2connect.id
  
  # Allow HTTPS from internet
  ingress {
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }
  
  # Allow HTTP→HTTPS redirect
  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }
  
  egress {
    from_port   = 0
    to_port     = 65535
    protocol    = "tcp"
    cidr_blocks = ["10.0.0.0/16"]
  }
}

resource "aws_security_group" "services" {
  vpc_id = aws_vpc.b2connect.id
  
  # Allow from ALB only
  ingress {
    from_port       = 8080
    to_port         = 8080
    protocol        = "tcp"
    security_groups = [aws_security_group.alb.id]
  }
  
  # No inbound from internet (only from ALB)
  # Explicit outbound allowed (to databases, external APIs)
}

resource "aws_security_group" "databases" {
  vpc_id = aws_vpc.b2connect.id
  
  # Allow only from services
  ingress {
    from_port       = 5432
    to_port         = 5432
    protocol        = "tcp"
    security_groups = [aws_security_group.services.id]
  }
  
  # No outbound (databases don't initiate connections)
  egress {
    from_port = -1
    to_port   = -1
    protocol  = "-1"
    cidr_blocks = []  # Explicit deny
  }
}
```

**DDoS & WAF Configuration:**

```csharp
// AWS WAF Rules for B2Connect
public class WafRulesConfiguration
{
    public static IEnumerable<(string Name, string Rule)> GetRules()
    {
        yield return ("RateLimitRule", "Limit to 2000 requests per 5 min per IP");
        yield return ("GeoBlockingRule", "Block non-EU countries");
        yield return ("SqliRule", "Block SQL injection patterns");
        yield return ("XssRule", "Block XSS attempts");
        yield return ("LargeBodyRule", "Block requests > 10MB");
        yield return ("TenantHeaderValidation", "Require valid X-Tenant-ID header");
    }
}

// mTLS Configuration (between services)
public class MtlsConfiguration
{
    // Service → Service: mTLS with client certificates
    // All internal service-to-service communication encrypted
    
    public static void ConfigureServiceCertificates(
        IServiceCollection services,
        IConfiguration config)
    {
        var clientCert = new X509Certificate2(
            config["Certificates:Service:ClientCertPath"],
            config["Certificates:Service:Password"]
        );
        
        var handler = new HttpClientHandler();
        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        handler.ClientCertificates.Add(clientCert);
        handler.ServerCertificateCustomValidationCallback = ValidateServerCertificate;
        
        services.AddHttpClient<ICatalogServiceClient>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://catalog:8080"))
            .ConfigurePrimaryHttpMessageHandler(_ => handler);
    }
}
```

**Definition of Done:**
- [ ] VPC created with 3 subnets (public, private-services, private-databases)
- [ ] Security Groups configured (principle of least privilege)
- [ ] Load Balancer (ALB/Azure LB) in public subnet
- [ ] DDoS Protection enabled (AWS Shield, Azure DDoS)
- [ ] WAF rules deployed
- [ ] mTLS certificates for service-to-service communication
- [ ] Rate limiting rules per IP and per tenant
- [ ] Tests: Traffic isolation, network policy validation

**Effort:** 40 hours (1 DevOps Engineer, 1.5 weeks)

---

### ✅ P0.5: Key Management (Azure KeyVault / Vault)

**Regulatory Driver:** NIS2 Art. 21(4), GDPR Art. 32(1)(a)

**Business Impact:**
- ❌ Ohne Key Management: Keys hardcoded = compromise = all data exposed
- ✅ Mit KeyVault: Rotated keys, audit trails, access control

**Implementation:**

```csharp
// Azure Key Vault Integration
public class KeyVaultConfiguration
{
    public static void AddKeyVaultSecrets(
        IConfigurationBuilder config,
        IHostEnvironment env)
    {
        if (env.IsProduction())
        {
            var kvUrl = config.Build()["KeyVault:Uri"];
            
            var credential = new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    ExcludeVisualStudioCodeCredential = false,
                    ExcludeEnvironmentCredentials = false
                }
            );
            
            config.AddAzureKeyVault(
                new Uri(kvUrl),
                credential
            );
        }
    }
}

// Local Development (use User Secrets, NOT hardcoded)
// In development: dotnet user-secrets init
// Then: dotnet user-secrets set "Encryption:Key" "..."
// Never commit to git!

// Encryption Key Rotation Policy
public class KeyRotationPolicy
{
    // NIS2 requirement: Keys rotated annually (365 days)
    
    public DateTime NextRotationDate { get; set; }  // = Today + 365 days
    public List<(DateTime Date, string Status)> RotationHistory { get; set; }
}
```

**Definition of Done:**
- [ ] Azure KeyVault (or HashiCorp Vault) provisioned
- [ ] All secrets in vault, none hardcoded
- [ ] Key rotation policy documented (annual)
- [ ] Access control: only relevant services can access
- [ ] Audit logging: who accessed what key, when
- [ ] Tests: Key retrieval, rotation, expired key handling

**Effort:** 20 hours (1 DevOps Engineer, 1 week)

---

### 📋 P0.6: E-Commerce Legal Compliance (B2B & B2C)

**Regulatory Driver:** PAngV, VVVG, TMG, AStV, VerpackG, ODR-VO

**Business Impact:**
- ❌ Ohne Legal Compliance: Bußgelder 5.000-300.000€, Abmahnungen, Geschäftstätigkeit unmöglich
- ✅ Mit Legal Compliance: Shops können rechtskonform verkaufen, Enterprise-Kundenverträge möglich

**Technical Requirements (B2C):**

| Requirement | Details | Implementation |
|-------------|---------|----------------|
| **14-Day Withdrawal Right** | Customers have 14 days from delivery to return | Return form, auto-generate return labels |
| **Price Transparency** | Show final price (incl. VAT) + shipping before checkout | Calculate per shipping country |
| **Terms & Conditions** | Must be accepted before purchase | Checkbox + timestamp + audit log |
| **Data Privacy** | Privacy statement + link required | CMS page + footer link |
| **Impressum** | Company info required | Editable per shop in settings |
| **Refund Processing** | Process within 14 days of withdrawal | Automated refund via payment gateway |
| **Invoice Archival** | Store 10 years (per shop) | Encrypted PDF storage + audit trail |
| **Order Confirmation** | Send immediately after purchase | Email with order details + terms |

**Technical Requirements (B2B):**

| Requirement | Details | Implementation |
|-------------|---------|----------------|
| **VAT-ID Validation** | Verify EU VAT-IDs with VIES | API call before checkout |
| **Reverse Charge** | No VAT if buyer has valid VAT-ID | Tax calculation logic |
| **Invoice Format** | Support ZUGFeRD (Germany) + UBL (EU) | XML export + PDF rendering |
| **Payment Terms** | Configurable (Net 30, Net 60, etc.) | Flexible per shop |
| **INCOTERMS** | Support shipping terms (DDP, DDU, etc.) | Selector in shipping config |
| **Minimum Order Qty** | Configurable per product | Validation before checkout |
| **EDI/API Integration** | Support automated order processing | Webhook for large partners |

**Implementation Tasks (P0.6):**

```csharp
// 1. Withdrawal/Return Management
public class WithdrawalManagement
{
    public class ReturnRequest
    {
        public Guid OrderId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int WithdrawalDaysRemaining => 
            14 - (int)(DateTime.Now - DeliveryDate).TotalDays;
        
        public bool IsWithinWithdrawalPeriod => 
            WithdrawalDaysRemaining > 0;
        
        // B2C: Must accept return
        // B2B: Can refuse if agreed in terms
    }
    
    public async Task<ReturnLabel> GenerateReturnLabelAsync(
        ReturnRequest request,
        string carrierCode)
    {
        if (!request.IsWithinWithdrawalPeriod)
            throw new InvalidOperationException("Withdrawal period expired");
        
        var label = await _carrierService.CreateReturnLabelAsync(
            request.OrderId,
            carrierCode
        );
        
        await _auditService.LogAsync(
            action: "ReturnLabelGenerated",
            entityId: request.OrderId,
            changes: new { ReturnLabel = label.Number }
        );
        
        return label;
    }
}

// 2. Price Calculation (Country-Specific)
public class PriceCalculationService
{
    public PriceBreakdown CalculateFinalPrice(
        decimal productPrice,
        string destinationCountry,
        ShippingMethod shippingMethod,
        bool isB2bWithVatId = false)
    {
        var basePrice = productPrice;
        var vatRate = GetVatRate(destinationCountry);
        
        // B2B: Check if reverse charge applies
        decimal vatAmount = 0;
        if (!isB2bWithVatId)
        {
            vatAmount = basePrice * vatRate;
        }
        
        var priceWithVat = basePrice + vatAmount;
        
        var shippingCost = _shippingService.GetCost(
            destinationCountry,
            shippingMethod
        );
        
        var finalPrice = priceWithVat + shippingCost;
        
        return new PriceBreakdown
        {
            ProductPrice = productPrice,
            VatRate = vatRate,
            VatAmount = vatAmount,
            PriceIncludingVat = priceWithVat,
            ShippingCost = shippingCost,
            ShippingCostVisible = true,  // PAngV requirement
            FinalPrice = finalPrice,
            OriginalPrice = null  // Only if discount applied
        };
    }
}

// 3. VAT-ID Validation (B2B)
public class VatIdValidationService
{
    public async Task<VatValidationResult> ValidateVatIdAsync(
        string country,
        string vatId)
    {
        // Call VIES (VAT Information Exchange System)
        var viesResponse = await _viesClient.ValidateAsync(country, vatId);
        
        if (!viesResponse.IsValid)
            throw new InvalidOperationException($"Invalid VAT-ID: {vatId}");
        
        return new VatValidationResult
        {
            IsValid = true,
            CountryCode = viesResponse.CountryCode,
            VatNumber = viesResponse.VatNumber,
            CompanyName = viesResponse.Name,
            CompanyAddress = viesResponse.Address,
            ValidatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(365)  // Re-validate yearly
        };
    }
    
    // Reverse Charge Logic
    public bool ShouldApplyReverseCharge(
        VatValidationResult vatValidation,
        string buyerCountry,
        string sellerCountry)
    {
        // Reverse charge: Seller doesn't charge VAT if buyer is EU business
        // Applies when: buyer has valid VAT-ID AND buyer is in different EU country
        
        return vatValidation.IsValid && 
               buyerCountry != sellerCountry &&
               IsEuCountry(buyerCountry);
    }
}

// 4. Invoice Management (10-Year Archival)
public class InvoiceManagementService
{
    public async Task<Invoice> CreateInvoiceAsync(
        Order order,
        OrderConfirmation confirmation)
    {
        var invoice = new Invoice
        {
            InvoiceNumber = GenerateUniqueInvoiceNumber(order.TenantId),
            OrderId = order.Id,
            TenantId = order.TenantId,
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14),
            
            // Customer info (encrypted)
            CustomerNameEncrypted = order.CustomerNameEncrypted,
            CustomerEmailEncrypted = order.CustomerEmailEncrypted,
            CustomerAddressEncrypted = order.CustomerAddressEncrypted,
            
            // Line items
            LineItems = order.Items.Select(i => new InvoiceLineItem
            {
                ProductSku = i.Sku,
                Description = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.Price,
                VatRate = i.VatRate,
                Total = i.Price * i.Quantity
            }).ToList(),
            
            // Totals
            SubTotal = order.SubTotal,
            VatAmount = order.VatAmount,
            ShippingCost = order.ShippingCost,
            Total = order.Total,
            
            // Signature (eIDAS support)
            DigitalSignature = null,  // Optional for B2C, mandatory for eInvoicing
            SignedAt = null
        };
        
        // Save original (immutable)
        var pdfBytes = await _pdfGenerator.GenerateAsync(invoice);
        await _invoiceStorage.SaveAsync(
            invoiceNumber: invoice.InvoiceNumber,
            tenantId: order.TenantId,
            pdfContent: pdfBytes,
            encrypted: true  // AES-256 encryption
        );
        
        // Audit log
        await _auditService.LogAsync(
            action: "InvoiceCreated",
            entityType: "Invoice",
            entityId: invoice.Id,
            tenantId: order.TenantId,
            changes: new
            {
                InvoiceNumber = invoice.InvoiceNumber,
                Amount = invoice.Total,
                Status = "Created"
            }
        );
        
        // Send to customer
        await _emailService.SendInvoiceAsync(
            order.CustomerEmailEncrypted,
            invoice
        );
        
        return invoice;
    }
    
    // Retention: 10 years per German law (min 6 years GDPR, 10 years German tax law)
    public async Task ArchiveOldInvoicesAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddYears(-10);
        var oldInvoices = await _invoices
            .Where(x => x.CreatedAt < cutoffDate)
            .ToListAsync();
        
        foreach (var invoice in oldInvoices)
        {
            await _archive.MoveAsync(invoice.InvoiceNumber);
            invoice.IsArchived = true;
            invoice.ArchivedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
    }
}

// 5. Shop-Specific Legal Documents
public class LegalDocumentsService
{
    public class ShopLegalDocuments
    {
        public Guid ShopId { get; set; }
        
        // Required
        public string Impressum { get; set; }  // Company info (from shop settings)
        public string PrivacyStatement { get; set; }  // GDPR privacy info
        public string TermsAndConditions { get; set; }  // B2C/B2B specific
        
        // B2C specific
        public string WithdrawalForm { get; set; }  // 14-day return form
        public string ShippingTerms { get; set; }
        
        // B2B specific
        public string B2bTerms { get; set; }  // Modified warranty, payment terms
        public string OrderProcess { get; set; }
        public string PaymentTermsDescription { get; set; }
        
        // Timestamps (immutable)
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedByUserId { get; set; }
        
        // Versioning (for compliance audit)
        public int Version { get; set; }
        public List<DocumentHistory> History { get; set; }
    }
    
    public async Task UpdateLegalDocumentsAsync(
        Guid shopId,
        ShopLegalDocuments documents,
        string updatedByUserId)
    {
        var existing = await _documents.GetAsync(shopId);
        
        if (existing != null)
        {
            // Archive previous version
            existing.History.Add(new DocumentHistory
            {
                Version = existing.Version,
                SavedAt = existing.UpdatedAt,
                SavedByUserId = existing.UpdatedByUserId,
                Content = existing.TermsAndConditions  // JSON snapshot
            });
            existing.Version++;
        }
        
        documents.UpdatedAt = DateTime.UtcNow;
        documents.UpdatedByUserId = updatedByUserId;
        
        await _documents.SaveAsync(documents);
        
        // Customer emails for significant changes
        if (WasSignificantChange(existing, documents))
        {
            await _notificationService.NotifyCustomersAsync(
                shopId,
                "Unsere Geschäftsbedingungen haben sich geändert"
            );
        }
    }
}

// 6. B2B Specific: Payment Terms & INCOTERMS
public class B2bOrderService
{
    public class B2bOrder
    {
        public Guid Id { get; set; }
        public string VatIdValidated { get; set; }
        public bool ReverseChargeApplied { get; set; }
        
        // Payment terms
        public int NetDays { get; set; } = 30;  // Net 30, Net 60, etc.
        public DateTime DueDate => CreatedAt.AddDays(NetDays);
        
        // Shipping terms
        public string Incoterm { get; set; }  // DDP, DDU, CIF, FOB, etc.
        public bool BuyerPaysShipping => Incoterm == "DDU" || Incoterm == "FOB";
        
        // Minimum order
        public decimal MinimumOrderValue { get; set; }
        
        DateTime CreatedAt { get; set; }
    }
    
    public async Task<ValidationResult> ValidateB2bOrderAsync(B2bOrder order)
    {
        var errors = new List<string>();
        
        // Validate VAT-ID (must be valid)
        var vatValidation = await _vatService.ValidateVatIdAsync(
            order.CustomerVatId
        );
        if (!vatValidation.IsValid)
            errors.Add($"Invalid VAT-ID: {order.CustomerVatId}");
        
        // Validate minimum order
        if (order.Total < order.MinimumOrderValue)
            errors.Add($"Order below minimum: {order.MinimumOrderValue}");
        
        // Validate INCOTERMS
        if (!ValidIncoterms.Contains(order.Incoterm))
            errors.Add($"Invalid INCOTERM: {order.Incoterm}");
        
        return new ValidationResult { IsValid = errors.Count == 0, Errors = errors };
    }
}
```

**Definition of Done:**
- [ ] Withdrawal/return system implemented (14-day period)
- [ ] Price calculation per country (VAT rates correct)
- [ ] Terms & Conditions acceptance with audit log
- [ ] Invoice generation & 10-year archival with encryption
- [ ] B2B: VAT-ID validation via VIES API
- [ ] B2B: Reverse charge logic working
- [ ] B2B: Payment terms (Net 30, 60, etc.) configurable
- [ ] B2B: INCOTERMS support (DDP, DDU, CIF, etc.)
- [ ] Tests: 20+ test cases covering all requirements
- [ ] Performance: Invoice generation < 2s

**Effort:** 60 hours (2 Backend Developers, 1.5 weeks)

---

### 🤖 P0.7: AI Act Compliance (Risk Management & Transparency)

**Regulatory Driver:** EU AI Act Art. 6, 8, 22, 35 | **Deadline:** 12. Mai 2026

**Business Impact:** 
- ❌ Ohne AI Act Compliance: Non-compliance fines up to €30M, Geschäftstätigkeit in EU unmöglich
- ✅ Mit AI Act Compliance: Trustworthy AI label, Enterprise contracts mit AI-Anforderungen

**What is "AI" in B2Connect?**

B2Connect nutzt AI möglicherweise für:
1. **Recommendation Engine**: Product recommendations (could be high-risk if affects purchasing decisions)
2. **Search Ranking**: Elasticsearch relevance (could be high-risk if systematically biases results)
3. **Duplicate Detection**: Customer duplicate checking (could be high-risk if blocks legitimate accounts)
4. **Fraud Detection**: Payment fraud detection (HIGH-RISK per AI Act Art. 6)
5. **Dynamic Pricing**: Price optimization per tenant (could affect competition)
6. **Content Moderation**: User review moderation (could be high-risk if silences legitimate reviews)

**AI Act Risk Classification:**

```csharp
public enum AiRiskLevel
{
    Prohibited = 0,           // Banned entirely (e.g., social credit)
    HighRisk = 1,            // Requires full compliance: documentation, testing, monitoring
    LimitedRisk = 2,         // Requires transparency (e.g., "AI-generated")
    MinimalRisk = 3          // General compliance (e.g., recommendation engines)
}

// B2Connect AI Systems Classification:
// - Fraud Detection (AI Act Annex III): HIGH-RISK ⚠️
// - Duplicate Detection: LIMITED-RISK
// - Recommendations: MINIMAL-RISK
// - Content Moderation: LIMITED-RISK (depends on implementation)
// - Dynamic Pricing: MINIMAL-RISK (not safety-critical)
```

**Technical Requirements:**

| Requirement | Details | Acceptance Criteria |
|-------------|---------|-------------------|
| **AI Risk Register** | Catalog all AI systems, risk level assessment | Document for each AI system |
| **High-Risk AI Documentation** | Technical documentation per Art. 11 | Training data, validation results, limitations |
| **Transparency Logs** | Record every AI decision affecting users | User can request log entries |
| **Audit Trail** | Complete history of AI model versions | Model name, version, deployment date |
| **Human Oversight** | Qualified person reviews high-risk outputs | For fraud detection: manual review | 
| **Bias Testing** | Detect discriminatory outcomes | Test across demographic groups |
| **Performance Monitoring** | Track AI system performance degradation | Alert if drift detected (>5%) |
| **User Notification** | Users informed when AI affects them | "This recommendation is AI-powered" |
| **Opt-Out Capability** | Users can disable AI-based decisions (where possible) | Fallback to non-AI method |

**Implementation Tasks:**

```csharp
// 1. AI Risk Register Entity
public class AiSystem
{
    public Guid Id { get; set; }
    public string Name { get; set; }  // e.g., "Fraud Detection Engine"
    public string Description { get; set; }
    public AiRiskLevel RiskLevel { get; set; }
    public string Purpose { get; set; }  // e.g., "Detect payment fraud"
    
    // Documentation
    public string TrainingDataDocumentation { get; set; }  // Source, size, preprocessing
    public string ValidationResultsDocumentation { get; set; }  // Accuracy metrics
    public string LimitationsDocumentation { get; set; }  // Known biases, edge cases
    public string HumanReviewProcedure { get; set; }  // How humans override AI decisions
    
    // Versioning
    public string ModelVersion { get; set; }  // e.g., "v1.2.3-prod"
    public DateTime DeployedAt { get; set; }
    public DateTime? RetiredAt { get; set; }
    
    // Contact
    public string ResponsiblePersonName { get; set; }  // Required per Art. 22
    public string ResponsiblePersonEmail { get; set; }
}

// 2. AI Decision Log (for transparency)
public class AiDecisionLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string AiSystemName { get; set; }
    public string DecisionType { get; set; }  // e.g., "FraudCheck", "ProductRecommendation"
    public object DecisionInput { get; set; }  // What the AI was asked to decide
    public object DecisionOutput { get; set; }  // What the AI decided
    public float ConfidenceScore { get; set; }  // 0.0-1.0
    public bool WasHumanReviewed { get; set; }
    public bool WasHumanOverridden { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // User Right to Explanation (Art. 22)
    public string ExplanationForUser { get; set; }  // "Your payment was flagged due to: unusual amount for this region"
}

// 3. High-Risk AI: Fraud Detection with Audit Trail
public class FraudDetectionAiService
{
    private readonly IFraudDetectionModel _model;
    private readonly IAiDecisionLogRepository _logs;
    private readonly ILogger<FraudDetectionAiService> _logger;
    
    public async Task<FraudCheckResult> CheckTransactionAsync(
        Guid tenantId,
        Transaction transaction,
        string userId)
    {
        var aiRiskScore = await _model.PredictAsync(transaction);  // 0.0-1.0
        
        var decisionLog = new AiDecisionLog
        {
            TenantId = tenantId,
            UserId = userId,
            AiSystemName = "Fraud Detection (v1.2.3)",
            DecisionType = "FraudCheck",
            DecisionInput = new
            {
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                MerchantCountry = transaction.MerchantCountry,
                CustomerHistoricalBehavior = "within normal"
            },
            DecisionOutput = new
            {
                IsFraudulent = aiRiskScore > 0.7,
                RiskScore = aiRiskScore
            },
            ConfidenceScore = aiRiskScore,
            ExplanationForUser = GenerateExplanation(aiRiskScore, transaction),
            CreatedAt = DateTime.UtcNow
        };
        
        // Log decision for transparency
        await _logs.AddAsync(decisionLog);
        
        // For HIGH-RISK decisions (>0.9), flag for human review
        if (aiRiskScore > 0.9)
        {
            decisionLog.WasHumanReviewed = true;
            _logger.LogWarning(
                "HIGH-RISK FRAUD DECISION: {TenantId} / {UserId} / Score: {Score}",
                tenantId, userId, aiRiskScore
            );
            
            // Notify fraud team for manual review
            await _fraudTeamService.NotifyAsync(decisionLog);
        }
        
        return new FraudCheckResult
        {
            IsFraudulent = aiRiskScore > 0.7,
            RiskScore = aiRiskScore,
            RequiresManualReview = aiRiskScore > 0.9,
            DecisionLogId = decisionLog.Id  // User can request explanation
        };
    }
    
    private string GenerateExplanation(float riskScore, Transaction tx)
    {
        var reasons = new List<string>();
        
        if (tx.Amount > 5000)
            reasons.Add("Large amount for this merchant type");
        
        if (tx.MerchantCountry != tx.BillingCountry)
            reasons.Add("Transaction cross-border");
        
        return string.Join(", ", reasons);
    }
}

// 4. Bias Testing (Required for all AI systems)
public class AiBiasTester
{
    public async Task<BiasTestResult> TestForBiasAsync(
        string aiSystemName,
        List<TestDataPoint> testData)
    {
        var results = new BiasTestResult();
        
        // Test across demographic groups
        var byGender = testData.GroupBy(x => x.Gender);
        var byAge = testData.GroupBy(x => x.AgeGroup);
        var byRegion = testData.GroupBy(x => x.Region);
        
        // Check if AI decisions systematically differ by group
        foreach (var genderGroup in byGender)
        {
            var acceptanceRate = genderGroup
                .Count(x => x.AiDecision == "approve") 
                / (float)genderGroup.Count();
            
            results.AcceptanceByGender[genderGroup.Key] = acceptanceRate;
            
            // Flag if difference > 5% (typical threshold)
            if (Math.Abs(acceptanceRate - results.OverallAcceptanceRate) > 0.05f)
            {
                results.BiasDetected = true;
                results.BiasReason = $"Gender disparity: {genderGroup.Key} {acceptanceRate:P}";
            }
        }
        
        return results;
    }
}

// 5. Performance Monitoring for AI Drift
public class AiPerformanceMonitor : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        // Check monthly if AI model performance degraded
        using var timer = new PeriodicTimer(TimeSpan.FromDays(30));
        
        while (await timer.WaitForNextTickAsync(ct))
        {
            foreach (var aiSystem in await _aiSystems.GetAllAsync())
            {
                var performance = await _model.MeasurePerformanceAsync(aiSystem.ModelVersion);
                
                // Compare to baseline
                var degradation = aiSystem.BaselineAccuracy - performance.CurrentAccuracy;
                
                if (degradation > 0.05)  // >5% drift
                {
                    _logger.LogCritical(
                        "AI MODEL DRIFT: {System} degraded by {Degradation:P}",
                        aiSystem.Name, degradation
                    );
                    
                    // Trigger retraining or rollback
                    await _alerts.SendAsync(
                        "AI System Performance Degradation",
                        $"{aiSystem.Name} requires investigation"
                    );
                }
            }
        }
    }
}

// 6. User Right to Explanation (AI Act Art. 22)
public class AiExplanationService
{
    public async Task<AiExplanationDto> GetExplanationAsync(
        Guid decisionLogId,
        Guid requestingUserId)
    {
        var log = await _logs.GetAsync(decisionLogId);
        
        // Verify user has right to this explanation
        if (log.UserId != requestingUserId)
            throw new UnauthorizedAccessException();
        
        return new AiExplanationDto
        {
            DecisionType = log.DecisionType,
            WhatWasDecided = log.DecisionOutput,
            WhyItWasDecided = log.ExplanationForUser,
            WasItReviewedByHuman = log.WasHumanReviewed,
            CanIDisputeIt = true,
            DisputeContactEmail = "ai-disputes@b2connect.com",
            RelatedArticles = new[]
            {
                "EU AI Act Article 22 (Right to Explanation)",
                "GDPR Article 22 (Automated Decision Making)"
            }
        };
    }
}
```

**Definition of Done:**
- [ ] AI Risk Register created with all AI systems documented
- [ ] High-risk AI systems (e.g., fraud detection) have full documentation
- [ ] AI Decision Logs implemented and functional
- [ ] Bias Testing framework implemented (test across demographics)
- [ ] Performance Monitoring for AI drift (monthly checks)
- [ ] User Right to Explanation API implemented
- [ ] Responsible Person (Art. 22) assigned for each AI system
- [ ] Tests: 10+ test cases covering AI risk scenarios
- [ ] Transparency: Users notified when AI affects them
- [ ] Audit: Complete history of AI decisions accessible

**Effort:** 50 hours (1 Backend Developer + 1 Security Engineer, 1.5 weeks)

---

## 📊 PHASE 0 SUMMARY

| Component | Owner | Duration | Status | Go/No-Go |
|-----------|-------|----------|--------|----------|
| P0.1: Audit Logging | Security Eng | 1 week | Planned | ✅ Required |
| P0.2: Encryption | Security Eng | 1 week | Planned | ✅ Required |
| P0.3: Incident Response | Sec + DevOps | 1.5 weeks | Planned | ✅ Required |
| P0.4: Network Segmentation | DevOps | 1.5 weeks | Planned | ✅ Required |
| P0.5: Key Management | DevOps | 1 week | Planned | ✅ Required |
| P0.6: E-Commerce Legal | Backend Dev | 1.5 weeks | Planned | ✅ Required |
| P0.7: AI Act Compliance | Backend + Security | 1.5 weeks | Planned | ✅ Required |
| **Total Phase 0** | **3-4 FTE** | **9-10 weeks** | **Parallel to MVP** | **Gate before Phase 1** |

### Testing Gate Before Phase 1 Deployment

```
Phase 0 MUST have 100% completion of:
✅ Audit logs captured for all CRUD operations
✅ All PII encrypted at rest (AES-256)
✅ Incident detection rules running (brute force, exfiltration)
✅ NIS2 notification capability verified (< 24h)
✅ Network segmentation enforced
✅ Key rotation policy in place
✅ E-Commerce legal components working (returns, VAT, invoices, terms)
✅ B2B & B2C checkouts both functional with compliance
✅ AI Act compliance: Risk register, transparency logs, audit trails
✅ No high-risk AI systems without approved risk assessments

If any ❌, HOLD Phase 1 Features from production.
```

---

## 📅 PHASE 1: MVP WITH COMPLIANCE INTEGRATION (Weeks 11-18)

### Overview

Phase 1 liefert die **Business Features** (Authentication, Catalog, Orders) mit **integrierter Compliance**.

**Team:** 3-4 Backend Devs + 2 Frontend Devs  
**Duration:** 8 Weeks  
**Go/No-Go Gate:** Phase 0 ✅ + P1 Features ✅

---

### 🔐 F1.1: Multi-Tenant Authentication & JWT with Compliance

**Functional Requirements:**
- JWT token generation (1h access, 7d refresh)
- Tenant isolation (X-Tenant-ID header mandatory)
- Role-based access control (Admin, Manager, Customer)
- Email verification (anti-spam)

**Compliance Requirements:**
- Token audit logging (who logged in, when)
- Suspicious activity detection (5+ failed logins → block)
- Password policy enforcement (12+ chars, uppercase, numbers, symbols)
- Password hashing: Argon2, not MD5/SHA1
- Session timeout: 15 min inactivity (configurable per tenant)

**Acceptance Criteria:**
- [ ] User can register with email verification
- [ ] Login generates JWT token with claims
- [ ] Token automatically refreshed within 1h
- [ ] Audit log: every login attempt (success/failure)
- [ ] 5+ failed logins from same IP → 10min lockout
- [ ] Password changed → all active sessions invalidated
- [ ] Tests: 10+ test cases (happy path, edge cases, compliance scenarios)

---

### 📦 F1.2: Product Catalog (with Caching & Compliance)

**Functional Requirements:**
- CRUD operations for products
- Categorization & search
- Multi-language support
- Inventory tracking

**Compliance Requirements:**
- Audit logging for all product changes
- Soft deletes (never hard delete)
- Tenant isolation (product visible only to own tenant)
- Cost data encrypted (suppliers, internal costs)

**Implementation:**

```csharp
public class Product : AggregateRoot
{
    public Guid TenantId { get; set; }              // Tenant isolation
    public string Sku { get; set; }
    public string Name { get; set; }
    public string DescriptionEncrypted { get; set; } // Internal descriptions encrypted
    public string SupplierNameEncrypted { get; set; } // Supplier names encrypted
    public decimal PublicPrice { get; set; }
    public string CostPriceEncrypted { get; set; }   // Cost encrypted
    
    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    
    // Audit trail
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
}

// Caching strategy
public class CatalogCacheService
{
    private readonly IDistributedCache _redis;
    private readonly IProductRepository _repo;
    
    public async Task<List<Product>> GetProductsByCategoryAsync(
        Guid tenantId,
        string category)
    {
        var cacheKey = $"catalog:{tenantId}:{category}";
        var cached = await _redis.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<List<Product>>(cached);
        }
        
        // Cache miss → query DB
        var products = await _repo.GetByCategoryAsync(tenantId, category);
        
        // Cache for 5 minutes
        await _redis.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(products),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            }
        );
        
        return products;
    }
}
```

**Acceptance Criteria:**
- [ ] Products CRUD working
- [ ] Audit logs for every change
- [ ] Soft deletes implemented
- [ ] Caching: 5-min TTL for product listing
- [ ] Cost data encrypted
- [ ] Tenant isolation tested (cross-tenant access impossible)
- [ ] Tests: 15+ test cases

---

### 🛒 F1.3: Shopping Cart & Checkout (with Compliance)

**Functional Requirements:**
- Add/remove items from cart
- Checkout flow
- Order creation
- Payment processing (Stripe/PayPal)

**Compliance Requirements:**
- PII not stored (only reference to user)
- Billing address encrypted
- Order audit trail
- Payment failure notification

**Acceptance Criteria:**
- [ ] Cart persisted in Redis (session-based)
- [ ] Checkout validates shipping address
- [ ] Order created with audit entry
- [ ] Payment processed via secure gateway
- [ ] Billing address encrypted
- [ ] Tests: 12+ test cases

---

### 📊 F1.4: Admin Dashboard with Compliance Audit

**Functional Requirements:**
- Tenant management
- User management
- Order viewing
- Basic analytics

**Compliance Requirements:**
- All admin actions logged
- Read-only audit trail visible
- Admin access restricted (role-based)
- Session timeout enforcement

**Acceptance Criteria:**
- [ ] Admin can create/delete users
- [ ] All admin actions in audit log
- [ ] Audit log visible in dashboard (read-only)
- [ ] Admin logout after 30min inactivity
- [ ] Tests: 10+ test cases

---

## Phase 1 Summary

| Feature | Owner | Duration | Compliance | Status |
|---------|-------|----------|-----------|--------|
| F1.1: Auth | Backend Dev | 2 weeks | Audit, Lockout | Planned |
| F1.2: Catalog | Backend Dev | 2 weeks | Encryption, Logging | Planned |
| F1.3: Checkout | Backend Dev | 2 weeks | PII Protection | Planned |
| F1.4: Admin | Backend + FrontEnd | 2 weeks | Access Control | Planned |
| **Total Phase 1** | **3-4 Backend, 2 FrontEnd** | **8 weeks** | **100% integrated** | **Gate** |

---

## 📅 PHASE 2: SCALE WITH COMPLIANCE (Weeks 15-24)

### Overview

Phase 2 skaliert die Lösung auf **10.000+ concurrent users** mit:
- Auto-scaling configuration
- Advanced caching (user-specific data)
- Database optimization (read replicas)
- Performance monitoring

All scaling operations must preserve compliance boundaries.

---

### P2.1: Database Read Replicas

**Requirement:** PostgreSQL 1 Writer + 3 Readers for load distribution

**Compliance Impact:**
- Replicas must be encrypted (same as primary)
- Audit logs replicated (immutability maintained)
- Backups: primary only (not from replicas)

**Tasks:**
- [ ] Primary DB (writer) with streaming replication
- [ ] 3 Read Replicas (eu-central-1a, b, c)
- [ ] Connection pooling (PgBouncer)
- [ ] Read-only queries routed to replicas
- [ ] Tests: Replication lag monitoring

---

### P2.2: Redis Cluster (High Availability)

**Requirement:** Redis Cluster (3+ nodes) with persistence

**Tasks:**
- [ ] 3-node Redis Cluster with sentinel
- [ ] AOF persistence (Append-Only File)
- [ ] Annual key rotation
- [ ] Tests: Node failure + recovery

---

### P2.3: Elasticsearch Cluster (Search at Scale)

**Requirement:** 3-node Elasticsearch Cluster with sharding

**Tasks:**
- [ ] Index sharding per tenant
- [ ] Daily index rotation
- [ ] Backup to S3/Azure Blob
- [ ] Tests: Search performance, failover

---

### P2.4: Auto-Scaling Configuration

**Requirement:** Scale services based on metrics

```yaml
AutoScaling Rules:
  - Metric: CPU > 70% → Scale up (+1 instance)
  - Metric: CPU < 20% → Scale down (-1 instance)
  - Metric: Response Time > 500ms → Scale up
  - Min Instances: 2 (for HA)
  - Max Instances: 10
  - Scale-up Time: < 2 minutes
```

**Tasks:**
- [ ] Horizontal Pod Autoscaler (HPA) configured
- [ ] Metrics Server installed
- [ ] Tests: Load testing, auto-scale verification

---

## 📅 PHASE 3: PRODUCTION HARDENING (Weeks 25-30)

### Overview

Phase 3 macht die Lösung **production-ready** für 100K+ users:
- Chaos engineering (what breaks?)
- Load testing (Black Friday simulation)
- Disaster recovery drills
- Compliance audit

---

### P3.1: Load Testing

**Scenario 1: Normal Load**
```
- 1000 concurrent users per shop
- 100 shops
- Total: 100.000 concurrent users
- Ramp-up: 10 min
- Duration: 1 hour
- Expectations:
  - API response < 100ms (P95)
  - No errors (0% failure rate)
  - CPU < 70%
  - Memory < 80%
```

**Scenario 2: Black Friday (5x normal)**
```
- 5000 concurrent users per shop
- 100 shops
- Total: 500.000 concurrent users
- Ramp-up: 2 min (sudden spike)
- Duration: 30 min
- Expectations:
  - API response < 500ms (P95) [degraded but acceptable]
  - <1% failure rate
  - Auto-scale triggered
  - No data loss
```

**Tools:**
- k6 (load testing framework)
- Grafana (metrics visualization)
- Tests: 2 weeks before launch

---

### P3.2: Chaos Engineering

**Experiment 1: Service Down**
```
- Kill 1 random service instance
- Expected: Auto-restart within 30 sec
- Verify: No customer impact
```

**Experiment 2: Database Failover**
```
- Kill primary PostgreSQL
- Expected: Failover to replica within 10 sec
- Verify: No data loss
```

**Experiment 3: Network Latency**
```
- Inject 500ms latency on service-to-service calls
- Expected: Circuit breaker triggers, fallback used
- Verify: System remains responsive
```

---

### P3.3: Compliance Audit

**NIS2 Audit Checklist:**
- [ ] All systems supply chain components identified
- [ ] Vulnerabilities scanned (OWASP dependency check)
- [ ] Penetration testing completed
- [ ] Incident response procedures tested
- [ ] Encryption key rotation verified
- [ ] Backup & recovery tested
- [ ] Audit logs immutable and encrypted
- [ ] GDPR right-to-be-forgotten tested
- [ ] Data residency verified (EU-only)

---

## 🎯 IMPLEMENTATION ROADMAP (Timeline)

```
WEEK 1-10   Phase 0: Compliance Foundation (PARALLEL)
            - P0.1: Audit Logging ✓
            - P0.2: Encryption ✓
            - P0.3: Incident Response ✓
            - P0.4: Network Segmentation ✓
            - P0.5: Key Management ✓
            - P0.6: E-Commerce Legal (B2B/B2C) ✓
            - P0.7: AI Act Compliance ✓
            
WEEK 11-18  Phase 1: MVP + Compliance
            - F1.1: Multi-Tenant Auth ✓
            - F1.2: Product Catalog ✓
            - F1.3: Checkout ✓
            - F1.4: Admin Dashboard ✓
            
WEEK 19-28  Phase 2: Scale with Compliance
            - P2.1: Database Replication ✓
            - P2.2: Redis Cluster ✓
            - P2.3: Elasticsearch Cluster ✓
            - P2.4: Auto-Scaling ✓
            
WEEK 29-34  Phase 3: Production Hardening
            - P3.1: Load Testing ✓
            - P3.2: Chaos Engineering ✓
            - P3.3: Compliance Audit ✓
            
WEEK 35+    Production Launch (100K+ users ready, AI Act compliant)
```

---

## 📋 SUCCESS CRITERIA (Go/No-Go Gates)

### Gate 1: Phase 0 Complete (Week 6)
```
✅ Audit Logging: All CRUD operations logged, tested
✅ Encryption: AES-256 at rest, tested end-to-end
✅ Incident Response: Detection rules running, < 24h notification
✅ Network: Segmentation enforced, no cross-subnet traffic
✅ Key Management: Vault configured, rotation policy active

If ANY ❌ → HOLD all deployments until fixed
```

### Gate 2: Phase 1 Complete (Week 14)
```
✅ MVP Features: All 4 features working
✅ Compliance: 100% audit logging, encryption integrated
✅ Tests: 50+ test cases, >80% code coverage
✅ Performance: API response < 200ms (P95), 99.5% uptime

If ANY ❌ → HOLD production deployment
```

### Gate 3: Phase 2 Complete (Week 24)
```
✅ Scaling: 10,000+ concurrent users handled
✅ HA: No single point of failure
✅ Performance: API response < 100ms (P95)
✅ Compliance: Scaling respects security boundaries

If ANY ❌ → HOLD production deployment
```

### Gate 4: Phase 3 Complete (Week 30)
```
✅ Load Testing: Black Friday scenario passed (5x normal load)
✅ Chaos Engineering: All failure scenarios handled
✅ Compliance Audit: 100% NIS2/GDPR compliance verified
✅ Disaster Recovery: RTO < 4h, RPO < 1h proven

If ANY ❌ → HOLD production launch
```

---

## 📞 Contact & Escalation

| Role | Responsibility | Escalation |
|------|-----------------|-----------|
| Security Lead | Compliance requirements | C-Level |
| DevOps Lead | Infrastructure, scaling | CTO |
| Product Manager | Feature prioritization | CEO |
| Compliance Officer | Regulatory interpretation | Legal |

---

## Anhang A: Regulatory Reference

### NIS2 Directive (Netz- und Informationssicherheitsgesetz 2.0)

**In Kraft:** 13. September 2024 → Umsetzung bis 17. Oktober 2024

**Key Requirements:**

1. **Art. 21: Security Measures**
   - Risk assessment (jährlich)
   - Incident handling
   - Business continuity
   - Encryption (mandatory)

2. **Art. 23: Incident Notification**
   - Competent authority: < 24 hours
   - Affected customers: as soon as possible (no specific deadline)
   - Documentation: incident report

3. **Art. 24: Supply Chain Security**
   - Supplier risk assessment
   - Dependency management
   - Vulnerability disclosure

---

### GDPR (Datenschutz-Grundverordnung)

**In Kraft:** 25. Mai 2018

**Key Requirements:**

1. **Art. 5: Principles** (Lawfulness, transparency, purpose limitation, minimization, accuracy, integrity, confidentiality)
2. **Art. 32: Security of processing** (Encryption, pseudonymization, testing)
3. **Art. 33: Breach notification** (to authority)
4. **Art. 34: Breach notification** (to individuals, if high risk)

---

### DORA (Digital Operational Resilience Act)

**In Kraft:** 16. Dezember 2022 → Umsetzung bis 17. Januar 2025

**Key Requirements:**

1. **Art. 6: ICT Risk Management Framework**
2. **Art. 10: Tools & controls for ICT-related operational risk**
3. **Art. 19: Incident reporting** (major incidents to authority)

---

## Anhang B: Testing Beispiele

Siehe: [docs/COMPLIANCE_TESTING_EXAMPLES.md](COMPLIANCE_TESTING_EXAMPLES.md)

---

**Document Owner:** Security & Architecture Team  
**Last Reviewed:** 28. Dezember 2025  
**Next Review:** 15. Januar 2026
