using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace B2X.Notifications.Adapters
{
    public class NoOpEmailAdapter : IEmailAdapter
    {
        private readonly ILogger<NoOpEmailAdapter> _log;
        public NoOpEmailAdapter(ILogger<NoOpEmailAdapter> log) => _log = log;

        public Task SendAsync(string tenantId, object? payload)
        {
            _log.LogInformation("[NoOpEmail] tenant={Tenant}", tenantId);
            return Task.CompletedTask;
        }
    }
}
