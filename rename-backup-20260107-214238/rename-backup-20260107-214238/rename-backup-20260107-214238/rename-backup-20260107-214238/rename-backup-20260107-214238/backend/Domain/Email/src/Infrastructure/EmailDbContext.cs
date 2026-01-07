using System.Text.Json;
using B2Connect.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Email.Infrastructure;

/// <summary>
/// Database Context für Email-Monitoring
/// </summary>
public class EmailDbContext : DbContext
{
    public EmailDbContext(DbContextOptions<EmailDbContext> options)
        : base(options)
    {
    }

    public DbSet<EmailEvent> EmailEvents { get; set; }
    public DbSet<EmailMessage> EmailMessages { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // EmailEvent configuration
        modelBuilder.Entity<EmailEvent>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.EmailId)
                .IsRequired();

            entity.Property(e => e.TenantId)
                .IsRequired();

            entity.Property(e => e.EventType)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.Metadata)
                .HasMaxLength(1000);

            entity.Property(e => e.UserAgent)
                .HasMaxLength(500);

            entity.Property(e => e.IpAddress)
                .HasMaxLength(45); // IPv6 addresses

            entity.Property(e => e.OccurredAt)
                .IsRequired();

            // Indexes for performance
            entity.HasIndex(e => e.EmailId);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.OccurredAt });
            entity.HasIndex(e => new { e.TenantId, e.EventType, e.OccurredAt });

            entity.ToTable("email_events");
        });

        // EmailMessage configuration
        modelBuilder.Entity<EmailMessage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TenantId)
                .IsRequired();

            entity.Property(e => e.To)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Cc)
                .HasMaxLength(1000);

            entity.Property(e => e.Bcc)
                .HasMaxLength(1000);

            entity.Property(e => e.Subject)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Body)
                .IsRequired();

            entity.Property(e => e.TemplateKey)
                .HasMaxLength(100);

            entity.Property(e => e.Variables)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions()) ?? new Dictionary<string, object>(StringComparer.Ordinal));

            entity.Property(e => e.Priority)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.LastError)
                .HasMaxLength(2000);

            entity.Property(e => e.MessageId)
                .HasMaxLength(100);

            // Indexes for performance
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => new { e.Status, e.NextRetryAt });
            entity.HasIndex(e => new { e.TenantId, e.CreatedAt });
            entity.HasIndex(e => e.ScheduledFor);

            entity.ToTable("email_messages");
        });

        // EmailTemplate configuration
        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TenantId)
                .IsRequired();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Subject)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Body)
                .IsRequired();

            entity.Property(e => e.Variables)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions()) ?? new Dictionary<string, string>(StringComparer.Ordinal));

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100);

            // Indexes for performance
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.Key })
                .IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.IsActive });
            entity.HasIndex(e => new { e.TenantId, e.CreatedAt });

            entity.ToTable("email_templates");
        });
    }
}
