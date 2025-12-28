# 🧪 Compliance Testing Examples

**Reference:** [EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md)

---

## P0.1: Audit Logging - Test Examples

### Test 1: Create Operation is Logged

```csharp
[Fact]
public async Task CreateProduct_LogsAuditEntry()
{
    // Arrange
    var tenantId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var product = new Product 
    { 
        TenantId = tenantId,
        Sku = "TEST-001",
        Name = "Test Product",
        CreatedBy = userId
    };
    
    // Act
    await _context.Products.AddAsync(product);
    await _context.SaveChangesAsync();
    
    // Assert - Verify Audit Log
    var auditLog = await _context.AuditLogs
        .FirstAsync(x => 
            x.EntityType == "Product" && 
            x.Action == "CREATE" &&
            x.EntityId == product.Id.ToString());
    
    Assert.NotNull(auditLog);
    Assert.Equal(tenantId, auditLog.TenantId);      // Tenant isolation
    Assert.Equal(userId, auditLog.UserId);          // Who made change
    Assert.NotEmpty(auditLog.NewValues);            // JSON captured
    Assert.Null(auditLog.OldValues);                // No previous values
    Assert.NotNull(auditLog.Hash);                  // Tamper detection
    Assert.NotEqual(default, auditLog.CreatedAt);   // Timestamp
}
```

### Test 2: Update Operation is Logged

```csharp
[Fact]
public async Task UpdateProduct_LogsAuditEntry()
{
    // Arrange
    var product = await _context.Products.FirstAsync();
    var oldName = product.Name;
    var userId = Guid.NewGuid();
    
    // Act
    product.Name = "Updated Name";
    _context.Products.Update(product);
    await _context.SaveChangesAsync();
    
    // Assert
    var auditLog = await _context.AuditLogs
        .Where(x => x.EntityType == "Product" && x.Action == "UPDATE")
        .OrderByDescending(x => x.CreatedAt)
        .FirstAsync();
    
    Assert.NotNull(auditLog);
    var oldValues = JsonSerializer.Deserialize<Dictionary<string, object>>(auditLog.OldValues);
    var newValues = JsonSerializer.Deserialize<Dictionary<string, object>>(auditLog.NewValues);
    
    Assert.Equal(oldName, oldValues["Name"].ToString());
    Assert.Equal("Updated Name", newValues["Name"].ToString());
}
```

### Test 3: Audit Log Cannot Be Modified

```csharp
[Fact]
public async Task AuditLog_Immutable_CannotBeModified()
{
    // Arrange
    var log = await _context.AuditLogs.FirstAsync();
    var originalHash = log.Hash;
    
    // Act - Try to modify
    log.Action = "HACKED";
    
    // Assert - Should throw
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(
        () => _context.SaveChangesAsync()
    );
    
    Assert.Contains("immutable", exception.Message.ToLower());
}
```

### Test 4: Tenant Isolation - Cannot Access Other Tenant's Logs

```csharp
[Fact]
public async Task AuditLog_TenantIsolation_CannotAccessOtherTenantLogs()
{
    // Arrange
    var tenantA = Guid.NewGuid();
    var tenantB = Guid.NewGuid();
    
    // Create audit logs for both tenants
    await _context.AuditLogs.AddAsync(new AuditLogEntry 
    { 
        TenantId = tenantA, 
        EntityType = "Product",
        Action = "CREATE"
    });
    await _context.AuditLogs.AddAsync(new AuditLogEntry 
    { 
        TenantId = tenantB, 
        EntityType = "Product", 
        Action = "CREATE"
    });
    await _context.SaveChangesAsync();
    
    // Act - Query as TenantA
    var tenantALogs = await _context.AuditLogs
        .Where(x => x.TenantId == tenantA)
        .ToListAsync();
    
    // Assert
    Assert.Single(tenantALogs);
    Assert.All(tenantALogs, log => Assert.Equal(tenantA, log.TenantId));
}
```

### Test 5: SIEM Export Format

```csharp
[Fact]
public async Task AuditLog_CanExportToSyslog()
{
    // Arrange
    var log = await _context.AuditLogs.FirstAsync();
    
    // Act
    var syslogEntry = _auditService.ExportToSyslog(log);
    
    // Assert - Verify format
    Assert.Contains(log.TenantId.ToString(), syslogEntry);
    Assert.Contains(log.Action, syslogEntry);
    Assert.Contains(log.EntityType, syslogEntry);
    Assert.Matches(@"ISO 8601.*datetime", syslogEntry); // ISO timestamp
}
```

---

## P0.2: Encryption at Rest - Test Examples

### Test 1: PII is Encrypted at Rest

