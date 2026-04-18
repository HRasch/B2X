using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace B2X.Notifications.Adapters
{
    public class NoOpSmsAdapter : ISmsAdapter
    {
        private readonly ILogger<NoOpSmsAdapter> _log;
        public NoOpSmsAdapter(ILogger<NoOpSmsAdapter> log) => _log = log;

        public Task SendAsync(string tenantId, object? payload)
        {
            _log.LogInformation("[NoOpSms] tenant={Tenant}", tenantId);
            return Task.CompletedTask;
        }
    }
}
