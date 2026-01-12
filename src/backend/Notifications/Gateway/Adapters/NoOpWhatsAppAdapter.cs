using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace B2X.Notifications.Adapters
{
    public class NoOpWhatsAppAdapter : IWhatsAppAdapter
    {
        private readonly ILogger<NoOpWhatsAppAdapter> _log;
        public NoOpWhatsAppAdapter(ILogger<NoOpWhatsAppAdapter> log) => _log = log;

        public Task SendAsync(string tenantId, object? payload)
        {
            _log.LogInformation("[NoOpWhatsApp] tenant={Tenant}", tenantId);
            return Task.CompletedTask;
        }
    }
}