```csharp
[Fact]
public async Task User_EmailEncrypted_InDatabase()
{
    // Arrange
    var user = new User 
    { 
        EmailAddressEncrypted = "john.doe@example.com",
        PhoneNumberEncrypted = "+49123456789"
    };
    
    // Act
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    
    // Assert - Verify stored as encrypted (not plaintext)
    var dbResult = await _context.Database
        .SqlQuery<string>(@"
            SELECT email_address_encrypted 
            FROM users 
            WHERE id = {0}
        ", user.Id)
        .FirstAsync();
    
    Assert.NotEqual("john.doe@example.com", dbResult);
    Assert.True(IsBase64String(dbResult));
    Assert.DoesNotContain("john", dbResult);  // Plaintext not visible
    Assert.DoesNotContain("example", dbResult);
}
```

### Test 2: Decryption Works Transparently

```csharp
[Fact]
public async Task User_AccessEmail_AutomaticallyDecrypted()
{
    // Arrange
    var plainEmail = "john.doe@example.com";
    var user = new User { EmailAddressEncrypted = plainEmail };
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    
    // Act - Retrieve and check automatic decryption
    _context.ChangeTracker.Clear();  // Clear cache
    var retrieved = await _context.Users.FirstAsync();
    
    // Assert - Value converter handles decryption automatically
    Assert.Equal(plainEmail, retrieved.EmailAddressEncrypted);
}
```

### Test 3: Same Value = Different Ciphertexts (Random IV)

```csharp
[Fact]
public void Encrypt_DifferentIVEachTime_ProducesDifferentCiphertexts()
{
    // Arrange
    var plaintext = "john.doe@example.com";
    
    // Act
    var encrypted1 = _encryptionService.Encrypt(plaintext);
    var encrypted2 = _encryptionService.Encrypt(plaintext);
    var encrypted3 = _encryptionService.Encrypt(plaintext);
    
    // Assert - All different (due to random IV)
    Assert.NotEqual(encrypted1, encrypted2);
    Assert.NotEqual(encrypted2, encrypted3);
    Assert.NotEqual(encrypted1, encrypted3);
    
    // But all decrypt to same value
    Assert.Equal(plaintext, _encryptionService.Decrypt(encrypted1));
    Assert.Equal(plaintext, _encryptionService.Decrypt(encrypted2));
    Assert.Equal(plaintext, _encryptionService.Decrypt(encrypted3));
}
```

### Test 4: Key Rotation Decrypts Old & New Keys

```csharp
[Fact]
public async Task KeyRotation_OldDataStillDecryptable()
{
    // Arrange
    var plaintext = "sensitive-data";
    var encryptedWithOldKey = _encryptionService.Encrypt(plaintext);
    
    // Act - Rotate key
    await _encryptionService.RotateKeyAsync();
    
    // Assert - Can still decrypt data encrypted with old key
    var decrypted = _encryptionService.Decrypt(encryptedWithOldKey);
    Assert.Equal(plaintext, decrypted);
    
    // But new data encrypted with new key
    var newEncrypted = _encryptionService.Encrypt(plaintext);
    Assert.NotEqual(encryptedWithOldKey, newEncrypted);
}
```

### Test 5: Cost Data Encrypted

```csharp
[Fact]
public async Task Product_CostPrice_Encrypted()
{
    // Arrange
    var product = new Product
    {
        Sku = "PROD-001",
        PublicPrice = 99.99m,  // Visible
        CostPriceEncrypted = "25.50"  // Encrypted
    };
    
    // Act
    await _context.Products.AddAsync(product);
    await _context.SaveChangesAsync();
    
    // Assert - Cost is encrypted
    var dbResult = await _context.Database
        .SqlQuery<string>(@"
            SELECT cost_price_encrypted 
            FROM products 
            WHERE id = {0}
        ", product.Id)
        .FirstAsync();
    
    Assert.NotContain("25.50", dbResult);
}
```

---

## P0.3: Incident Response - Test Examples

### Test 1: Brute Force Detection

```csharp
[Fact]
public async Task DetectBruteForceAttack_When5FailedLoginsIn10Min()
{
    // Arrange
    var ipAddress = "192.168.1.100";
    var utcNow = DateTime.UtcNow;
    
    // Simulate 6 failed login attempts in 10 minutes
    for (int i = 0; i < 6; i++)
    {
        await _metricsService.RecordFailedLoginAsync(
            ipAddress, 
            "invalid-password",
            timestamp: utcNow.AddMinutes(i)
        );
    }
    
    // Act
    await _incidentDetectionService.DetectSecurityIncidentsAsync(CancellationToken.None);
    
    // Assert - Incident created
    var incident = await _incidentRepository.GetAsync(x =>
        x.Type == "BruteForceAttack" &&
        x.Context.ContainsKey("IpAddress") &&
        x.Context["IpAddress"].ToString() == ipAddress
    );
    
    Assert.NotNull(incident);
    Assert.Equal("High", incident.Severity);
}
```

