using B2X.Identity.Interfaces;
using B2X.Identity.Models;

namespace B2X.Identity.Infrastructure.ExternalServices;

/// <summary>
/// Faker ERP Provider für Testing und Entwicklung
/// Liefert realistische Mock-Daten ohne echte ERP-Verbindung
///
/// Use Cases:
/// - Integration Tests ohne ERP-System
/// - Local Development ohne Produktionsverbindung
/// - Demo-Umgebungen
/// - Fehlertoleranz-Tests
/// </summary>
public class FakeErpProvider : IErpProvider
{
    private readonly ILogger<FakeErpProvider> _logger;

    public string ProviderName => "Fake";

    // Seed-Daten für konsistente Test-Szenarien
    private static readonly Dictionary<string, ErpCustomerDto> FakeCustomerDatabase = new(StringComparer.Ordinal)
    {
        // B2C Customers
        {
            "CUST-001", new ErpCustomerDto
            {
                CustomerNumber = "CUST-001",
                CustomerName = "Max Mustermann",
                Email = "max.mustermann@example.com",
                Phone = "+49 30 123456",
                ShippingAddress = "Musterstraße 1, 10115 Berlin",
                BillingAddress = "Musterstraße 1, 10115 Berlin",
                Country = "DE",
                BusinessType = "PRIVATE",
                LastModifiedDate = DateTime.UtcNow.AddDays(-30),
                IsActive = true,
                Status = "ACTIVE",
                CreditLimit = null
            }
        },
        {
            "CUST-002", new ErpCustomerDto
            {
                CustomerNumber = "CUST-002",
                CustomerName = "Erika Musterfrau",
                Email = "erika.musterfrau@example.com",
                Phone = "+49 69 654321",
                ShippingAddress = "Frankfurter Allee 123, 60311 Frankfurt",
                BillingAddress = "Frankfurter Allee 123, 60311 Frankfurt",
                Country = "DE",
                BusinessType = "PRIVATE",
                LastModifiedDate = DateTime.UtcNow.AddDays(-15),
                IsActive = true,
                Status = "ACTIVE",
                CreditLimit = null
            }
        },
        // B2B Customers
        {
            "CUST-100", new ErpCustomerDto
            {
                CustomerNumber = "CUST-100",
                CustomerName = "TechCorp GmbH",
                Email = "info@techcorp.de",
                Phone = "+49 2xx 98765432",
                ShippingAddress = "Technologiepark 5, 80939 München",
                BillingAddress = "Technologiepark 5, 80939 München",
                Country = "DE",
                BusinessType = "BUSINESS",
                LastModifiedDate = DateTime.UtcNow.AddDays(-7),
                IsActive = true,
                Status = "ACTIVE",
                CreditLimit = 50000.00m
            }
        },
        {
            "CUST-101", new ErpCustomerDto
            {
                CustomerNumber = "CUST-101",
                CustomerName = "InnovateLabs AG",
                Email = "contact@innovatelabs.at",
                Phone = "+43 1 123456789",
                ShippingAddress = "Innovationstraße 42, 1020 Wien",
                BillingAddress = "Innovationstraße 42, 1020 Wien",
                Country = "AT",
                BusinessType = "BUSINESS",
                LastModifiedDate = DateTime.UtcNow.AddDays(-5),
                IsActive = true,
                Status = "ACTIVE",
                CreditLimit = 75000.00m
            }
        },
        {
            "CUST-102", new ErpCustomerDto
            {
                CustomerNumber = "CUST-102",
                CustomerName = "Global Solutions SA",
                Email = "sales@globalsolutions.ch",
                Phone = "+41 44 123456",
                ShippingAddress = "Börsenstraße 7, 8001 Zürich",
                BillingAddress = "Börsenstraße 7, 8001 Zürich",
                Country = "CH",
                BusinessType = "BUSINESS",
                LastModifiedDate = DateTime.UtcNow.AddDays(-3),
                IsActive = true,
                Status = "ACTIVE",
                CreditLimit = 100000.00m
            }
        }
    };

    // Mapping für E-Mail-Suche
    private static readonly Dictionary<string, string> EmailToCustomerNumber = new(StringComparer.Ordinal)
    {
        { "max.mustermann@example.com", "CUST-001" },
        { "erika.musterfrau@example.com", "CUST-002" },
        { "info@techcorp.de", "CUST-100" },
        { "contact@innovatelabs.at", "CUST-101" },
        { "sales@globalsolutions.ch", "CUST-102" }
    };

    // Mapping für Firmennamen-Suche
    private static readonly Dictionary<string, string> CompanyNameToCustomerNumber = new(StringComparer.Ordinal)
    {
        { "techcorp gmbh", "CUST-100" },
        { "innovatelabs ag", "CUST-101" },
        { "global solutions sa", "CUST-102" }
    };

