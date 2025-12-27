using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models = B2Connect.Shared.User.Models;
using B2Connect.Shared.User.Interfaces;
using B2Connect.Shared.User.Infrastructure.Data;

namespace B2Connect.Shared.User.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IAddressRepository
/// Handles all address-related database operations
/// Implements multi-tenancy and soft deletes
/// </summary>
public class AddressRepository : IAddressRepository
{
    private readonly UserDbContext _context;
    private readonly ILogger<AddressRepository> _logger;

    public AddressRepository(UserDbContext context, ILogger<AddressRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get an address by ID
    /// </summary>
    public async Task<Models.Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Get all addresses for a user
    /// </summary>
    public async Task<IEnumerable<Models.Address>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get default address for a user (optional: filter by address type)
    /// </summary>
    public async Task<Models.Address?> GetDefaultAddressAsync(Guid userId, string addressType, CancellationToken cancellationToken = default)
    {
        var query = _context.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.IsDefault && !a.IsDeleted);

        if (!string.IsNullOrWhiteSpace(addressType))
            query = query.Where(a => a.AddressType == addressType);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Create a new address
    /// </summary>
    public async Task<Models.Address> CreateAsync(Models.Address address, CancellationToken cancellationToken = default)
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created address {AddressId} for user {UserId}",
            address.Id,
            address.UserId);

        return address;
    }

    /// <summary>
    /// Update an existing address
    /// </summary>
    public async Task<Models.Address> UpdateAsync(Models.Address address, CancellationToken cancellationToken = default)
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        _context.Addresses.Update(address);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Updated address {AddressId} for user {UserId}",
            address.Id,
            address.UserId);

        return address;
    }

    /// <summary>
    /// Soft delete an address
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (address == null)
        {
            _logger.LogWarning(
                "Attempted to delete non-existent address {AddressId}",
                id);
            return;
        }

        // Soft delete
        address.IsDeleted = true;
        address.DeletedAt = DateTime.UtcNow;

        await UpdateAsync(address, cancellationToken);

        _logger.LogInformation(
            "Soft deleted address {AddressId}",
            id);
    }
}
