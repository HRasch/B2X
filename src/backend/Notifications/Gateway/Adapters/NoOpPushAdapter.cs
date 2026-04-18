using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace B2X.Notifications.Adapters
{
    public class NoOpPushAdapter : IPushAdapter
    {
        private readonly ILogger<NoOpPushAdapter> _log;
        public NoOpPushAdapter(ILogger<NoOpPushAdapter> log) => _log = log;

        public Task SendAsync(string tenantId, object? payload)
        {
            _log.LogInformation("[NoOpPush] tenant={Tenant}", tenantId);
            return Task.CompletedTask;
        }
    }
}
