using Microsoft.EntityFrameworkCore;
using B2X.Store.Core.Store.Entities;
using B2X.Store.Core.Store.Interfaces;
using B2X.Store.Infrastructure.Common.Data;

namespace B2X.Store.Infrastructure.Store.Repositories;

/// <summary>
/// Repository implementation for PaymentMethod entity
/// Store-specific repository for Payment Method management
/// </summary>
public class PaymentMethodRepository : B2X.Store.Infrastructure.Common.Repositories.Repository<PaymentMethod>, IPaymentMethodRepository
{
    public PaymentMethodRepository(StoreDbContext context) : base(context)
    {
    }

    public async Task<PaymentMethod?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Code == code.ToUpper(), cancellationToken);
    }

    public async Task<ICollection<PaymentMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.StoreId == storeId && p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<PaymentMethod>> GetByProviderAsync(string provider, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Provider == provider && p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<PaymentMethod>> GetByCurrencyAsync(string currencyCode, Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.StoreId == storeId
                && p.IsActive
                && (p.SupportedCurrencies == null || p.SupportedCurrencies.Contains(currencyCode)))
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}

