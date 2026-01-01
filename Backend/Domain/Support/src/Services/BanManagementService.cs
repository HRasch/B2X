using B2Connect.Domain.Support.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace B2Connect.Domain.Support.Services;

/// <summary>
/// Service for managing IP/session bans based on malicious activity
/// </summary>
public interface IBanManagementService
{
    /// <summary>
    /// Checks if a request should be banned
    /// </summary>
    Task<BanCheckResult> CheckBanStatusAsync(string identifier, string requestType);

    /// <summary>
    /// Records a malicious attempt and potentially applies a ban
    /// </summary>
    Task RecordMaliciousAttemptAsync(string identifier, string requestType, SecurityThreatLevel threatLevel);

    /// <summary>
    /// Lifts a permanent ban (admin only)
    /// </summary>
    Task<bool> LiftPermanentBanAsync(string identifier, string adminUserId);

    /// <summary>
    /// Gets ban statistics for monitoring
    /// </summary>
    Task<BanStatistics> GetBanStatisticsAsync();
}

/// <summary>
/// Result of ban status check
/// </summary>
public class BanCheckResult
{
    public bool IsBanned { get; set; }
    public BanType BanType { get; set; }
    public DateTime? BanExpiresAt { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int StrikeCount { get; set; }
}

/// <summary>
/// Type of ban applied
/// </summary>
public enum BanType
{
    None,
    Temporary,
    Permanent
}

/// <summary>
/// Ban statistics for monitoring
/// </summary>
public class BanStatistics
{
    public int ActiveTemporaryBans { get; set; }
    public int ActivePermanentBans { get; set; }
    public int TotalBansToday { get; set; }
    public int TotalBansThisWeek { get; set; }
    public Dictionary<string, int> BansByThreatLevel { get; set; } = new();
    public Dictionary<string, int> BansByRequestType { get; set; } = new();
}

/// <summary>
/// Ban configuration settings
/// </summary>
public class BanConfiguration
{
    public int MaxStrikesBeforeTemporaryBan { get; set; } = 3;
    public int MaxStrikesBeforePermanentBan { get; set; } = 10;
    public TimeSpan TemporaryBanDuration { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan StrikeExpirationTime { get; set; } = TimeSpan.FromDays(7);
    public bool EnableProgressiveBanning { get; set; } = true;
    public Dictionary<SecurityThreatLevel, int> ThreatLevelWeights { get; set; } = new()
    {
        { SecurityThreatLevel.Safe, 0 },
        { SecurityThreatLevel.Suspicious, 1 },
        { SecurityThreatLevel.Malicious, 3 },
        { SecurityThreatLevel.Critical, 5 }
    };
}

/// <summary>
/// Represents a ban record
/// </summary>
public class BanRecord
{
    public string Identifier { get; set; } = string.Empty;
    public BanType BanType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
    public int StrikeCount { get; set; }
    public List<BanStrike> Strikes { get; set; } = new();
    public string? LiftedBy { get; set; }
    public DateTime? LiftedAt { get; set; }
}

/// <summary>
/// Represents a strike against an identifier
/// </summary>
public class BanStrike
{
    public DateTime Timestamp { get; set; }
    public SecurityThreatLevel ThreatLevel { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// In-memory implementation of ban management service
/// </summary>
public class InMemoryBanManagementService : IBanManagementService
{
    private readonly BanConfiguration _config;
    private readonly ILogger<InMemoryBanManagementService> _logger;
    private readonly Dictionary<string, BanRecord> _banRecords = new();
    private readonly object _lock = new();

    public InMemoryBanManagementService(
        IOptions<BanConfiguration> config,
        ILogger<InMemoryBanManagementService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public async Task<BanCheckResult> CheckBanStatusAsync(string identifier, string requestType)
    {
        await Task.CompletedTask; // For async compatibility

        lock (_lock)
        {
            if (!_banRecords.TryGetValue(identifier, out var record))
            {
                return new BanCheckResult
                {
                    IsBanned = false,
                    BanType = BanType.None,
                    StrikeCount = 0
                };
            }

            // Check if ban has expired
            if (record.BanType == BanType.Temporary && record.ExpiresAt.HasValue && record.ExpiresAt.Value < DateTime.UtcNow)
            {
                // Ban expired, remove it
                _banRecords.Remove(identifier);
                _logger.LogInformation("Temporary ban expired for identifier {Identifier}", identifier);

                return new BanCheckResult
                {
                    IsBanned = false,
                    BanType = BanType.None,
                    StrikeCount = record.StrikeCount
                };
            }

            // Clean expired strikes
            CleanExpiredStrikes(record);

            return new BanCheckResult
            {
                IsBanned = true,
                BanType = record.BanType,
                BanExpiresAt = record.ExpiresAt,
                Reason = record.Reason,
                StrikeCount = record.StrikeCount
            };
        }
    }

    public async Task RecordMaliciousAttemptAsync(string identifier, string requestType, SecurityThreatLevel threatLevel)
    {
        await Task.CompletedTask; // For async compatibility

        lock (_lock)
        {
            if (!_banRecords.TryGetValue(identifier, out var record))
            {
                record = new BanRecord
                {
                    Identifier = identifier,
                    CreatedAt = DateTime.UtcNow,
                    Strikes = new List<BanStrike>()
                };
                _banRecords[identifier] = record;
            }

            // Clean expired strikes first
            CleanExpiredStrikes(record);

            // Add new strike
            var strike = new BanStrike
            {
                Timestamp = DateTime.UtcNow,
                ThreatLevel = threatLevel,
                RequestType = requestType,
                Reason = $"Malicious {threatLevel.ToString().ToLower()} activity detected"
            };

            record.Strikes.Add(strike);
            record.StrikeCount = record.Strikes.Count;

            // Calculate weighted strike count
            var weightedStrikes = CalculateWeightedStrikes(record);

            _logger.LogWarning("Malicious attempt recorded for {Identifier}: {ThreatLevel} ({WeightedStrikes} weighted strikes)",
                identifier, threatLevel, weightedStrikes);

            // Apply progressive banning if enabled
            if (_config.EnableProgressiveBanning)
            {
                ApplyProgressiveBan(record, weightedStrikes, threatLevel, requestType);
            }
        }
    }

    public async Task<bool> LiftPermanentBanAsync(string identifier, string adminUserId)
    {
        await Task.CompletedTask; // For async compatibility

        lock (_lock)
        {
            if (!_banRecords.TryGetValue(identifier, out var record))
            {
                return false;
            }

            if (record.BanType != BanType.Permanent)
            {
                return false;
            }

            record.LiftedBy = adminUserId;
            record.LiftedAt = DateTime.UtcNow;
            record.BanType = BanType.None;
            record.ExpiresAt = null;

            _logger.LogInformation("Permanent ban lifted for {Identifier} by admin {AdminUserId}",
                identifier, adminUserId);

            return true;
        }
    }

    public async Task<BanStatistics> GetBanStatisticsAsync()
    {
        await Task.CompletedTask; // For async compatibility

        lock (_lock)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);

            var stats = new BanStatistics();

            foreach (var record in _banRecords.Values)
            {
                // Count active bans
                if (record.BanType == BanType.Temporary &&
                    (!record.ExpiresAt.HasValue || record.ExpiresAt.Value > now))
                {
                    stats.ActiveTemporaryBans++;
                }
                else if (record.BanType == BanType.Permanent && !record.LiftedAt.HasValue)
                {
                    stats.ActivePermanentBans++;
                }

                // Count bans created today
                if (record.CreatedAt.Date == today)
                {
                    stats.TotalBansToday++;
                }

                // Count bans created this week
                if (record.CreatedAt >= weekStart)
                {
                    stats.TotalBansThisWeek++;
                }

                // Count by threat level (use the highest threat level from strikes)
                var maxThreatLevel = record.Strikes.Max(s => s.ThreatLevel);
                var threatKey = maxThreatLevel.ToString();
                stats.BansByThreatLevel[threatKey] = stats.BansByThreatLevel.GetValueOrDefault(threatKey) + 1;

                // Count by request type
                stats.BansByRequestType[record.RequestType] = stats.BansByRequestType.GetValueOrDefault(record.RequestType) + 1;
            }

            return stats;
        }
    }

    private void CleanExpiredStrikes(BanRecord record)
    {
        var cutoff = DateTime.UtcNow - _config.StrikeExpirationTime;
        record.Strikes.RemoveAll(s => s.Timestamp < cutoff);
        record.StrikeCount = record.Strikes.Count;
    }

    private int CalculateWeightedStrikes(BanRecord record)
    {
        return record.Strikes.Sum(s => _config.ThreatLevelWeights.GetValueOrDefault(s.ThreatLevel, 1));
    }

    private void ApplyProgressiveBan(BanRecord record, int weightedStrikes, SecurityThreatLevel threatLevel, string requestType)
    {
        // Apply temporary ban
        if (weightedStrikes >= _config.MaxStrikesBeforeTemporaryBan && record.BanType == BanType.None)
        {
            record.BanType = BanType.Temporary;
            record.ExpiresAt = DateTime.UtcNow + _config.TemporaryBanDuration;
            record.Reason = $"Temporary ban due to {weightedStrikes} malicious attempts";
            record.RequestType = requestType;

            _logger.LogWarning("Applied temporary ban to {Identifier} for {Duration} (Reason: {Reason})",
                record.Identifier, _config.TemporaryBanDuration, record.Reason);
        }
        // Apply permanent ban
        else if (weightedStrikes >= _config.MaxStrikesBeforePermanentBan && record.BanType == BanType.Temporary)
        {
            record.BanType = BanType.Permanent;
            record.ExpiresAt = null;
            record.Reason = $"Permanent ban due to {weightedStrikes} repeated malicious attempts";

            _logger.LogError("Applied permanent ban to {Identifier} (Reason: {Reason})",
                record.Identifier, record.Reason);
        }
    }
}