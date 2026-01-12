using B2X.Shared.Monitoring.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace B2X.Shared.Monitoring.Data.Configurations;

/// <summary>
/// Configurations for monitoring entities - minimal EF Core mappings.
/// </summary>
public class MonitoringEntityConfigurations :
    IEntityTypeConfiguration<SchedulerJobEntity>,
    IEntityTypeConfiguration<JobExecutionLogEntity>,
    IEntityTypeConfiguration<ConnectedServiceEntity>,
    IEntityTypeConfiguration<ResourceMetricsEntity>,
    IEntityTypeConfiguration<ResourceAlertEntity>,
    IEntityTypeConfiguration<ServiceErrorEntity>,
    IEntityTypeConfiguration<ConnectionTestResultEntity>,
    IEntityTypeConfiguration<CommunicationErrorEntity>
{
    /// <summary>
    /// Configures SchedulerJobEntity.
    /// </summary>
    public void Configure(EntityTypeBuilder<SchedulerJobEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => new { e.TenantId, e.Status });
    }

    /// <summary>
    /// Configures JobExecutionLogEntity.
    /// </summary>
    public void Configure(EntityTypeBuilder<JobExecutionLogEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => new { e.TenantId, e.JobId });
        builder.HasOne(e => e.Job).WithMany(j => j.ExecutionLogs)
            .HasForeignKey(e => e.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Configures ConnectedServiceEntity.
    /// </summary>
    public void Configure(EntityTypeBuilder<ConnectedServiceEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => new { e.TenantId, e.Status });
    }

    /// <summary>
    /// Configures ResourceMetricsEntity.
    /// </summary>
    public void Configure(EntityTypeBuilder<ResourceMetricsEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => new { e.TenantId, e.ServiceId });
        builder.HasOne(e => e.Service).WithMany(s => s.ResourceMetrics)
            .HasForeignKey(e => e.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Configures ResourceAlertEntity.
    /// </summary>
    public void Configure(EntityTypeBuilder<ResourceAlertEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => new { e.TenantId, e.ServiceId });
        // ResourceAlertEntity does not have a navigation property back to service in current design
    }

    /// <summary>
    /// Configures ServiceErrorEntity.
    /// </summary>
    public void Configure(EntityTypeBuilder<ServiceErrorEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => new { e.TenantId, e.ServiceId });
        builder.HasOne(e => e.Service).WithMany(s => s.ServiceErrors)
            .HasForeignKey(e => e.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Configures ConnectionTestResultEntity.
    /// </summary>
    public void Configure(EntityTypeBuilder<ConnectionTestResultEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => new { e.TenantId, e.ServiceId });
        builder.HasOne(e => e.Service).WithMany(s => s.ConnectionTestResults)
            .HasForeignKey(e => e.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Configures CommunicationErrorEntity.
    /// </summary>
    public void Configure(EntityTypeBuilder<CommunicationErrorEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => new { e.TenantId, e.ServiceId });
        builder.HasOne(e => e.Service).WithMany(s => s.CommunicationErrors)
            .HasForeignKey(e => e.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
