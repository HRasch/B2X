using System.Collections.Concurrent;

namespace B2X.Gateway.Store.Search.Services;

public record JobStatus(string JobId, string Status, DateTimeOffset StartedAt, DateTimeOffset? CompletedAt = null, string? Message = null);

public class JobStatusStore
{
    private readonly ConcurrentDictionary<string, JobStatus> _store = new();

    public void Set(string jobId, JobStatus status) => _store[jobId] = status;

    public JobStatus? Get(string jobId) => _store.TryGetValue(jobId, out var s) ? s : null;
}
