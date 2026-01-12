using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace B2X.Notifications.Tests
{
    public class HealthTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HealthTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HealthEndpoint_ReturnsOk()
        {
            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/health");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }
}
