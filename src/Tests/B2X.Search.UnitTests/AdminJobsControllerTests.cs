using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using B2X.Gateway.Store.Search.Controllers;
using B2X.Gateway.Store.Search.Jobs;
using B2X.Gateway.Store.Search.Services;

namespace B2X.Search.UnitTests;

public class AdminJobsControllerTests
{
    [Fact]
    public async Task Run_ReturnsAccepted_And_StatusReturnedByGet()
    {
        var store = new JobStatusStore();
        store.Set("create-indices-job", new JobStatus("create-indices-job", "running", System.DateTimeOffset.UtcNow));

        var mockJob = new Mock<CreateIndicesJob>(MockBehavior.Loose, null!, null!);
        var controller = new AdminJobsController(store, mockJob.Object);

        var postResult = await controller.Run(CancellationToken.None);
        Assert.IsType<AcceptedResult>(postResult);

        var getResult = controller.Status() as OkObjectResult;
        Assert.NotNull(getResult);
        var status = getResult!.Value as JobStatus;
        Assert.NotNull(status);
        Assert.Equal("running", status!.Status);
    }
}
