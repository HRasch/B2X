using System.Reflection;
using B2Connect.Core.Interfaces;
using B2Connect.Types.Domain;
using B2Connect.Shared.Monitoring.Data.Entities;
using B2Connect.Shared.Monitoring.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Shared.Monitoring.Data;

/// <summary>
/// Entity Framework Core DbContext for Monitoring Service
/// Handles scheduler job state and monitoring data persistence
/// </summary>
public class MonitoringDbContext : DbContext
{
    private readonly ITenantContext? _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonitoringDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MonitoringDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    /// <param name="tenantContext">The tenant context for multi-tenant isolation.</param>
    public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Gets or sets the scheduler jobs DbSet.
    /// </summary>
    public DbSet<SchedulerJobEntity> SchedulerJobs => Set<SchedulerJobEntity>();

    /// <summary>
    /// Gets or sets the job execution logs DbSet.
    /// </summary>
    public DbSet<JobExecutionLogEntity> JobExecutionLogs => Set<JobExecutionLogEntity>();

    /// <summary>
    /// Gets or sets the connected services DbSet.
    /// </summary>
    public DbSet<ConnectedServiceEntity> ConnectedServices => Set<ConnectedServiceEntity>();

    /// <summary>
    /// Gets or sets the resource metrics DbSet.
    /// </summary>
    public DbSet<ResourceMetricsEntity> ResourceMetrics => Set<ResourceMetricsEntity>();

    /// <summary>
    /// Gets or sets the resource alerts DbSet.
    /// </summary>
    public DbSet<ResourceAlertEntity> ResourceAlerts => Set<ResourceAlertEntity>();

    /// <summary>
    /// Gets or sets the service errors DbSet.
    /// </summary>
    public DbSet<ServiceErrorEntity> ServiceErrors => Set<ServiceErrorEntity>();

    /// <summary>
    /// Gets or sets the connection test results DbSet.
    /// </summary>
    public DbSet<ConnectionTestResultEntity> ConnectionTestResults => Set<ConnectionTestResultEntity>();

    /// <summary>
    /// Gets or sets the communication errors DbSet.
    /// </summary>
    public DbSet<CommunicationErrorEntity> CommunicationErrors => Set<CommunicationErrorEntity>();

    /// <summary>
    /// Configures the model for the context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Multi-tenant query filters
        if (_tenantContext != null)
        {
            // Apply tenant isolation to all entities that have TenantId
            modelBuilder.Entity<SchedulerJobEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
            modelBuilder.Entity<JobExecutionLogEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
            modelBuilder.Entity<ConnectedServiceEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
            modelBuilder.Entity<ResourceMetricsEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
            modelBuilder.Entity<ResourceAlertEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
            modelBuilder.Entity<ServiceErrorEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
            modelBuilder.Entity<ConnectionTestResultEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
            modelBuilder.Entity<CommunicationErrorEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
        }
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set audit properties for entities being added/modified
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditableEntity auditable)
            {
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = now;
                    auditable.ModifiedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditable.ModifiedAt = now;
                }
            }

            // Set tenant ID for new entities if tenant context is available
            if (entry.State == EntityState.Added && entry.Entity is ITenantEntity tenantEntity && _tenantContext != null)
            {
                tenantEntity.TenantId = _tenantContext.TenantId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}