### Test 2: Data Exfiltration Detection

```csharp
[Fact]
public async Task DetectDataExfiltration_When3xNormalDownload()
{
    // Arrange
    var normalDailyVolume = 1000;  // MB
    await _metricsService.SetAverageDailyDownloadVolumeAsync(normalDailyVolume);
    
    // Simulate today's download = 3x normal
    await _metricsService.RecordDownloadAsync(
        volume: normalDailyVolume * 3,  // 3000 MB
        timestamp: DateTime.UtcNow
    );
    
    // Act
    await _incidentDetectionService.DetectSecurityIncidentsAsync(CancellationToken.None);
    
    // Assert
    var incident = await _incidentRepository.GetAsync(x =>
        x.Type == "DataExfiltration"
    );
    
    Assert.NotNull(incident);
    Assert.Equal("Critical", incident.Severity);
}
```

### Test 3: NIS2 Notification < 24 Hours

```csharp
[Fact]
public async Task Nis2Notification_SentWithin24Hours()
{
    // Arrange
    var incident = new SecurityIncident
    {
        Type = "UnauthorizedAccess",
        Severity = "Critical",
        DetectedAt = DateTime.UtcNow,
        TenantId = Guid.NewGuid()
    };
    await _incidentRepository.CreateAsync(incident);
    
    // Act
    await _nis2NotificationService.NotifySecurityIncidentAsync(incident, CancellationToken.None);
    
    // Assert
    var notification = await _notificationRepository.GetAsync(incident.Id);
    Assert.NotNull(notification.NotificationSentAt);
    
    var timeSinceDetection = notification.NotificationSentAt - incident.DetectedAt;
    Assert.True(
        timeSinceDetection.TotalHours <= 24,
        $"NIS2 notification sent after {timeSinceDetection.TotalHours} hours (must be ≤ 24)"
    );
}
```

### Test 4: Authorities Notified Correctly

```csharp
[Fact]
public async Task Nis2Notification_SendsToCorrectAuthority()
{
    // Arrange
    var tenant = new Tenant { Country = "DE" };  // Germany
    var incident = new SecurityIncident
    {
        TenantId = tenant.Id,
        Type = "DataBreach"
    };
    
    // Act
    var authorities = _nis2NotificationService.GetCompetentAuthorities(tenant.Id);
    
    // Assert - Germany → BSI
    Assert.Contains("bsi@bsi.bund.de", authorities);
}
```

---

## P0.4: Network Segmentation - Test Examples

### Test 1: Services Not Accessible from Internet

```csharp
[Fact]
public async Task CatalogService_NotAccessibleDirectlyFromInternet()
{
    // Act - Try to access service directly (should fail)
    var http = new HttpClient();
    var exception = await Assert.ThrowsAsync<HttpRequestException>(
        () => http.GetAsync("http://catalog:7005/products")
    );
    
    // Assert - Connection refused (port not exposed)
    Assert.Contains("refused", exception.Message.ToLower());
}
```

### Test 2: Only Load Balancer Accepts Internet Traffic

```csharp
[Fact]
public async Task LoadBalancer_AcceptsHttpsFromInternet()
{
    // Act
    var response = await _httpClient.GetAsync("https://api.b2connect.eu/products");
    
    // Assert
    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal("HTTPS", response.RequestMessage.Scheme.ToUpper());
}
```

### Test 3: Database Not Accessible from Internet

```csharp
[Fact]
public void Database_NotExposedToInternet()
{
    // Arrange
    var dbSecurityGroup = _infraService.GetSecurityGroup("databases");
    
    // Assert - No inbound rules from 0.0.0.0/0
    var insecureRules = dbSecurityGroup.InboundRules
        .Where(r => r.SourceCidr == "0.0.0.0/0")
        .ToList();
    
    Assert.Empty(insecureRules);
}
```

---

## Phase 1: Feature Integration Tests

### F1.1: Authentication with Compliance