    public FakeErpProvider(ILogger<FakeErpProvider> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Sucht Kunden nach Kundennummer in der Fake-Datenbank
    /// </summary>
    public Task<ErpCustomerDto?> GetCustomerByNumberAsync(string customerNumber, CancellationToken ct = default)
    {
        _logger.LogInformation("[FAKE ERP] GetCustomerByNumber: {CustomerNumber}", customerNumber);

        if (string.IsNullOrWhiteSpace(customerNumber))
        {
            _logger.LogWarning("[FAKE ERP] GetCustomerByNumber: Empty customer number provided");
            return Task.FromResult<ErpCustomerDto?>(null);
        }

        if (FakeCustomerDatabase.TryGetValue(customerNumber, out var customer))
        {
            _logger.LogInformation("[FAKE ERP] Found customer: {CustomerName}", customer.CustomerName);
            return Task.FromResult<ErpCustomerDto?>(CloneCustomer(customer));
        }

        _logger.LogWarning("[FAKE ERP] Customer not found: {CustomerNumber}", customerNumber);
        return Task.FromResult<ErpCustomerDto?>(null);
    }

    /// <summary>
    /// Sucht Kunden nach E-Mail in der Fake-Datenbank
    /// </summary>
    public Task<ErpCustomerDto?> GetCustomerByEmailAsync(string email, CancellationToken ct = default)
    {
        _logger.LogInformation("[FAKE ERP] GetCustomerByEmail: {Email}", email);

        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("[FAKE ERP] GetCustomerByEmail: Empty email provided");
            return Task.FromResult<ErpCustomerDto?>(null);
        }

        var normalizedEmail = email.ToLowerInvariant();
        if (EmailToCustomerNumber.TryGetValue(normalizedEmail, out var customerNumber))
        {
            return GetCustomerByNumberAsync(customerNumber, ct);
        }

        _logger.LogWarning("[FAKE ERP] Customer not found by email: {Email}", email);
        return Task.FromResult<ErpCustomerDto?>(null);
    }

    /// <summary>
    /// Sucht B2B-Kunden nach Firmenname in der Fake-Datenbank
    /// </summary>
    public Task<ErpCustomerDto?> GetCustomerByCompanyNameAsync(string companyName, CancellationToken ct = default)
    {
        _logger.LogInformation("[FAKE ERP] GetCustomerByCompanyName: {CompanyName}", companyName);

        if (string.IsNullOrWhiteSpace(companyName))
        {
            _logger.LogWarning("[FAKE ERP] GetCustomerByCompanyName: Empty company name provided");
            return Task.FromResult<ErpCustomerDto?>(null);
        }

        var normalizedCompanyName = companyName.ToLowerInvariant();
        if (CompanyNameToCustomerNumber.TryGetValue(normalizedCompanyName, out var customerNumber))
        {
            return GetCustomerByNumberAsync(customerNumber, ct);
        }

        // Fuzzy-Matching für teilweise Übereinstimmungen
        var fuzzyMatch = CompanyNameToCustomerNumber.Keys
            .FirstOrDefault(cn => cn.Contains(normalizedCompanyName) || normalizedCompanyName.Contains(cn));

        if (fuzzyMatch != null && CompanyNameToCustomerNumber.TryGetValue(fuzzyMatch, out var fuzzyCustomerNumber))
        {
            _logger.LogInformation("[FAKE ERP] Found customer via fuzzy match: {FuzzyMatch}", fuzzyMatch);
            return GetCustomerByNumberAsync(fuzzyCustomerNumber, ct);
        }

        _logger.LogWarning("[FAKE ERP] Customer not found by company name: {CompanyName}", companyName);
        return Task.FromResult<ErpCustomerDto?>(null);
    }

    /// <summary>
    /// Fake-Implementierung: Gibt immer true zurück (Fake-Service ist "immer verfügbar")
    /// </summary>
    public Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("[FAKE ERP] Checking availability - always returns true");
        return Task.FromResult(true);
    }

    /// <summary>
    /// Gibt Fake-Synchronisierungsstatus zurück
    /// </summary>
    public Task<ErpSyncStatusDto> GetSyncStatusAsync(CancellationToken ct = default)
    {
        var status = new ErpSyncStatusDto
        {
            LastSyncTime = DateTime.UtcNow.AddHours(-2),
            CachedCustomerCount = FakeCustomerDatabase.Count,
            IsConnected = true,
            ErpSystemType = ProviderName,
            Message = $"Faker provider is available with {FakeCustomerDatabase.Count} customers"
        };

        _logger.LogInformation("[FAKE ERP] Sync status: {TotalCustomers} customers, last sync {LastSyncTime}",
            status.CachedCustomerCount, status.LastSyncTime);

        return Task.FromResult(status);
    }

    /// <summary>
    /// Helper: Deep-Clone eines Customers um Datenverschmutzung zu vermeiden
    /// </summary>
    private static ErpCustomerDto CloneCustomer(ErpCustomerDto customer)
    {
        return new ErpCustomerDto
        {
            CustomerNumber = customer.CustomerNumber,
            CustomerName = customer.CustomerName,
            Email = customer.Email,
            Phone = customer.Phone,
            ShippingAddress = customer.ShippingAddress,
            BillingAddress = customer.BillingAddress,
            Country = customer.Country,
            BusinessType = customer.BusinessType,
            LastModifiedDate = customer.LastModifiedDate,
            IsActive = customer.IsActive,
            Status = customer.Status,
            CreditLimit = customer.CreditLimit
        };
    }
}
