using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using B2Connect.ERP.Abstractions;

namespace B2Connect.Domain.ERP.Sync
{
    /// <summary>
    /// Sync Record for ERP entity mapping - based on eGate SyncRecord pattern
    /// Maintains mapping between B2Connect internal IDs and external ERP IDs
    /// </summary>
    public class ErpSyncRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TenantId { get; set; } = string.Empty;
        public string ProviderId { get; set; } = string.Empty; // "enventa", "sap", etc.
        public string EntityType { get; set; } = string.Empty; // "Article", "Customer", "Order"

        // B2Connect internal identifiers
        public Guid B2ConnectId { get; set; }
        public string B2ConnectEntityType { get; set; } = string.Empty;

        // ERP external identifiers
        public string ErpId { get; set; } = string.Empty;
        public string ErpEntityType { get; set; } = string.Empty;

        // Sync metadata
        public long ErpRowVersion { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastSyncUtc { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? LastModifiedUtc { get; set; }

        // Sync status
        public SyncStatus Status { get; set; } = SyncStatus.Active;
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; }

        // Optimistic concurrency
        public Guid Version { get; set; } = Guid.NewGuid();
    }

    /// <summary>
    /// Sync Status enum
    /// </summary>
    public enum SyncStatus
    {
        Active,
        Pending,
        Failed,
        Deleted,
        Ignored
    }

    /// <summary>
    /// Sync Record Repository interface
    /// </summary>
    public interface IErpSyncRecordRepository
    {
        Task<ErpSyncRecord?> GetByB2ConnectIdAsync(Guid b2ConnectId, string entityType);
        Task<ErpSyncRecord?> GetByErpIdAsync(string erpId, string entityType, string tenantId, string providerId);
        Task<IEnumerable<ErpSyncRecord>> GetByEntityTypeAsync(string entityType, string tenantId, string providerId);
        Task<IEnumerable<ErpSyncRecord>> GetPendingSyncAsync(string tenantId, string providerId, int limit = 1000);
        Task<IEnumerable<ErpSyncRecord>> GetFailedSyncAsync(string tenantId, string providerId, int limit = 100);

        Task CreateAsync(ErpSyncRecord record);
        Task UpdateAsync(ErpSyncRecord record);
        Task DeleteAsync(Guid id);

        Task<int> GetSyncCountAsync(string tenantId, string providerId, SyncStatus status);
        Task MarkAsSyncedAsync(Guid id, long newRowVersion);
        Task MarkAsFailedAsync(Guid id, string errorMessage);
        Task IncrementRetryCountAsync(Guid id);
    }

    /// <summary>
    /// Sync Record Manager - handles mapping logic
    /// Based on eGate's sync record management
    /// </summary>
    public class ErpSyncRecordManager
    {
        private readonly IErpSyncRecordRepository _repository;

        public ErpSyncRecordManager(IErpSyncRecordRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get or create sync record for B2Connect entity
        /// </summary>
        public async Task<ErpSyncRecord> GetOrCreateForB2ConnectAsync(
            Guid b2ConnectId,
            string entityType,
            string tenantId,
            string providerId)
        {
            var existing = await _repository.GetByB2ConnectIdAsync(b2ConnectId, entityType);
            if (existing != null)
                return existing;

            var record = new ErpSyncRecord
            {
                B2ConnectId = b2ConnectId,
                B2ConnectEntityType = entityType,
                TenantId = tenantId,
                ProviderId = providerId,
                EntityType = entityType,
                Status = SyncStatus.Pending
            };

            await _repository.CreateAsync(record);
            return record;
        }

        /// <summary>
        /// Get or create sync record for ERP entity
        /// </summary>
        public async Task<ErpSyncRecord> GetOrCreateForErpAsync(
            string erpId,
            string entityType,
            string tenantId,
            string providerId,
            long rowVersion = 0)
        {
            var existing = await _repository.GetByErpIdAsync(erpId, entityType, tenantId, providerId);
            if (existing != null)
            {
                // Update row version if changed
                if (existing.ErpRowVersion != rowVersion)
                {
                    existing.ErpRowVersion = rowVersion;
                    existing.LastModifiedUtc = DateTimeOffset.UtcNow;
                    await _repository.UpdateAsync(existing);
                }
                return existing;
            }

            var record = new ErpSyncRecord
            {
                ErpId = erpId,
                ErpEntityType = entityType,
                TenantId = tenantId,
                ProviderId = providerId,
                EntityType = entityType,
                ErpRowVersion = rowVersion,
                Status = SyncStatus.Pending
            };

            await _repository.CreateAsync(record);
            return record;
        }

        /// <summary>
        /// Link B2Connect and ERP entities
        /// </summary>
        public async Task LinkEntitiesAsync(
            Guid b2ConnectId,
            string erpId,
            string entityType,
            string tenantId,
            string providerId,
            long rowVersion = 0)
        {
            var record = await GetOrCreateForB2ConnectAsync(b2ConnectId, entityType, tenantId, providerId);
            record.ErpId = erpId;
            record.ErpRowVersion = rowVersion;
            record.Status = SyncStatus.Active;
            record.LastSyncUtc = DateTimeOffset.UtcNow;

            await _repository.UpdateAsync(record);
        }

        /// <summary>
        /// Mark entity as synced
        /// </summary>
        public async Task MarkSyncedAsync(Guid b2ConnectId, string entityType, long newRowVersion)
        {
            var record = await _repository.GetByB2ConnectIdAsync(b2ConnectId, entityType);
            if (record != null)
            {
                await _repository.MarkAsSyncedAsync(record.Id, newRowVersion);
            }
        }

        /// <summary>
        /// Handle sync failure with retry logic
        /// </summary>
        public async Task HandleSyncFailureAsync(
            Guid b2ConnectId,
            string entityType,
            string errorMessage,
            int maxRetries = 3)
        {
            var record = await _repository.GetByB2ConnectIdAsync(b2ConnectId, entityType);
            if (record == null)
            {
                return;
            }

            record.ErrorMessage = errorMessage;
            record.RetryCount++;

            if (record.RetryCount >= maxRetries)
            {
                record.Status = SyncStatus.Failed;
            }

            await _repository.UpdateAsync(record);
        }

        /// <summary>
        /// Get entities that need syncing (delta sync)
        /// </summary>
        public async Task<IEnumerable<ErpSyncRecord>> GetEntitiesForSyncAsync(
            string tenantId,
            string providerId,
            string entityType,
            int limit = 1000)
        {
            return await _repository.GetPendingSyncAsync(tenantId, providerId, limit);
        }

        /// <summary>
        /// Clean up old sync records
        /// </summary>
        public async Task CleanupOldRecordsAsync(
            string tenantId,
            string providerId,
            TimeSpan olderThan)
        {
            var cutoff = DateTimeOffset.UtcNow - olderThan;
            // Implementation would depend on repository capabilities
            // Could archive or delete records older than cutoff
        }
    }

    /// <summary>
    /// Sync Operation Result
    /// </summary>
    public class SyncOperationResult
    {
        public int ProcessedCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public int SkippedCount { get; set; }
        public TimeSpan Duration { get; set; }
        public List<SyncError> Errors { get; set; } = new();
    }

    /// <summary>
    /// Sync Error details
    /// </summary>
    public class SyncError
    {
        public Guid B2ConnectId { get; set; }
        public string ErpId { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}