```csharp
[Fact]
public async Task Login_LoggedInAuditTrail()
{
    // Arrange
    var user = await CreateTestUserAsync("john@example.com", "Password123!");
    
    // Act
    var loginResult = await _authService.LoginAsync(
        "john@example.com",
        "Password123!",
        ipAddress: "192.168.1.1"
    );
    
    // Assert
    Assert.True(loginResult.Success);
    
    // Verify audit log
    var auditLog = await _context.AuditLogs
        .FirstAsync(x => 
            x.EntityType == "Login" && 
            x.Action == "SUCCESS");
    
    Assert.NotNull(auditLog);
    Assert.Equal(user.Id, auditLog.UserId);
}

[Fact]
public async Task FailedLogin_LocksAfter5Attempts()
{
    // Arrange
    var ipAddress = "192.168.1.100";
    
    // Act - 5 failed logins
    for (int i = 0; i < 5; i++)
    {
        await _authService.LoginAsync(
            "john@example.com",
            "wrong-password",
            ipAddress: ipAddress
        );
    }
    
    // Assert - 6th attempt fails immediately (lockout)
    var sixthAttempt = await _authService.LoginAsync(
        "john@example.com",
        "Password123!",  // Correct password
        ipAddress: ipAddress
    );
    
    Assert.False(sixthAttempt.Success);
    Assert.Contains("locked", sixthAttempt.Error.ToLower());
}
```

### F1.2: Catalog with Encryption

```csharp
[Fact]
public async Task Product_SupplierEncrypted_InDatabase()
{
    // Arrange
    var product = new Product
    {
        Sku = "TEST-001",
        Name = "Test Product",
        SupplierNameEncrypted = "Acme Corp"
    };
    
    // Act
    await _context.Products.AddAsync(product);
    await _context.SaveChangesAsync();
    
    // Assert
    var dbValue = await _context.Database
        .SqlQuery<string>(@"
            SELECT supplier_name_encrypted 
            FROM products 
            WHERE id = {0}
        ", product.Id)
        .FirstAsync();
    
    Assert.NotContain("Acme", dbValue);  // Encrypted
}
```

### F1.3: Checkout with Tenant Isolation

```csharp
[Fact]
public async Task Order_TenantIsolation_CannotViewOtherTenantOrders()
{
    // Arrange
    var tenantA = Guid.NewGuid();
    var tenantB = Guid.NewGuid();
    
    var orderA = await CreateTestOrderAsync(tenantA);
    var orderB = await CreateTestOrderAsync(tenantB);
    
    // Act - Query as TenantA
    var ordersForTenantA = await _orderService.GetOrdersAsync(tenantA);
    
    // Assert
    Assert.Contains(orderA.Id, ordersForTenantA.Select(o => o.Id));
    Assert.DoesNotContain(orderB.Id, ordersForTenantA.Select(o => o.Id));
}
```

---

## Phase 2: Load Testing Targets

### Test: 1000 Concurrent Users

```bash
k6 scenario:
  - Virtual Users: 1000
  - Ramp-up: 10 minutes
  - Duration: 1 hour
  - Endpoints tested: /products, /search, /checkout
  
Target Metrics:
  ✅ API response time < 100ms (P95)
  ✅ CPU < 70%
  ✅ Memory < 80%
  ✅ No errors (0% failure rate)
```

### Test: Black Friday (5x Normal Load)

```bash
k6 scenario:
  - Virtual Users: 5000
  - Ramp-up: 2 minutes (sudden spike)
  - Duration: 30 minutes
  - Endpoints tested: /products, /search, /checkout
  
Target Metrics:
  ✅ API response time < 500ms (P95) [degraded OK]
  ✅ < 1% failure rate
  ✅ Auto-scaling triggered (verified)
  ✅ No data loss
```

---

## Phase 3: Chaos Engineering Tests

### Test: Service Restart

```csharp
[Fact]
public async Task ServiceDown_AutoRestarts_Within30Seconds()
{
    // Arrange
    var service = "catalog-service";
    
    // Act - Kill service
    await _kubernetesService.DeletePodAsync(service);
    await Task.Delay(TimeSpan.FromSeconds(5));
    
    // Assert - Pod restarted
    var pods = await _kubernetesService.GetPodsAsync(service);
    Assert.NotEmpty(pods);
    Assert.All(pods, p => Assert.True(p.IsRunning));
}
```

### Test: Database Failover

```csharp
[Fact]
public async Task DatabaseFailover_Within10Seconds()
{
    // Arrange
    var primaryDb = "postgres-primary";
    
    // Act - Kill primary
    await _infraService.StopVMAsync(primaryDb);
    
    // Assert - Failover to replica within 10 seconds
    var startTime = DateTime.UtcNow;
    while ((DateTime.UtcNow - startTime).TotalSeconds < 10)
    {
        try
        {
            var connection = new NpgsqlConnection(_config.DbConnectionString);
            await connection.OpenAsync();
            connection.Dispose();
            break;  // Success
        }
        catch
        {
            await Task.Delay(500);
        }
    }
    
    var failoverTime = (DateTime.UtcNow - startTime).TotalSeconds;
    Assert.True(failoverTime < 10, $"Failover took {failoverTime}s (must be < 10s)");
}
```

---

**All tests must pass before gate approval.**

