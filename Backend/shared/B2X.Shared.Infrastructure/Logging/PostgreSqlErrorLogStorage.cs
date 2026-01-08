using Microsoft.EntityFrameworkCore;

namespace B2X.Shared.Infrastructure.Logging;

/// <summary>
/// PostgreSQL implementation of error log storage.
/// Uses EF Core for data access with optimized batch operations.
/// </summary>
public class PostgreSqlErrorLogStorage : IErrorLogStorage
{
    private readonly ErrorLogDbContext _context;

    public PostgreSqlErrorLogStorage(ErrorLogDbContext context)
    {
        _context = context;
    }

    public async Task StoreAsync(ErrorLogEntry entry, CancellationToken cancellationToken = default)
    {
        _context.ErrorLogs.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task StoreBatchAsync(IEnumerable<ErrorLogEntry> entries, CancellationToken cancellationToken = default)
    {
        await _context.ErrorLogs.AddRangeAsync(entries, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ErrorLogEntry>> GetRecentAsync(
        int count = 100,
        string? tenantId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ErrorLogs.AsQueryable();

        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(e => e.TenantId == tenantId);
        }

        return await query
            .OrderByDescending(e => e.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ErrorLogEntry>> GetByFingerprintAsync(
        string fingerprint,
        int count = 50,
        CancellationToken cancellationToken = default)
    {
        return await _context.ErrorLogs
            .Where(e => e.Fingerprint == fingerprint)
            .OrderByDescending(e => e.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<ErrorStatistics> GetStatisticsAsync(
        string? tenantId = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ErrorLogs.AsQueryable();

        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(e => e.TenantId == tenantId);
        }

        if (from.HasValue)
        {
            query = query.Where(e => e.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(e => e.Timestamp <= to.Value);
        }

        var errors = await query.ToListAsync(cancellationToken);

        var stats = new ErrorStatistics
        {
            TotalErrors = errors.Count,
            UniqueFingerprints = errors.Where(e => e.Fingerprint != null).Select(e => e.Fingerprint).Distinct().Count(),
            CriticalErrors = errors.Count(e => e.Severity == "fatal" || e.Severity == "critical"),
            WarningErrors = errors.Count(e => e.Severity == "warning"),
        };

        // Group by component
        stats.ErrorsByComponent = errors
            .Where(e => e.ComponentName != null)
            .GroupBy(e => e.ComponentName!)
            .ToDictionary(g => g.Key, g => g.Count());

        // Group by route
        stats.ErrorsByRoute = errors
            .Where(e => e.RoutePath != null)
            .GroupBy(e => e.RoutePath!)
            .ToDictionary(g => g.Key, g => g.Count());

        // Top errors by fingerprint
        stats.TopErrors = errors
            .Where(e => e.Fingerprint != null)
            .GroupBy(e => e.Fingerprint!)
            .Select(g => new ErrorFrequency
            {
                Fingerprint = g.Key,
                Message = g.First().Message,
                Count = g.Count(),
                FirstSeen = g.Min(e => e.Timestamp),
                LastSeen = g.Max(e => e.Timestamp),
            })
            .OrderByDescending(f => f.Count)
            .Take(10)
            .ToList();

        return stats;
    }
}

/// <summary>
/// EF Core DbContext for error logs.
/// Separate context to keep error logging isolated from main application data.
/// </summary>
public class ErrorLogDbContext : DbContext
{
    public ErrorLogDbContext(DbContextOptions<ErrorLogDbContext> options) : base(options)
    {
    }

    public DbSet<ErrorLogEntry> ErrorLogs => Set<ErrorLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ErrorLogEntry>(entity =>
        {
            entity.ToTable("error_logs");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Timestamp)
                .HasColumnName("timestamp")
                .IsRequired();

            entity.Property(e => e.ClientTimestamp)
                .HasColumnName("client_timestamp");

            entity.Property(e => e.Severity)
                .HasColumnName("severity")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Message)
                .HasColumnName("message")
                .IsRequired();

            entity.Property(e => e.StackTrace)
                .HasColumnName("stack_trace");

            entity.Property(e => e.Fingerprint)
                .HasColumnName("fingerprint")
                .HasMaxLength(64);

            entity.Property(e => e.ComponentName)
                .HasColumnName("component_name")
                .HasMaxLength(255);

            entity.Property(e => e.RoutePath)
                .HasColumnName("route_path")
                .HasMaxLength(500);

            entity.Property(e => e.RouteName)
                .HasColumnName("route_name")
                .HasMaxLength(255);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(255);

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .HasMaxLength(255);

            entity.Property(e => e.UserAgent)
                .HasColumnName("user_agent");

            entity.Property(e => e.Url)
                .HasColumnName("url");

            entity.Property(e => e.ClientIp)
                .HasColumnName("client_ip")
                .HasMaxLength(45); // IPv6 max length

            entity.Property(e => e.Metadata)
                .HasColumnName("metadata")
                .HasColumnType("jsonb");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            // Indexes for common queries
            entity.HasIndex(e => e.Timestamp)
                .HasDatabaseName("idx_error_logs_timestamp")
                .IsDescending();

            entity.HasIndex(e => e.Fingerprint)
                .HasDatabaseName("idx_error_logs_fingerprint");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("idx_error_logs_tenant");

            entity.HasIndex(e => e.Severity)
                .HasDatabaseName("idx_error_logs_severity");

            // Composite index for tenant + timestamp queries
            entity.HasIndex(e => new { e.TenantId, e.Timestamp })
                .HasDatabaseName("idx_error_logs_tenant_timestamp")
                .IsDescending(false, true);
        });
    }
}
