using B2X.Api.Models.Erp;
using Microsoft.Extensions.Logging;

namespace B2X.Api.Validation;

public class QuarantineService
{
    private readonly ILogger<QuarantineService> _logger;
    private readonly IQuarantineRepository _repository;

    public QuarantineService(
        ILogger<QuarantineService> logger,
        IQuarantineRepository repository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task QuarantineAsync(ErpData data, ValidationResult result, CancellationToken ct = default)
    {
        _logger.LogWarning("Quarantining invalid ERP data: {DataId}, Reason: {Reason}",
            data.Id, result.Message);

        var quarantineRecord = new QuarantinedRecord
        {
            Id = Guid.NewGuid().ToString(),
            OriginalData = data,
            ValidationResult = result,
            QuarantinedAt = DateTime.UtcNow,
            Status = QuarantineStatus.PendingReview
        };

        await _repository.AddAsync(quarantineRecord, ct);
    }

    public async Task<IEnumerable<QuarantinedRecord>> GetPendingRecordsAsync(int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        return await _repository.GetPendingAsync(page, pageSize, ct);
    }

    public async Task ResolveRecordAsync(string recordId, QuarantineResolution resolution, CancellationToken ct = default)
    {
        var record = await _repository.GetByIdAsync(recordId, ct);
        if (record == null)
        {
            throw new KeyNotFoundException($"Quarantine record {recordId} not found");
        }

        record.Status = resolution.Status;
        record.ResolvedAt = DateTime.UtcNow;
        record.ResolutionNotes = resolution.Notes;

        await _repository.UpdateAsync(record, ct);

        _logger.LogInformation("Resolved quarantine record {RecordId} with status {Status}",
            recordId, resolution.Status);
    }
}

public interface IQuarantineRepository
{
    Task AddAsync(QuarantinedRecord record, CancellationToken ct = default);
    Task<QuarantinedRecord?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IEnumerable<QuarantinedRecord>> GetPendingAsync(int page, int pageSize, CancellationToken ct = default);
    Task UpdateAsync(QuarantinedRecord record, CancellationToken ct = default);
}

public class QuarantinedRecord
{
    public string Id { get; set; } = string.Empty;
    public ErpData OriginalData { get; set; } = new();
    public ValidationResult ValidationResult { get; set; } = new();
    public DateTime QuarantinedAt { get; set; }
    public QuarantineStatus Status { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNotes { get; set; }
}

public enum QuarantineStatus
{
    PendingReview,
    Approved,
    Rejected,
    Fixed
}

public class QuarantineResolution
{
    public QuarantineStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}
