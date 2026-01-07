using Microsoft.AspNetCore.Mvc;
using B2Connect.Gateway.Store.Search.Services;
using B2Connect.Gateway.Store.Search.Jobs;

namespace B2Connect.Gateway.Store.Search.Controllers;

[ApiController]
[Route("api/admin/jobs/indices")]
public class AdminJobsController : ControllerBase
{
    private readonly JobStatusStore _statusStore;
    private readonly CreateIndicesJob _job;

    public AdminJobsController(JobStatusStore statusStore, CreateIndicesJob job)
    {
        _statusStore = statusStore;
        _job = job;
    }

    [HttpGet("/status")]
    public IActionResult Status()
    {
        var status = _statusStore.Get("create-indices-job");
        if (status is null)
        {
            return NotFound();
        }

        return Ok(status);
    }

    [HttpPost("/run")]
    public async Task<IActionResult> Run(CancellationToken cancellationToken)
    {
        _ = Task.Run(() => _job.RunAsync(cancellationToken), cancellationToken);
        return Accepted();
    }
}
