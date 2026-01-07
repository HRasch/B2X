using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using B2Connect.Domain.Search.Services;
using B2Connect.Gateway.Store.Search.Jobs;
using B2Connect.Gateway.Store.Search.Services;

namespace B2Connect.Search.UnitTests;

public class CreateIndicesJobTests
{
    [Fact]
    public async Task RunAsync_CallsEnsureIndexForEachLanguage_AndSetsStatus()
    {
        var mockElastic = new Mock<IElasticService>();
        var store = new JobStatusStore();
        var job = new CreateIndicesJob(mockElastic.Object, store);

        await job.RunAsync(CancellationToken.None);

        mockElastic.Verify(e => e.EnsureIndexAsync(null, "en"), Times.Once);
        mockElastic.Verify(e => e.EnsureIndexAsync(null, "de"), Times.Once);
        mockElastic.Verify(e => e.EnsureIndexAsync(null, "fr"), Times.Once);

        var status = store.Get("create-indices-job");
        Assert.NotNull(status);
        Assert.Equal("completed", status!.Status);
    }
}
