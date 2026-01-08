// <copyright file="FakeCrmProvider.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Contracts;
using B2X.ERP.Core;
using B2X.ERP.Models;

namespace B2X.ERP.Providers.Fake;

/// <summary>
/// Fake implementation of the CRM provider interface for development and testing.
/// Provides mock data for customer relationship management operations.
/// </summary>
public sealed class FakeCrmProvider : ICrmProvider
{
    /// <inheritdoc/>
    public string ProviderType => "fake-crm";

    /// <inheritdoc/>
    public string Version => "1.0.0-fake";

    /// <inheritdoc/>
    public bool IsConnected => true;

    /// <inheritdoc/>
    public Task<ProviderResult<CrmContact>> GetContactAsync(
        string contactId,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmContact>.Success(CreateFakeContact(contactId)));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<CrmContact>>> GetContactsAsync(
        IEnumerable<string> contactIds,
        TenantContext context,
        CancellationToken ct = default)
    {
        var contacts = contactIds.Select(id => CreateFakeContact(id)).ToArray();
        return Task.FromResult(ProviderResult<IReadOnlyList<CrmContact>>.Success(contacts));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<CrmActivity>>> GetActivitiesAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default)
    {
        var activities = new[] { CreateFakeActivity(customerId) };
        return Task.FromResult(ProviderResult<IReadOnlyList<CrmActivity>>.Success(activities));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyDictionary<string, IReadOnlyList<CrmActivity>>>> GetActivitiesBatchAsync(
        IEnumerable<string> customerIds,
        TenantContext context,
        CancellationToken ct = default)
    {
        var result = customerIds.ToDictionary(
            id => id,
            id => (IReadOnlyList<CrmActivity>)new[] { CreateFakeActivity(id) });
        return Task.FromResult(ProviderResult<IReadOnlyDictionary<string, IReadOnlyList<CrmActivity>>>.Success(result));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<PagedResult<CrmOrderHistory>>> GetOrderHistoryAsync(
        string customerId,
        int pageSize,
        string? continuationToken,
        TenantContext context,
        CancellationToken ct = default)
    {
        var orders = new[] { CreateFakeOrderHistory(customerId) };
        var result = new PagedResult<CrmOrderHistory>(orders, null, 1);
        return Task.FromResult(ProviderResult<PagedResult<CrmOrderHistory>>.Success(result));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<IReadOnlyList<CrmAddress>>> GetAddressesAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default)
    {
        var addresses = new[] { CreateFakeAddress() };
        return Task.FromResult(ProviderResult<IReadOnlyList<CrmAddress>>.Success(addresses));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<CrmPricing>> GetCustomerPricingAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmPricing>.Success(CreateFakeCustomerPricing()));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<CrmContact>> CreateContactAsync(
        string customerId,
        CrmContactRequest request,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmContact>.Success(CreateFakeContact(Guid.NewGuid().ToString())));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<CrmContact>> UpdateContactAsync(
        string contactId,
        CrmContactRequest request,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmContact>.Success(CreateFakeContact(contactId)));
    }

    /// <inheritdoc/>
    public Task<ProviderResult<CrmActivity>> LogActivityAsync(
        CrmActivityRequest request,
        TenantContext context,
        CancellationToken ct = default)
    {
        return Task.FromResult(ProviderResult<CrmActivity>.Success(CreateFakeActivity(request.CustomerId)));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Fake provider - no resources to dispose
    }

    // ========================================
    // Private helper methods
    // ========================================

    private static CrmContact CreateFakeContact(string contactId) => new CrmContact
    {
        Id = contactId,
        CustomerId = "CUST001",
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@example.com",
        Phone = "+1234567890",
        Position = "Manager",
        Department = "Sales",
        IsPrimary = true,
        IsActive = true,
        CreatedAt = DateTimeOffset.UtcNow.AddDays(-30),
        ModifiedAt = DateTimeOffset.UtcNow
    };

    private static CrmActivity CreateFakeActivity(string customerId) => new CrmActivity
    {
        Id = Guid.NewGuid().ToString(),
        CustomerId = customerId,
        Type = CrmActivityType.Call,
        Subject = "Customer inquiry",
        Description = "Called customer regarding order status",
        Status = CrmActivityStatus.Completed,
        Priority = CrmActivityPriority.Normal,
        DueDate = null,
        CompletedAt = DateTimeOffset.UtcNow.AddHours(-2),
        CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
        ModifiedAt = DateTimeOffset.UtcNow
    };

    private static CrmOrderHistory CreateFakeOrderHistory(string customerId) => new CrmOrderHistory
    {
        OrderId = "ORD001",
        OrderNumber = "ORD001",
        CustomerId = customerId,
        OrderDate = DateTimeOffset.UtcNow.AddDays(-7),
        Status = "Completed",
        TotalAmount = 299.99m,
        Currency = "EUR",
        ItemCount = 3
    };

    private static CrmAddress CreateFakeAddress() => new CrmAddress
    {
        Id = Guid.NewGuid().ToString(),
        Type = CrmAddressType.Billing,
        Street1 = "123 Fake Street",
        Street2 = null,
        City = "Fake City",
        PostalCode = "12345",
        CountryCode = "DE",
        IsDefault = true,
        IsActive = true
    };

    private static CrmPricing CreateFakeCustomerPricing() => new CrmPricing
    {
        CustomerId = "CUST001",
        ProductId = "PROD001",
        Price = 89.99m,
        Currency = "EUR",
        DiscountPercent = 10.0m,
        ValidFrom = DateTimeOffset.UtcNow.AddDays(-30),
        ValidUntil = DateTimeOffset.UtcNow.AddDays(30),
        PriceListId = "VIP",
        PriceListName = "VIP Customer Pricing"
    };
}
