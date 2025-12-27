using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models = B2Connect.Shared.User.Models;
using B2Connect.Shared.User.Interfaces;
using B2Connect.Shared.User.Infrastructure.Data;

namespace B2Connect.Shared.User.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of IUserRepository
/// Handles all user-related database operations
/// Implements multi-tenancy and soft deletes
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(UserDbContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    public async Task<Models.User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id && !u.IsDeleted)
            .Include(u => u.Profile)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Get a user by email (tenant-scoped)
    /// </summary>
    public async Task<Models.User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(email))
            return null;

        return await _context.Users
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId && u.Email == email && !u.IsDeleted)
            .Include(u => u.Profile)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Get all users for a tenant (tenant-scoped, excludes deleted)
    /// </summary>
    public async Task<IEnumerable<Models.User>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId && !u.IsDeleted)
            .Include(u => u.Profile)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    public async Task<Models.User> CreateAsync(Models.User user, CancellationToken cancellationToken = default)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created user {UserId} for tenant {TenantId}",
            user.Id,
            user.TenantId);

        return user;
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    public async Task<Models.User> UpdateAsync(Models.User user, CancellationToken cancellationToken = default)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Updated user {UserId} for tenant {TenantId}",
            user.Id,
            user.TenantId);

        return user;
    }

    /// <summary>
    /// Soft delete a user (mark as deleted)
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning(
                "Attempted to delete non-existent user {UserId}",
                id);
            return;
        }

        // Soft delete
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;

        await UpdateAsync(user, cancellationToken);

        _logger.LogInformation(
            "Soft deleted user {UserId}",
            id);
    }

    /// <summary>
    /// Check if email already exists (tenant-scoped)
    /// </summary>
    public async Task<bool> ExistsByEmailAsync(Guid tenantId, string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        return await _context.Users
            .AnyAsync(u => u.TenantId == tenantId && u.Email == email && !u.IsDeleted, cancellationToken);
    }
}
