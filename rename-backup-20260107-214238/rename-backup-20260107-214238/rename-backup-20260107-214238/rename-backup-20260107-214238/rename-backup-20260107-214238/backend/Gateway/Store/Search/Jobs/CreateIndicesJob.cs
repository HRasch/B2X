using B2Connect.Domain.Search.Services;
using B2Connect.Gateway.Store.Search.Services;

namespace B2Connect.Gateway.Store.Search.Jobs;

public class CreateIndicesJob
{
    private readonly IElasticService _elastic;
    private readonly JobStatusStore _statusStore;
    private static readonly string[] Languages = new[] { "en", "de", "fr" };

    public CreateIndicesJob(IElasticService elastic, JobStatusStore statusStore)
    {
        _elastic = elastic;
        _statusStore = statusStore;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var jobId = "create-indices-job";
        _statusStore.Set(jobId, new JobStatus(jobId, "running", DateTimeOffset.UtcNow));

        try
        {
            foreach (var lang in Languages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _elastic.EnsureIndexAsync(null, lang).ConfigureAwait(false);
            }

            _statusStore.Set(jobId, new JobStatus(jobId, "completed", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
        }
        catch (Exception ex)
        {
            _statusStore.Set(jobId, new JobStatus(jobId, "failed", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, ex.Message));
            throw;
        }
    }
}
