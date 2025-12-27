namespace B2Connect.Shared.User.Interfaces;

using B2Connect.Shared.User.Models;

/// <summary>
/// Address repository contract
/// </summary>
public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Address>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Address?> GetDefaultAddressAsync(Guid userId, string addressType, CancellationToken cancellationToken = default);
    Task<Address> CreateAsync(Address address, CancellationToken cancellationToken = default);
    Task<Address> UpdateAsync(Address address, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
