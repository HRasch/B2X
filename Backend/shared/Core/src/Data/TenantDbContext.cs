using Microsoft.EntityFrameworkCore;
using B2Connect.Shared.Data.Interceptors;

namespace B2Connect.Shared.Data
{
    /// <summary>
    /// Base DbContext that automatically applies tenant filtering and command interception
    /// </summary>
    public abstract class TenantDbContext : DbContext
    {
        private readonly TenantCommandInterceptor _commandInterceptor;
        private readonly TenantQueryInterceptor _queryInterceptor;

        protected TenantDbContext(DbContextOptions options) : base(options)
        {
            _commandInterceptor = new TenantCommandInterceptor();
            _queryInterceptor = new TenantQueryInterceptor();
        }

        /// <summary>
        /// Configures the model with tenant interceptors
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_commandInterceptor, _queryInterceptor);
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Override to customize tenant behavior per context
        /// </summary>
        protected virtual void OnTenantConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Default implementation does nothing
            // Override in derived classes to customize tenant behavior
        }

        /// <summary>
        /// Saves changes with tenant validation
        /// </summary>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ValidateTenantContext();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// Saves changes with tenant validation (async)
        /// </summary>
        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            ValidateTenantContext();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ValidateTenantContext()
        {
            // Ensure tenant context is available for operations that modify tenant data
            var entries = ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.Entity is ITenantEntity)
                .ToList();

            if (entries.Any() && !TenantContext.CurrentTenantId.HasValue)
            {
                throw new InvalidOperationException(
                    "Tenant context is required for operations on tenant entities. " +
                    "Ensure TenantContextMiddleware is configured and tenant ID is available.");
            }
        }
    }

    /// <summary>
    /// Extension methods for configuring tenant DbContext
    /// </summary>
    public static class TenantDbContextExtensions
    {
        /// <summary>
        /// Adds tenant interceptors to the DbContext options
        /// </summary>
        public static DbContextOptionsBuilder UseTenantInterceptors(
            this DbContextOptionsBuilder optionsBuilder)
        {
            return optionsBuilder
                .AddInterceptors(
                    new TenantCommandInterceptor(),
                    new TenantQueryInterceptor());
        }

        /// <summary>
        /// Configures PostgreSQL with tenant-aware connection
        /// </summary>
        public static DbContextOptionsBuilder UseB2ConnectPostgres(
            this DbContextOptionsBuilder optionsBuilder,
            string connectionString)
        {
            return optionsBuilder
                .UseNpgsql(connectionString)
                .UseTenantInterceptors();
        }
    }</content>
<parameter name = "filePath" >/ Users / holger / Documents / Projekte / B2Connect / backend / Shared / Core / src / Data / TenantDbContext.cs