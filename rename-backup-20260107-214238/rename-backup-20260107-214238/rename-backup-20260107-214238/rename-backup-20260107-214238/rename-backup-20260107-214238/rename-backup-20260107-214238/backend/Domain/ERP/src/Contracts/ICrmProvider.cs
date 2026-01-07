// <copyright file="ICrmProvider.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Core;
using B2Connect.ERP.Models;

namespace B2Connect.ERP.Contracts;

/// <summary>
/// Customer Relationship Management (CRM) provider interface.
/// Specialized interface for customer and contact operations.
/// </summary>
public interface ICrmProvider
{
    /// <summary>
    /// Gets a contact by ID.
    /// </summary>
    Task<ProviderResult<CrmContact>> GetContactAsync(
        string contactId,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets multiple contacts by IDs in a single operation.
    /// </summary>
    Task<ProviderResult<IReadOnlyList<CrmContact>>> GetContactsAsync(
        IEnumerable<string> contactIds,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets activities for a customer.
    /// </summary>
    Task<ProviderResult<IReadOnlyList<CrmActivity>>> GetActivitiesAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets activities for multiple customers in a single batch operation.
    /// Returns a dictionary mapping customer IDs to their activities.
    /// </summary>
    Task<ProviderResult<IReadOnlyDictionary<string, IReadOnlyList<CrmActivity>>>> GetActivitiesBatchAsync(
        IEnumerable<string> customerIds,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets order history for a customer.
    /// </summary>
    Task<ProviderResult<PagedResult<CrmOrderHistory>>> GetOrderHistoryAsync(
        string customerId,
        int pageSize,
        string? continuationToken,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets customer addresses.
    /// </summary>
    Task<ProviderResult<IReadOnlyList<CrmAddress>>> GetAddressesAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets customer-specific pricing (discounts, special prices).
    /// </summary>
    Task<ProviderResult<CrmPricing>> GetCustomerPricingAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Creates a new contact for a customer.
    /// </summary>
    Task<ProviderResult<CrmContact>> CreateContactAsync(
        string customerId,
        CrmContactRequest request,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Updates an existing contact.
    /// </summary>
    Task<ProviderResult<CrmContact>> UpdateContactAsync(
        string contactId,
        CrmContactRequest request,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Logs a customer activity.
    /// </summary>
    Task<ProviderResult<CrmActivity>> LogActivityAsync(
        CrmActivityRequest request,
        TenantContext context,
        CancellationToken ct = default);
}

/// <summary>
/// Request to create or update a contact.
/// </summary>
public sealed record CrmContactRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Position { get; init; }
    public string? Department { get; init; }
    public bool IsPrimary { get; init; }
}

/// <summary>
/// Request to log a customer activity.
/// </summary>
public sealed record CrmActivityRequest
{
    public required string CustomerId { get; init; }
    public required string Type { get; init; }
    public required string Subject { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset? ScheduledAt { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}
