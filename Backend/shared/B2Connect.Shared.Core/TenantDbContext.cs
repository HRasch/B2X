using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace B2Connect.Shared.Core
{
    /// <summary>
    /// Base DbContext that automatically applies tenant filtering and command interception
    /// </summary>
    public abstract class TenantDbContext : DbContext
    {
        protected readonly ITenantContext _tenantContext;

        /// <summary>
        /// Gets or sets whether tenant ownership validation should be skipped.
        /// Useful for testing or administrative operations.
        /// </summary>
        public bool SkipTenantValidation { get; set; }

        /// <summary>
        /// Gets or sets whether tenant query filters should be skipped.
        /// Useful for testing or administrative operations.
        /// </summary>
        public bool SkipTenantFilters { get; set; }

        protected TenantDbContext(
            DbContextOptions options,
            ITenantContext tenantContext)
            : base(options)
        {
            _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Add tenant interceptors
            optionsBuilder.AddInterceptors(
                new Interceptors.TenantCommandInterceptor(_tenantContext),
                new Interceptors.TenantQueryInterceptor(_tenantContext));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Note: Query filters are applied in derived DbContexts where tenant context is available
            // This base class only provides the interceptor registration
        }

        /// <summary>
        /// Validates that all tracked entities belong to the current tenant
        /// </summary>
        public void ValidateTenantOwnership()
        {
            if (SkipTenantValidation)
            {
                // Validation disabled for testing or administrative operations
                return;
            }

            var tenantId = _tenantContext.GetCurrentTenantId();
            if (tenantId == Guid.Empty)
            {
                // No tenant context - skip validation (useful for tests or system operations)
                return;
            }

            var invalidEntities = ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.Entity is ITenantEntity)
                .Select(e => e.Entity as ITenantEntity)
                .Where(te => te != null && te.TenantId != Guid.Empty && te.TenantId != tenantId)
                .ToList();

            if (invalidEntities.Any())
            {
                throw new InvalidOperationException(
                    $"Found {invalidEntities.Count} entities that do not belong to the current tenant. " +
                    "This indicates a security issue where entities from other tenants are being tracked.");
            }
        }

        public override int SaveChanges()
        {
            ValidateTenantOwnership();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ValidateTenantOwnership();
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}